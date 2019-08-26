using System;
using System.Linq;
using LinearAlgebra;
using static System.Math;

namespace Optimization
{
  public class Update
  {
    public static void UpdateSpso2006(Particle p, 
      double w, double c1, double c2, Random random)
    {
      for (var i = 0; i < p.Velocity.Length; i++)
      {
        p.Velocity[i] = w * p.Velocity[i] 
          + RandomExtension.Uniform(random, 0, c1) 
          * (p.PreviousBest[i] - p.Position[i]) 
          + RandomExtension.Uniform(random, 0, c2) 
          * (p.LocalBest[i] - p.Position[i]);
        
        p.Position[i] += p.Velocity[i];
      }
    }

    public static void UpdateSpso2007(Particle p, 
      double w, double c1, double c2, Random random)
    {
      if (p.LocalBestValue.Equals(p.PreviousBestValue))
      {
        for (var i = 0; i < p.Velocity.Length; i++)
        {
          p.Velocity[i] = p.Velocity[i]
            + RandomExtension.Uniform(random, 0, c1) 
            * (p.PreviousBest[i] - p.Position[i]);
          
          p.Position[i] += p.Velocity[i];
        }
      }
      else UpdateSpso2006(p, w, c1, c2, random);
    }

    // Todo: Fix strong bias to (0, 0, ..., 0)
    public static void UpdateSpso2011(Particle particle,
      double w, double c1, double c2, Random random)
    {
      var p = new Vector(particle.PreviousBest);
      var l = new Vector(particle.LocalBest);
      var x = new Vector(particle.Position);
      var v = new Vector(particle.Velocity);

      // If local best == previous best
      var localBestEqualsPrevious =
        Abs(particle.LocalBestValue - particle.PreviousBestValue)
        < Defs.Precision;

      var center = localBestEqualsPrevious
        ? x + c1 * (p + x) / 2
        : x + c1 * (p + l - 2 * x) / 3;

      var x2 = new Vector();
      x2.AddRange(center.Select((t, i) =>
        RandomExtension.Uniform(random, t, Abs(t - x[i]))));

      var vNew = w * v + x2 - x;
      var xNew = x + vNew;

      particle.Velocity = vNew.ToArray();
      particle.Position = xNew.ToArray();
    }
  }
}