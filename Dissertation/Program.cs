using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation
{
    class Program
    {
        static void Main(string[] args)
        {
            var myDissertationCode = new DissertationMain(RunInParallel:false);
            myDissertationCode.run();


            //double[] arr = new double[] {0, 0, 1, 1, 1, 0, 0, 4, 6, 3, 3, 2};
            //Matrix1 a = new Matrix1(arr);
            //Matrix1 a_sorted = a.sort();
            //Matrix1 a_unique = a.unique();

            Console.WriteLine("Program executed successfully  ...");
            Console.ReadLine();
        }
    }
}
