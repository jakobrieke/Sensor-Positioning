using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' De Jong’s function 3 (Step Function)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  x = {-5.12~-5, -5.12~-5, -5.12~-5, -5.12~-5, -5.12~-5}
    ///     ''' Range
    ///     '''  -5.12 ~ 5.12 
    ///     ''' Refference:
    ///     '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ///     ''' </remarks>
    public class clsBenchDeJongFunction3 : absObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchDeJongFunction3()
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

            double ret = 0;
            for (int i = 0; i <= 4; i++)
            {
                if ((x[i] >= -5.12) && (x[i] <= 5.12))
                    ret += Math.Floor(x[i]);
                else
                    // out of range
                    ret += Math.Abs(x[i]);// penarty
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
            return 5;
        }
    }
}

