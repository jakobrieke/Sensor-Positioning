using System;
using System.Collections.Generic;
using LoMath;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// Point Class
  /// </summary>
  /// <remarks></remarks>
  public class LoPoint : LoVector, IComparable
  {
    private AbsObjectiveFunction _func;
    private double _evaluatedValue;

    public LoPoint(AbsObjectiveFunction func, IReadOnlyCollection<double> vars,
      double value)
    {
      _func = func;
      _evaluatedValue = value;
      AddRange(vars);
    }

    /// <summary>
    /// Create a copy of a point.
    /// </summary>
    /// <param name="point"></param>
    /// <remarks></remarks>
    public LoPoint(LoPoint point)
    {
      _func = point.GetFunc();
      AddRange(point);
      _evaluatedValue = point.Value;
    }

    /// <summary>
    /// Create a point at (0, 0, 0, ..., 0) with the same dimension as func.
    /// </summary>
    /// <param name="func"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction func)
    {
      _func = func;
      AddRange(new double[func.Dimension()]);
      _evaluatedValue = _func.F(this);
    }

    /// <summary>
    /// Create a point from a given position.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="vars"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction func, 
      IReadOnlyCollection<double> vars)
    {
      _func = func;
      AddRange(vars);
      _evaluatedValue = _func.F(this);
    }

    /// <summary>
    /// Create a point from a given position.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="vars"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction func, IEnumerable<double> vars)
    {
      _func = func;
      AddRange(vars);
      _evaluatedValue = _func.F(this);
    }

    /// <summary>
    /// Create a point at (0, 0, 0, ..., 0) with a specified dimensionality.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="dimension"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction func, int dimension)
    {
      _func = func;
      AddRange(new double[dimension]);
    }

    /// <summary>
    /// Compare(ICompareble)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <remarks>
    /// larger Me than obj is -1. smaller Me than obj is 1.
    /// Equal is return to Zero
    /// </remarks>
    public int CompareTo(object obj)
    {
      // Nothing check
      if (obj == null)
        return 1;

      // Type check
      if (GetType() != obj.GetType())
        throw new ArgumentException("Different type", "obj");

      // Compare
      var mineValue = Value;
      var compareValue = ((LoPoint) obj).Value;
      
      if (mineValue < compareValue) return -1;
      
      return mineValue > compareValue ? 1 : 0;
    }

    /// <summary>
    /// Re Evaluate
    /// </summary>
    /// <remarks></remarks>
    public void ReEvaluate()
    {
      _evaluatedValue = _func.F(this);
    }

    /// <summary>
    /// Get Function
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public AbsObjectiveFunction GetFunc()
    {
      return _func;
    }

    /// <summary>
    /// EvaluateValue
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public double Value => _evaluatedValue;

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="range">-ai_range to ai_range</param>
    /// <param name="rand">Random object</param>
    /// <remarks></remarks>
    public void InitValue(double range, Random rand)
    {
      for (var i = 0; i <= _func.Dimension() - 1; i++)
      {
        Add(Math.Abs(2.0 * range) * rand.NextDouble() - range);
      }
    }

    /// <summary>
    /// Copy clsPoint
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public LoPoint Copy()
    {
      return new LoPoint(this);
    }

    /// <summary>
    /// Copy clsPoint
    /// </summary>
    /// <param name="point"></param>
    /// <remarks></remarks>
    public void CopyFrom(LoPoint point)
    {
      for (var i = 0; i <= point.Count - 1; i++)
      {
        this[i] = point[i];
      }
      _evaluatedValue = point.Value;
    }
  }
}