using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Newtonsoft.Json;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using System.Diagnostics;

namespace GeneticAlgorithm
{
    class Population
    {
        private GAOptions options;
        private Individual[] population;
        public Individual bestIndividual;
        private float[] fitnesses;
        private int bestIndex = 0;

        public float maxFitness = -float.PositiveInfinity;
        public float minFitness = float.PositiveInfinity;
        private GPGPU gpu;
        private float[] dev_fitnesses;
        private float[] fs;
        private FitnessParameter[] dev_fitnessParams;
        private FitnessParameter[] fitnessParams;
        private bool runInParallelGpu = false;
        private ResearchData researchData;
        private float[,] dev_groundTruth;
        private float[] dev_userTrust;
        private UserUpdate[, ,] dev_updates;
        //private FitnessData fitnessData;
        
        public Population(GAOptions GeneticAlgorithmOptions, bool runOnGpu = false)
        {
            options = GeneticAlgorithmOptions;
            
            runInParallelGpu = runOnGpu;

            init();
            if (runInParallelGpu)
                initGPU();
        }

        private void init()
        {
            population = new Individual[options.NumberOfIndividuals];
            for (int i = 0; i < options.NumberOfIndividuals; i++)
                population[i] = new Individual();


            string sampleGenome = "";
            for (int j = 0; j < Fitness.nInputVars; j++)
                sampleGenome += "ABCD";

            // Declare some arrays like normal
            fs = new float[options.NumberOfIndividuals];
            fitnessParams = new FitnessParameter[options.NumberOfIndividuals];
            researchData = new ResearchData();
            //fitnessData = new FitnessData(researchData.GroundTruth, researchData.UserTrusts, researchData.Updates, researchData.nSections);
            
        }

        private void initGPU()
        {
            // Translate all members with the Cudafy attribute in the given type to CUDA and compile.
            CudafyModule km = CudafyTranslator.Cudafy(typeof(Population), typeof(UserUpdate), typeof(Fitness), typeof(FitnessParameter), typeof(PredictionPerformances), typeof(Experiment), typeof(SimOptions));

            // Get the first CUDA device and load the module generated above.
            gpu = CudafyHost.GetDevice(CudafyModes.Target, 0);
            gpu.LoadModule(km);

            // Allocate the memory on the GPU of same size as specified arrays
            dev_fitnesses = gpu.Allocate<float>(fs);
            dev_fitnessParams = gpu.Allocate<FitnessParameter>(options.NumberOfIndividuals);
            dev_groundTruth = gpu.CopyToDevice(researchData.GroundTruth);
            dev_userTrust = gpu.CopyToDevice(researchData.UserTrusts);
            dev_updates = gpu.CopyToDevice(researchData.Updates);
            
            
            //FitnessData dev_fitnessData = gpu.CopyToDevice(fitnessData);
            
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
            if(!runInParallelGpu)
                fitnesses = calculatePopulationFitness();
            else
                fitnesses = calculatePopulationFitnessGPU();
            calculationTime.Stop();

            for (i = 0; i < population.Length; i++)
                population[i].fitness = fitnesses[i];

            maxFitness = fitnesses.Max();
            minFitness = fitnesses.Min();

            for (i = 0; i < population.Length; i++)
            {
                if (fitnesses[i] == maxFitness)
                {
                    bestIndex = i;
                    break;
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
                    crossoverPosition1 = (int)(GARandomNumberGenerator.getNextDouble() * momDNA.Length);
                    crossoverPosition2 = (int)(GARandomNumberGenerator.getNextDouble() * momDNA.Length);
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
                    if (GARandomNumberGenerator.getNextDouble() < options.MutationProbability)
                        child1DNA[j] = !child1DNA[j];
                    if (GARandomNumberGenerator.getNextDouble() < options.MutationProbability)
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
            population[0] = bestIndividual; // clone();

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





        // most time consuming, can be run in parallel
        private float[] calculatePopulationFitness()
        {
            for (int i = 0; i < fitnessParams.Length; i++)
                fs[i] = Fitness.fitness(researchData.GroundTruth, researchData.UserTrusts, researchData.Updates, fitnessParams[i]);

            return fs;
        }






        ///// parallel
        public float[] calculatePopulationFitnessGPU()
        {

            // Copy the arrays 'a' and 'b' to the GPU
            gpu.CopyToDevice(fs, dev_fitnesses);
            gpu.CopyToDevice(fitnessParams, dev_fitnessParams);
            

            gpu.Launch((int)Math.Pow(2, 16) - 1, (int)Math.Pow(2, 10)).calculateFitnessOnDevice(dev_fitnesses, dev_groundTruth, dev_userTrust, dev_updates, dev_fitnessParams);

            // Copy the array 'c' back from the GPU to the CPU
            gpu.CopyFromDevice(dev_fitnesses, fs);

            return fs;
        }

        public void relaseGpuMemory()
        {
            if (runInParallelGpu)
            {
                //free the memory allocated on the GPU
                gpu.FreeAll();
            }
        }


        [Cudafy]
        public static void calculateFitnessOnDevice(GThread thread, float[] dev_fitnesses, float[,] groundTruth, float[] userTrusts, UserUpdate[, ,] updates, FitnessParameter[] dev_fitnessParams)
        {
            int tid = thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x;

            if (tid < dev_fitnesses.Length)
                dev_fitnesses[tid] = Fitness.fitness(groundTruth, userTrusts, updates, dev_fitnessParams[tid]);
        }
        ////

    }
}
