using System;
using System.Collections.Generic;
using System.Text;
using LibOptimization.Optimization;
using LOMath;

namespace Util
{
  /// <summary>
  /// common use
  /// </summary>
  /// <remarks></remarks>
  public class clsUtil
  {
    /// <summary>
    /// Normal Distribution
    /// </summary>
    /// <param name="ai_ave">Average</param>
    /// <param name="ai_sigma2">Varianse s^2</param>
    /// <returns></returns>
    /// <remarks>
    /// using Box-Muller method
    /// </remarks>
    public static double NormRand(double ai_ave = 0, double ai_sigma2 = 1)
    {
      double x = XorShiftPRNGSingleton.GetInstance().NextDouble();
      double y = XorShiftPRNGSingleton.GetInstance().NextDouble();

      double c = Math.Sqrt(-2.0 * Math.Log(x));
      if ((0.5 - XorShiftPRNGSingleton.GetInstance().NextDouble() > 0.0))
        return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave;
      return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave;
    }

    /// <summary>
    /// Normal Distribution using Box-Muller method
    /// </summary>
    /// <param name="oRand"></param>
    /// <param name="ai_ave">ai_ave</param>
    /// <param name="ai_sigma2">Varianse s^2</param>
    /// <returns></returns>
    public static double NormRand(Random oRand, double ai_ave = 0,
      double ai_sigma2 = 1)
    {
      double x = oRand.NextDouble();
      double y = oRand.NextDouble();

      double c = Math.Sqrt(-2.0 * Math.Log(x));
      if ((0.5 - XorShiftPRNGSingleton.GetInstance().NextDouble() > 0.0))
        return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave;
      return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave;
    }

    /// <summary>
    /// Cauchy Distribution
    /// </summary>
    /// <param name="ai_mu">default:0</param>
    /// <param name="ai_gamma">default:1</param>
    /// <returns></returns>
    /// <remarks>
    /// http://www.sat.t.u-tokyo.ac.jp/~omi/random_variables_generation.html#Cauchy
    /// </remarks>
    public static double CauchyRand(double ai_mu = 0, double ai_gamma = 1)
    {
      return ai_mu + ai_gamma *
             Math.Tan(Math.PI *
                      (XorShiftPRNGSingleton.GetInstance().NextDouble() -
                       0.5));
    }

    /// <summary>
    /// Generate Random permutation
    /// </summary>
    /// <param name="ai_max">0 to ai_max-1</param>
    /// <returns></returns>
    public static List<int> RandomPermutaion(int ai_max)
    {
      return RandomPermutaion(0, ai_max, new int[] { });
    }

    /// <summary>
    /// Generate Random permutation
    /// </summary>
    /// <param name="ai_max">0 to ai_max-1</param>
    /// <param name="ai_removeIndex">RemoveIndex</param>
    /// <returns></returns>
    public static List<int> RandomPermutaion(int ai_max, int ai_removeIndex)
    {
      return RandomPermutaion(0, ai_max, 
        new[] {ai_removeIndex});
    }

    /// <summary>
    /// Generate Random permutation with range (ai_min to ai_max-1)
    /// </summary>
    /// <param name="ai_min">start value</param>
    /// <param name="ai_max">ai_max-1</param>
    /// <param name="ai_removeIndexArray">remove index array</param>
    /// <returns></returns>
    public static List<int> RandomPermutaion(int ai_min, int ai_max,
      int[] ai_removeIndexArray)
    {
      int nLength = ai_max - ai_min;
      if (nLength == 0 || nLength < 0)
        return new List<int>();

      List<int> ary = new List<int>(Convert.ToInt32(nLength * 1.5));
      if (ai_removeIndexArray == null)
      {
        for (int ii = ai_min; ii <= ai_max - 1; ii++)
          ary.Add(ii);
      }
      else if (ai_removeIndexArray.Length > 0)
      {
        int removeArIndexFrom = 0;
        int removeArIndexTo = ai_removeIndexArray.Length;
        for (int ii = ai_min; ii <= ai_max - 1; ii++)
        {
          if (removeArIndexFrom == removeArIndexTo)
            ary.Add(ii);
          else if (ai_removeIndexArray[removeArIndexFrom] == ii)
            removeArIndexFrom += 1;
          else
            ary.Add(ii);
        }
      }
      else
        for (int ii = ai_min; ii <= ai_max - 1; ii++)
          ary.Add(ii);

      // Fisher–Yates shuffle
      int n = ary.Count;
      while (n > 1)
      {
        n -= 1;
        int k = XorShiftPRNGSingleton.GetInstance().Next(0, n + 1);
        int tmp = ary[k];
        ary[k] = ary[n];
        ary[n] = tmp;
      }

      return ary;
    }

    /// <summary>
    /// Random sort
    /// </summary>
    /// <param name="arPoint"></param>
    public static void RandomizeArray(ref List<LoPoint> arPoint)
    {
      // Fisher–Yates shuffle
      int n = arPoint.Count;
      while (n > 1)
      {
        n -= 1;
        int k = XorShiftPRNGSingleton.GetInstance().Next(0, n + 1);
        var tmp = arPoint[k];
        arPoint[k] = arPoint[n];
        arPoint[n] = tmp;
      }
    }

    /// <summary>
    /// For Debug
    /// </summary>
    /// <param name="ai_opt"></param>
    /// <param name="ai_precision"></param>
    /// <param name="ai_isOutValue"></param>
    /// <param name="ai_isOnlyIterationCount"></param>
    /// <remarks></remarks>
    public static void DebugValue(AbsOptimization ai_opt, int ai_precision = 0,
      bool ai_isOutValue = true, bool ai_isOnlyIterationCount = false)
    {
      if (ai_opt == null) return;

      if (ai_isOnlyIterationCount)
      {
        Console.WriteLine("IterationCount:," + $"{ai_opt.IterationCount}");
        return;
      }

      if (ai_isOutValue)
      {
        Console.WriteLine(
          "TargetFunction:" + ai_opt.ObjectiveFunction.GetType().Name + 
          " Dimension:" + ai_opt.ObjectiveFunction.Dimension());
        Console.WriteLine("OptimizeMethod:" + ai_opt.GetType().Name);
        Console.WriteLine("Eval          :" + $"{ai_opt.Result.Eval}");
        Console.WriteLine("IterationCount:" + $"{ai_opt.IterationCount}");
        Console.WriteLine("Result        :");
        
        var str = new StringBuilder();
        
        foreach (var value in ai_opt.Result)
        {
          str.Append(ai_precision <= 0 ? 
            value.ToString() : value.ToString("F3"));
          str.AppendLine("");
        }

        Console.WriteLine(str.ToString());
      }
      else
        Console.WriteLine("Eval          :" +
                          string.Format("{0}", ai_opt.Result.Eval));
    }

    /// <summary>
    /// For Debug
    /// </summary>
    /// <param name="ai_results"></param>
    /// <remarks></remarks>
    public static void DebugValue(List<LoPoint> ai_results)
    {
      if (ai_results == null || ai_results.Count == 0)
        return;
      for (var i = 0; i <= ai_results.Count - 1; i++)
      {
        Console.WriteLine($"Eval          : {ai_results[i].Eval}");
      }
      Console.WriteLine();
    }

    /// <summary>
    /// Check Criterion
    /// </summary>
    /// <param name="ai_eps">EPS</param>
    /// <param name="ai_comparisonA"></param>
    /// <param name="ai_comparisonB"></param>
    /// <param name="ai_tiny"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static bool IsCriterion(double ai_eps, LoPoint ai_comparisonA,
      LoPoint ai_comparisonB, double ai_tiny = 0.0000000000001)
    {
      return IsCriterion(ai_eps, ai_comparisonA.Eval, ai_comparisonB.Eval,
        ai_tiny);
    }

    /// <summary>
    /// Check Criterion
    /// </summary>
    /// <param name="ai_eps">EPS</param>
    /// <param name="ai_comparisonA"></param>
    /// <param name="ai_comparisonB"></param>
    /// <param name="ai_tiny"></param>
    /// <returns></returns>
    /// <remarks>
    /// Reffrence:
    /// William H. Press, Saul A. Teukolsky, William T. Vetterling, Brian P. Flannery,
    /// "NUMRICAL RECIPIES 3rd Edition: The Art of Scientific Computing", Cambridge University Press 2007, pp505-506
    /// </remarks>
    public static bool IsCriterion(double ai_eps, double ai_comparisonA,
      double ai_comparisonB, double ai_tiny = 0.0000000000001)
    {
      // check division by zero
      var denominator = Math.Abs(ai_comparisonB) + 
                        Math.Abs(ai_comparisonA) + ai_tiny;
      if (denominator == 0) return true;

      // check criterion
      var temp = 2.0 * Math.Abs(ai_comparisonB - ai_comparisonA) / denominator;
      return temp < ai_eps;
    }

    /// <summary>
    /// Random position generator
    /// </summary>
    /// <param name="ai_min"></param>
    /// <param name="ai_max"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static double GenRandomRange(double ai_min, double ai_max)
    {
      var ret = Math.Abs(ai_max - ai_min) *
        XorShiftPRNGSingleton.GetInstance().NextDouble() + ai_min;
      return ret;
    }

    /// <summary>
    /// Random position generator
    /// </summary>
    /// <param name="oRand"></param>
    /// <param name="ai_min"></param>
    /// <param name="ai_max"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static double GenRandomRange(Random oRand, double ai_min,
      double ai_max)
    {
      var ret = Math.Abs(ai_max - ai_min) * oRand.NextDouble() + ai_min;
      return ret;
    }

    /// <summary>
    /// Random position generator(array)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="initPosition"></param>
    /// <param name="lower"></param>
    /// <param name="upper"></param>
    /// <returns></returns>
    public static double[] GenRandomPosition(AbsObjectiveFunction obj,
      double[] initPosition, double lower, double upper)
    {
      var result = new double[obj.Dimension()];
      
      if (initPosition != null && initPosition.Length == obj.Dimension())
      {
        // using InitialPosition
        for (var j = 0; j <= obj.Dimension() - 1; j++)
        {
          result[j] = GenRandomRange(lower, upper) + initPosition[j];
        }
      }
      else
      {
        // Not using InitialPosition
        for (var j = 0; j < obj.Dimension(); j++)
        {
          result[j] = GenRandomRange(lower, upper);
        }
      }

      return result;
    }

    /// <summary>
    /// to csv
    /// </summary>
    /// <param name="arP"></param>
    /// <remarks></remarks>
    public static void ToCSV(LoPoint arP)
    {
      foreach (var p in arP) Console.Write("{0},", p);
      Console.WriteLine("");
    }

    /// <summary>
    /// to csv
    /// </summary>
    /// <param name="arP"></param>
    /// <remarks></remarks>
    public static void ToCSV(IEnumerable<LoPoint> arP)
    {
      foreach (var p in arP) ToCSV(p);
      Console.WriteLine("");
    }

    /// <summary>
    /// eval output for debug
    /// </summary>
    /// <param name="arP"></param>
    /// <remarks></remarks>
    public static void ToEvalList(List<LoPoint> arP)
    {
      foreach (var p in arP)
        Console.WriteLine("{0}", p.Eval);
    }

    /// <summary>
    /// Eval list
    /// </summary>
    /// <param name="arP"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static List<clsEval> GetSortedEvalList(List<LoPoint> arP)
    {
      var sortedEvalList = new List<clsEval>();
      for (var i = 0; i <= arP.Count - 1; i++)
      {
        sortedEvalList.Add(new clsEval(i, arP[i].Eval));
      }
      sortedEvalList.Sort();
      return sortedEvalList;
    }

    /// <summary>
    /// Best clsPoint
    /// </summary>
    /// <param name="ai_points"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoPoint GetBestPoint(List<LoPoint> ai_points,
      bool isCopy = false)
    {
      if (ai_points == null)
        return null /* TODO Change to default(_) if this is not a reference type */;
      if (ai_points.Count == 0)
        return null /* TODO Change to default(_) if this is not a reference type */;
      if (ai_points.Count == 1) return ai_points[0];

      var best = ai_points[0];
      for (var i = 1; i <= ai_points.Count - 1; i++)
      {
        if (best.Eval > ai_points[i].Eval)
          best = ai_points[i];
      }

      return isCopy == false ? best : best.Copy();
    }

    /// <summary>
    /// Find current best index from List(of clsPoint)
    /// </summary>
    /// <param name="ai_points"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static int FindCurrentIndex(List<LoPoint> ai_points)
    {
      var bestIndex = 0;
      var bestEval = ai_points[0].Eval;
      
      for (var i = 0; i <= ai_points.Count - 1; i++)
      {
        if (!(ai_points[i].Eval < bestEval)) continue;
        
        bestEval = ai_points[i].Eval;
        bestIndex = i;
      }

      return bestIndex;
    }

    /// <summary>
    /// Limit solution space
    /// </summary>
    /// <param name="temp"></param>
    /// <param name="LowerBounds"></param>
    /// <param name="UpperBounds"></param>
    /// <remarks></remarks>
    public static void LimitSolutionSpace(LoPoint temp, double[] LowerBounds,
      double[] UpperBounds)
    {
      if (UpperBounds != null && LowerBounds != null)
      {
        for (var ii = 0; ii <= temp.Count - 1; ii++)
        {
          double upper;
          double lower;
          if (UpperBounds[ii] > LowerBounds[ii])
          {
            upper = UpperBounds[ii];
            lower = LowerBounds[ii];
          }
          else if (UpperBounds[ii] < LowerBounds[ii])
          {
            upper = LowerBounds[ii];
            lower = UpperBounds[ii];
          }
          else
            throw new Exception(
              "Error! upper bound and lower bound are equal.");

          if (temp[ii] > lower && temp[ii] < upper)
          {
          }
          else if (temp[ii] > lower)
            temp[ii] = lower;
          else if (temp[ii] < upper)
            temp[ii] = upper;
        }
      }

      temp.ReEvaluate();
    }

    /// <summary>
    /// calc length from each points
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static bool IsExistZeroLength(LoPoint[] points)
    {
      // var isCanCrossover = true;
      LoVector vec;
      for (var i = 0; i <= points.Length - 2; i++)
      {
        vec = points[i] - points[i + 1];
        if (vec.NormL1() == 0) return true;
      }

      vec = points[points.Length - 1] - points[0];
      
      return vec.NormL1() == 0;
    }

    /// <summary>
    /// Overflow check for debug
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static bool CheckOverflow(double v)
    {
      return double.IsInfinity(v) || 
             double.IsNaN(v) || 
             double.IsNegativeInfinity(v) || 
             double.IsPositiveInfinity(v);
    }

    /// <summary>
    /// Overflow check for debug
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool CheckOverflow(LoPoint p)
    {
      foreach (var v in p)
      {
        if (double.IsInfinity(v))
          return true;
        if (double.IsNaN(v))
          return true;
        if (double.IsNegativeInfinity(v))
          return true;
        if (double.IsPositiveInfinity(v))
          return true;
      }

      return false;
    }

    /// <summary>
    /// Overflow check for debug
    /// </summary>
    /// <param name="listP"></param>
    /// <returns></returns>
    public static bool CheckOverflow(IEnumerable<LoPoint> listP)
    {
      foreach (var temp in listP)
      {
        foreach (var v in temp)
        {
          if (double.IsInfinity(v)) return true;
          if (double.IsNaN(v)) return true;
          if (double.IsNegativeInfinity(v)) return true;
          if (double.IsPositiveInfinity(v)) return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Set initial point
    /// </summary>
    /// <param name="pupulation"></param>
    /// <param name="initialPosition"></param>
    public static void SetInitialPoint2(List<LoPoint> pupulation,
      double[] initialPosition)
    {
      if (pupulation == null || pupulation.Count <= 0) return;
      
      var func = pupulation[0].GetFunc();

      if (initialPosition == null ||
          initialPosition.Length != func.Dimension()) return;
      
      var index = Convert.ToInt32(pupulation.Count / (double) 10);
      if (index < 1) index = 1;
      
      for (var i = 0; i <= index - 1; i++)
      {
        for (var j = 0; j <= func.Dimension() - 1; j++)
        {
          pupulation[i][j] = initialPosition[j];
        }
        pupulation[i].ReEvaluate();
      }
    }

    /// <summary>
    /// LibOptimization.Results() to my Matrix class
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public static LoMatrix ToConvertMat(List<LoPoint> results)
    {
      var row = results.Count;
      var col = results[0].Count;
      var ret = new LoMatrix(row, col);
      for (var i = 0; i <= row - 1; i++)
      {
        for (var j = 0; j <= col - 1; j++) ret[i][j] = results[i][j];
      }

      return ret;
    }

    /// <summary>
    /// average
    /// Memo: same -> (List(of double)).Average 
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static double Average(LoVector vec)
    {
      var ret = 0.0;
      foreach (var value in vec) ret += value;
      return ret / vec.Count;
    }
  }
}