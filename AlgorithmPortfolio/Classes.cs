using MachineLearningCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmPortfolio
{
    public class DatabaseRecord
    {
        public int AlgorithmID { get; set; }
        public int PopulationGroup { get; set; }
        public bool MaxTrust { get; set; }
        public int RealTag { get; set; }
        public int PredictedTag { get; set; }
        public int MaxvoteTag { get; set; }
        //public double LessPenalties { get; set; }
        public double Pop { get; set; }
        public double Pou { get; set; }
        public int Hour { get; set; }
        public int WeekDay { get; set; }
        public int Section { get; set; }
    }

    public class PortfolioClassificationDataPoint : DataPoint
    {
        public DatabaseRecord DatabaseRecord { get; set; }
    }
}
