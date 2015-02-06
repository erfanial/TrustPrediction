using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GenomeOperations
    {
        //private static float[] dnaToInputVars(string dna)
        //{
        //    byte[] bytes = new byte[dna.Length * sizeof(char)];
        //    System.Buffer.BlockCopy(dna.ToCharArray(), 0, bytes, 0, bytes.Length);
        //    float[] ins = new float[Fitness.nInputVars];
        //    for (int i = 0; i < Fitness.nInputVars; i++)
        //        ins[i] = BitConverter.ToSingle(bytes, 4 * i);
        //    return ins;
        //}

        //public static float[] bits2floats(bool[] bits)
        //{
        //    //bits = new int[] { 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1 };
        //    int s = 0;
        //    byte[] bytes = new byte[Fitness.nInputVars * 4];
        //    float[] ins = new float[Fitness.nInputVars];
        //    for (int b = 0; b < bytes.Length; b++)
        //    {
        //        s = 0;
        //        for (int j = 0; j < 8; j++)
        //            if (bits[b * 8 + j])
        //                s += (int)Math.Pow(2, (float)(8 - j - 1));
        //        bytes[b] = (byte)s;
        //    }
        //    for (int i = 0; i < ins.Length; i++)
        //    {
        //        ins[i] = BitConverter.ToSingle(bytes, 4 * i);
        //        if (float.IsNaN(ins[i]))
        //            ins[i] = 0;
        //    }
        //    return ins;
        //}

        //private static byte[] GetBytes(string str)
        //{
        //    byte[] bytes = new byte[str.Length * sizeof(char)];
        //    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        //    return bytes;
        //}

        //private static string GetString(byte[] bytes)
        //{
        //    char[] chars = new char[bytes.Length / sizeof(char)];
        //    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        //    return new string(chars);
        //}

        //private static bool[] decodeDNA(string encoded_dna)
        //{
        //    int s = 0;
        //    byte[] bytes = new byte[Fitness.nInputVars * 4];
        //    StringBuilder sb = new StringBuilder();
        //    for (int b = 0; b < bytes.Length; b++)
        //    {
        //        s = 0;
        //        for (int j = 0; j < 8; j++)
        //            if (dna[b * 8 + j])
        //                s += (int)Math.Pow(2, (float)(8 - j - 1));
        //        bytes[b] = (byte)s;
        //    }
        //    for (int i = 0; i < bytes.Length; i++)
        //        sb.Append(BitConverter.ToString(bytes, 4 * i));
        //    return sb.ToString();
        //}
    }
}
