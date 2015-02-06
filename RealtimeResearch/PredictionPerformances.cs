using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;

namespace RealtimeResearch
{
    [Cudafy]
    struct PredictionPerformances
    {
        public double occupancyPerformance;
        public double occupancyPerformanceRandom;
        public double trustPerformance;
        public double trustPerformanceRandom;
    }
}
