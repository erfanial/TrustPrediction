using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Collections;

namespace Dissertation
{
    public class Experiment
    {
        private SqlServerConnector db;
        private string query;
        private Dictionary<string, List<string>> dbResult;
        private int experimentMethodCode;
        private int experimentGroup;
        private string experimentName;
        private Random randomNumberGenerator;
        private List<SectionOccupancy> Occupancies;
        private List<UserTagReport> TotalReportedUserTags;
        private List<User> Users;
        private List<UserTagReport> UserTags;
        
        public Experiment(string _experimentName, int _experimentMethodCode, int _experimentGroup)
        {
            db = new SqlServerConnector();

            experimentMethodCode = _experimentMethodCode;
            experimentGroup = _experimentGroup;
            experimentName = _experimentName;

            randomNumberGenerator = new Random();
        }

        private void init()
        {
            // now read all the updates that people made
            TotalReportedUserTags = ReadTotalReportedUserTagsFromDatabase();

            // see how many days are the tags in
            int maxDays = (from t in TotalReportedUserTags orderby t.Day descending select t.Day).ToArray<int>().First();


            // obtain the ground truth
            // the ground truth is the real occupancy tags of sections throughout the experiment
            Occupancies = ReadGroundTruthFromDatabase(maxDays);
        }

        private List<UserTagReport> ReadTotalReportedUserTagsFromDatabase()
        {
            var ruts = new List<UserTagReport>();
            query = "SELECT [usergroup] ,[uid] ,[user_section] ,[real_section] ,[user_tag] ,[real_tag] ,[user_trust] ,[day] ,[weekday] ,[hour] ,[randcol] ,[randusercol]  FROM [DissertationSimDB].[dbo].[" + Constants.UserUpdatesTable + "] where usergroup = " + experimentGroup + " order by usergroup,day,hour,user_section";
            dbResult = db.ExecuteQuery(query);
            for (int i = 0; i < dbResult["usergroup"].Count; i++)
            {
                var rut = new UserTagReport();
                rut.UserGroup = experimentGroup;
                rut.Day = int.Parse(dbResult["day"][i]);
                rut.Weekday = int.Parse(dbResult["weekday"][i]);
                rut.Hour = int.Parse(dbResult["hour"][i]);
                rut.Section = int.Parse(dbResult["user_section"][i]);
                rut.RealSection = int.Parse(dbResult["real_section"][i]);
                rut.Tag = int.Parse(dbResult["user_tag"][i]);
                rut.RealTag = int.Parse(dbResult["real_tag"][i]);
                rut.UserID = int.Parse(dbResult["uid"][i]);
                rut.UserTrust = int.Parse(dbResult["user_trust"][i]);
                rut.RandomColumnValue = double.Parse(dbResult["randcol"][i]);
                rut.RandomUserValue = double.Parse(dbResult["randusercol"][i]);
                ruts.Add(rut);
            }
            return ruts;
        }

        private List<SectionOccupancy> ReadGroundTruthFromDatabase(int maxDays)
        {
            if (maxDays > 90)
                maxDays = 90;
            var gt = new List<SectionOccupancy>();
            query = "SELECT dbo.sectionProbabilities.section_id, dbo.sectionProbabilities.prob, dbo.cardata.day, dbo.cardata.hour, dbo.cardata.weekday FROM dbo.cardata INNER JOIN dbo.sectionProbabilities ON dbo.cardata.id = dbo.sectionProbabilities.hour_id WHERE day<=" + maxDays + " ORDER BY dbo.cardata.day,dbo.cardata.hour,dbo.sectionProbabilities.section_id";
            dbResult = db.ExecuteQuery(query);
            for (int i = 0; i < dbResult["prob"].Count; i++)
            {
                var so = new SectionOccupancy();
                so.Day = int.Parse(dbResult["day"][i]);
                so.Weekday = int.Parse(dbResult["weekday"][i]);
                so.Hour = int.Parse(dbResult["hour"][i]);
                so.Section = int.Parse(dbResult["section_id"][i]);
                so.OccupancyValue = double.Parse(dbResult["prob"][i]);
                so.OccupancyTag = new DataSimulation.SectionOccupancyValueToTagPolicy().ConvertOccupancyValueToTag(so.OccupancyValue, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies);
                so.index = i;
                gt.Add(so);
            }
            return gt;
        }


        public void run()
        {
            init();
            
            // delete already existing data from database
            db.ExecuteNonQuery("DELETE FROM " + Constants.ExperimentsTable + " WHERE algorithm_id = " + experimentMethodCode + " and population_group = " + experimentGroup);

            // At this stage we have everything we need to run the experiment
            // We should run the experiment on 5 different parameters mentioned in ExperimentInputParams
            var inputParams = new ExperimentInputParams();
            Users = new List<User>(50000);
            for (int i = 0; i < Users.Capacity; i++)
                Users.Add(null);

            foreach (var pop in inputParams.percentOfParticipants)
            {
                foreach (var pou in inputParams.percentOfUpdatesUsed)
                {
                    for (int i = 0; i < Users.Capacity; i++)
                        Users[i] = null;
                    UserTags = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    UserTags = (from tag in TotalReportedUserTags where tag.RandomUserValue <= pop && tag.RandomColumnValue <= pou select tag).ToList();
                    var usersids = (from tag in UserTags select new { id = tag.UserID, trust = tag.UserTrust }).Distinct().ToList();
                    for (int i = 0; i < usersids.Count; i++)
                        Users[usersids[i].id] = new User(usersids[i].id, usersids[i].trust, 50);

                    foreach (var dc in inputParams.discounted)
                        foreach (var mt in inputParams.maxTrust)
                            foreach (var pot in inputParams.percentOfTraining)
                            {
                                var options = new ExperimentOptions(dc, mt, pop, pou, pot, experimentMethodCode, experimentGroup);
                                if (UserTags.Count > 0)
                                {
                                    Method method = new Method(Occupancies, Users, UserTags, options);

                                    MethodPerformance performance = method.Execute();

                                    int experiment_id = storeExperiment2db(performance, options);
                                }
                            }
                }
            }
        }

        private int storeExperiment2db(MethodPerformance performance, ExperimentOptions options)
        {
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO " + Constants.ExperimentsTable + " (algorithm_id,population_group,algorithm_name,discounted,max_trust,pop,pou,pot,occupancy_pred_rate,occupancy_mse,trust_pred_rate,trust_mse,processing_time,random_rate,random_mse,pcc,scc,maxvote_rate,maxvote_mse,averagePenalties_pred,averagePenalties_maxvote,averagePenalties_random) VALUES (");

            string[] vals = new string[] {
                experimentMethodCode.ToString(),
                experimentGroup.ToString(), 
                "'" + experimentName + "'",
                options.discounted ? "1" : "0",
                options.maxTrust ? "1" : "0", 
                options.pop.ToString(), 
                options.pou.ToString(), 
                options.pot.ToString(),
                performance.OccupancyTagVectorDifferencePerformance[0].rate.ToString(), 
                performance.OccupancyTagVectorDifferencePerformance[0].mse.ToString(),
                performance.TrustPredictionVectorDifference[0].rate.ToString(), 
                performance.TrustPredictionVectorDifference[0].mse.ToString(),
                performance.TotalProcessorTicks.ToString(),
                performance.OccupancyTagVectorDifferencePerformance[2].rate.ToString(), 
                performance.OccupancyTagVectorDifferencePerformance[2].mse.ToString(),
                performance.Pcc.ToString(), 
                performance.Scc.ToString(),
                performance.OccupancyTagVectorDifferencePerformance[1].rate.ToString(), 
                performance.OccupancyTagVectorDifferencePerformance[1].mse.ToString(),
                performance.OccupancyPenaltyPerformance[0].rate.ToString(), 
                performance.OccupancyPenaltyPerformance[1].rate.ToString(), 
                performance.OccupancyPenaltyPerformance[2].rate.ToString()
            };

            string values = string.Join(",", vals);
            query.Append(values + ")");
            string qstring = query.ToString();
            if (!db.ExecuteNonQuery(qstring))
                Console.WriteLine("Failed");
            else
            {
                int experiment_id = int.Parse(db.ExecuteQuery("select top 1 id from experiments order by timeOfCompletion desc")["id"][0]);
                //query.Clear();
                //foreach (var item in Occupancies)
                //{
                //    if(!item.IsTrainingDay)
                //        // store to simulated occupancies table
                //        query.Append(string.Format("INSERT INTO [dbo].[SimulatedOccupancies] ([experiment_id] ,[real_tag] ,[predicted_tag] ,[maxvote_tag], [hour], [weekday], [day], [section]) VALUES ('{0}',{1},{2},{3},{4}, {5}, {6}, {7});\n",
                //            experiment_id, item.OccupancyTag, item.PredictedOccupancyTag, item.MaxVoteOccupancyTag, item.Hour, item.Weekday, item.Day, item.Section));
                //}
                //qstring = query.ToString();
                //if (!db.ExecuteNonQuery(qstring))
                //    Console.WriteLine("Failed");
                //else
                    return experiment_id;
            }
            return -1;
        }

    }
}
