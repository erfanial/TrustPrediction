using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;

namespace RealtimeResearch
{
    class RealtimeResearch
    {
        private ResearchData data;
        private List<SimOptions> options;
        private List<PredictionPerformances> results;
        private List<double> executionTimes;

        private int _numerOfThreadsNotYetCompleted = 0;
        private ManualResetEvent _doneEvent;

        
        public RealtimeResearch()
        {
            data = new ResearchData();
            
            options = new List<SimOptions>();
            results = new List<PredictionPerformances>();
            executionTimes = new List<double>();

            init();
        }

        private void init()
        {
            InitializeExperiments();

        }

        private void InitializeExperiments()
        {
            double[] simopt2 = new double[] { 1 };
            double[] simopt3 = new double[] { 1, 1.05, 1.1, 1.15, 1.2, 1.25, 1.3, 1.4, 1.5, 2, 3 };
            double[] simopt4 = new double[] { 0.5,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,20,30,50,100 };
            double[] simopt5 = new double[] { 1,5,10,15,20,25,30,40,50,100,200,500,1000 };
            double[] simopt6 = new double[] { 0.000001, 0.000005, 0.0001, 0.0005, 0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1,5,10 };

            foreach(double opt2 in simopt2)
                foreach (double opt3 in simopt3)
                    foreach (double opt4 in simopt4)
                        foreach (double opt5 in simopt5)
                            foreach (double opt6 in simopt6)
                                options.Add(new SimOptions(1, opt2, opt3, opt4, opt5, opt6));
        }

        public void run(Boolean parallel = true)
        {
            Experiment experiment = new Experiment(data, new SimOptions(1,1,1.1,5.4,26,0.0006));
            experiment.execute();
        }
    }
}
