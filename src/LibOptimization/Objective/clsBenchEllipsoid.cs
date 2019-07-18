using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Ellipsoid function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -5.12 to 5.12
    ///     ''' Referrence:
    ///     ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ///     ''' </remarks>
    public class clsBenchEllipsoid : absObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_dim">Set dimension</param>
        ///         ''' <remarks></remarks>
        public clsBenchEllipsoid(int ai_dim)
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

            double ret = 0.0;
            for (int i = 0; i <= this.dimension - 1; i++)
            {
                double temp = (Math.Pow(1000, (i / (double)(this.dimension - 1)))) * ai_var[i];
                ret += Math.Pow(temp, 2);
            }

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

