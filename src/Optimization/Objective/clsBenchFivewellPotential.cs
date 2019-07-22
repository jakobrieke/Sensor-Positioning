using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Fivewell-Potential
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(4.92, -9.89) = -1.4616
    ///     ''' Range:
    ///     '''  -20 to 20
    ///     ''' Referrence:
    ///     ''' Ilya Pavlyukevich, "Levy flights, non-local search and simulated annealing", Journal of Computational Physics 226 (2007) 1830-1844
    ///     ''' </remarks>
    public class clsBenchFivewellPotential : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchFivewellPotential()
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

            if (this.Dimension() != x.Count)
                return 0;

            double ret = 0.0;
            ret = 1 - 1 / (1 + 0.05 * (Math.Pow(x[0], 2) + Math.Pow((x[1] - 10), 2))) - 1 / (1 + 0.05 * Math.Pow(((x[0] - 10)), 2) + Math.Pow(x[1], 2)) - 1.5 / (1 + 0.03 * Math.Pow(((x[0] + 10)), 2) + Math.Pow(x[1], 2)) - 2 / (1 + 0.05 * Math.Pow(((x[0] - 5)), 2) + Math.Pow((x[1] + 10), 2)) - 1 / (1 + 0.1 * Math.Pow(((x[0] + 5)), 2) + Math.Pow((x[1] + 10), 2));
            ret = ret * (1 + 0.0001 * Math.Pow((Math.Pow(x[0], 2) + Math.Pow(x[1], 2)), 1.2));
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

