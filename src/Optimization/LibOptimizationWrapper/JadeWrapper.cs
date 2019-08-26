using System;
using LibOptimization.Optimization;

namespace Optimization.LibOptimizationWrapper
{
  public class JadeWrapper : Optimization
  {
    public clsJADE Jade;
    public double[] InitialPosition;
    public int PopulationSize = 40;
    public bool IsUseCriterion = false;
    public double[] LowerBounds;
    public double[] UpperBounds;
    public Random Random;
    public double C = 0.1;
    private Point _best;

    public JadeWrapper(Objective fitness, SearchSpace searchSpace) : 
      base(fitness, searchSpace)
    {
      Jade = new clsJADE(new ObjectiveWrapper(fitness, searchSpace.Dimension));
    }

    public override void Init()
    {
      ResetIterations();
      Jade.C = C;
      Jade.IsUseCriterion = IsUseCriterion;
      Jade.LowerBounds = LowerBounds;
      Jade.UpperBounds = UpperBounds;
      Jade.PopulationSize = PopulationSize;
      Jade.InitialPosition = InitialPosition;
      Jade.Random = Random;
      Jade.Init();
      _best = new Point(Jade.Result.ToArray(), Jade.Result.Value);
    }

    public override void Update()
    {
      Jade.Iterate(1);
      _best = new Point(Jade.Result.ToArray(), Jade.Result.Value);
    }

    public override Point Best()
    {
      return _best;
    }
  }
}