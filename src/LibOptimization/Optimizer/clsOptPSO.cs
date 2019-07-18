using System;
using System.Collections.Generic;
using System.Text;
using Util;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// Basic Particle Swarm Optimization
  /// </summary>
  /// <remarks>
  /// Features:
  ///  -Swarm Intelligence algorithm.
  ///  -Derivative free optimization algorithm.
  /// 
  /// Reference:
  /// [1] James Kennedy and Russell Eberhart, "Particle Swarm Optimization",
  ///     Proceedings of IEEE the International Conference on Neural Networks，
  ///     1995
  /// [2] Y. Shi and Russell Eberhart, "A Modified Particle Swarm Optimizer",
  ///     Proceedings of Congress on Evolu-tionary Computation, 79-73., 1998
  /// [3] R. C. Eberhart and Y. Shi, "Comparing inertia weights and constriction
  ///     factors in particle swarm optimization", In Proceedings of the
  ///     Congress on Evolutionary Computation, vol. 1, pp. 84–88, IEEE,
  ///     La Jolla, Calif, USA, July 2000.
  /// </remarks>
  public class clsOptPSO : absOptimization
  {
    /// <summary>Max iteration count(Default:20,000)</summary>
    public override int Iteration { get; set; } = 20000;

    /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
    public double EPS { get; set; } = 0.000001; // 1e-6

    /// <summary>
    /// higher N percentage particles are finished at the time of same evaluate value.
    /// This parameter is valid is when IsUseCriterion is true.
    /// </summary>
    public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()

    // for IsCriterion()
    private int _higherNPercentIndex;

    /// <summary>
    /// A list of particles which make up the swarm.
    /// </summary>
    private readonly List<clsParticle> m_swarm = new List<clsParticle>();

    // TODO Change to default(_) if this is not a reference type
    private clsPoint m_globalBest;

    // -------------------------------------------------------------------
    // Coefficient of PSO
    // -------------------------------------------------------------------
    /// <summary>
    /// Swarm Size(Default:100)
    /// </summary>
    public int SwarmSize = 100;

    /// <summary>
    /// Inertia weight. Weigth=1.0(orignal paper 1995),
    /// Weight=0.729(Default setting)
    /// </summary>
    public double Weight = 0.729;

    /// <summary>
    /// Velocity coefficient(affected by personal best).
    /// C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)
    /// </summary>
    public double C1 = 1.49445;

    /// <summary>
    /// Velocity coefficient(affected by global best). C1 = C2 = 2.0
    /// (orignal paper 1995), C1 = C2 = 1.49445(Default setting)
    /// </summary>
    public double C2 = 1.49445;
    
    /// <summary>
    /// Construct a new PSO from an objective function.
    /// </summary>
    /// <param name="ai_func"></param>
    public clsOptPSO(absObjectiveFunction ai_func)
    {
      m_func = ai_func;
    }
    
    public override void Init()
    {
      try
      {
        // Init member variables
        m_iteration = 0;
        m_swarm.Clear();

        // Init position
        for (var i = 0; i < SwarmSize; i++)
        {
          // Position
          var array = clsUtil.GenRandomPosition(m_func, InitialPosition,
            InitialValueRangeLower, InitialValueRangeUpper);

          var tempPosition = new clsPoint(m_func, array);
          
          var tempBestPosition = tempPosition.Copy();

          // Velocity
          var tempVelocity = clsUtil.GenRandomPosition(m_func,
            null, // TODO Change to default(_) if this is not a reference type
            InitialValueRangeLower, InitialValueRangeUpper);

          // Create swarm
          m_swarm.Add(new clsParticle(tempPosition, tempVelocity,
            tempBestPosition));
        }

        // Sort Evaluate
        m_swarm.Sort();
        m_globalBest = m_swarm[0].BestPoint.Copy();

        // Detect HigherNPercentIndex
        _higherNPercentIndex = Convert.ToInt32(m_swarm.Count * HigherNPercent);
        
        if (_higherNPercentIndex == m_swarm.Count ||
            _higherNPercentIndex >= m_swarm.Count)
        {
          _higherNPercentIndex = m_swarm.Count - 1;
        }
      }
      catch (Exception ex)
      {
        m_error.SetError(true, clsError.ErrorType.ERR_INIT, ex.Message);
      }
      finally
      {
        GC.Collect();
      }
    }

    /// <summary>
    /// Do optimize
    /// </summary>
    /// <param name="iteration"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool DoIteration(int iteration = 0)
    {
      // Check Last Error
      if (IsRecentError() || Iteration <= m_iteration) return true;

      iteration = iteration == 0 ? 
        Iteration - m_iteration - 1 : 
        Math.Min(iteration, Iteration - m_iteration) - 1;
      
      for (var iterate = 0; iterate <= iteration; iterate++)
      {
        m_iteration += 1;

        if (IsUseCriterion)
        {
          // Higher N percentage particles are finished at the time of
          // same evaluate value
          if (clsUtil.IsCriterion(EPS, m_globalBest,
            m_swarm[_higherNPercentIndex].BestPoint))
            return true;
        }

        // PSO process
        foreach (var particle in m_swarm)
        {
          // replace personal best
          if (particle.Point.Eval < particle.BestPoint.Eval)
          {
            particle.BestPoint = particle.Point.Copy();

            // Replace global best
            if (particle.Point.Eval < m_globalBest.Eval)
            {
              m_globalBest = particle.Point.Copy();
            }
          }

          // Update the velocity 
          for (var i = 0; i < m_func.NumberOfVariable(); i++)
          {
            var r1 = m_rand.NextDouble();
            var r2 = m_rand.NextDouble();
            var newV = Weight * particle.Velocity[i] +
                       C1 * r1 * (particle.BestPoint[i] - particle.Point[i]) +
                       C2 * r2 * (m_globalBest[i] - particle.Point[i]);
            particle.Velocity[i] = newV;

            // Update the position
            var newPos = particle.Point[i] + particle.Velocity[i];
            particle.Point[i] = newPos;
          }

          particle.Point.ReEvaluate();
        }

        // sort by eval
        m_swarm.Sort();
      }

      return false;
    }

    /// <summary>
    /// Recent Error
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool IsRecentError()
    {
      return m_error.IsError();
    }

    /// <summary>
    /// Result
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public override clsPoint Result
    {
      get
      {
        // find best index
        // var bestIndex = 0;
        var bestEval = m_swarm[0].BestPoint.Eval;
        for (var i = 0; i <= m_swarm.Count - 1; i++)
        {
          if (!(m_swarm[i].BestPoint.Eval < bestEval)) continue;
          
          bestEval = m_swarm[i].BestPoint.Eval;
          // bestIndex = i;
        }

        return m_swarm[0].BestPoint.Copy();
      }
    }

    /// <summary>
    /// for Debug
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public override List<clsPoint> Results
    {
      get
      {
        m_swarm.Sort();
        var result = new List<clsPoint>(m_swarm.Count);
        foreach (var p in m_swarm)
        {
          result.Add(p.BestPoint.Copy());
        }
        return result;
      }
    }

    public override string ToString()
    {
      var str = new StringBuilder();
      var i = 0;
      
      foreach (var p in m_swarm)
      {
        str.Append(p.BestPoint.Eval);
        str.Append(" ");
        str.Append(p.BestPoint);
        if (i++ < m_swarm.Count - 1) str.Append("\n");
      }

      return str.ToString();
    }
  }
}