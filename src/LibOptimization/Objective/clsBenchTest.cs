using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' x1^3 + x2^3 - 0*x1*x2 + 27
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' </remarks>
    public class clsBenchTest : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchTest()
        {
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

            double ret = Math.Pow(ai_var[0], 3) + Math.Pow(ai_var[1], 3) - 9 * ai_var[0] * ai_var[1] + 27;
            return ret;
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            List<double> ret = new List<double>();
            ret.Add(3 * Math.Pow(ai_var[0], 2) - 9 * ai_var[1]);
            ret.Add(3 * Math.Pow(ai_var[1], 2) - 9 * ai_var[0]);
            return ret;
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            List<List<double>> ret = new List<List<double>>();

            ret.Add(new List<double>());
            ret[0].Add(6 * ai_var[0]);
            ret[0].Add(0);
            ret.Add(new List<double>());
            ret[1].Add(0);
            ret[1].Add(6 * ai_var[1]);

            return ret;
        }

        public override int Dimension()
        {
            return 2;
        }
    }

    /// <summary>
    ///     ''' x1^4 - 20*x1^2 + 20*x1 + x2^4 - 20*x2^2 + 20*x2
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' </remarks>
    public class clsBenchTest2 : AbsObjectiveFunction
    {
        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsBenchTest2()
        {
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

            double ret = Math.Pow(ai_var[0], 4) - 20 * Math.Pow(ai_var[0], 2) + 20 * ai_var[0] + Math.Pow(ai_var[1], 4) - 20 * Math.Pow(ai_var[1], 2) + 20 * ai_var[1];
            return ret;
        }

        public override List<double> Gradient(List<double> ai_var)
        {
            List<double> ret = new List<double>();
            ret.Add(4 * Math.Pow(ai_var[0], 3) - 40 * ai_var[0] + 20);
            ret.Add(4 * Math.Pow(ai_var[1], 3) - 40 * ai_var[1] + 20);
            return ret;
        }

        public override List<List<double>> Hessian(List<double> ai_var)
        {
            return null;
        }

        public override int Dimension()
        {
            return 2;
        }
    }
}

