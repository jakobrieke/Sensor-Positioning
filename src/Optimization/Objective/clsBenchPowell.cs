using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
  /// <summary>
  ///     ''' Benchmark function
  ///     ''' Powell function
  ///     ''' </summary>
  ///     ''' <remarks>
  ///     ''' Minimum:
  ///     '''  F(0,0,0,0) = 0
  ///     ''' </remarks>
  public class clsBenchPowell : AbsObjectiveFunction
  {
    /// <summary>
    ///         ''' Default constructor
    ///         ''' </summary>
    ///         ''' <remarks></remarks>
    public clsBenchPowell()
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

      double x1 = ai_var[0];
      double x2 = ai_var[1];
      double x3 = ai_var[2];
      double x4 = ai_var[3];
      double ret = Math.Pow((x1 - 10 * x2), 2) + 5 * Math.Pow((x3 - x4), 2) + Math.Pow((x2 + 2 * x3), 4) + 10 * Math.Pow((x1 - x4), 4);
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
      return 4;
    }
  }
}