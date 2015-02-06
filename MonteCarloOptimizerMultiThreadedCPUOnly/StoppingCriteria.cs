using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class StoppingCriteria
    {
        public static string    NUMBER_OF_TRIALS = "numOfGen";
        public static string    MINIMUM_PERFORMANCE = "minPerf";
        public static float     STOPPING_CRITERIA_VALUE;


        public StoppingCriteria(string _condition, float _val)
        {
            Condition = _condition;
            Value = _val;
        }

        public string Condition { get; set; }

        public float Value { get; set; }
    }
}
