using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Griewank function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -512 to 512
    ///     ''' Referrence:
    ///     ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ///     ''' </remarks>
    public class clsBenchGriewank : AbsObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchGriewank(int ai_dim)
        {
            this.dimension = ai_dim;
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

            if (this.dimension != ai_var.Count)
                return 0;

            double a = 0.0;
            double b = 0.0;
            for (int i = 0; i <= this.dimension - 1; i++)
            {
                a += (Math.Pow(ai_var[i], 2));
                b *= Math.Cos(ai_var[i] / Math.Sqrt(i + 1));
            }
            double ret = a / 4000.0 - b + 1;

            return ret;
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            throw new NotImplementedException();
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            throw new NotImplementedException();
        }

        public override int NumberOfVariable()
        {
            return this.dimension;
        }
    }
}

