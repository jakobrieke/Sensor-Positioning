using System;
using System.Collections.Generic;
using LOMath;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// Point Class
  /// </summary>
  /// <remarks></remarks>
  public class LoPoint : LoVector, IComparable
  {
    // TODO Change to default(_) if this is not a reference type
    private AbsObjectiveFunction m_func;

    private double m_evaluateValue;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <remarks></remarks>
    private LoPoint() {}

    /// <summary>
    /// copy constructor
    /// </summary>
    /// <param name="ai_vertex"></param>
    /// <remarks></remarks>
    public LoPoint(LoPoint ai_vertex)
    {
      m_func = ai_vertex.GetFunc();
      AddRange(ai_vertex);
      m_evaluateValue = ai_vertex.Eval;
    }

    /// <summary>
    /// Create a point at (0, 0, 0, ..., 0) with the same dimension as ai_func.
    /// </summary>
    /// <param name="ai_func"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction ai_func)
    {
      m_func = ai_func;
      AddRange(new double[ai_func.NumberOfVariable()]);
      m_evaluateValue = m_func.F(this);
    }

    /// <summary>
    /// Create a point from a given position.
    /// </summary>
    /// <param name="ai_func"></param>
    /// <param name="ai_vars"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction ai_func, 
      IReadOnlyCollection<double> ai_vars)
    {
      m_func = ai_func;
      AddRange(ai_vars);
      m_evaluateValue = m_func.F(this);
    }

    /// <summary>
    /// Create a point from a given position.
    /// </summary>
    /// <param name="ai_func"></param>
    /// <param name="ai_vars"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction ai_func, IEnumerable<double> ai_vars)
    {
      m_func = ai_func;
      AddRange(ai_vars);
      m_evaluateValue = m_func.F(this);
    }

    /// <summary>
    /// Create a point at (0, 0, 0, ..., 0) with a specified dimensionality.
    /// </summary>
    /// <param name="ai_func"></param>
    /// <param name="ai_dim"></param>
    /// <remarks></remarks>
    public LoPoint(AbsObjectiveFunction ai_func, int ai_dim)
    {
      m_func = ai_func;
      AddRange(new double[ai_dim]);
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
      m_evaluateValue = m_func.F(this);
    }

    /// <summary>
    /// Get Function
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public AbsObjectiveFunction GetFunc()
    {
      return m_func;
    }

    /// <summary>
    /// EvaluateValue
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public double Eval => m_evaluateValue;

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="ai_range">-ai_range to ai_range</param>
    /// <param name="ai_rand">Random object</param>
    /// <remarks></remarks>
    public void InitValue(double ai_range, Random ai_rand)
    {
      for (var i = 0; i <= m_func.NumberOfVariable() - 1; i++)
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
      m_evaluateValue = ai_point.Eval;
    }
  }
}