using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dissertation;

namespace TrustPredictionRealData
{
    class Program
    {
        static void Main(string[] args)
        {
            var transfer = new RealDataTransfer();
            transfer.TransferDataFromKparkServerToLocalMsSql();

            var KparkRealtimeAlgorithmAnalysis = new ExamineKparkRealtimeAlgorithm();
            KparkRealtimeAlgorithmAnalysis.OptimizeFusingOptions();

            //var myDissertationCode = new DissertationMain(RunInParallel: false, useRealData:true);
            //myDissertationCode.run();
            
            Console.WriteLine("Program executed successfully  ...");
            Console.ReadLine();
        }
    }
}
