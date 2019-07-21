using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Ackley's function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -32.768 to 32.768
    ///     ''' Referrence:
    ///     ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ///     ''' </remarks>
    public class clsBenchAckley : AbsObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchAckley(int ai_dim)
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
                a += Math.Pow(ai_var[i], 2);
                b += Math.Cos(2 * Math.PI * ai_var[i]);
            }
            double ret = 20 - 20 * Math.Exp(-0.2 * Math.Sqrt((1 / (double)this.dimension) * a)) + Math.E - Math.Exp((1 / (double)this.dimension) * b);

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

