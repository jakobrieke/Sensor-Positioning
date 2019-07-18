using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
  /// <summary>
  ///     ''' Benchmark function
  ///     ''' Booth Function
  ///     ''' </summary>
  ///     ''' <remarks>
  ///     ''' Minimum:
  ///     '''  F(1,3) = 0
  ///     ''' Range:
  ///     '''  -10 to 10
  ///     ''' Referrence:
  ///     ''' http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page816.htm
  ///     ''' </remarks>
  public class clsBenchBoothFunction : absObjectiveFunction
  {
    /// <summary>
    ///         ''' Default constructor
    ///         ''' </summary>
    ///         ''' <remarks></remarks>
    public clsBenchBoothFunction()
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

      return Math.Pow((x[0] + 2 * x[1] - 7), 2) + Math.Pow((2 * x[0] + x[1] - 5), 2);
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