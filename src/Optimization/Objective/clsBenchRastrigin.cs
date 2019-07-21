using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Rastrigin function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' Range:
    ///     '''  -5.12 to 5.12
    ///     ''' Referrence:
    ///     ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ///     ''' </remarks>
    public class clsBenchRastrigin : AbsObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchRastrigin(int ai_dim)
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

            double A = 10.0;
            double ret = 0.0;
            for (int i = 0; i <= this.dimension - 1; i++)
                ret += Math.Pow(ai_var[i], 2) - A * Math.Cos(2.0 * Math.PI * ai_var[i]);
            ret += A * this.dimension;

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

        public override int NumberOfVariable()
        {
            return this.dimension;
        }
    }
}

