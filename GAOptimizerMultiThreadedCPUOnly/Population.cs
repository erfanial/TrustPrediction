using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace GeneticAlgorithm
{
    class Population
    {
        private GAOptions options;
        private Individual[] population;
        public Individual bestIndividual;
        private float[] fitnesses;
        private Fitness[] fitnessObjects;
        private int bestIndex = 0;

        public float maxFitness = -float.PositiveInfinity;
        public float minFitness = float.PositiveInfinity;
        private ResearchData researchData;
        private ResearchData[] researchDatas;
        private FitnessParameter[] fitnessParams;
        private int _numerOfThreadsNotYetCompleted;
        private ManualResetEvent _doneEvent;
        
        public Population(GAOptions GeneticAlgorithmOptions)
        {
            options = GeneticAlgorithmOptions;

            init();
        }

        private void init()
        {
            Console.WriteLine("Initializing...");
            population = new Individual[options.NumberOfIndividuals];
            for (int i = 0; i < options.NumberOfIndividuals; i++)
                population[i] = new Individual();


            string sampleGenome = "";
            for (int j = 0; j < Fitness.nInputVars; j++)
                sampleGenome += "ABCD";

            Console.WriteLine("Loading Data...");
            researchData = new ResearchData();
            researchDatas = new ResearchData[population.Length];
            for (int i = 0; i < population.Length; i++)
                researchDatas[i] = researchData;
            fitnessParams = new FitnessParameter[options.NumberOfIndividuals];
        }

        public void runGeneration()
        {
            int i, j;
            float f, maxf = 0;
            float[] vars;

            for (i = 0; i < population.Length; i++)
                fitnessParams[i] = new FitnessParameter(population[i].DNA);

            
            

            Stopwatch calculationTime = new Stopwatch();
            calculationTime.Start();
            calculatePopulationFitnessParallel();
            calculationTime.Stop();

            for (i = 0; i < population.Length; i++)
                population[i].fitness = fitnesses[i];

            maxFitness = -float.PositiveInfinity;

            for (i = 0; i < population.Length; i++)
            {
                if (fitnesses[i] > maxFitness)
                {
                    bestIndex = i;
                    maxFitness = fitnesses[i];
                }
            }
            bestIndividual = population[bestIndex]; // clone();
            return;
        }

        

        public void evolve()
        {
            int i, j;
            //Individual[] children = new Individual[population.Length];
            
            // all population should reproduce
            Individual mom, dad, child1, child2;
            bool[] momDNA, dadDNA, child1DNA, child2DNA;
            for (i = 0; i <= population.Length-1; i += 2)
            {
                mom = population[i]; // clone();
                dad = population[i + 1]; // clone();
                momDNA = mom.binaryDNA();
                dadDNA = dad.binaryDNA();

                ////////////////////////// crossover and mutation
                int crossoverPosition1,crossoverPosition2,temp;
                while(true)
                {
                    crossoverPosition1 = (int)(GARandomNumberGenerator.getNextfloat() * momDNA.Length);
                    crossoverPosition2 = (int)(GARandomNumberGenerator.getNextfloat() * momDNA.Length);
                    if(crossoverPosition1 != crossoverPosition2)
                        break;
                }
                if(crossoverPosition1 > crossoverPosition2){
                    temp = crossoverPosition2;
                    crossoverPosition2 = crossoverPosition1;
                    crossoverPosition1 = temp;
                }

                child1DNA = new bool[momDNA.Length];
                child2DNA = new bool[momDNA.Length];
                for (j = 0; j < momDNA.Length; j++)
                {
                    // crossover
                    if (j >= crossoverPosition1 && j <= crossoverPosition2)
                    {
                        child1DNA[j] = dadDNA[j];
                        child2DNA[j] = momDNA[j];
                    }
                    else
                    {
                        child1DNA[j] = momDNA[j];
                        child2DNA[j] = dadDNA[j];
                    }

                    // mutation
                    if (GARandomNumberGenerator.getNextfloat() < options.MutationProbability)
                        child1DNA[j] = !child1DNA[j];
                    if (GARandomNumberGenerator.getNextfloat() < options.MutationProbability)
                        child2DNA[j] = !child2DNA[j];
                }
                child1 = new Individual(child1DNA);
                child2 = new Individual(child2DNA);
                //children[i] = child1;
                //children[i+1] = child2;
                population[i] = child1;
                population[i + 1] = child2;
                /////////////////////////////////////
            }

            // insert the children into the population except the best individual
            //population = children;
            population[bestIndex] = bestIndividual; // clone();

            // scramble population;
            int[] scrambledIndexes = new int[population.Length];
            for (i = 0; i < population.Length; i++)
                scrambledIndexes[i] = i;
            scrambledIndexes = ShuffleArray(scrambledIndexes);
            Individual[] p = new Individual[population.Length];
            for (i = 0; i < population.Length; i++)
                p[i] = population[scrambledIndexes[i]];
            population = p;
            return;
        }

        private int[] ShuffleArray(int[] array)
        {
            Random r = new Random();
            for (int i = array.Length; i > 0; i--)
            {
                int j = r.Next(i);
                int k = array[j];
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            return array;
        }





        //// most time consuming, can be run in parallel
        //private float[] calculatePopulationFitness()
        //{
        //    for (int i = 0; i < fitnessParams.Length; i++)
        //        fs[i] = Fitness.fitness(researchData, fitnessParams[i]);

        //    return fs;
        //}


        // distribute processing on threads
        public void calculatePopulationFitnessParallel()
        {
            int nExperiments = population.Length;
            _numerOfThreadsNotYetCompleted = nExperiments;
            _doneEvent = new ManualResetEvent(false);
            fitnessObjects = new Fitness[nExperiments];
            fitnesses = new float[nExperiments];

            for (int i = 0; i < nExperiments; i++)
            {
                fitnessObjects[i] = new Fitness(researchDatas[i], fitnessParams[i]);
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
                fitnesses[threadIndex] = fitnessObjects[threadIndex].calculate();
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                    _doneEvent.Set();
            }
        }
    }
}
