using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;

namespace GeneticAlgorithm
{
    [Cudafy]
    public struct PredictionPerformances
    {
        public float occupancyPerformance;
        public float occupancyPerformanceRandom;
        public float trustPerformance;
        public float trustPerformanceRandom;
    }
}
