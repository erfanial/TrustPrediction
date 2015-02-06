using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class SimOptions
    {
        public float I;
        public float lambda_promote;
        public float lambda_punish;
        public float certainty_coeff;
        public float score_coeff;
        public float decay;
        public float[] decayOfInteration;

        public static float[,] optionRange = new float[,] { {1,1}, {1,1}, {1,5}, {1,10}, {1,1000}, {0,0.1f} };

        public SimOptions(float val_I = 1, float val_lambda_promote = 1, float val_lambda_punish = (float)1.1, float val_certainty_coeff = 5, float val_score_coeff = 25, float val_decay = (float)0.0005)
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
