using System.Collections.Generic;
using LinearAlgebra;

namespace Optimization
{
  public abstract class DifferentialEvolution : StochisticOptimization
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

    public DifferentialEvolution(Objective fitness, 
      SearchSpace searchSpace) : base(fitness, searchSpace)
    {}

    public virtual void Init(int populationSize)
    {
      ResetIterations();
      Generation = 0;
      Population = new List<Point>(populationSize);
      CR = new List<double>(populationSize);
      F = new List<double>(populationSize);

      for (var i = 0; i < populationSize; i++)
      {
        CR.Add(1);
        F.Add(1);
        
        var pos = new Vector(SearchSpace.RandPos(Random));
        Population.Add(new Point(pos, Fitness.Eval(pos)));
      }
    }

    public override void Init()
    {
      Init(40);
    }

    protected override void Update()
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