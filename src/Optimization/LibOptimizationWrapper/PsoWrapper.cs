using System;
using LibOptimization.Optimization;

namespace Optimization.LibOptimizationWrapper
{
  public class PsoWrapper : Optimization
  {
    public readonly clsOptPSO Pso;
    public double[] InitialPosition;
    public int SwarmSize = 40;
    public bool IsUseCriterion = false;
    public double W = 0.729;
    public double C1 = 1.49445;
    public double C2 = 1.49445;
    public Random Random;
    private Point _best;

    public PsoWrapper(Objective fitness, SearchSpace searchSpace) : 
      base(fitness, searchSpace)
    {
      Pso = new clsOptPSO(new ObjectiveWrapper(fitness, searchSpace.Dimension));
    }

    public override void Init()
    {
      if (Random == null) Random = MersenneTwister.MTRandom.Create();
      
      ResetIterations();
      Pso.IsUseCriterion = IsUseCriterion;
//      Pso.InitialPosition = SearchSpace.RandPos();
      Pso.SwarmSize = SwarmSize;
      Pso.C1 = C1;
      Pso.C2 = C2;
      Pso.Weight = W;
      Pso.Random = Random;
      Pso.Init();
      _best = new Point(Pso.Result.ToArray(), Pso.Result.Value);
    }

    public override void Update()
    {
      Pso.Iterate(1);
      _best = new Point(Pso.Result.ToArray(), Pso.Result.Value);
    }

    public override Point Best()
    {
      return _best;
    }
  }
}