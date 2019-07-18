using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' De Jong’s function 4 (qudratic with gauss Function)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  x = {0, ..., 0}
    ///     ''' Range
    ///     '''  -1.28 ~ 1.28
    ///     ''' Refference:
    ///     '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ///     ''' </remarks>
    public class clsBenchDeJongFunction4 : absObjectiveFunction
    {
        private List<double> normRand = new List<double>();

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchDeJongFunction4()
        {
            for (int i = 0; i <= 29; i++)
                this.normRand.Add(Util.clsUtil.NormRand());
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
            for (int i = 0; i <= 29; i++)
            {
                if ((x[i] >= -1.28) && (x[i] <= 1.28))
                    ret += (i + 1) * x[i] * x[i] * x[i] * x[i] + this.normRand[i];
                else
                    // out of range
                    ret += Math.Abs(i);// penarty
            }

            return ret; // ??? 
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
            return 30;
        }
    }
}

