using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class MonteCarlo
    {
        private StoppingCriteria _stoppingCriteria;
        private bool time2stop = false;
        private ResearchData _researchData;
        private PredictionPerformances bestPerformance;
        private SimOptions bestOption;
        private float bestPerformanceNumber = float.MinValue;
        private Random rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);

        private const int trialGroupSize = 1000;
        private SimOptions[] options;
        private PredictionPerformances[] performances;
        private Experiment[] experiments;

        private int _numerOfThreadsNotYetCompleted;
        private ManualResetEvent _doneEvent;
            

        public MonteCarlo(StoppingCriteria stoppingCriteria)
        {
            _stoppingCriteria = stoppingCriteria;
            init();
        }

        private void init()
        {
            _researchData = new ResearchData();
            options = new SimOptions[trialGroupSize];
            performances = new PredictionPerformances[trialGroupSize];
            experiments = new Experiment[trialGroupSize];
            bestOption = new SimOptions();
        }

        public void run()
        {
            int nTrials = 0;
            while (!time2stop)
            {

                initOptions();
                runTrialsParallel();
                findBestOption();

                //PredictionPerformances trialPerformance = runTrial(out options);
                //if (trialPerformance.occupancyPerformance > bestPerformanceNumber)
                //{
                //    bestPerformance = trialPerformance;
                //    bestPerformanceNumber = trialPerformance.trustPerformance;
                //    bestOptions = options;
                //}

                nTrials += trialGroupSize;
                Console.WriteLine(nTrials + " : %" + (bestPerformanceNumber*100) + " : " + bestOption.I + " " + bestOption.lambda_promote + " " + bestOption.lambda_punish + " " + bestOption.certainty_coeff + " " + bestOption.score_coeff + " " + bestOption.decay);

                if (_stoppingCriteria.Condition == StoppingCriteria.MINIMUM_PERFORMANCE && bestPerformanceNumber >= _stoppingCriteria.Value)
                    time2stop = true;

                if (_stoppingCriteria.Condition == StoppingCriteria.NUMBER_OF_TRIALS && nTrials >= _stoppingCriteria.Value)
                    time2stop = true;
            }
        }

        private void initOptions()
        {
            float[,] optionRange = SimOptions.optionRange;
            int numberOfVars = (int)optionRange.GetLength(0);
            float[] optionsValues = new float[numberOfVars];
            for (int o = 0; o < trialGroupSize; o++)
            {
                for (int i = 0; i < numberOfVars; i++)
                    optionsValues[i] = (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (optionRange[i, 1] - optionRange[i, 0]) + optionRange[i, 0];
                options[o] = new SimOptions(optionsValues[0], optionsValues[1], optionsValues[2], optionsValues[3], optionsValues[4], optionsValues[5]);
            }
        }

        private void runTrialsParallel()
        {
            _numerOfThreadsNotYetCompleted = trialGroupSize;
            _doneEvent = new ManualResetEvent(false);
            for (int e = 0; e < trialGroupSize; e++)
            {
                experiments[e] = new Experiment(_researchData, options[e]);
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
            float p = 0;
            for (int e = 0; e < trialGroupSize; e++)
            {
                p = performances[e].occupancyPerformance;
                if (p > bestPerformanceNumber)
                {
                    bestPerformanceNumber = p;
                    bestPerformance = performances[e];
                    bestOption = options[e];
                }
            }
        }

        //private PredictionPerformances runTrial(out SimOptions outputSim)
        //{
        //    SimOptions options;
        //    float[,] optionRange = SimOptions.optionRange;
        //    float[] optionsValues = new float[(int)optionRange.GetLength(0)];
        //    for (int i = 0; i < optionRange.GetLength(0); i++)
        //        optionsValues[i] = (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (float)rand.NextDouble() * (optionRange[i, 1] - optionRange[i, 0]) + optionRange[i, 0];
        //    options = new SimOptions(optionsValues[0], optionsValues[1], optionsValues[2], optionsValues[3], optionsValues[4], optionsValues[5]);
        //    outputSim = options;

        //    PredictionPerformances performance = (new Experiment(_researchData, options)).execute();
        //    return performance;
        //}

    }
}
