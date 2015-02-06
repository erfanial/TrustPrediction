using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation
{
    class LinearCorrelation
    {
        private double[] x;
        private double[] y;

        private double prob;
        private double r;

        public LinearCorrelation(double[] firstVector, double[] secondVector)
        {
            x = firstVector;
            y = secondVector;
        }

        public double ComputeSpearmanRankCorrelation(double[] X, double[] Y)
        {
            int iFirst = 0, iAfterLast = X.Length - 1;
            int n = iAfterLast - iFirst;
            List<TSpearmanHelper> List = new List<TSpearmanHelper>(n);
            for (int i = n - 1; i >= 0; i--)
            {
                TSpearmanHelper r = new TSpearmanHelper();
                r.X = X[iFirst + i];
                r.Y = Y[iFirst + i];
                r.RankByX = 0;
                r.RankByY = 0;
                List.Add(r);
            }
            TSpearmanHelper[] ByXList = List.OrderBy(r => r.X).ToArray();
            for (int i = n - 1; i >= 0; i--) ByXList[i].RankByX = i + 1;
            TSpearmanHelper[] ByYList = List.OrderBy(r => r.Y).ToArray();
            for (int i = n - 1; i >= 0; i--) ByYList[i].RankByY = i + 1;
            long SumRankDiff = List.Aggregate((long)0, (total, r) => total += lsqr(r.RankByX - r.RankByY));
            double RankCorrelation = 1 - (double)(6 * SumRankDiff) / (n * ((long)n * n - 1));
            return RankCorrelation;
        }

        private long lsqr(long d) { return d * d; }

        public void GetPearson(ref double pcc)
        {
            //will regularize the unusual case of complete correlation
            const double TINY = 1.0e-20;
            int j, n = x.Length;
            Double yt, xt, t, df;
            Double syy = 0.0, sxy = 0.0, sxx = 0.0, ay = 0.0, ax = 0.0;
            for (j = 0; j < n; j++)
            {
                //finds the mean
                ax += x[j];
                ay += y[j];
            }
            ax /= n;
            ay /= n;
            for (j = 0; j < n; j++)
            {
                // compute correlation coefficient
                xt = x[j] - ax;
                yt = y[j] - ay;
                sxx += xt * xt;
                syy += yt * yt;
                sxy += xt * yt;
            }
            r = sxy / (Math.Sqrt(sxx * syy) + TINY);
            //for a large n
            prob = erfcc(Math.Abs(r * Math.Sqrt(n - 1.0)) / 1.4142136);

            pcc = r;
        }

        private Double erfcc(Double func_input)
        {
            Double t, z, ans;
            z = Math.Abs(func_input);
            t = 1.0 / (1.0 + 0.5 * z);
            ans = t * Math.Exp(-z * z - 1.26551223 + t * (1.00002368 +
                                   t * (0.37409196 + t * (0.09678418 +
                                  t * (-0.18628806 + t * (0.27886807 +
                                  t * (-1.13520398 + t * (1.48851587 +
                             t * (-0.82215223 + t * 0.17087277)))))))));
            return func_input >= 0.0 ? ans : 2.0 - ans;
        }
    }


    public class TSpearmanHelper
    {
        public double X, Y;
        public int RankByX, RankByY;
    }
}
