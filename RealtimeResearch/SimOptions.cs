using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeResearch
{
    class SimOptions
    {
        public double I;
        public double lambda_promote;
        public double lambda_punish;
        public double certainty_coeff;
        public double score_coeff;
        public double decay;

        public SimOptions(double val_I = 1, double val_lambda_promote = 1, double val_lambda_punish = 1.1, double val_certainty_coeff = 5, double val_score_coeff = 25, double val_decay = 0.0005)
        {
            I = val_I;
            lambda_promote = val_lambda_promote;
            lambda_punish = val_lambda_punish;
            certainty_coeff = val_certainty_coeff;
            score_coeff = val_score_coeff;
            decay = val_decay;
        }
    }

    
}
