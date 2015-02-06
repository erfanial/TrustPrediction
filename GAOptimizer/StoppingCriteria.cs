using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class StoppingCriteria
    {
        public static string    NUMBER_OF_GENERATIONS = "numOfGen";
        public static string    MINIMUM_FITNESS = "minFit";
        public static string    NUMBER_OF_STALLS = "numOfStall";
        public static double    STOPPING_CRITERIA_VALUE;


        public StoppingCriteria(string _condition, double _val)
        {
            Condition = _condition;
            Value = _val;

        }

        public string Condition { get; set; }

        public double Value { get; set; }
    }
}
