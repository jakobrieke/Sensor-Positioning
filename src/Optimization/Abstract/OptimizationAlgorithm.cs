using System.Collections.Generic;
using LibOptimization.Optimization;

namespace Optimization
{
  public abstract class OptimizationAlgorithm
  {
    public ObjectiveFunction Fitness;
    
    public SearchSpace SearchSpace;

    public int Iteration;

    public OptimizationAlgorithm(ObjectiveFunction fitness, 
      SearchSpace searchSpace)
    {
      Fitness = fitness;
      SearchSpace = searchSpace;
    }

    public virtual void Init()
    {
      Iteration = 0;
    }

    public virtual void Iterate()
    {
      Iteration++;
    }

    public abstract Point Best();
    
    private class AbsOptimizationWrapper : AbsOptimization
    {
      private OptimizationAlgorithm _algorithm;
      
      public AbsOptimizationWrapper(OptimizationAlgorithm alg)
      {
        _algorithm = alg;
      }

      public override void Init()
      {
        _algorithm.Init();
      }

      public override bool Iterate(int iteration = 0)
      {
        _algorithm.Iterate();
        return false;
      }

      public override LoPoint Result
      {
        get
        {
          var best = _algorithm.Best();
          return new LoPoint(null, best.Position, best.Value);
        }
      }

      public override List<LoPoint> Results { get; }
      
      public override bool IsRecentError()
      {
        return false;
      }

      public override int Iteration { get => _algorithm.Iteration; set {} }
    }
    
    public static implicit operator AbsOptimization(OptimizationAlgorithm alg)
    {
      return new AbsOptimizationWrapper(alg);
    }
  }
}