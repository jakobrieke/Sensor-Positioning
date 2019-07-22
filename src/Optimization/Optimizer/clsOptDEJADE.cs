using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// Adaptive Differential Evolution Algorithm JADE
  /// </summary>
  /// <remarks>
  /// Features:
  /// - Derivative free optimization algorithm
  /// - Similar to GA algorithm
  /// 
  /// Reference:
  /// [1] Z.-H. Zhan, J. Zhang, Y. Li, and H. Chung, “JADE: Adaptive
  ///     Differential Evolution With Optional External Archive” IEEE Trans.
  ///     Systems, Man, and Cybernetics-Part B, vol. 39, no. 6, pp. 1362-1381,
  ///     Dec. 2009.
  /// [2] Setsuko Sakai, Tetsugo Takahata, "Improvement of Adaptive
  ///     Differential Evolution Algorithm JADE Considering Parameter
  ///     Correlation", Mathematical Model under Uncertainty and Related
  ///     Topics
  ///  
  /// Implemented by N.Tomi(tomi.nori+github at gmail.com)
  /// </remarks>
  public class clsOptDEJADE : AbsOptimization
  {
    // ----------------------------------------------------------------
    // Common parameters
    // ----------------------------------------------------------------
    /// <summary>Max iteration count</summary>
    public override int Iteration { get; set; } = 20000;

    /// <summary>epsilon(Default:1e-8) for Criterion</summary>
    public double EPS { get; set; } = 0.000000001;

    /// <summary>
    /// higher N percentage particles are finished at the time of same evaluate value.
    /// This parameter is valid is when IsUseCriterion is true.
    /// </summary>
    public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()

    private int HigherNPercentIndex; // for IsCriterion())

    /// <summary>Upper bound(limit solution space)</summary>
    public double[] UpperBounds { get; set; } = null;

    /// <summary>Lower bound(limit solution space)</summary>
    public double[] LowerBounds { get; set; } = null;

    // ----------------------------------------------------------------
    // DE parameters
    // ----------------------------------------------------------------
    /// <summary>
    /// Population Size(Default:100)
    /// </summary>
    public int PopulationSize { get; set; } = 100;

    /// <summary>population</summary>
    private List<LoPoint> m_parents = new List<LoPoint>();

    /// <summary>archive</summary>
    private List<LoPoint> m_archive = new List<LoPoint>();

    /// <summary>Constant raio 0 to 1(Adaptive paramter for muF, muCR)(Default:0.1)</summary>
    public double C { get; set; } = 0.1;

    /// <summary>Adapative cross over ratio(Default:0.5)</summary>
    public double MuCR { get; set; } = 0.5;

    /// <summary>Adapative F(Default:0.5)</summary>
    public double MuF { get; set; } = 0.5;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_func">Objective Function</param>
    /// <remarks>
    /// </remarks>
    public clsOptDEJADE(AbsObjectiveFunction ai_func)
    {
      _func = ai_func;
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <remarks></remarks>
    public override void Init()
    {
      try
      {
        // init meber varibles
        _iteration = 0;
        m_parents.Clear();
        _error.Clear();
        m_archive.Clear();

        // init muF, muCR
        MuCR = 0.5;
        MuF = 0.5;

        // bound check
        if (UpperBounds != null && LowerBounds != null)
        {
          if (UpperBounds.Length != _func.Dimension())
            throw new Exception("UpperBounds.Length is different");
          if (LowerBounds.Length != _func.Dimension())
            throw new Exception("LowerBounds.Length is different");
        }

        // generate population
        for (int i = 0; i <= PopulationSize - 1; i++)
        {
          // initial position
          var array = clsUtil.GenRandomPosition(_func, InitialPosition,
            InitialValueRangeLower, InitialValueRangeUpper);

          // bound check
          var tempPoint = new LoPoint(_func, array);
          if (UpperBounds != null && LowerBounds != null)
            clsUtil.LimitSolutionSpace(tempPoint, LowerBounds, UpperBounds);

          // save point
          m_parents.Add(tempPoint);
        }

        // Sort Evaluate
        m_parents.Sort();

        // Detect HigherNPercentIndex
        HigherNPercentIndex = Convert.ToInt32(m_parents.Count * HigherNPercent);
        if (HigherNPercentIndex == m_parents.Count ||
            HigherNPercentIndex >= m_parents.Count)
          HigherNPercentIndex = m_parents.Count - 1;
      }
      catch (Exception ex)
      {
        _error.SetError(true, clsError.ErrorType.ERR_INIT);
      }
    }

    /// <summary>
    /// Do Iteration
    /// </summary>
    /// <param name="iteration">Iteration count. When you set zero, use the default value.</param>
    /// <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
    /// <remarks></remarks>
    public override bool Iterate(int iteration = 0)
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
      for (int iterate = 0; iterate <= iteration; iterate++)
      {
        // Counting generation
        _iteration += 1;

        // Sort Evaluate
        m_parents.Sort();

        // check criterion
        if (IsUseCriterion)
        {
          // higher N percentage particles are finished at the time of same evaluate value.
          if (clsUtil.IsCriterion(EPS, m_parents[0].Eval,
            m_parents[HigherNPercentIndex].Eval))
            return true;
        }

        // --------------------------------------------------------------------------------------------
        // DE process
        // --------------------------------------------------------------------------------------------
        // Mutation and Crossover
        var sumF = 0.0;
        var sumFSquare = 0.0;
        var sumCR = 0.0;
        var countSuccess = 0;
        for (var i = 0; i <= PopulationSize - 1; i++)
        {
          // update F
          double F;
          while (true)
          {
            F = clsUtil.CauchyRand(MuF, 0.1);
            if (F < 0)
              continue;
            if (F > 1)
              F = 1.0;
            break;
          }

          // update CR 0 to 1
          var CR = clsUtil.NormRand(MuCR, 0.1);
          if (CR < 0)
            CR = 0.0;
          else if (CR > 1)
            CR = 1.0;

          // pick pBest
          LoPoint pbest /* TODO Change to default(_) if this is not a reference type */;
          var pBestIndex =
            Convert.ToInt32(PopulationSize * Random.NextDouble());
          if (pBestIndex <= 2)
          {
            pbest = Random.NextDouble() > 0.5 ? m_parents[0] : m_parents[1];
          }
          else if (pBestIndex == PopulationSize)
            pbest = m_parents[PopulationSize - 1]; // worst
          else
            pbest = m_parents[Random.Next(0, pBestIndex)];

          // xi,g
          var xi = m_parents[i];

          // pick xr1,g different parent without i
          var tempIndex1 = clsUtil.RandomPermutaion(m_parents.Count, i);
          var r1Index = tempIndex1[0];
          LoPoint p1 = m_parents[r1Index];

          // pick xr2~,g different parent without i, xr1,g
          var sumIndex = m_parents.Count + m_archive.Count;
          var tempIndex2 =
            clsUtil.RandomPermutaion(0, sumIndex, new[] {i, r1Index});
          var r2Index = tempIndex2[0];
          LoPoint
            p2 = null /* TODO Change to default(_) if this is not a reference type */;
          if (r2Index >= m_parents.Count)
          {
            r2Index = r2Index - m_parents.Count;
            p2 = m_archive[r2Index];
          }
          else
            p2 = m_parents[r2Index];

          // Mutation and Crossover
          var child = new LoPoint(ObjectiveFunction);
          var j = Random.Next() % ObjectiveFunction.Dimension();
          var D = ObjectiveFunction.Dimension() - 1;

          // crossover
          for (var k = 0; k <= ObjectiveFunction.Dimension() - 1; k++)
          {
            if (Random.NextDouble() < CR || k == D)
              child[j] = xi[j] + F * (pbest[j] - xi[j]) + F * (p1[j] - p2[j]);
            else
              child[j] = xi[k];
            j = (j + 1) % ObjectiveFunction.Dimension(); // next
          }

          child.ReEvaluate(); // Evaluate child

          // Limit solution space
          clsUtil.LimitSolutionSpace(child, LowerBounds, UpperBounds);

          // Survive
          if (child.Eval < m_parents[i].Eval)
          {
            // add archive
            m_archive.Add(m_parents[i].Copy());

            // replace
            m_parents[i] = child;

            // for adaptive parameter
            sumF += F;
            sumFSquare += Math.Pow(F, 2);
            sumCR += CR;
            countSuccess += 1;
          }
        } // population iteration

        // remove archive
        var removeCount = m_archive.Count - PopulationSize;
        if (removeCount > 0)
        {
          clsUtil.RandomizeArray(ref m_archive);
          m_archive.RemoveRange(m_archive.Count - removeCount, removeCount);
        }

        // calc muF, muCR
        if (countSuccess > 0)
        {
          // μCR = (1 − c) · μCR + c · meanA(SCR)
          MuCR = (1 - C) * MuCR + C * (sumCR / countSuccess);
          // μF = (1 − c) · μF + c · meanL (SF)
          MuF = (1 - C) * MuF + C * (sumFSquare / sumF);
        }
        else
        {
          MuCR = (1 - C) * MuCR;
          MuF = (1 - C) * MuF;
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
      get { return clsUtil.GetBestPoint(m_parents, true); }
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
      get { return m_parents; }
    }
  }
}