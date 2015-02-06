using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GA
    {
        private GAOptions options;
        private Population population;
        public Dictionary<string, string> updateInformation;

        private int generation = 0;
        private int nStalls = 0;
        private float previousMaxFitness = -float.PositiveInfinity;
        public bool continueExecution = true;
        
        public GA(GAOptions ga_options)
        {
            options = ga_options;
            init();
        }

        private void init()
        {
            population = new Population(options, false);
            updateInformation = new Dictionary<string, string>();

        }

        public void run()
        {
            Stopwatch calculationTime = new Stopwatch();
            
            if (continueExecution)
            {
                generation++;

                calculationTime.Start();
                population.runGeneration();
                calculationTime.Stop();
                
                updateInformation["GenerationNumber"] = generation.ToString();
                updateInformation["MaxPopulationFitness"] = population.maxFitness.ToString();
                float[] vars = population.bestIndividual.DNA;
                for (int i = 0; i < Fitness.nInputVars; i++)
                    updateInformation["Var" + (i + 1)] = vars[i].ToString();
                float customFitness = population.bestIndividual.fitness;

                if (generation == 1)
                {
                    previousMaxFitness = population.maxFitness;
                }
                else
                {
                    if (previousMaxFitness == population.maxFitness)
                    {
                        nStalls++;
                    }
                    else
                    {
                        nStalls = 0;
                        previousMaxFitness = population.maxFitness;
                    }
                }


                if (options.StopCriteria.Condition == StoppingCriteria.NUMBER_OF_GENERATIONS && generation >= options.StopCriteria.Value)
                    continueExecution = false;
                if (options.StopCriteria.Condition == StoppingCriteria.MINIMUM_FITNESS && population.maxFitness >= options.StopCriteria.Value)
                    continueExecution = false;
                if (options.StopCriteria.Condition == StoppingCriteria.NUMBER_OF_STALLS && nStalls >= options.StopCriteria.Value)
                    continueExecution = false;

                calculationTime.Restart();
                if (continueExecution)
                    population.evolve();
                else
                    population.relaseGpuMemory();
                calculationTime.Stop();
            }
        }

        private float[] bits2floats(bool[] bits)
        {
            //bits = new int[] { 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1 };
            int s = 0;
            byte[] bytes = new byte[Fitness.nInputVars * 4];
            float[] ins = new float[Fitness.nInputVars];
            for (int b = 0; b < bytes.Length; b++)
            {
                s = 0;
                for (int j = 0; j < 8; j++)
                    if (bits[b * 8 + j])
                        s += (int)Math.Pow(2, (double)(8 - j - 1));
                bytes[b] = (byte)s;
            }
            for (int i = 0; i < ins.Length; i++)
                ins[i] = BitConverter.ToSingle(bytes, 4 * i);
            return ins;
        }
        
    }
}
