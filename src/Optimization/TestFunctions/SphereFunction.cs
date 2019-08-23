using System;
using System.Linq;
using LinearAlgebra;
using NUnit.Framework;
using static System.Math;

namespace Optimization
{
  public class SphereFunction : ObjectiveFunction
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x, 2));
    }

    [Test]
    public static void Test()
    {
      var f = new SphereFunction();
      var value = f.Eval(new Vector(1.0, 1, 1, 1));
      Console.WriteLine(value);
      Assert.True(value == 4);
    }
  }

  public class F2 : ObjectiveFunction
  {
    public override double Eval(Vector position)
    {
      return position.Aggregate((r, x) => Abs(r) + Abs(x)) +
             position.Aggregate((r, x) => Abs(r) * Abs(x));
    }
  }

  public class F3 : ObjectiveFunction
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
  public class F5 : ObjectiveFunction
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
  
  public class F6 : ObjectiveFunction
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x + 0.5, 2));
    }
  }
  
  public class F7 : ObjectiveFunction
  {
    public override double Eval(Vector position)
    {
      return position.Select((x, i) => 
        i * Pow(x, 4) + MTRandom.Uniform(0, 0.99999999999999)).Sum();
    }
  }
}