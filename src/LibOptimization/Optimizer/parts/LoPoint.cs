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
    // TODO Change to default(_) if this is not a reference type
    private AbsObjectiveFunction _func;
    private double _evaluatedValue;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <remarks></remarks>
    private LoPoint() {}

    /// <summary>
    /// Create a copy of a point.
    /// </summary>
    /// <param name="point"></param>
    /// <remarks></remarks>
    public LoPoint(LoPoint point)
    {
      _func = point.GetFunc();
      AddRange(point);
      _evaluatedValue = point.Eval;
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
    /// <param name="ai_obj"></param>
    /// <returns></returns>
    /// <remarks>
    /// larger Me than obj is -1. smaller Me than obj is 1.
    /// Equal is return to Zero
    /// </remarks>
    public int CompareTo(object ai_obj)
    {
      // Nothing check
      if (ai_obj == null)
        return 1;

      // Type check
      if (GetType() != ai_obj.GetType())
        throw new ArgumentException("Different type", "obj");

      // Compare
      var mineValue = Eval;
      var compareValue = ((LoPoint) ai_obj).Eval;
      
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
    public double Eval => _evaluatedValue;

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="ai_range">-ai_range to ai_range</param>
    /// <param name="ai_rand">Random object</param>
    /// <remarks></remarks>
    public void InitValue(double ai_range, Random ai_rand)
    {
      for (var i = 0; i <= _func.Dimension() - 1; i++)
      {
        Add(Math.Abs(2.0 * ai_range) * ai_rand.NextDouble() - ai_range);
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
    /// <param name="ai_point"></param>
    /// <remarks></remarks>
    public void CopyFrom(LoPoint ai_point)
    {
      for (var i = 0; i <= ai_point.Count - 1; i++)
      {
        this[i] = ai_point[i];
      }
      _evaluatedValue = ai_point.Eval;
    }
  }
}