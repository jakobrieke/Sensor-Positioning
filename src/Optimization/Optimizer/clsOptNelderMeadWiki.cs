using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// Nelder Mead Method Wikipedia ver
  /// </summary>
  /// <remarks>
  /// Features:
  ///  -Derivative free optimization algorithm.
  ///  -Also known as "Down hill simplex" or "simplex method".
  /// 
  /// Reference:
  /// http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95
  /// 
  /// Implement:
  /// N.Tomi(tomi.nori+github at gmail.com)
  /// </remarks>
  public class ClsOptNelderMeadWiki : AbsOptimization
  {
    /// <summary>Max iteration count(Default:5,000)</summary>
    public override int Iteration { get; set; } = 5000;

    /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
    public double EPS { get; set; } = 0.000001;

    // -------------------------------------------------------------------
    // Coefficient of Simplex Operator
    // -------------------------------------------------------------------
    /// <summary>Refrection coeffcient(default:1.0)</summary>
    public readonly double Refrection = 1.0;

    /// <summary>Expantion coeffcient(default:2.0)</summary>
    public readonly double Expantion = 2.0;

    /// <summary>Contraction coeffcient(default:-0.5)</summary>
    public readonly double Contraction = -0.5;

    /// <summary>Shrink coeffcient(default:0.5)</summary>
    public readonly double Shrink = 0.5;

    private List<LoPoint> m_points = new List<LoPoint>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_func">Optimize Function</param>
    /// <remarks></remarks>
    public ClsOptNelderMeadWiki(AbsObjectiveFunction ai_func)
    {
      _func = ai_func;
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <remarks>
    /// All vertexs are made at random.
    /// </remarks>
    public override void Init()
    {
      try
      {
        // init meber varibles
        _error.Clear();
        _iteration = 0;
        m_points.Clear();

        // initial position
        double[][] tempSimplex = null;
        tempSimplex = new double[_func.NumberOfVariable() + 1][];
        
        for (var i = 0; i <= tempSimplex.Length - 1; i++)
        {
          tempSimplex[i] = clsUtil.GenRandomPosition(_func,
            InitialPosition, InitialValueRangeLower,
            InitialValueRangeUpper);
        }

        Init(tempSimplex);
      }
      catch (Exception ex)
      {
        _error.SetError(true, clsError.ErrorType.ERR_INIT);
      }
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="initPoint"></param>
    /// <remarks>
    /// Set simplex
    /// </remarks>
    public void Init(double[][] initPoint)
    {
      try
      {
        // init meber varibles
        _error.Clear();
        _iteration = 0;
        m_points.Clear();

        // Check number of variable
        if (_func.NumberOfVariable() < 2)
        {
          _error.SetError(true, clsError.ErrorType.ERR_INIT);
          return;
        }

        // Check Simplex
        // MEMO:Target function variable is the same as vertex of simplex.
        if (initPoint.Length != (_func.NumberOfVariable() + 1))
        {
          _error.SetError(true, clsError.ErrorType.ERR_INIT);
          return;
        }

        // Generate vertex
        for (int i = 0; i <= _func.NumberOfVariable(); i++)
          m_points.Add(new LoPoint(_func,
            new List<double>(initPoint[i])));

        // Sort Evaluate
        m_points.Sort();
      }
      catch (Exception ex)
      {
        _error.SetError(true, clsError.ErrorType.ERR_INIT);
      }
    }

    /// <summary>
    /// Do optimization
    /// </summary>
    /// <param name="iteration">Iteration count. When you set zero,
    /// use the default value.</param>
    /// <returns>
    /// True: Stopping Criterion.
    /// False: Do not Stopping Criterion
    /// </returns>
    /// <remarks></remarks>
    public override bool DoIteration(int iteration = 0)
    {
      // Check Last Error
      if (IsRecentError())
        return true;

      // Do Iterate
      if (Iteration <= _iteration)
        return true;
      iteration = iteration == 0
        ? Iteration - _iteration - 1
        : Math.Min(iteration, Iteration - _iteration) - 1;
      for (var iterate = 0; iterate <= iteration; iterate++)
      {
        // Counting generation
        _iteration += 1;

        // Check criterion
        m_points.Sort();
        if (clsUtil.IsCriterion(EPS, m_points[0].Eval,
          m_points[m_points.Count - 1].Eval))
          return true;

        // -----------------------------------------------------
        // The following is optimization by Nelder-Mead Method.
        // -----------------------------------------------------
        // Calc centroid
        var centroid = GetCentroid(m_points);

        // Reflection
        var refrection =
          ModifySimplex(WorstPoint, centroid, Refrection);
        if (BestPoint.Eval <= refrection.Eval &&
            refrection.Eval < Worst2ndPoint.Eval)
          WorstPoint = refrection;
        else if (refrection.Eval < BestPoint.Eval)
        {
          var expansion =
            ModifySimplex(WorstPoint, centroid, Expantion);

          WorstPoint = expansion.Eval < refrection.Eval ? 
            expansion : refrection;
        }
        else
        {
          // Contraction
          var contraction =
            ModifySimplex(WorstPoint, centroid, Contraction);
          if (contraction.Eval < WorstPoint.Eval)
            WorstPoint = contraction;
          else
            // Reduction(Shrink) BestPoint以外を縮小
            CalcShrink(Shrink);
        }
      }

      return false;
    }

    /// <summary>
    /// Best result
    /// </summary>
    /// <returns>Best point class</returns>
    /// <remarks></remarks>
    public override LoPoint Result
    {
      get { return clsUtil.GetBestPoint(m_points, true); }
    }

    /// <summary>
    /// Get recent error infomation
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public clsError.clsErrorInfomation GetLastErrorInfomation()
    {
      return _error.GetLastErrorInfomation();
    }

    /// <summary>
    /// Get recent error
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool IsRecentError()
    {
      return _error.IsError();
    }

    /// <summary>
    /// All Result
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// for Debug
    /// </remarks>
    public override List<LoPoint> Results
    {
      get { return m_points; }
    }

    /// <summary>
    /// Calc Centroid
    /// </summary>
    /// <param name="ai_vertexs"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private LoPoint GetCentroid(List<LoPoint> ai_vertexs)
    {
      var ret = new LoPoint(ai_vertexs[0]);

      var numVar = ai_vertexs[0].Count;
      for (var i = 0; i <= numVar - 1; i++)
      {
        var temp = 0.0;
        for (var numVertex = 0;
          numVertex < ai_vertexs.Count - 1; // Except Worst
          numVertex++)
        {
          temp += ai_vertexs[numVertex][i];
        }
        
        ret[i] = temp / (ai_vertexs.Count - 1);
      }

      ret.ReEvaluate();
      return ret;
    }

    /// <summary>
    /// Simplex
    /// </summary>
    /// <param name="tgt">Target vertex</param>
    /// <param name="base">Base vertex</param>
    /// <param name="coeff">Coeffcient</param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    private LoPoint ModifySimplex(LoPoint tgt, LoPoint @base, 
      double coeff)
    {
      var ret = new LoPoint(_func);
      for (var i = 0; i <= ret.Count - 1; i++)
      {
        var temp = @base[i] + coeff * (@base[i] - tgt[i]);
        ret[i] = temp;
      }

      ret.ReEvaluate();
      return ret;
    }

    /// <summary>
    /// Shrink(Except best point)
    /// </summary>
    /// <param name="coeff">Shrink coefficient</param>
    /// <remarks>
    /// </remarks>
    private void CalcShrink(double coeff)
    {
      for (var i = 1; i <= m_points.Count - 1; i++) // expect BestPoint
      {
        for (var j = 0; j <= m_points[0].Count - 1; j++)
        {
          var temp = BestPoint[j] +
                     coeff * (m_points[i][j] - BestPoint[j]);
          m_points[i][j] = temp;
        }

        m_points[i].ReEvaluate();
      }
    }

    private LoPoint BestPoint
    {
      get => m_points[0];
      set => m_points[0] = value;
    }

    private LoPoint WorstPoint
    {
      get => m_points[m_points.Count - 1];
      set => m_points[m_points.Count - 1] = value;
    }

    private LoPoint Worst2ndPoint
    {
      get => m_points[m_points.Count - 2];
      set => m_points[m_points.Count - 2] = value;
    }
  }
}