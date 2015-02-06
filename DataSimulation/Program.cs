using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            //use Docs\Insering Initial ABM Data\run.m to make the initial queries and execute them on the database
            // you can find the database in the Docs\Insering Initial ABM Data\Database folder

            var ds = new DataSimulation();
            ds.Execute();

        }
    }
}
