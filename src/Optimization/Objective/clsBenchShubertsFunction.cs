using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' ShubertsFunction
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  Fmin = −186.7309
    ///     ''' Range:
    ///     '''  −10≦x1 , x2≦10
    ///     ''' Referrence:
    ///     ''' [1]Test fXin-She Yang, "Test Problems in Optimization", arXiv(http://arxiv.org/abs/1008.0549)
    ///     ''' </remarks>
    public class clsBenchShubertsFunction : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchShubertsFunction()
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
            var x1 = x[0];
            var x2 = x[1];
            double temp1 = (Math.Cos(1 + (1 + 1) * x1) + 2 * Math.Cos(2 + (2 + 1) * x1) + 3 * Math.Cos(3 + (3 + 1) * x1) + 4 * Math.Cos(4 + (4 + 1) * x1) + 5 * Math.Cos(5 + (5 + 1) * x1));
            double temp2 = (Math.Cos(1 + (1 + 1) * x2) + 2 * Math.Cos(2 + (2 + 1) * x2) + 3 * Math.Cos(3 + (3 + 1) * x2) + 4 * Math.Cos(4 + (4 + 1) * x2) + 5 * Math.Cos(5 + (5 + 1) * x2));

            return temp1 * temp2;
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
            return 2;
        }
    }
}

