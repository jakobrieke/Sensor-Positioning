using System.Collections.Generic;

namespace Optimization
{
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
        
        var min = ParticleSwarm.ArgMin(particles[i].Neighbours);
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
      uint k = 3)
    {
      if (particles.Count == 0) return;
      
      var result = new bool[particles.Count, particles.Count];

      for (var i = 0; i < particles.Count; i++)
      {
        result[i, i] = true;
        var rand = MersenneTwister.MTRandom.Create();
        
        for (var j = 0; j < k; j++)
        {
          result[i, rand.Next(0, particles.Count - 1)] = true;
        }
      }

      for (var j = 0; j < particles.Count; j++)
      {
        var particle = particles[j];
        particle.Neighbours = new List<Particle>();

        for (var i = 0; i < particles.Count; i++)
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
        var min = ParticleSwarm.ArgMin(particle.Neighbours);
        min.PreviousBest.CopyTo(particle.LocalBest, 0);
        particle.LocalBestValue = min.PreviousBestValue;
      }
    }
  }
}