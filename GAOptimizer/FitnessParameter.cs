using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;
using System.Runtime.InteropServices;

namespace GeneticAlgorithm
{
    [Cudafy]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FitnessParameter
    {
        public float var1;
        public float var2;
        public float var3;
        public float var4;
        public float var5;
        public const int nSections = 16;
        public const int nHours = 168;
        public const int nIterations = nHours * 12;
        public const int nUsers = 21944;

        //public fixed float predictedSectionOccupancy[nSections * nHours];
        //public fixed float predictedUsersTrust[nUsers];
        //public fixed float predictedUsersScore[nUsers];

        public FitnessParameter(float variable1, float variable2, float variable3, float variable4, float variable5)
        {
            var1 = variable1;
            var2 = variable2;
            var3 = variable3;
            var4 = variable4;
            var5 = variable5;
        }

        public FitnessParameter(float[] variables)
        {
            var1 = variables[0];
            var2 = variables[1];
            var3 = variables[2];
            var4 = variables[3];
            var5 = variables[4];
        }
    }
}
