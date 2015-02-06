using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeResearch
{
    class Program
    {
        static void Main(string[] args)
        {
            RealtimeResearch research = new RealtimeResearch();
            research.run();

            Console.ReadLine();
        }
    }
}
