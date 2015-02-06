using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class MonteCarloIterationSpecific
    {
        private StoppingCriteria _stoppingCriteria;
        private bool _runInParallel;
        private bool time2stop = false;
        private ResearchData _researchData;
        private PredictionPerformances bestPerformance;
        private SimOptions bestOption;
        private float bestPerformanceNumber = float.MinValue;
        private Random rand = new Random();

        private const int trialGroupSize = 1000;
        private SimOptions[] options;
        private PredictionPerformances[] performances;
        private ExperimentIterationSpecific[] experiments;

        private int _numerOfThreadsNotYetCompleted;
        private ManualResetEvent _doneEvent;


        public MonteCarloIterationSpecific(StoppingCriteria stoppingCriteria, bool runInParallel = false)
        {
            _stoppingCriteria = stoppingCriteria;
            _runInParallel = runInParallel;
            init();
        }

        private void init()
        {
            _researchData = new ResearchData();
            options = new SimOptions[trialGroupSize];
            performances = new PredictionPerformances[trialGroupSize];
            experiments = new ExperimentIterationSpecific[trialGroupSize];
            bestOption = new SimOptions();
        }

        public void run()
        {
            int nTrials = 0;
            float currentPerformanceNumber = float.NegativeInfinity;
            while (!time2stop)
            {
                initOptions();
                runTrialsParallel();
                findBestOption();

                if (currentPerformanceNumber < bestPerformanceNumber)
                {
                    currentPerformanceNumber = bestPerformanceNumber;
                    WriteBest(bestOption);
                }

                nTrials += trialGroupSize;
                Console.WriteLine(nTrials + " : %" + (bestPerformanceNumber * 100));

                if (_stoppingCriteria.Condition == StoppingCriteria.MINIMUM_PERFORMANCE && bestPerformanceNumber >= _stoppingCriteria.Value)
                    time2stop = true;

                if (_stoppingCriteria.Condition == StoppingCriteria.NUMBER_OF_TRIALS && nTrials >= _stoppingCriteria.Value)
                    time2stop = true;
            }

        }

        public static void WriteBest(SimOptions option)
        {
            using (FileStream fs = File.Open(@"bestOption.json", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, option);
            }
        }

        private void initOptions()
        {
            int nIterations = _researchData.Updates[0].Count;

            float[,] optionRange = SimOptions.optionRange;
            int numberOfVars = (int)optionRange.GetLength(0);
            float[] optionsValues = new float[numberOfVars];
            SimOptions currentOption;
            for (int o = 0; o < trialGroupSize; o++)
            {
                for (int i = 0; i < numberOfVars; i++)
                    optionsValues[i] = (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (optionRange[i, 1] - optionRange[i, 0]) + optionRange[i, 0];
                currentOption = new SimOptions(optionsValues[0], optionsValues[1], optionsValues[2], optionsValues[3], optionsValues[4], optionsValues[5]);
                //currentOption = new SimOptions(1, 1, 4.410256f, 1.1148f, 1.014489f);

                currentOption.decayOfInteration = new float[nIterations];
                for (int i = 0; i < nIterations; i++)
                    currentOption.decayOfInteration[i] = (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (optionRange[5, 1] - optionRange[5, 0]) + optionRange[5, 0];
                options[o] = currentOption;
            }
        }

        private void runTrialsParallel()
        {
            _numerOfThreadsNotYetCompleted = trialGroupSize;
            _doneEvent = new ManualResetEvent(false);
            for (int e = 0; e < trialGroupSize; e++)
            {
                experiments[e] = new ExperimentIterationSpecific(_researchData, options[e]);
                ThreadPool.QueueUserWorkItem(ThreadPoolCallback, e);
            }
            _doneEvent.WaitOne();
        }

        private void ThreadPoolCallback(Object threadContext)
        {
            try
            {
                int threadIndex = (int)threadContext;
                performances[threadIndex] = experiments[threadIndex].execute();
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                    _doneEvent.Set();
            }
        }

        private void findBestOption()
        {
            for (int e = 0; e < trialGroupSize; e++)
            {
                if (performances[e].occupancyPerformance > bestPerformanceNumber)
                {
                    bestPerformanceNumber = performances[e].occupancyPerformance;
                    bestPerformance = performances[e];
                    bestOption = options[e];
                }
            }
        }

    }
}
