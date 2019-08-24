using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Schaffer function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -100 to 100
    ///     ''' Referrence:
    ///     ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ///     ''' </remarks>
    public class clsBenchSchaffer : AbsObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchSchaffer(int ai_dim)
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
            for (int i = 0; i <= this.dimension - 2; i++)
                ret += (Math.Pow((Math.Pow(ai_var[i], 2) + Math.Pow(ai_var[i + 1], 2)), 0.25)) * (Math.Pow(Math.Sin(50 * Math.Pow(((Math.Pow(ai_var[i], 2) + Math.Pow(ai_var[i + 1], 2))), 0.1)), 2) + 1.0);

            return ret;
        }

        public new double F(double[] ai_var)
        {
            return this.F(new List<double>(ai_var));
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            throw new NotImplementedException();
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            throw new NotImplementedException();
        }

        public override int Dimension()
        {
            return this.dimension;
        }
    }
}

