using Dissertation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustAnalizer.TagProcessors
{
    class TrustPredictionMethod
    {
        public virtual int ProcessTags(List<UserTagReport> tags) { return 0; }

        public virtual void updateTrustFromObservation(int PredictedTag){ }
    }
}
