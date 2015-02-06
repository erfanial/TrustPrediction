using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation.TagProcessors
{
    class GompertzProcessor
    {
        double lamda_standard = 0.7;
        double lamda_penalty = 0.8;

        DataSimulation.SectionOccupancyValueToTagPolicy policy = new DataSimulation.SectionOccupancyValueToTagPolicy();
        ExperimentInputParams param = new ExperimentInputParams();

        public int ProcessTags(List<UserTagReport> tags, List<User> Users, ExperimentOptions Options, int RealTag, bool isTraining)
        {
            int PredictedTag;

            if (isTraining)
            {
                PredictedTag = RealTag;
            }
            else
            {
                if (Options.maxTrust)
                {
                    PredictedTag = (from report in tags orderby Users[report.UserID].PredictedTrust descending, report.RandomColumnValue select report.Tag).ToArray()[0];
                }
                else
                {
                    double sum1 = 0, sum2 = 0;
                    int t;
                    foreach (var report in tags)
                    {
                        t = Users[report.UserID].PredictedTrust;
                        sum1 += t * report.Tag;
                        sum2 += t;
                    }
                    double a1 = (sum1 / sum2 - 1) / (DataSimulation.DefaultGroupSimulationOptions.TagOccupancies.Length - 1 - 1);
                    PredictedTag = policy.ConvertOccupancyValueToTag(a1, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies);
                }
            }

            return PredictedTag;
        }

        public void UpdateTrustFromObservation(List<UserTagReport> tags, List<User> Users) // workes on updates
        {
            int n = tags.Count;
            int[] user_ids = new int[n];
            double[] x = new double[n];
            double[] p = new double[n];
            double eps = Math.Pow(10, -20);
            for (int i = 0; i < n; i++)
            {
                user_ids[i] = tags[i].UserID;
                x[i] = tags[i].Tag;
                p[i] = 1.0 / n;
            }

            double d = double.PositiveInfinity;
            double r, s, b;
            double[] a = new double[n];
            double[] a2 = new double[n];
            double[] p_old = new double[n];
            while (d > 0.0001)
            {
                r = 0;
                for (int i = 0; i < n; i++)
                    r += p[i] * x[i];

                s = 0;
                for (int i = 0; i < n; i++)
                {
                    a[i] = Math.Pow(x[i] - r, 2);
                    s += a[i];
                }

                b = 0;
                for (int i = 0; i < n; i++)
                {
                    if (s > 0)
                        a2[i] = s / (a[i] + eps);
                    else
                        a2[i] = 1 / eps;
                    b += a2[i];
                }

                d = 0;
                for (int i = 0; i < n; i++)
                {
                    p_old[i] = p[i];
                    p[i] = a2[i] / b + eps;
                    d += Math.Abs(p[i] - p_old[i]);
                }
            }

            double[] p_norm = new double[n];
            double pmin = double.PositiveInfinity, pmax = double.NegativeInfinity;
            for (int i = 0; i < n; i++)
            {
                if (pmin > p[i])
                    pmin = p[i];
                if (pmax < p[i])
                    pmax = p[i];
            }

            for (int i = 0; i < n; i++)
                p_norm[i] = (double)Math.Sign(2 * (p[i] - pmin + eps) / (pmax - pmin + eps) - 1);

            double R = 0, lamda, rating;
            List<double> c;
            int ind;
            for (int i = 0; i < n; i++)
            {
                ind = user_ids[i];
                //if (ind == 10005)
                //    Console.WriteLine("Halt");

                c = Users[ind].cooperativeRatings;
                c.Add(p_norm[i]);


                double sum = 0, suml = 0, pnorm;
                for (int j = 0; j < c.Count; j++)
                {
                    pnorm = c[j];
                    if (c[j] == 1)
                        lamda = lamda_standard;
                    else
                        lamda = lamda_penalty;
                    sum += pnorm * Math.Pow(lamda, c.Count - j - 1);
                    //if (experimentID == 21)
                    //    suml += Math.Pow(lamda, c.Count - i - 1);
                }

                R = Gompertz(sum);
                //if (experimentID == 21)
                //    R = (sum / suml + 1) / 2;

                if (R < 0.01)
                    R = 0.01;
                if (R > 0.99)
                    R = 0.99;
                R = Math.Ceiling(R * 99) / 99;

                Users[ind].PredictedTrust = (int)(R * 100);
            }
        }

        private double Gompertz(double x)
        {
            double a = 1, b = -2.6, c = -0.85;
            return a * Math.Exp(b * Math.Exp(c * x));
        }
    }
}
