using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;
using Dissertation;
using TrustAnalizer.TagProcessors;

namespace TrustAnalizer
{
    public class AnalizeSimulatedTrust
    {
        private SimulatedTagsLoader loader;

        public AnalizeSimulatedTrust()
        {
            loader = new SimulatedTagsLoader();
        }

        public void analyze()
        {
            string CopyToExcel = "";
            TrustPredictionMethod method = new TrustPredictionMethod();
            int nUsers = 200000;
            int[] nTagsPerUser = new int[] { 10, 100, 1000 };
            foreach (int ntags in nTagsPerUser)
            {
                var totalTags = loader.simulate(nUsers, ntags);
                var Users = (from tag in totalTags group tag by tag.UserID into g select new { userid = g.Key, nTags = g.ToList().Count, tags = g.ToList() }).ToList();
                var TagNumberGroups = (from u in Users group u by u.nTags into g orderby g.Key select new { num = g.Key, users = g.ToList() }).ToList();

                for (int methodNumber = 0; methodNumber < 3; methodNumber++)
                {
                    string s = "";
                    switch (methodNumber)
                    {
                        case 0:
                            method = new MLEProcessor();
                            s += "MLE\t";
                            break;
                        case 1:
                            method = new BayesianProcessor();
                            s += "Bayesian\t";
                            break;
                        case 2:
                            method = new BetaProcessor();
                            s += "Beta\t";
                            break;
                        default:
                            break;
                    }


                    // lets see how well prediction is and takes
                    foreach (var g1 in TagNumberGroups)
                    {
                        Console.WriteLine(g1.num);
                        int SumRates = 0;
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        foreach (var user in g1.users)
                            SumRates += method.ProcessTags(user.tags);
                        stopwatch.Stop();
                        //double Error = 1 - ((double)SumRates / g1.users.Count) / 100;
                        //s += Error.ToString() + "\t";
                        long ticks = stopwatch.ElapsedTicks / g1.users.Count;
                        s += ticks.ToString() + "\t";
                    }
                    CopyToExcel += s + "\n";
                }
            }


            using (StreamWriter file = new StreamWriter("CopyToExcel_ticks.txt"))
                file.Write(CopyToExcel);
        }
    }
}
