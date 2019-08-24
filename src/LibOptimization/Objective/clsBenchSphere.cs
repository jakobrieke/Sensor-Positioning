using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Sphere Function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -5.12 to 5.12
    ///     ''' Referrence:
    ///     ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ///     ''' </remarks>
    public class clsBenchSphere : AbsObjectiveFunction
    {
        private int dimension;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_dim">Set dimension</param>
        ///         ''' <remarks></remarks>
        public clsBenchSphere(int ai_dim)
        {
            dimension = ai_dim;
        }

        /// <summary>
        ///         ''' Target Function
        ///         ''' </summary>
        ///         ''' <param name="ai_var"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override double F(List<double> ai_var)
        {
            if (ai_var == null)
                return 0;

            double ret = 0;
            for (int i = 0; i <= dimension - 1; i++)
                ret += Math.Pow(ai_var[i], 2);
            return ret;
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            List<double> ret = new List<double>();
            for (int i = 0; i <= dimension - 1; i++)
                ret.Add(2.0 * ai_var[i]);
            return ret;
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            List<List<double>> ret = new List<List<double>>();
            for (int i = 0; i <= dimension - 1; i++)
            {
                ret.Add(new List<double>());
                for (int j = 0; j <= dimension - 1; j++)
                {
                    if (i == j)
                        ret[i].Add(2.0);
                    else
                        ret[i].Add(0);
                }
            }
            return ret;
        }

        public override int Dimension()
        {
            return dimension;
        }
    }
}

