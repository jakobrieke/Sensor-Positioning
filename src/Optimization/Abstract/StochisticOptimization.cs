using System;
using MersenneTwister;

namespace Optimization
{
  public abstract class StochisticOptimization : Optimization
  {
    public Random Random;

    protected StochisticOptimization(Objective fitness, 
      SearchSpace searchSpace) : base(fitness, searchSpace)
    {
      Random = MTRandom.Create(MTEdition.Original_19937);
    }
  }
}