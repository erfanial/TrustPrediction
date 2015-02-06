using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class Program
    {
        static void Main(string[] args)
        {
            StoppingCriteria criteria = new StoppingCriteria(StoppingCriteria.MINIMUM_PERFORMANCE, 0.95f);
            var mc = new MonteCarlo(criteria);
            mc.run();
            
            Console.ReadLine();
        }
    }
}
