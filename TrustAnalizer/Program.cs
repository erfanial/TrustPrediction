using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustAnalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            AnalizeSimulatedTrust analizer = new AnalizeSimulatedTrust();

            analizer.analyze();

            Console.WriteLine("Complete...");
            Console.ReadLine();
        }
    }
}
