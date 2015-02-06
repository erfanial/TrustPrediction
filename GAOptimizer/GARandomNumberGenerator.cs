using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GARandomNumberGenerator
    {
        private static Random rand = new Random();

        public GARandomNumberGenerator()
        {

        }

        public static double getNextDouble()
        {
            return rand.NextDouble();
        }
    }
}
