using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Schwefel function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(420.96875,...,420.96875) = 0
    ///     ''' Range:
    ///     '''  -512 to 512
    ///     ''' Referrence:
    ///     ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ///     ''' </remarks>
    public class clsBenchSchwefel : absObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchSchwefel(int ai_dim)
        {
            this.dimension = ai_dim;
        }

        /// <summary>
        ///         ''' Target Function
        ///         ''' </summary>
        ///         ''' <param name="x"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override double F(List<double> x)
        {
            if (x == null)
                return 0;

            if (this.dimension != x.Count)
                return 0;

            double ret = 0.0;
            for (int i = 0; i <= this.dimension - 1; i++)
            {
                if ((x[i] >= -512) && (x[i] <= 512))
                    ret += -x[i] * Math.Sin(Math.Sqrt(Math.Abs(x[i])));
                else
                    // out of range
                    ret += Math.Abs(x[i]);
            }
            ret = ret + 418.982887272434 * this.dimension;

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

