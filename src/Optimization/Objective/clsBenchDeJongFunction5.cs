using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' De Jong’s function 5
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  x = {-32,-32}
    ///     '''  f(x) ~ 1
    ///     ''' Range
    ///     '''  -65.536 ~ 65.536
    ///     ''' Refference:
    ///     '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ///     ''' </remarks>
    public class clsBenchDeJongFunction5 : AbsObjectiveFunction
    {
        private readonly double[][] a = new[] { new double[] { -32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32 }, new double[] { -32, -32, -32, -32, -32, -16, -16, -16, -16, -16, 0, 0, 0, 0, 0, 16, 16, 16, 16, 16, 32, 32, 32, 32, 32 } };

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchDeJongFunction5()
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

            // Correc value
            // Taken from : http://jp.mathworks.com/help/gads/example-minimizing-de-jongs-fifth-function.html
            // x = {-16.1292, -15.8214}
            // f(x) = 6.9034

            // range check
            if ((x[0] >= -65.536) && (x[0] <= 65.536))
            {
                if ((x[1] >= -65.536) && (x[1] <= 65.536))
                {
                    double ret = 1 / (double)500;
                    for (int j = 0; j <= 24; j++)
                        ret += 1 / (j + 1 + Math.Pow((x[0] - a[0][j]), 6) + Math.Pow((x[1] - a[1][j]), 6));

                    return 1 / ret;
                }
            }

            // out of range
            return Math.Abs(x[0]) + Math.Abs(x[1]) + 1000;
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

