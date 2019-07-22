using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' De Jong’s function 1 (Sphere Function)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0, 0, 0) = 0
    ///     ''' Range
    ///     '''  -5.12 ~ 5.12 
    ///     ''' Refference:
    ///     '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ///     ''' </remarks>
    public class clsBenchDeJongFunction1 : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchDeJongFunction1()
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
            for (int i = 0; i <= 2; i++)
            {
                if ((x[i] >= -5.12) && (x[i] <= 5.12))
                    ret += Math.Pow(x[i], 2);
                else
                    // out of range
                    ret += Math.Abs(x[i]) * 2;// penarty
            }
            return ret;
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            List<double> ret = new List<double>();
            for (int i = 0; i <= 2; i++)
                ret.Add(2.0 * ai_var[i]);
            return ret;
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            List<List<double>> ret = new List<List<double>>();
            for (int i = 0; i <= 2; i++)
            {
                ret.Add(new List<double>());
                for (int j = 0; j <= 2; j++)
                {
                    if (i == j)
                        ret[i].Add(2.0);
                    else
                        ret[i].Add(0);
                }
            }
            return ret;
        }

        public override int Dimension()
        {
            return 3;
        }
    }
}

