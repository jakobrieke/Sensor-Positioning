using System.Linq;
using LinearAlgebra;
using static System.Math;

namespace Optimization
{
  public class SphereFunction : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x, 2));
    }
  }

  public class F2 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Aggregate((r, x) => Abs(r) + Abs(x)) +
             position.Aggregate((r, x) => Abs(r) * Abs(x));
    }
  }

  public class F3 : Objective
  {
    public override double Eval(Vector position)
    {
      var result = 0.0;
      
      for (var i = 0; i < position.Count; i++)
      {
        result += Pow(position.GetRange(0, i).Sum(), 2);
      }

      return result;
    }
  }
  
  /// <summary>
  /// Rosenbrock function (unimodal for D = 2 and 3, multiple minima in higher
  /// dimensions.
  /// </summary>
  public class F5 : Objective
  {
    public override double Eval(Vector position)
    {
      var result = 0.0;
      
      for (var i = 0; i < position.Count - 1; i++)
      {
        result += 100 * Pow(position[i + 1] - Pow(position[i], 2), 2) +
                  Pow(position[i] - 1, 2);
      }

      return result;
    }
  }
  
  public class F6 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x + 0.5, 2));
    }
  }
  
  public class F7 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Select((x, i) => 
        i * Pow(x, 4) 
        + new MathNet.Numerics.Random.MersenneTwister().NextDouble()).Sum();
    }
  }
}