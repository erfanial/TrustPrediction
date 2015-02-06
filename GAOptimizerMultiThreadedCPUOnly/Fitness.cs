using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Fitness
    {
        public static int nInputVars = 5;
        private ResearchData _researchData;
        private FitnessParameter _fitnessParams;

        public Fitness(ResearchData researchData, FitnessParameter fitnessParams)
        {
            _researchData = researchData;
            _fitnessParams = fitnessParams;
        }

        public float calculate()
        {
            Experiment exp = new Experiment(_researchData, new SimOptions(1, _fitnessParams.var1, _fitnessParams.var2, _fitnessParams.var3, _fitnessParams.var4, _fitnessParams.var5));
            PredictionPerformances performance = exp.execute();

            return performance.occupancyPerformance;
        }
    }
}
