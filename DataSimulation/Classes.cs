using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSimulation
{
    public class Classes { }

    public class Update
    {
        public int UserID { get; set; }
        public int Section { get; set; }
        public int RealSection { get; set; }
        public int Tag { get; set; }
        public int RealTag { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Day { get; set; }
    }

    public class User
    {
        public int ID { get; set; } // same as the ABM ID
        public int Trust { get; set; } // from 1 to 99
        public double UserRandomColumn { get; set; } // to quickly filter user portions
    }

    public class SectionOccupancy
    {
        public int Section { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Day { get; set; }
        public int OccupancyTag { get; set; }
        public double OccupancyValue { get; set; }
    }

    public class SectionOccupancyValueToTagPolicy
    {
        public int ConvertOccupancyValueToTag(double value, double[] Policy)
        {
            if (value < Policy[0] || value > Policy[Policy.Length - 1])
                throw new Exception("The value must be within the range of the occupancy policy");
            for (int i = 0; i < Policy.Length - 1; i++)
                if (value >= Policy[i] && value <= Policy[i + 1])
                    return (i + 1);
            return -1;
        }
    }



    public class InitialTrustAssignmentPolicy
    {
        public static string UNIFORM = "uniform";
        public static string NOMIAL = "nomial";
    }

    public class InitialTrustAssignment
    {
        public string Policy;
        public virtual int CreateInitialTrust() { return 0; }
    }

    public class InitialUniformTrustAssignment : InitialTrustAssignment
    {
        public string Policy = InitialTrustAssignmentPolicy.UNIFORM;
        public double LowerBound, UpperBound;
        private Random random;

        public InitialUniformTrustAssignment(double lowerBound, double upperBound)
        {
            random = new Random();
            if (lowerBound < 0 || upperBound > 1)
                throw new Exception("The bounds should be between [0,1]");
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public override int CreateInitialTrust()
        {
            int trust = -1;
            while (true)
            {
                double val = (random.NextDouble() * (UpperBound - LowerBound)) + LowerBound;
                trust = (int)Math.Ceiling(val * 100);
                if (trust >= 1 && trust <= 99)
                    break;
            }
            return trust;
        }
    }

    public class NormalDistributionParams
    {
        public double Mean { get; set; }
        public double Std { get; set; }
        public NormalDistributionParams(double mean, double std)
        {
            Mean = mean;
            Std = std;
        }
    }

    public class InitialNomialTrustAssignment : InitialTrustAssignment
    {
        // in this class the assigned trust values are based on a series of polinomial distributions with the probability of a distribution getting selected
        public string Policy = InitialTrustAssignmentPolicy.NOMIAL;
        public List<NormalDistributionParams> NormalParams;
        public List<double> SelectionProbability;
        private Random random;

        public InitialNomialTrustAssignment(List<NormalDistributionParams> normalParams, List<double> selectionProbability)
        {
            random = new Random();
            NormalParams = normalParams;
            SelectionProbability = selectionProbability;
        }

        public override int CreateInitialTrust()
        {
            int trust = -1, distToChoose = 0;
            double temp, sum = 0, u1, u2, mean, std, randStdNormal, randNormal;
            while (true)
            {
                // first we choose which distribution to choose from
                temp = random.NextDouble();
                sum = 0;
                for (int i = 0; i < SelectionProbability.Count; i++)
                {
                    if (temp >= sum && temp <= sum + SelectionProbability[i])
                    {
                        distToChoose = i;
                        break;
                    }
                    sum += SelectionProbability[i];
                }
                mean = NormalParams[distToChoose].Mean;
                std = NormalParams[distToChoose].Std;

                u1 = random.NextDouble(); //these are uniform(0,1) random doubles
                u2 = random.NextDouble();
                randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)

                trust = (int)Math.Ceiling(randNormal * 100);
                if (trust >= 1 && trust <= 99)
                    break;
            }
            return trust;
        }
    }

    public class GroupSimulationOptions
    {
        public InitialTrustAssignment TrustInitializer { get; set; }
        public double[][] SectionPriorities { get; set; }
        public double[][] SectionAssignmentProbabilityMap { get; set; }
        public double[] TagOccupancies { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
    }

    public class DefaultGroupSimulationOptions
    {
        public static InitialTrustAssignment TrustInitializer { get; set; }
        public static double[][] SectionPriorities = new double[][] { new double[] { 3, 2, 2, 2 }, new double[] { 1, 1, 1, 1 }, new double[] { 1, 1, 1, 2 }, new double[] { 4, 3, 3, 3 } };
        public static double[][] SectionAssignmentProbabilityMap = new double[][] { new double[] { 0.01, 0.04, 0.01 }, new double[] { 0.04, 0.8, 0.04 }, new double[] { 0.01, 0.04, 0.01 } }; // has to be 3x3
        public static double[] TagOccupancies = new double[] { 0, 0.5, 0.95, 1 };
        public static string Name { get; set; }
        public static string ID { get; set; }
    }

    public class InitialSectionAssigner
    {
        Random random = new Random();
        double[][] SectionPriorities;
        double[][] SectionAssignmentProbabilityMap;

        public InitialSectionAssigner(double[][] sectionPriorities, double[][] sectionAssignmentProbabilityMap)
        {
            SectionPriorities = sectionPriorities;
            SectionAssignmentProbabilityMap = sectionAssignmentProbabilityMap;
        }

        public void RandomlyInitializeSectionInformation(out int Section, out int RealSection)
        {
            while (true)
            {
                int TotalRows = SectionPriorities.Length;
                int TotalCols = SectionPriorities[0].Length;
                int row = random.Next(TotalRows);
                int col = random.Next(TotalCols);

                RealSection = row + col * TotalRows + 1;
                double sum = 0, temp = random.NextDouble();
                int selectedProbRow = 0, selectedProbCol = 0;
                for (int i = 0; i < SectionAssignmentProbabilityMap.Length; i++)
                    for (int j = 0; j < SectionAssignmentProbabilityMap[0].Length; j++)
                        if (temp >= sum && temp <= sum + SectionAssignmentProbabilityMap[i][j])
                        {
                            selectedProbRow = i - 1;
                            selectedProbCol = j - 1;
                            break;
                        }
                        else
                        {
                            sum += SectionAssignmentProbabilityMap[i][j];
                        }

                row = row + selectedProbRow;
                col = col + selectedProbCol;
                Section = row + col * TotalRows + 1;

                if (Section <= 16 && Section >= 1)
                    break;
            }
        }
    }

    public class InitialTagAssigner
    {
        Random random = new Random();
        public double[][] DeltaT_LookupTable;
        public int[] Deltas;
        public int PossibleTags;

        public InitialTagAssigner(int possibleTags)
        {
            // create lookup table for delta based on trust (Delta(t))
            int range = possibleTags - 1;
            int totalDeltas = 2 * range + 1, deltaMin = -range, deltaMax = range;

            PossibleTags = possibleTags;
            Deltas = new int[totalDeltas];
            DeltaT_LookupTable = new double[totalDeltas][];
            for (int delta = -range; delta <= range; delta++)
            {
                Deltas[delta + range] = delta;
                DeltaT_LookupTable[delta + range] = new double[99];

                for (int t = 1; t <= 99; t++)
                    DeltaT_LookupTable[delta + range][t - 1] = PdeltaT(delta, deltaMax, deltaMin, ((double)t) / 100.0); // equation 2
            }

            //string[] str = new string[5];
            //for (int i = 0; i < 5; i++)
            //    str[i] = string.Join(" ", DeltaT_LookupTable[i]);
            //var s = "a = [" + string.Join(";", str) + "]; imagesc(a')";
        }

        private double PdeltaT(int delta, int deltaMax, int deltaMin, double t)
        {
            double alpha = 0.8, trust = t;
            double root2 = Math.Sqrt(2);

            // both returns are correct
            //return (erf((delta+0.5)/((1/trust-alpha)*Math.Sqrt(2))) - erf((delta-0.5)/((1/trust-alpha)*Math.Sqrt(2))))/(erf((deltaMax+0.5)/((1/trust-alpha)*Math.Sqrt(2))) - erf((deltaMin-0.5)/((1/trust-alpha)*Math.Sqrt(2))));
            return (erf((delta + 0.5) / (sigma(t) * root2)) - erf((delta - 0.5) / (sigma(t) * root2))) / (erf((deltaMax + 0.5) / (sigma(t) * root2)) - erf((deltaMin - 0.5) / (sigma(t) * root2))); // equation 2
        }

        private double erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        private double sigma(double t)
        {
            return 1 / t - 0.8;
        } // alpha = 0.8

        public int AssignTag(int Realtag, int UserTrust)
        {
            int delta = 0;
            while (true)
            {
                double sum = 0, temp = random.NextDouble();
                for (int i = 0; i < Deltas.Length; i++)
                {
                    if (temp >= sum && temp <= sum + DeltaT_LookupTable[i][UserTrust - 1])
                    {
                        delta = Deltas[i];
                        break;
                    }
                    sum += DeltaT_LookupTable[i][UserTrust - 1];
                }
                if (Realtag + delta >= 1 && Realtag + delta <= PossibleTags)
                    break;
            }
            return Realtag + delta;
        }
    }
}
