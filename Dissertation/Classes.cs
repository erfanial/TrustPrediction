using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation
{
    class Classes
    {
    }

    public class Constants
    {
        public static string UserUpdatesTable = "";
        public static string ExperimentsTable = "";
    }

    public class SectionOccupancy
    {
        public int Day { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Section { get; set; }
        public double OccupancyValue { get; set; }
        public int OccupancyTag { get; set; }
        public int PredictedOccupancyTag { get; set; }
        public int MaxVoteOccupancyTag { get; set; }
        public int RandomOccupancyTag { get; set; }
        public int index { get; set; }
        public bool IsTrainingDay { get; set; }
        public bool CountInEvaluation { get; set; }

        public SectionOccupancy()
        {
            PredictedOccupancyTag = 1;
            MaxVoteOccupancyTag = 1;
            RandomOccupancyTag = 1;
        }
    }

    public class UserTagReport
    {
        public int Day { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Section { get; set; }
        public int RealSection { get; set; }
        public int UserGroup { get; set; }
        public int UserID { get; set; }
        public int RealTag { get; set; }
        public int Tag { get; set; }
        public int UserTrust { get; set; }
        public double RandomColumnValue { get; set; }
        public double RandomUserValue { get; set; }
    }

    public class User
    {
        public int ID { get; set; }
        public int RealTrust { get; set; }
        public int PredictedTrust { get; set; }

        public double MLE_SUM { get; set; }
        public int NTags { get; set; }

        public double[] Bayesian_TrustLikelihood { get; set; }

        public List<double> cooperativeRatings;

        public double R;
        public double S;

        public User(int id, int realTrust, int predictedTrust)
        {
            ID = id;
            RealTrust = realTrust;
            PredictedTrust = predictedTrust;

            MLE_SUM = 0.0;
            NTags = 0;

            Bayesian_TrustLikelihood = new double[99];
            for (int i = 0; i < 99; i++)
                Bayesian_TrustLikelihood[i] = ((double)i + 1) / 99.0;

            cooperativeRatings = new List<double>();
            R = 0;
            S = 0;
        }
    }

    public class DayHourSection
    {
        public int Day { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Section { get; set; }
        public List<UserTagReport> Reports { get; set; }
        public DayHourSection(List<UserTagReport> reports, int day = -1, int hour = -1, int section = -1, int weekday = -1)
        {
            Day = day;
            Weekday = weekday;
            Hour = hour;
            Section = section;
            Reports = reports;
        }
    }

    public class MethodPerformance
    {
        public Performance[] OccupancyTagVectorDifferencePerformance { get; set; }
        public Performance[] OccupancyPenaltyPerformance { get; set; }
        public Performance[] TrustPredictionVectorDifference { get; set; }
        public double Pcc { get; set; }
        public double Scc { get; set; }
        public double TotalProcessorTicks { get; set; }

        private List<SectionOccupancy> Occupancies;
        private List<User> Users;
        private int StopTrainingDay;

        public MethodPerformance(List<SectionOccupancy> occupancies, List<User> users, int stopTrainingDay, long ticks = 0)
        {
            TotalProcessorTicks = ticks;
            StopTrainingDay = stopTrainingDay;
            Occupancies = occupancies;
            Users = users;
            GetOccupancyPenaltySumation();
            GetOccupancyTagVectorDifference();
            GetTrustVectorDifference();
            GetCorrelationCoefficients();
        }

        private void GetOccupancyTagVectorDifference()
        {
            double sum11 = 0, sum12 = 0, sum13 = 0, sum21 = 0, sum22 = 0, sum23 = 0;
            double MaxDifference = (double)DataSimulation.DefaultGroupSimulationOptions.TagOccupancies.Length - 2;
            int counter = 0;
            for (int i = 0; i < Occupancies.Count; i++)
                if (Occupancies[i].CountInEvaluation)
                    if (Occupancies[i].Day >= StopTrainingDay)
                    {
                        var oc = Occupancies[i];
                        sum11 += (double)Math.Abs(oc.OccupancyTag - oc.PredictedOccupancyTag) / MaxDifference;
                        sum12 += (double)Math.Abs(oc.OccupancyTag - oc.MaxVoteOccupancyTag) / MaxDifference;
                        sum13 += (double)Math.Abs(oc.OccupancyTag - oc.RandomOccupancyTag) / MaxDifference;
                        sum21 += Math.Pow(oc.OccupancyTag - oc.PredictedOccupancyTag, 2);
                        sum22 += Math.Pow(oc.OccupancyTag - oc.MaxVoteOccupancyTag, 2);
                        sum23 += Math.Pow(oc.OccupancyTag - oc.RandomOccupancyTag, 2);
                        counter++;
                    }
            OccupancyTagVectorDifferencePerformance = new Performance[] { new Performance(1 - sum11 / counter, sum21 / counter), new Performance(1 - sum12 / counter, sum22 / counter), new Performance(1 - sum13 / counter, sum23 / counter) }; // pred, maxvote, random
        }

        private void GetOccupancyPenaltySumation()
        {
            double sum11 = 0, sum12 = 0, sum13 = 0, sum21 = 0, sum22 = 0, sum23 = 0;
            int counter = 0;
            var PenaltyMatrix = new ExperimentInputParams().PenaltyMatrix;
            for (int i = 0; i < Occupancies.Count; i++)
                if (Occupancies[i].CountInEvaluation)
                    if (Occupancies[i].Day >= StopTrainingDay)
                    {
                        var oc = Occupancies[i];
                        sum11 += PenaltyMatrix[oc.OccupancyTag - 1][oc.PredictedOccupancyTag - 1];
                        sum12 += PenaltyMatrix[oc.OccupancyTag - 1][oc.MaxVoteOccupancyTag - 1];
                        sum13 += PenaltyMatrix[oc.OccupancyTag - 1][oc.RandomOccupancyTag - 1];
                        counter++;
                    }
            OccupancyPenaltyPerformance = new Performance[] { new Performance(sum11 / counter, -1), new Performance(sum12 / counter, -1), new Performance(sum13 / counter, -1) }; // pred, maxvote, random
        }

        private void GetTrustVectorDifference()
        {
            double sum11 = 0, sum12 = 0, sum13 = 0, sum21 = 0, sum22 = 0, sum23 = 0;
            int counter = 0;
            Random random = new Random();
            for (int i = 0; i < Users.Count; i++)
            {
                var rt = random.NextDouble();
                var u = Users[i];
                if (u != null)
                {
                    sum11 += (double)Math.Abs(u.RealTrust - u.PredictedTrust);
                    sum13 += (double)Math.Abs(u.RealTrust - rt * 100);
                    sum21 += Math.Pow(u.RealTrust - u.PredictedTrust, 2);
                    sum23 += Math.Pow(u.RealTrust - rt, 2);
                    counter++;
                }
            }
            TrustPredictionVectorDifference = new Performance[] { new Performance(1 - (sum11 / 100) / counter, (sum21 / 10000) / counter), new Performance(1 - (sum13 / 100) / counter, (sum23 / 10000) / counter) }; // pred, random
        }

        private void GetCorrelationCoefficients()
        {
            var q1 = (from u in Users where u != null select u.PredictedTrust).ToArray();
            var q2 = (from u in Users where u != null select u.RealTrust).ToArray();
            double[] predicted_trust_array = new double[q1.Count()], user_trust_array = new double[q2.Count()];
            for (int i = 0; i < q1.Length; i++)
            {
                predicted_trust_array[i] = (double)q1[i] / 100;
                user_trust_array[i] = (double)q2[i] / 100;
            }
            LinearCorrelation lc = new LinearCorrelation(predicted_trust_array, user_trust_array);
            double pcc = -10;
            lc.GetPearson(ref pcc);
            double scc = lc.ComputeSpearmanRankCorrelation(predicted_trust_array, user_trust_array);
            if(double.IsNaN(pcc))
                pcc = -2;
            if(double.IsNaN(scc))
                scc = -2;

            Pcc = pcc;
            Scc = scc;
        }
    }

    public class Performance
    {
        public double rate;
        public double mse;

        public Performance(double r, double m)
        {
            rate = r;
            mse = m;
        }
    }

    public class ExperimentOptions
    {
        public double pop { get; set; }
        public double pou { get; set; }
        public double pot { get; set; }
        public bool discounted { get; set; }
        public bool maxTrust { get; set; }
        public int ExperimentMethodCode { get; set; }
        public int ExperimentGroup { get; set; }

        public ExperimentOptions(bool Discounted, bool MaxTrust, double Pop, double Pou, double Pot, int experimentMethodCode, int populationGroup)
        {
            discounted = Discounted;
            maxTrust = MaxTrust;
            pop = Pop;
            pou = Pou;
            pot = Pot;
            ExperimentMethodCode = experimentMethodCode;
            ExperimentGroup = populationGroup;
        }
    }
}
