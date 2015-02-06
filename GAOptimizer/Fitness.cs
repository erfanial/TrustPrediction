using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cudafy;

namespace GeneticAlgorithm
{
    class Fitness
    {
        public static int nInputVars = 5;

        [Cudafy]
        public static float fitness(float[,] groundTruth, float[] userTrusts, UserUpdate[,,] updates, FitnessParameter fitnessParams)
        {
            return Experiment.execute(groundTruth, userTrusts, updates, fitnessParams);
        }
    }
}
