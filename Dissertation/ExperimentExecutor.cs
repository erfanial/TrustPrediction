using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissertation
{
    class ExperimentExecutor
    {
        private int[] experimentIDs;
        private Experiment[] experiments;
        private ManualResetEvent[] doneEvents;
        private SqlServerConnector db;


        public ExperimentExecutor(int[] ids)
        {
            experimentIDs = ids;
            db = new SqlServerConnector();
            Console.WriteLine("Initializing experiments...");
            initExperiments();
        }

        private void initExperiments()
        {
            experiments = new Experiment[(int) experimentIDs.Length];
            for (int i = 0; i < experiments.Length; i++)
                assignExperimentDetails(i);
            
        }

        private void assignExperimentDetails(int i)
        {
            int experimentID = experimentIDs[i];
            int experimentGroupCode = (int)(experimentID / 100);
            int experimentMethodCode = (int)(experimentID - experimentGroupCode * 100);

            string experimentName = "";

            switch (experimentMethodCode)
            {
                case 10:
                    experimentName = "MLE";
                    break;
                case 11:
                    experimentName = "Bayesian";
                    break;
                case 20:
                    experimentName = "Gompertz";
                    break;
                case 21:
                    experimentName = "Robust Average";
                    break;
                case 23:
                    experimentName = "Beta";
                    break;
                case 30:
                    experimentName = "Realtime";
                    break;
                case 31:
                    experimentName = "Portfolio_Regression";
                    break;
                case 32:
                    experimentName = "Portfolio_Classification";
                    break;
                default:
                    throw(new Exception("Experiment Not Implemented"));
            }
            experiments[i] = new Experiment(experimentName, experimentMethodCode, experimentGroupCode);            
        }


        public void execute() // generate_results
        {
            for (int i = 0; i < experimentIDs.Length; i++)
            {
                Console.WriteLine("Experiment {0} started at {1}", experimentIDs[i], string.Format("{0:HH:mm:ss tt}", DateTime.Now));

                experiments[i].run();

                Console.WriteLine("Experiment {0} ended at {1}", experimentIDs[i], string.Format("{0:HH:mm:ss tt}", DateTime.Now));
            }

        }

        public void executeParallel()
        {
            int nExperiments = experimentIDs.Length;
            doneEvents = new ManualResetEvent[nExperiments];

            for (int i = 0; i < nExperiments; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                Experiment e = experiments[i];
                ThreadPool.QueueUserWorkItem(ThreadPoolCallback, i);
            }

            // Wait for all threads in pool to calculate.
            WaitHandle.WaitAll(doneEvents);
            Console.WriteLine("All calculations are complete.");
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            experiments[threadIndex].run();
            Console.WriteLine("thread {0} finished...", threadIndex);
            doneEvents[threadIndex].Set();
        }
    }
}
