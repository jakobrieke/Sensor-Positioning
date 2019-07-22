using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' De Jong’s function 2 (2D Rosenblock Function)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(1, 1) = 0
    ///     ''' Range
    ///     '''  -2.048 ~ 2.048
    ///     ''' Refference:
    ///     '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ///     ''' </remarks>
    public class clsBenchDeJongFunction2 : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchDeJongFunction2()
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

            if ((x[0] >= -2.048) && (x[0] <= 2.048))
            {
                if ((x[1] >= -2.048) && (x[1] <= 2.048))
                    return 100 * (Math.Pow((Math.Pow(x[0], 2) - x[1]), 2)) + Math.Pow((1 - x[0]), 2);
            }

            // out of range
            return Math.Abs(x[0]) * Math.Abs(x[1]) * 100; // penarty
        }

        public override List<double> Gradient(List<double> x)
        {
            throw new NotImplementedException();
        }

        public override List<List<double>> Hessian(List<double> x)
        {
            throw new NotImplementedException();
        }

        public override int Dimension()
        {
            return 2;
        }
    }
}

