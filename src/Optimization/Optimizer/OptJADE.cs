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
  public class OptJADE : AbsOptimization
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
    public double[] UpperBounds { get; set; }

    /// <summary>Lower bound(limit solution space)</summary>
    public double[] LowerBounds { get; set; }

    // ----------------------------------------------------------------
    // DE parameters
    // ----------------------------------------------------------------
    /// <summary>
    /// Population Size(Default:100)
    /// </summary>
    public int PopulationSize { get; set; } = 100;

    /// <summary>population</summary>
    private List<LoPoint> _parents = new List<LoPoint>();

    /// <summary>archive</summary>
    private List<LoPoint> _archive = new List<LoPoint>();

    /// <summary>Constant raio 0 to 1(Adaptive paramter for muF, muCR)(Default:0.1)</summary>
    public double C { get; set; } = 0.1;

    /// <summary>Adapative cross over ratio(Default:0.5)</summary>
    public double MuCR { get; set; } = 0.5;

    /// <summary>Adapative F(Default:0.5)</summary>
    public double MuF { get; set; } = 0.5;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="objective">Objective Function</param>
    /// <remarks>
    /// </remarks>
    public OptJADE(AbsObjectiveFunction objective)
    {
      _func = objective;
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <remarks></remarks>
    public override void Init()
    {
      try
      {
        // Init member variables
        _iteration = 0;
        _parents.Clear();
        _error.Clear();
        _archive.Clear();

        // Init muF, muCR
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
        for (var i = 0; i < PopulationSize; i++)
        {
          // initial position
          var array = clsUtil.GenRandomPosition(_func, InitialPosition,
            InitialValueRangeLower, InitialValueRangeUpper);

          // bound check
          var tempPoint = new LoPoint(_func, array);
          if (UpperBounds != null && LowerBounds != null)
          {
            clsUtil.LimitSolutionSpace(tempPoint, LowerBounds, UpperBounds);
          }

          // save point
          _parents.Add(tempPoint);
        }

        // Sort Evaluate
        _parents.Sort();

        // Detect HigherNPercentIndex
        HigherNPercentIndex = Convert.ToInt32(_parents.Count * HigherNPercent);
        if (HigherNPercentIndex == _parents.Count ||
            HigherNPercentIndex >= _parents.Count)
          HigherNPercentIndex = _parents.Count - 1;
      }
      catch (Exception ex)
      {
        _error.SetError(true, clsError.ErrorType.ERR_INIT, ex.Message);
      }
    }

    /// <summary>
    /// Do Iteration
    /// </summary>
    /// <param name="iteration">Iteration count. When set zero, use the
    /// default value.</param>
    /// <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
    /// <remarks></remarks>
    public override bool Iterate(int iteration = 0)
    {
      // Check if there is an error and if the optimization has been initialized
      if (IsRecentError() || Iteration <= _iteration) return true;
      
      iteration = iteration == 0
        ? Iteration - _iteration - 1
        : Math.Min(iteration, Iteration - _iteration) - 1;
      
      for (var iterate = 0; iterate <= iteration; iterate++)
      {
        // Counting generation
        _iteration += 1;

        // Sort Evaluate
        _parents.Sort();

        // check criterion
        if (IsUseCriterion)
        {
          // higher N percentage particles are finished at the time of same
          // evaluate value.
          if (clsUtil.IsCriterion(EPS, _parents[0].Eval,
            _parents[HigherNPercentIndex].Eval))
            return true;
        }

        // ---------------------------------------------------------------------
        // DE process
        // ---------------------------------------------------------------------
        // Mutation and Crossover
        var sumF = 0.0;
        var sumFSquare = 0.0;
        var sumCr = 0.0;
        var countSuccess = 0;
        
        // Population iteration
        for (var i = 0; i < PopulationSize; i++)
        {
          // update F
          double f;
          while (true)
          {
            f = clsUtil.CauchyRand(MuF, 0.1);
            if (f < 0)
              continue;
            if (f > 1)
              f = 1.0;
            break;
          }

          // Update CR 0 to 1
          var cr = clsUtil.NormRand(MuCR, 0.1);
          if (cr < 0) cr = 0.0;
          else if (cr > 1) cr = 1.0;

          // pick pBest
          LoPoint pBest;
          var pBestIndex =
            Convert.ToInt32(PopulationSize * Random.NextDouble());
          if (pBestIndex <= 2)
          {
            pBest = Random.NextDouble() > 0.5 ? _parents[0] : _parents[1];
          }
          else if (pBestIndex == PopulationSize)
            pBest = _parents[PopulationSize - 1]; // worst
          else
            pBest = _parents[Random.Next(0, pBestIndex)];

          // xi,g
          var xi = _parents[i];

          // pick xr1,g different parent without i
          var tempIndex1 = clsUtil.RandomPermutaion(_parents.Count, i);
          var r1Index = tempIndex1[0];
          var p1 = _parents[r1Index];

          // pick xr2~,g different parent without i, xr1,g
          var sumIndex = _parents.Count + _archive.Count;
          var tempIndex2 = clsUtil.RandomPermutaion(
            0, sumIndex, new[] {i, r1Index});
          var r2Index = tempIndex2[0];
          LoPoint p2;
          if (r2Index >= _parents.Count)
          {
            r2Index -= _parents.Count;
            p2 = _archive[r2Index];
          }
          else
            p2 = _parents[r2Index];

          // Mutation and Crossover
          var child = new LoPoint(ObjectiveFunction);
          var j = Random.Next() % ObjectiveFunction.Dimension();
          var d = ObjectiveFunction.Dimension() - 1;

          // crossover
          for (var k = 0; k < ObjectiveFunction.Dimension(); k++)
          {
            if (Random.NextDouble() < cr || k == d)
              child[j] = xi[j] + f * (pBest[j] - xi[j]) + f * (p1[j] - p2[j]);
            else
              child[j] = xi[k];
            j = (j + 1) % ObjectiveFunction.Dimension(); // next
          }

          child.ReEvaluate(); // Evaluate child

          // Limit solution space
          clsUtil.LimitSolutionSpace(child, LowerBounds, UpperBounds);
          
          if (child.Eval >= _parents[i].Eval) continue;
          
          // Add archive
          _archive.Add(_parents[i].Copy());

          // Replace
          _parents[i] = child;

          // For adaptive parameter
          sumF += f;
          sumFSquare += Math.Pow(f, 2);
          sumCr += cr;
          countSuccess += 1;
        }

        // Remove archive
        var removeCount = _archive.Count - PopulationSize;
        if (removeCount > 0)
        {
          clsUtil.RandomizeArray(ref _archive);
          _archive.RemoveRange(_archive.Count - removeCount, removeCount);
        }

        // Calculate muF, muCR
        if (countSuccess > 0)
        {
          // μCR = (1 − c) · μCR + c · meanA(SCR)
          MuCR = (1 - C) * MuCR + C * (sumCr / countSuccess);
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
    public override LoPoint Result => 
      clsUtil.GetBestPoint(_parents, true);

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
    public override List<LoPoint> Results => _parents;
  }
}