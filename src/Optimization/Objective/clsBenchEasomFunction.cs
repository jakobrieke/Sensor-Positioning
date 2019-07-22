using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Easom Function
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(pi, pi) = -1
    ///     ''' Range:
    ///     '''  −100≦x1 , x2≦100
    ///     ''' Referrence:
    ///     ''' [1]Test fXin-She Yang, "Test Problems in Optimization", arXiv(http://arxiv.org/abs/1008.0549)
    ///     ''' </remarks>
    public class clsBenchEasomFunction : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchEasomFunction()
        {
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

            double x1 = x[0];
            double x2 = x[1];
            double ret = -Math.Cos(x1) * Math.Cos(x2) * Math.Exp(-(Math.Pow((x1 - Math.PI), 2) + Math.Pow((x2 - Math.PI), 2)));
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

        public override int Dimension()
        {
            return 2;
        }
    }
}

