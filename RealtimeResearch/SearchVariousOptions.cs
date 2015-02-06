using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealtimeResearch
{
    class SearchVariousOptions
    {
        private ResearchData data;
        private List<SimOptions> options;
        private List<PredictionPerformances> results;
        private List<double> executionTimes;

        private int _numerOfThreadsNotYetCompleted = 0;
        private ManualResetEvent _doneEvent;


        public SearchVariousOptions()
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
            if (!parallel)
                execute();
            else
                executeParallel();
            analyze(!parallel);
        }

        public void execute() // generate_results
        {
            Stopwatch stopwatch = new Stopwatch();
            SimOptions option;
            Experiment exp;
            for (int i = 0; i < options.Count; i++)
            {
                option = options[i];
                exp = new Experiment(data, option);

                stopwatch.Restart();
                results.Add(exp.execute());
                stopwatch.Stop();
                executionTimes.Add(stopwatch.ElapsedTicks);

                Console.WriteLine("Experiment {0} / {1} took {2} ticks, ({3} milliseconds)", i + 1, options.Count, stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);
            }

        }

        public void executeParallel()
        {
            int nExperiments = options.Count;
            _numerOfThreadsNotYetCompleted = nExperiments;
            _doneEvent = new ManualResetEvent(false);
            
            for (int i = 0; i < nExperiments; i++)
            {
                results.Add(new PredictionPerformances());
                ThreadPool.QueueUserWorkItem(ThreadPoolCallback, i);
            }

            // Wait for all threads in pool to calculate.
            _doneEvent.WaitOne();
            Console.WriteLine("All calculations are complete.");
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            try
            {
                int threadIndex = (int)threadContext;
                Console.WriteLine("thread {0} started...", threadIndex);
                Experiment exp = new Experiment(data, options[threadIndex]);
                results[threadIndex] = exp.execute();
                //experiments[threadIndex].run();
                Console.WriteLine("thread {0} finished...", threadIndex);
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                    _doneEvent.Set();
            }
        }

        private void analyze(Boolean saveExecutionTimes)
        {
            string json;
            json = JsonConvert.SerializeObject(results);
            using (StreamWriter writer = new StreamWriter(@"jsons/results.json"))
            {
                writer.Write(json);
            }

            if (saveExecutionTimes)
            {
                json = JsonConvert.SerializeObject(executionTimes);
                using (StreamWriter writer = new StreamWriter(@"jsons/executionTimes.json"))
                {
                    writer.Write(json);
                }
            }

            double best = 0;
            foreach (PredictionPerformances result in results)
                if (best < result.occupancyPerformance)
                    best = result.occupancyPerformance;
            Console.WriteLine("Best Occupancy Performance is " + best);

            best = 0;
            foreach (PredictionPerformances result in results)
                if (best < result.trustPerformance)
                    best = result.trustPerformance;
            Console.WriteLine("Best Trust Performance is " + best);
        }


        
    }
}
