using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Individual
    {
        public float[] DNA;
        public float fitness;

        public Individual(bool[] booleanDNA = null)
        {
            DNA = new float[Fitness.nInputVars];
            initDNA(booleanDNA);
        }

        private void initDNA(bool[] booleanDNA)
        {
            float initialGene,a,b,var;
            bool[] bits;
            int nVars = Fitness.nInputVars;
            if (booleanDNA == null)
            {
                for (int i = 0; i < nVars; i++)
                {
                    var = (float)GARandomNumberGenerator.getNextDouble();
                    a = (float)-Math.Pow(10, 1);
                    b = (float)Math.Pow(10, 1);
                    initialGene = (b - a) * var + a;
                    DNA[i] = initialGene;

                    //bits = float2bits(initialGene);
                    //for (int j = 0; j < 32; j++)
                    //    DNA[i * 32 + j] = bits[j];
                }
            }
            else
            {
                DNA = bits2floats(booleanDNA);
            }

        }

        private bool[] float2bits(float v)
        {
            byte[] bytes;
            int b;
            bool[] bits = new bool[32];
            bytes = BitConverter.GetBytes(v);
            // swap order
            for (int i = 0; i < bytes.Length; i++)
            {
                //b = (int)bytes[bytes.Length - i - 1];
                b = (int)bytes[i];
                for (int j = 0; j < 8; j++)
                {
                    bits[7 - j + i * 8] = (b - 2 * (int)(b / 2)) == 1?true:false;
                    b = (int)(b / 2);
                }
            }
            // end swap order
            return bits;
        }

        public bool[] binaryDNA()
        {
            int nVars = Fitness.nInputVars;
            bool[] fullBits = new bool[nVars * 32];
            bool[] bits;
            for (int i = 0; i < nVars; i++)
            {
                bits = float2bits(DNA[i]);
                for (int j = 0; j < 32; j++)
                    fullBits[i * 32 + j] = bits[j];
            }
            return fullBits;
        }

        public static float[] bits2floats(bool[] bits)
        {
            //bits = new int[] { 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1 };
            int s = 0;
            byte[] bytes = new byte[Fitness.nInputVars * 4];
            float[] ins = new float[Fitness.nInputVars];
            for (int b = 0; b < bytes.Length; b++)
            {
                s = 0;
                for (int j = 0; j < 8; j++)
                    if (bits[b * 8 + j])
                        s += (int)Math.Pow(2, (double)(8 - j - 1));
                bytes[b] = (byte)s;
            }
            for (int i = 0; i < ins.Length; i++)
            {
                ins[i] = BitConverter.ToSingle(bytes, 4 * i);
                if (float.IsNaN(ins[i]))
                    ins[i] = 0;
            }
            return ins;
        }











        //public int[] getDNA(bool syncWithGenesFirst = false)
        //{
        //    if (syncWithGenesFirst)
        //        syncDNABasedOnGenes();
        //    return DNA;
        //}

        //public float[] getGenes(bool syncWithDNAFirst = false)
        //{
        //    if (syncWithDNAFirst)
        //        syncGenesBasedOnDNA();
        //    return Genes;
        //}




        //public void syncGenesBasedOnDNA()
        //{
        //    Genes = bits2floats(DNA);
        //}

        //public void syncDNABasedOnGenes()
        //{
        //    int[] bits;
        //    for (int i = 0; i < Genes.Length; i++)
        //    {
        //        bits = float2bits(Genes[i]);

        //        for (int j = 0; j < 32; j++)
        //            DNA[i * 32 + j] = bits[j];
        //    }
        //}





        

    }
}
