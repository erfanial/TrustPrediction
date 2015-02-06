using DataSimulation;
using Dissertation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPredictionRealData.KparkFuser
{
    class KparkFuser
    {
        public Dissertation.SqlServerConnector db { get; set; }
        private SectionInformation[] Sections { get; set; }
        public User[] Users { get; set; }
        List<DatabaseOccuapncyUpdateAction> actions;
        List<UserUpdate> entireUpdates;
        SimOptions options;
        private Dictionary<int, List<double>> TrustUpdates;

        public KparkFuser(List<DatabaseOccuapncyUpdateAction> Actions, SimOptions Options, Dictionary<int, List<double>> TrustUpdates)
        {
            actions = Actions;
            options = Options;
            this.TrustUpdates = TrustUpdates;

            db = new Dissertation.SqlServerConnector();

            entireUpdates = GetEntireUpdatesFromLocalDatabase();

            const int nSections = 6;
            Sections = new SectionInformation[nSections];
            for (int i = 0; i < nSections; i++)
                Sections[i] = new SectionInformation() { LastUpdated = new DateTime(1000, 1, 1), Occupancy = -1 };
            Users = new User[50000];
            for (int i = 0; i < Users.Length; i++)
                Users[i] = new User() { Trust = 0.99, Score = 0.5 };
        }

        public void run(DateTime CurrentTime)
        {
            for (int s = 0; s < Sections.Length; s++)
                calculateOccupancyTrustValuesForSection(s, CurrentTime);            
        }

        private void calculateOccupancyTrustValuesForSection(int currentSection, DateTime CurrentTime)
        {
            string query, startDateTimeStr, endDateTimeStr;
            DateTime startDateTime, endDateTime;
            Dictionary<string, List<string>> queryResult;

            endDateTime = CurrentTime;

            startDateTime = endDateTime.Subtract(new TimeSpan(4, 0, 0));

            
            // at this point we have the data we need to run the fuser
            float iterationOccupancy;
            List<float> trusts;
            List<float> validity;
            float FinalX;
            float ParkingCondition = 1, ParkingConditionPercent = 0;
            float certainty;
            float C;
            int iterationsPerHour;
            float totalHoursOfUpdates = 0;
            float sectionOccupancy;
            int user_id;
            int update_id;
            float user_tag;

            var occupancyValueConverter = new SectionOccupancyValueToTagPolicy();
            var policy = new double[] { 0, 0.33, 0.66, 1 };
            
            
            Boolean noUpdates = false;
            iterationOccupancy = 1;
            DateTime currentTime = endDateTime;
            List<UserUpdate> updates = (from u in entireUpdates where u.section == currentSection + 1 && u.timestamp > startDateTime && u.timestamp < endDateTime select u).ToList();
            

            int nUpdates = updates.Count;
            int realTag = -1;

            if (nUpdates == 0)
            {
                noUpdates = true;
            }
            else
            {
                noUpdates = false;
                realTag = (int)updates[0].real_tag;
                ///////////// get trust of Users
                //T = [tags(:).trust];
                trusts = new List<float>(nUpdates);
                for (int i = 0; i < nUpdates; i++)
                    trusts.Add((float)Users[updates[i].user_id].Trust);
                
                validity = new List<float>(nUpdates);
                for (int i = 0; i < nUpdates; i++)
                    validity.Add(0);

                for (int i = nUpdates - 1; i >= 0; i--)
                {
                    validity[i] = trusts[i];
                    for (int j = i + 1; j < nUpdates; j++)
                        validity[i] *= 1 - trusts[j];
                }

                FinalX = 0;
                for (int i = 0; i < nUpdates; i++)
                    //FinalX += validity[i] * X_influence(updates[i].tag, updates[i].timestamp, currentTime);
                    FinalX += validity[i] * (updates[i].tag - options.I) / (((float)currentTime.Subtract(updates[i].timestamp).TotalMinutes) * options.decay + 1);
                iterationOccupancy = options.I + FinalX;
                DateTime lastUpdateTime = updates[updates.Count - 1].timestamp;

                
                
                ParkingConditionPercent = (float)((iterationOccupancy - UserUpdate.minTagValue) / (UserUpdate.maxTagValue - UserUpdate.minTagValue));
                ParkingCondition = (float)occupancyValueConverter.ConvertOccupancyValueToTag((double)ParkingConditionPercent, policy);

                for (int i = 0; i < nUpdates; i++)
                {
                    update_id = updates[i].update_id;
                    user_id = updates[i].user_id;
                    user_tag = updates[i].tag;


                    if (!updates[i].processed)
                    {
                        certainty = options.certainty_coeff / ((float)currentTime.Subtract(updates[i].timestamp).TotalMinutes);
                        if (ParkingCondition == user_tag)
                            C = options.lambda_promote * certainty;
                        else
                            C = options.lambda_punish * certainty * -1 * Math.Abs(ParkingCondition - user_tag);
                        var userScore = Users[user_id].Score + 0.125; // 0.125 is for the tag he submitted
                        userScore += C;
                        var userTrust = (float)((Math.Tanh(userScore / options.score_coeff) + 1) / 2);
                        var userRank = (int)Math.Floor(userTrust * 6);
                        Users[user_id].Trust = userTrust;
                        Users[user_id].Score = userScore;

                        // insert new trust value in TrustUpdates
                        if (!TrustUpdates.Keys.Contains(user_id))
                            TrustUpdates.Add(user_id, new List<double>());
                        // add the trust update to the records of user_id
                        // both if the records are empty
                        if(TrustUpdates[user_id].Count ==0)
                            TrustUpdates[user_id].Add(userTrust);
                        // and when the trust is different from the last recorded trust
                        else
                            if(TrustUpdates[user_id][TrustUpdates[user_id].Count-1] != userTrust)
                                TrustUpdates[user_id].Add(userTrust);
                        
                        updateTagProcessedFlag(false,update_id);
                    }
                }
            }



            if (!noUpdates)
            {
                Sections[currentSection].Occupancy = ParkingConditionPercent;
                Sections[currentSection].LastUpdated = currentTime;
                actions.Add(new DatabaseOccuapncyUpdateAction() { RealTag = realTag, SectionNumber = currentSection, Tag = (int)ParkingCondition, Timestamp = currentTime });
            }
            
        }

        private void updateTagProcessedFlag(bool flag, int update_id = -1)
        {
            foreach (var update in entireUpdates)
            {
                if (update_id == -1)
                    update.processed = flag;
                else
                    if (update.update_id == update_id)
                        update.processed = flag;
            }
        }

        public void ResetTagsProcessedField()
        {
            updateTagProcessedFlag(false);
        }

        private List<UserUpdate> GetEntireUpdatesFromLocalDatabase()
        {
            string query = string.Format("SELECT * FROM realUserUpdates");
            var queryResult = db.ExecuteQuery(query);
            List<UserUpdate> updates = new List<UserUpdate>();

            if (queryResult["uid"].Count > 0)
                for (int i = 0; i < queryResult["uid"].Count; i++)
                    updates.Add(new UserUpdate(){ real_tag = float.Parse(queryResult["real_tag"][i]), tag = float.Parse(queryResult["user_tag"][i]), section = int.Parse(queryResult["user_section"][i]), timestamp = Convert.ToDateTime(queryResult["date_received"][i]), update_id = int.Parse(queryResult["id"][i]), user_id = int.Parse(queryResult["uid"][i])});
                    
            else
                throw(new Exception("No user tags are available"));

            return updates;
        }        
    }

    public class UserUpdate
    {
        public static float minTagValue = 1;
        public static float maxTagValue = 3;
        public int update_id;
        public int user_id;
        public int section;
        public float tag;
        public float real_tag;
        public DateTime timestamp;
        public bool processed;
    }

    class SimOptions
    {
        public float I;
        public float lambda_promote;
        public float lambda_punish;
        public float certainty_coeff;
        public float score_coeff;
        public float decay;

        public static float[,] optionRange = new float[,] { { 1, 1 }, { 1, 1 }, { 1, 5 }, { 1, 10 }, { 0.001f, 1000 }, { 0, 0.1f } };

        public SimOptions(float val_I = 1, float val_lambda_promote = 1, float val_lambda_punish = 3.764678f, float val_certainty_coeff = 7.165505f, float val_score_coeff = 0.7852525f, float val_decay = (float)0.001736362)
        {
            I = val_I;
            lambda_promote = val_lambda_promote;
            lambda_punish = val_lambda_punish;
            certainty_coeff = val_certainty_coeff;
            score_coeff = val_score_coeff;
            decay = val_decay;
        }

        public string Stringify()
        {
            return I + " " + lambda_promote + " " + lambda_punish + " " + certainty_coeff + " " + score_coeff + " " + decay;
        }
    }

    class User
    {
        public double Trust { get; set; }
        public double Score { get; set; }
    }

    class SectionInformation
    {
        public float Occupancy { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
