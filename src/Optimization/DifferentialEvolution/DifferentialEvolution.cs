using System;
using System.Collections.Generic;
using LinearAlgebra;

namespace Optimization
{
  public abstract class DifferentialEvolution : OptimizationAlgorithm
  {
    public uint Generation { get; private set; }
    
    public List<Point> Population;
    
    /// <summary>
    /// CR is a list of crossover probability factors for each individual of the
    /// population.
    /// </summary>
    public List<double> CR;
    
    /// <summary>
    /// F is a list of mutation factors for each individual of the population.
    /// </summary>
    public List<double> F;

    public int RandomSeed = 1234;
    public Random Rand;

    public DifferentialEvolution(ObjectiveFunction fitness, 
      SearchSpace searchSpace) : base(fitness, searchSpace)
    {
    }

    public virtual void Init(int populationSize)
    {
      Generation = 0;
      Population = new List<Point>(populationSize);
      CR = new List<double>(populationSize);
      F = new List<double>(populationSize);

      for (var i = 0; i < populationSize; i++)
      {
        CR.Add(1);
        F.Add(1);
        
        var pos = new Vector(SearchSpace.RandPos());
        Population.Add(new Point(pos, Fitness.Eval(pos)));
      }

      Rand = MersenneTwister.MTRandom.Create(RandomSeed);
    }

    public override void Init()
    {
      Init(40);
    }

    public override void Iterate()
    {
      Mutation();
      Crossover();
      Selection();
      Generation++;
    }
    
    public abstract Vector Mutation();
    public abstract Vector Crossover();
    public abstract Point Selection();
  }
}