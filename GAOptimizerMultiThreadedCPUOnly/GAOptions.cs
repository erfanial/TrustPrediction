using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GAOptions
    {
        public int NumberOfIndividuals { get; set; }

        public float MutationProbability { get; set; }

        public StoppingCriteria StopCriteria { get; set; }

        public GAOptions(int numberOfIndividuals, float mutationProbability, StoppingCriteria stoppingCriteria)
        {
            NumberOfIndividuals = numberOfIndividuals;
            MutationProbability = mutationProbability;
            StopCriteria = stoppingCriteria;
        }
        
    }
}
