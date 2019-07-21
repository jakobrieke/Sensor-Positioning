using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using LOMath;
using sensor_positioning;
using static Geometry.Vector;

namespace LibOptimization.Optimization
{
  /// <summary>
  /// A Particle in terms of Particle Swarm Optimization is a point inside a
  /// n-dimensional space with a velocity applied to it which indicates where
  /// and with what speed the particle is currently moving inside the space.
  /// A particle is used to evaluate the search space at a given position
  /// with a given fitness function which is to be optimized.
  /// </summary>
  public class Particle
  {
    /// <summary>
    /// The current position of the particle.
    /// </summary>
    public double[] Position;
    /// <summary>
    /// The position the particle had before the last update.
    /// </summary>
    public double[] LastPosition;
    /// <summary>
    /// The best position which the particle found so far.
    /// </summary>
    public double[] PreviousBest;
    /// <summary>
    /// The current velocity with which the particle moves through the search
    /// space.
    /// </summary>
    public double[] Velocity;
    /// <summary>
    /// The best position inside the neighbourhood. 
    /// </summary>
    public double[] LocalBest;
    /// <summary>
    /// The evaluated value of the current position.
    /// </summary>
    public double PositionValue;
    /// <summary>
    /// The evaluated value of the previously best value.
    /// </summary>
    public double PreviousBestValue;
    /// <summary>
    /// The evaluated value of the best position inside the neighbourhood.
    /// </summary>
    public double LocalBestValue;
    /// <summary>
    /// A list of neighbours of the particle, the particle knows all values
    /// of its neighbours.
    /// </summary>
    public List<Particle> Neighbours;
  }


  /// <summary>
  /// A SearchSpace in terms of Particle Swarm Optimization is a n-dimensional
  /// space with a left and right border for each dimension.
  /// </summary>
  public struct SearchSpace
  {
    public readonly int Dimensions;
    public readonly double[][] Intervals;

    public SearchSpace(int dimensions, double size) : this()
    {
      Dimensions = dimensions;
      Intervals = new double[dimensions][];

      for (var i = 0; i < dimensions; i++)
      {
        Intervals[i] = new[] {-size, size};
      }
    }

    public SearchSpace(int dimensions, double left, double right)
    {
      if (left > right)
      {
        throw new ArgumentException(
          "Left border must be greater than right border.");
      }

      Dimensions = dimensions;
      Intervals = new double[dimensions][];

      for (var i = 0; i < dimensions; i++)
      {
        Intervals[i] = new[] {left, right};
      }
    }

    public SearchSpace(double[][] intervals)
    {
      Intervals = intervals;
      Dimensions = intervals.Length;
    }

    /// <summary>
    /// Get a random position inside the SearchSpace.
    /// </summary>
    /// <returns>
    /// An array with the same dimension as the SearchSpace.
    /// </returns>
    public double[] RandPos()
    {
      return Intervals.Select(i => MTRandom.Uniform(i[0], i[1])).ToArray();
    }
  }

  /// <summary>
  /// A swarm in terms of Particle Swarm Optimization is a set of Particles which
  /// move inside a SearchSpace to find the the minimum of a given fitness
  /// function. Also there is a topology defined between the particles which
  /// indicates which particle informs another particle.
  /// </summary>
  public class Swarm
  {
    /// <summary>
    /// An n dimensional space to move the particles in and update
    /// the fitness function on. Note that the fitness function must have the
    /// same number of parameters as the search space has dimensions.
    /// </summary>
    public SearchSpace SearchSpace;
    
    /// <summary>
    /// A function that takes an array of n numbers and returns a single number.
    /// </summary>
    public Func<double[], double> Fitness;

    /// <summary>
    /// A list of particles which make up the swarm.
    /// </summary>
    public List<Particle> Particles;

    /// <summary>
    /// A function that defines the topology (neighbourhood relation between
    /// all particles) of the swarm.
    /// </summary>
    public Action<List<Particle>> Topology;
    
    /// <summary>
    /// A function that indicates if the topology should be updated in the
    /// current iteration or not.
    /// </summary>
    public Func<Swarm, bool> ShouldTopoUpdate;
    
    /// <summary>
    /// A function that defines how the velocity and the position of a
    /// particle changes at each time step. The update method is called on each
    /// particle when running an iteration.
    /// </summary>
    public Action<Particle> Update;
    
    /// <summary>
    /// A function that defines in what ways a particle is
    /// restricted. If a particle has no restrictions it is also known as
    /// 'let them fly' method. The confinement! Method is called before the
    /// Update method.
    /// </summary>
    public Action<Particle, SearchSpace> Confinement;

    /// <summary>
    /// Indicates how many iterations where run so far on the swarm.
    /// An iterations marks a complete update of each particle inside the swarm.
    /// Note that this value is only strict forward if the swarm size is constant
    /// over all iterations. If not so, the interpretation of this values depends
    /// on the method used to change the swarm size.
    /// </summary>
    public int Iteration;
    
    /// <summary>
    /// Indicates how many evaluations are done on the swarm in total.
    /// </summary>
    public int Evaluations;

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
    /// <param name="topology"></param>
    /// <param name="shouldTopoUpdate"></param>
    /// <param name="update"></param>
    /// <param name="confinement"></param>
    public Swarm(SearchSpace searchSpace, Func<double[], double> fitness,
      Action<List<Particle>> topology, Func<Swarm, bool> shouldTopoUpdate,
      Action<Particle> update,
      Action<Particle, SearchSpace> confinement)
    {
      SearchSpace = searchSpace;
      Fitness = fitness;
      Topology = topology;
      ShouldTopoUpdate = shouldTopoUpdate;
      Update = update;
      Confinement = confinement;
      Iteration = -1;
      Evaluations = 0;
    }
    
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
      if (particles.Count == 1) return particles[0];

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
    /// The position and the corresponding value.
    /// </returns>
    public static Tuple<double[], double> GetGlobalBest(Swarm swarm)
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
    public void Initialize(double[][] startPosition, double[][] startVelocity)
    {
      Particles = new List<Particle>(startPosition.Length);

      for (var i = 0; i < startPosition.Length; i++)
      {
        var particle = new Particle
        {
          Position = startPosition[i],
          LastPosition = new double[SearchSpace.Dimensions],
          Velocity = startVelocity[i],
          PreviousBest = new double[SearchSpace.Dimensions],
          LocalBest = new double[SearchSpace.Dimensions]
        };

        particle.PositionValue = Fitness(particle.Position);
        particle.Position.CopyTo(particle.LastPosition, 0);
        particle.Position.CopyTo(particle.PreviousBest, 0);
        particle.PreviousBestValue = particle.PositionValue;

        Particles.Add(particle);
      }

      Topology(Particles);

      var (best, bestValue) = GetGlobalBest(this);
      GlobalBest = best;
      GlobalBestValue = bestValue;
      GlobalBestChanged = true;

      Iteration = 0;
      Evaluations = 0;
    }

    public void Initialize(double[][] startPosition)
    {
      var startVelocity = new double[startPosition.Length][];
      for (var i = 0; i < startVelocity.Length; i++)
      {
        startVelocity[i] = new double[SearchSpace.Dimensions];
        for (var j = 0; j < SearchSpace.Dimensions; j++)
        {
          startVelocity[i][j] = MTRandom.Uniform(
            SearchSpace.Intervals[j][0] - startPosition[i][j],
            SearchSpace.Intervals[j][1] - startPosition[i][j]);
        }
      }
      Initialize(startPosition, startVelocity);
    }
    
    /// <summary>
    /// Initialize a swarm with n particles.
    /// This creates n particles and initializes them with random values 
    /// inside the search space.
    /// A swarm has to be initialized before calling iterateX! on it.
    /// </summary>
    /// <param name="numberOfParticles"></param>
    public void Initialize(int numberOfParticles = 40)
    {
      var startPosition = new double[numberOfParticles][];
      for (var i = 0; i < numberOfParticles; i++)
      {
        startPosition[i] = SearchSpace.RandPos();
      }
      Initialize(startPosition);
    }

    private void UpdateParticle(Particle p)
    {
      p.Position.CopyTo(p.LastPosition, 0);
      Update(p);
      Confinement(p, SearchSpace);
      p.PositionValue = Fitness(p.Position);

      if (p.PositionValue < p.PreviousBestValue)
      {
        p.Position.CopyTo(p.PreviousBest, 0);
        p.PreviousBestValue = p.PositionValue;
      }

      if (p.PositionValue < p.LocalBestValue)
      {
        foreach (var neighbour in p.Neighbours)
        {
          p.Position.CopyTo(neighbour.LocalBest, 0);
          neighbour.LocalBestValue = p.PositionValue;
        }
      }
      
      Evaluations++;
    }
    
    /// <summary>
    /// Run one iteration on an initialized swarm.
    /// The iteration will update the particle positions, their previousBest,
    /// apply a confinement on the particles, update their
    /// localBests and increase the the number of evaluations, done
    /// for each particle in the swarm.
    /// </summary>
    public void Iterate()
    {
      var best = Particles[0];
      foreach (var particle in Particles)
      {
        UpdateParticle(particle);

        if (particle.PositionValue < best.PositionValue) best = particle;
      }
      
      if (best.PositionValue < GlobalBestValue)
      {
        best.Position.CopyTo(GlobalBest, 0);
        GlobalBestValue = best.PositionValue;
        GlobalBestChanged = true;
      }
      else GlobalBestChanged = false;

      if (ShouldTopoUpdate(this)) Topology(Particles);

      Iteration++;
    }

    /// <summary>
    ///  Run n iterations on an initialized swarm.
    /// </summary>
    /// <param name="maxIterations"></param>
    public void Iterate(int maxIterations)
    {
      for (var i = 0; i < maxIterations; i++) Iterate();
    }
    
    /// <summary>
    /// Update the swarm for a maximum number of fitness evaluations. An
    /// evaluation overhead of swarm-size - 1 arises if evaluations mod
    /// swarm-size > 0.
    /// </summary>
    /// <param name="evaluations"></param>
    public void IterateUntilEvals(int evaluations)
    {
      while (Evaluations < evaluations) Iterate();
    }

    /// <summary>
    /// Iterate until a certain minimum is reached.
    /// </summary>
    /// <param name="value">A minimum value that has to be reached.</param>
    public void IterateUntil(double value)
    {
      while (GlobalBestValue < value) Iterate();
    }
  }

  public static class Confinement
  {
    /// <summary>
    /// Apply no confinements to the particle.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sp"></param>
    public static void LetThemFly(Particle p, SearchSpace sp) {}

    /// <summary>
    /// Set the particles velocity to 0 and their position to the search 
    /// space interval border for each dimension where the particles 
    /// position is outside of the interval border.
    /// See also "Confinements and Biases in Particle Swarm Optimisation" by
    /// Maurice Clerc 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sp"></param>
    public static void Standard(Particle p, SearchSpace sp)
    {
      var i = 0;
      foreach (var interval in sp.Intervals)
      {
        if (p.Position[i] < interval[0])
        {
          p.Position[i] = interval[0];
          p.Velocity[i] = 0;
        }
        else if (p.Position[i] > interval[1])
        {
          p.Position[i] = interval[1];
          p.Velocity[i] = 0;
        }
        i++;
      }
    }

    /// <summary>
    /// See also "Confinements and Biases in Particle Swarm Optimisation" by
    /// Maurice Clerc 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sp"></param>
    public static void DeterministicBack(Particle p, SearchSpace sp)
    {
      var i = 0;
      foreach (var interval in sp.Intervals)
      {
        if (p.Position[i] < interval[0])
        {
          p.Position[i] = interval[0];
          p.Velocity[i] *= -0.5f;
        }
        else if (p.Position[i] > interval[1])
        {
          p.Position[i] = interval[1];
          p.Velocity[i] *= -0.5f;
        }
        i++;
      }
    }

    /// <summary>
    /// See also "Confinements and Biases in Particle Swarm Optimisation" by
    /// Maurice Clerc
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sp"></param>
    public static void RandomBack(Particle p, SearchSpace sp)
    {
      var i = 0;
      foreach (var interval in sp.Intervals)
      {
        if (p.Position[i] < interval[0])
        {
          p.Position[i] = interval[0];
          p.Velocity[i] *= -MTRandom.Uniform();
        }
        else if (p.Position[i] > interval[1])
        {
          p.Position[i] = interval[1];
          p.Velocity[i] *= -MTRandom.Uniform();
        }
        i++;
      }
    }

    public static void Hyperbolic(Particle p, SearchSpace sp)
    {
      var length = 0.0;
      
      if (p.Velocity.Sum() > 0)
      {
        length += sp.Intervals.Select((t, i) => 
          Math.Pow(p.Velocity[i] / (t[1] - p.LastPosition[i]), 2)).Sum();
      }
      else
      {
        length += sp.Intervals.Select((t, i) => 
          Math.Pow(p.Velocity[i] / (p.LastPosition[i] - t[0]), 2)).Sum();
      }

      p.Velocity = (new LoVector(p.Velocity) / (1 + length)).ToArray();
    }

    public static void HybridHyperbolicRandomBack(Particle p, SearchSpace sp)
    {
      if (MersenneTwister.MTRandom.Create().NextDouble() > 0.5)
      {
        Hyperbolic(p, sp);
      }
      else RandomBack(p, sp);
    }
  }

  public static class Topology
  {
    /// <summary>
    /// Apply a ring topology to an array of particles.
    /// </summary>
    /// <param name="particles">
    /// The array of particles to apply the topology to.
    /// </param>
    public static void RingTopology(List<Particle> particles)
    {
      for (var i = 0; i < particles.Count; i++)
      {
        particles[i].Neighbours = new List<Particle>
        {
          particles[i == 0 ? particles.Count - 1 : i - 1], 
          particles[i], 
          particles[i == particles.Count - 1 ? 0 : i + 1]
        };
        
        var min = Swarm.ArgMin(particles[i].Neighbours);
        min.PreviousBest.CopyTo(particles[i].LocalBest, 0);
        particles[i].LocalBestValue = min.PreviousBestValue;
      }
    }

    /// <summary>
    /// Apply a random topology to an array of particles.
    /// For more information please refers to "Back to random topology" - 
    /// Method 2 by Maurice Clerc, 27th March 2007.
    /// </summary>
    /// <param name="particles">
    /// The array of particles to apply the topology to.
    /// </param>
    /// <param name="k">
    /// The maximum possible number of neighbours > 0. If k is set to -1
    /// k will equal the number of particles.
    /// Defaults to 3.
    /// </param>
    public static void AdaptiveRandomTopology(List<Particle> particles,
      int k = 3)
    {
      var amount = particles.Count;

      if (amount == 0) return;

      k = k == -1 ? amount : k;
      var result = new bool[amount, amount];

      for (var i = 0; i < amount; i++)
      {
        result[i, i] = true;
        for (var j = 0; j < k; j++)
        {
          var rand = MersenneTwister.MTRandom.Create();
          var column = rand.Next();

          result[i, column] = true;
        }
      }

      for (var j = 0; j < amount; j++)
      {
        var particle = particles[j];
        particle.Neighbours = new List<Particle>();

        for (var i = 0; i < amount; i++)
        {
          if (result[i, j])
          {
            particle.Neighbours.Add(particles[i]);
          }
        }

        particles[j] = particle;
      }

      foreach (var particle in particles)
      {
        var min = Swarm.ArgMin(particle.Neighbours);
        min.PreviousBest.CopyTo(particle.LocalBest, 0);
        particle.LocalBestValue = min.PreviousBestValue;
      }
    }
  }
  
  public static class Pso
  {
    //
    // -- Update Functions
    //

    public static double W = 0.729;  // 1 / (2 * Math.Log(2));
    public static double C1 = 1.49445;  // 1 / 2f + Math.Log(2);
    public static double C2 = C1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    public static void UpdateSpso2006(Particle p)
    {
      for (var i = 0; i < p.Velocity.Length; i++)
      {
        p.Velocity[i] = 
          W * p.Velocity[i] + 
          MTRandom.Uniform(0, C1) * (p.PreviousBest[i] - p.Position[i]) + 
          MTRandom.Uniform(0, C2) * (p.LocalBest[i] - p.Position[i]);
        p.Position[i] += p.Velocity[i];
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    public static void UpdateSpso2007(Particle p)
    {
      if (p.LocalBestValue.Equals(p.PreviousBestValue))
      {
        for (var i = 0; i < p.Velocity.Length; i++)
        {
          p.Velocity[i] = 
            p.Velocity[i]
            + MTRandom.Uniform(0, C1) * (p.PreviousBest[i] - p.Position[i]);
          p.Position[i] += p.Velocity[i];
        }
      }
      else UpdateSpso2006(p);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="particle"></param>
    public static void UpdateSpso2011(Particle particle)
    {
      var value = Add(particle.PreviousBest, particle.LocalBest);
      value = Subtract(value, Multiply(particle.Position, 2f));

      if (Math.Abs(particle.LocalBestValue - particle.PreviousBestValue) 
          < Defs.Precision)
      {
        value = Subtract(particle.PreviousBest, particle.Position);
      }

      var g = Add(particle.Position, Multiply(Divide(value, 3f), C1));
      var x = new double[particle.Position.Length];

      for (var i = 0; i < particle.Position.Length; i++)
      {
        x[i] = MTRandom.Uniform(g[i], Math.Abs(g[i] - particle.Position[i]));
      }

      var lastPosition = new double[particle.Position.Length];
      particle.Position.CopyTo(lastPosition, 0);

      particle.Position = Add(Multiply(particle.Velocity, W), x);
      particle.Velocity = Subtract(
        Add(Multiply(particle.Velocity, W), x), lastPosition);
    }

    //
    // Standard Particle Swarm Definitions
    //

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="fitness"></param>
    /// <returns></returns>
    public static Swarm SwarmSpso2006(SearchSpace sp, 
      Func<double[], double> fitness)
    {
      return new Swarm(sp, fitness,
        Topology.RingTopology, swarm => false,
        UpdateSpso2006, Confinement.Standard);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="fitness"></param>
    /// <returns></returns>
    public static Swarm SwarmSpso2007(SearchSpace sp,
      Func<double[], double> fitness)
    {
      return new Swarm(sp, fitness,
        Topology.RingTopology, swarm => false,
        UpdateSpso2007, Confinement.Standard);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="fitness"></param>
    /// <returns></returns>
    public static Swarm SwarmSpso2011(SearchSpace sp,
      Func<double[], double> fitness)
    {
      return new Swarm(sp, fitness,
        swarm => Topology.AdaptiveRandomTopology(swarm),
        swarm => swarm.GlobalBestChanged,
        UpdateSpso2011, Confinement.DeterministicBack);
    }
  }
  
  public class SPSO2006 : AbsOptimization
  {
    private Swarm _swarm;
    
    public SPSO2006(Swarm swarm, AbsObjectiveFunction objective)
    {
      _swarm = swarm;
      ObjectiveFunction = objective;
      Results = new List<LoPoint>();
    }

    public override void Init()
    {
      if (InitialPosition != null)
      {
        var startPosition = new double[_swarm.SearchSpace.Dimensions][];
        for (var i = 0; i < _swarm.SearchSpace.Dimensions; i++)
        {
          startPosition[i] = new double[InitialPosition.Length];
          InitialPosition.CopyTo(startPosition[i], 0);
        }
        _swarm.Initialize(startPosition);
        
        foreach (var d in InitialPosition)
        {
          Console.Write(d + ", ");
        }
        Console.WriteLine("\b\b");
      }
      else _swarm.Initialize();
    }

    public override bool DoIteration(int iteration = 0)
    {
      _swarm.Iterate(iteration);
      _iteration += iteration;
      return true;
    }

    public override bool IsRecentError()
    {
      return false;
    }

    public override LoPoint Result => 
      new LoPoint(ObjectiveFunction, _swarm.GlobalBest);
    
    public override List<LoPoint> Results { get; }
    
    public override int Iteration 
    {
      get => _swarm.Iteration;
      set {}
    }
  }
  
  public class SPSO2007 : AbsOptimization
  {
    private Swarm _swarm;
    public double[][] Bounds;

    public SPSO2007(AbsObjectiveFunction objective)
    {
      ObjectiveFunction = objective;
      Results = new List<LoPoint>();
    }

    public override void Init()
    {
      var obj = (SensorPositionObj) ObjectiveFunction;
      _swarm = Pso.SwarmSpso2007(
        new SearchSpace(Bounds), 
        v => ObjectiveFunction.F(v.ToList()));
      _swarm.Initialize();
    }

    public override bool DoIteration(int iteration = 0)
    {
      _swarm.Iterate(iteration);
      _iteration += iteration;
      return true;
    }

    public override bool IsRecentError()
    {
      return false;
    }

    public override LoPoint Result => 
      new LoPoint(ObjectiveFunction, _swarm.GlobalBest);
    
    public override List<LoPoint> Results { get; }
    
    public override int Iteration 
    {
      get => _swarm.Iteration;
      set {}
    }
  }
  
  public class SPSO2011 : AbsOptimization
  {
    private Swarm _swarm;
    public double[][] Bounds;

    public SPSO2011(AbsObjectiveFunction objective)
    {
      ObjectiveFunction = objective;
      Results = new List<LoPoint>();
    }

    public override void Init()
    {
      _swarm = Pso.SwarmSpso2011(
        new SearchSpace(Bounds), 
        v => ObjectiveFunction.F(v.ToList()));
//      _swarm.Topology = p => Pso.AdaptiveRandomTopology(p, 3);
      _swarm.Topology = Topology.RingTopology;
      _swarm.Initialize();
    }

    public override bool DoIteration(int iteration = 0)
    {
      _swarm.Iterate(iteration);
      _iteration += iteration;
      return true;
    }

    public override bool IsRecentError()
    {
      return false;
    }

    public override LoPoint Result => 
      new LoPoint(ObjectiveFunction, _swarm.GlobalBest);

    public override List<LoPoint> Results { get; }
    
    public override int Iteration {
      get => _swarm.Iteration;
      set {}
    }
  }
}