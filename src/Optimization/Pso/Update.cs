using LinearAlgebra;
using static LinearAlgebra.VectorN;
using static System.Math;

namespace Optimization
{
  public class Update
  {
    public static void UpdateSpso2006(Particle p, double w, double c1, double c2)
    {
      for (var i = 0; i < p.Velocity.Length; i++)
      {
        p.Velocity[i] = 
          w * p.Velocity[i] + 
          MTRandom.Uniform(0, c1) * (p.PreviousBest[i] - p.Position[i]) + 
          MTRandom.Uniform(0, c2) * (p.LocalBest[i] - p.Position[i]);
        p.Position[i] += p.Velocity[i];
      }
    }
    
    public static void UpdateSpso2007(Particle p, double w, double c1, double c2)
    {
      if (p.LocalBestValue.Equals(p.PreviousBestValue))
      {
        for (var i = 0; i < p.Velocity.Length; i++)
        {
          p.Velocity[i] = 
            p.Velocity[i]
            + MTRandom.Uniform(0, c1) * (p.PreviousBest[i] - p.Position[i]);
          p.Position[i] += p.Velocity[i];
        }
      }
      else UpdateSpso2006(p, w, c1, c2);
    }
    
    public static void UpdateSpso2011(Particle particle, double w, double c1, 
      double c2)
    {
      var value = Add(particle.PreviousBest, particle.LocalBest);
      value = Subtract(value, Multiply(particle.Position, 2f));

      if (Abs(particle.LocalBestValue - particle.PreviousBestValue) 
          < Defs.Precision)
      {
        value = Subtract(particle.PreviousBest, particle.Position);
      }

      var g = Add(particle.Position, Multiply(Divide(value, 3f), c1));
      var x = new double[particle.Position.Length];

      for (var i = 0; i < particle.Position.Length; i++)
      {
        x[i] = MTRandom.Uniform(g[i], Abs(g[i] - particle.Position[i]));
      }

      var lastPosition = new double[particle.Position.Length];
      particle.Position.CopyTo(lastPosition, 0);

      particle.Position = Add(Multiply(particle.Velocity, w), x);
      particle.Velocity = Subtract(
        Add(Multiply(particle.Velocity, w), x), lastPosition);
    }
  }
}