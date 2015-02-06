using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Program
    {
        private static GA geneticAlgorithm;
        static void Main(string[] args)
        {
            StoppingCriteria criteria = new StoppingCriteria(StoppingCriteria.NUMBER_OF_GENERATIONS, 1000000);
            GAOptions options = new GAOptions(10, (float)0.001, criteria);
            geneticAlgorithm = new GA(options);

            while(geneticAlgorithm.continueExecution){
                geneticAlgorithm.run();

                Console.WriteLine("Generation: " + geneticAlgorithm.updateInformation["GenerationNumber"]);
                Console.WriteLine("Max Fitness: " + geneticAlgorithm.updateInformation["MaxPopulationFitness"]);
                for (int i = 0; i < Fitness.nInputVars; i++)
                    Console.WriteLine("Var"+(i+1)+": " + geneticAlgorithm.updateInformation["Var" + (i + 1)]);
                Console.WriteLine("-----");
            }
            Console.WriteLine("Genetic Algorithm has finished");
            Console.ReadLine();
        }
    }
}
