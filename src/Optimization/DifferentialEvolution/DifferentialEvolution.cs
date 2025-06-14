using System.Collections.Generic;
using LinearAlgebra;

namespace Optimization
{
  public abstract class DifferentialEvolution : SwarmOptimization
  {
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

    public override void Init(Vector[] startPositions)
    {
      var populationSize = startPositions.Length;
      
      ResetIterations();
      Population = new List<Point>(populationSize);
      CR = new List<double>(populationSize);
      F = new List<double>(populationSize);

      for (var i = 0; i < populationSize; i++)
      {
        CR.Add(1);
        F.Add(1);
        Population.Add(new Point(startPositions[i], 
          Fitness.Eval(startPositions[i])));
      }
    }

    protected override void Update()
    {
      Mutation();
      Crossover();
      Selection();
    }
    
    public abstract Vector Mutation();
    
    public abstract Vector Crossover();
    
    public abstract Point Selection();
  }
}