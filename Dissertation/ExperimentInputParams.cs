using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation
{
    public class ExperimentInputParams
    {
        public bool[] discounted = new bool[] { false, true }; // 
        public bool[] maxTrust = new bool[] { false, true }; //
        
        
        //public double[] percentOfParticipants = new double[] { 0.01, 0.25, 0.5, 0.75, 1 }; // 
        //public double[] percentOfUpdatesUsed = new double[] { 0.01,	0.05,	0.1,	0.15,	0.2,	0.25,	0.3,	0.35,	0.4,	0.45,	0.5,	0.55,	0.6,	0.65,	0.7,	0.75,	0.8,	0.85,	0.9,	0.95,	1 };
        //public double[] percentOfParticipants = new double[] { 1, 0.75, 0.5, 0.25, 0.01 };
        //public double[] percentOfUpdatesUsed = new double[] { 1	, 0.95,	0.9,	0.85,	0.8,	0.75,	0.7,	0.65,	0.6,	0.55,	0.5,	0.45,	0.4,	0.35,	0.3,	0.25,	0.2,	0.15,	0.1,	0.05,	0.01, };
        public double[] percentOfParticipants = new double[] { 1, 0.25, 0.01 };
        //public double[] percentOfUpdatesUsed = new double[] { 1, 0.5, 0.01, };
        public double[] percentOfUpdatesUsed = new double[] { 1, 0.9, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.1, 0.01, };
        


        public double[] percentOfTraining = new double[] { 0,0.3,0.6,0.9 }; // 

        public double[] tagOptions = new double[] { 1, 2, 3}; // should be positive integers starting from 1
        public double maxTagOptions;

        public double[][] PenaltyMatrix = new double[][] { new double[] { 0, 5, 10 }, new double[] { 1, 0, 4 }, new double[] { 20, 10, 0 }}; // [realTag][predictedTag]

        public List<double> possibleTrusts; // initialized by ExperimentExecutor

        public ExperimentInputParams()
        {
            possibleTrusts = new List<double>(99);
            for (int i = 0; i < possibleTrusts.Capacity; i++)
                possibleTrusts.Add( (i + 1.0) / 100.0 );

            maxTagOptions = tagOptions.Max();
        }
    }
}
