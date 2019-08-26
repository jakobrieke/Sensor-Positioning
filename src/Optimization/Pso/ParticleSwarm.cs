using System;
using System.Collections.Generic;
using System.Linq;
using MersenneTwister;
using static System.Math;

namespace Optimization
{
  /// <summary>
  /// A swarm in terms of Particle Swarm Optimization is a set of Particles
  /// which move inside a SearchSpace to find the the minimum of a given fitness
  /// function. Also there is a topology defined between the particles which
  /// indicates which particle informs another particle.
  /// </summary>
  public abstract class ParticleSwarm : StochisticOptimization
  {
    /// <summary>
    /// A list of particles which make up the swarm.
    /// </summary>
    public List<Particle> Particles;
    
    public double W = 1 / (2 * Log(2));  // ~0.729
    
    public double C1 = 1 / 2f + Log(2);  // ~1.49445
    
    public double C2 = 1 / 2f + Log(2);  // ~1.49445

    /// <summary>
    /// The best known position in the search space.
    /// </summary>
    public double[] GlobalBest;
    
    /// <summary>
    /// The value at the best known position.
    /// </summary>
    public double GlobalBestValue;
    
    /// <summary>
    /// Indicates if the global best was updated in the last iteration.
    /// </summary>
    public bool GlobalBestChanged;

    /// <summary>
    /// Create a new particle swarm.
    /// </summary>
    /// <param name="searchSpace"></param>
    /// <param name="fitness"></param>
    public ParticleSwarm(SearchSpace searchSpace, Objective fitness) : 
      base(fitness, searchSpace)
    {}

    /// <summary>
    /// A function that defines how the velocity and the position of a
    /// particle changes at each time step. The update method is called on each
    /// particle when running an iteration.
    /// </summary>
    public abstract void Update(Particle particle);

    /// <summary>
    /// A function restricts the position of a particle.
    /// If a particle has no restrictions it is also known as
    /// 'let them fly' method. The confinement! Method is called before the
    /// Update method.
    /// </summary>
    public abstract void Confinement(Particle particle);

    /// <summary>
    /// A function that defines a topology (neighbourhood relation between
    /// all particles) on the swarm.
    /// </summary>
    public abstract void Topology();

    /// <summary>
    /// A function that indicates if the topology should be updated in the
    /// current iteration or not.
    /// </summary>
    public abstract bool ShouldTopoUpdate();
    
    /// <summary>
    /// Find the particle with the smallest value at its position.
    /// </summary>
    /// <param name="particles">A list of particles.</param>
    /// <returns>
    /// The particle with the smallest position value.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Throws an argument exception if the list of particles is empty.
    /// </exception>
    public static Particle ArgMin(List<Particle> particles)
    {
      if (particles.Count == 0)
      {
        throw new ArgumentException("List of particles is empty.");
      }

      var result = particles[0];

      for (var i = 1; i < particles.Count; i++)
      {
        if (result.PositionValue > particles[i].PositionValue)
        {
          result = particles[i];
        }
      }

      return result;
    }

    /// <summary>
    /// Get the best position and it's corresponding value that has been 
    /// found so far by the particle swarm. Globally means in reference to 
    /// all particles contained in the swarm.
    /// Note that the swarm has to be initialized first.
    /// </summary>
    /// <param name="swarm">
    /// The swarm to get the so far globally found value from.
    /// </param>
    /// <returns>
    /// The position and its corresponding value.
    /// </returns>
    private static Tuple<double[], double> GetGlobalBest(ParticleSwarm swarm)
    {
      var min = swarm.Particles[0];

      for (var i = 1; i < swarm.Particles.Count; i++)
      {
        if (swarm.Particles[i].PreviousBestValue < min.PreviousBestValue)
        {
          min = swarm.Particles[i];
        }
      }

      var best = new double[min.PreviousBest.Length];
      min.PreviousBest.CopyTo(best, 0);
      
      return new Tuple<double[], double>(best, min.PreviousBestValue);
    }

    /// <summary>
    /// Initialize a swarm from a list of start positions and velocities.
    /// </summary>
    /// <param name="startPosition">
    /// An array of initial positions for each particle.
    /// </param>
    /// <param name="startVelocity">
    /// An array of initial velocities for each particle.
    /// </param>
    public void Init(double[][] startPosition, double[][] startVelocity)
    { 
      Particles = new List<Particle>(startPosition.Length);

      for (var i = 0; i < startPosition.Length; i++)
      {
        var particle = new Particle
        {
          Position = startPosition[i],
          LastPosition = new double[SearchSpace.Dimension],
          Velocity = startVelocity[i],
          PreviousBest = new double[SearchSpace.Dimension],
          LocalBest = new double[SearchSpace.Dimension]
        };
        
        particle.PositionValue = Fitness.Eval(particle.Position);
        particle.Position.CopyTo(particle.LastPosition, 0);
        particle.Position.CopyTo(particle.PreviousBest, 0);
        particle.PreviousBestValue = particle.PositionValue;

        Particles.Add(particle);
      }

      Topology();

      var (best, bestValue) = GetGlobalBest(this);
      GlobalBest = best;
      GlobalBestValue = bestValue;
      GlobalBestChanged = true;
      
      ResetIterations();
    }

    /// <summary>
    /// Initialize the swarm with a list of start positions for each particle.
    /// </summary>
    /// <param name="startPosition"></param>
    public void Init(double[][] startPosition)
    {
      var startVelocity = new double[startPosition.Length][];
      for (var i = 0; i < startVelocity.Length; i++)
      {
        startVelocity[i] = new double[SearchSpace.Dimension];
        for (var j = 0; j < SearchSpace.Dimension; j++)
        {
          startVelocity[i][j] = RandomExtension.Uniform(Random,
            SearchSpace.Intervals[j][0] - startPosition[i][j],
            SearchSpace.Intervals[j][1] - startPosition[i][j]);
        }
      }
      Init(startPosition, startVelocity);
    }
    
    /// <summary>
    /// Initialize a swarm with n particles.
    /// This creates n particles and initializes them with random values 
    /// inside the search space.
    /// </summary>
    /// <param name="numberOfParticles"></param>
    public void Init(int numberOfParticles)
    {
      var startPosition = new double[numberOfParticles][];
      for (var i = 0; i < numberOfParticles; i++)
      {
        startPosition[i] = SearchSpace.RandPos(Random);
      }
      Init(startPosition);
    }

    public override void Init()
    {
      Init(40);
    }

    /// <summary>
    /// Run one iteration on an initialized swarm.
    /// The iteration will update the particle positions, their previousBest,
    /// apply a confinement on the particles and update their localBests.
    /// </summary>
    public override void Update()
    {
      var best = Particles[0];
      foreach (var particle in Particles)
      {
        particle.Position.CopyTo(particle.LastPosition, 0);
        Update(particle);
        Confinement(particle);
        particle.PositionValue = Fitness.Eval(particle.Position);

        if (particle.PositionValue < particle.PreviousBestValue)
        {
          particle.Position.CopyTo(particle.PreviousBest, 0);
          particle.PreviousBestValue = particle.PositionValue;
        }

        if (particle.PositionValue < particle.LocalBestValue)
        {
          foreach (var neighbour in particle.Neighbours)
          {
            particle.Position.CopyTo(neighbour.LocalBest, 0);
            neighbour.LocalBestValue = particle.PositionValue;
          }
        }

        if (particle.PositionValue < best.PositionValue) best = particle;
      }
      
      if (best.PositionValue < GlobalBestValue)
      {
        best.Position.CopyTo(GlobalBest, 0);
        GlobalBestValue = best.PositionValue;
        GlobalBestChanged = true;
      }
      else GlobalBestChanged = false;

      if (ShouldTopoUpdate()) Topology();
    }

    public override Point Best()
    {
      return new Point(GlobalBest, GlobalBestValue);
    }
  }
}