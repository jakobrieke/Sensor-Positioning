using System.Linq;
using LoMath;
using static System.Math;

namespace Optimization
{
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
    
    public static void Bounce(Particle p, SearchSpace sp)
    {
      var i = 0;
      foreach (var interval in sp.Intervals)
      {
        if (p.Position[i] < interval[0])
        {
          p.Position[i] = interval[0];
          p.Velocity[i] *= -1;
        }
        else if (p.Position[i] > interval[1])
        {
          p.Position[i] = interval[1];
          p.Velocity[i] *= -1;
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
          Pow(p.Velocity[i] / (t[1] - p.LastPosition[i]), 2)).Sum();
      }
      else
      {
        length += sp.Intervals.Select((t, i) => 
          Pow(p.Velocity[i] / (p.LastPosition[i] - t[0]), 2)).Sum();
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
}