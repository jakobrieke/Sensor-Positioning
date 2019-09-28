using System;
using System.Linq;
using LinearAlgebra;
using static System.Math;

namespace Optimization
{
  /// <summary>
  /// Sphere function, minimum is 0.
  /// </summary>
  public class SphereFunction : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x, 2));
    }
  }

  /// <summary>
  /// Schwefel 2.22 function, minimum is 0.
  /// </summary>
  public class F2 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Aggregate((r, x) => Abs(r) + Abs(x)) +
             position.Aggregate((r, x) => Abs(r) * Abs(x));
    }
  }

  /// <summary>
  /// Schwefel 1.2 function, minimum is 0.
  /// </summary>
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
  /// Schwefel 2.21 function, minimum is 0.
  /// </summary>
  public class F4 : Objective
  {
    public override double Eval(Vector position)
    {
      var max = double.PositiveInfinity;
      
      foreach (var x in position)
      {
        if (x < max) max = Abs(x);
      }
      
      return max;
    }
  }
  
  /// <summary>
  /// Rosenbrock function (unimodal for D = 2 and 3, multiple minima in higher
  /// dimensions, minimum is 0
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
  
  /// <summary>
  /// Step function, minimum is 0.
  /// </summary>
  public class F6 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Floor(Pow(x + 0.5, 2)));
    }
  }
  
  /// <summary>
  /// Noisy Quartic function, minimum is 0.
  /// </summary>
  public class F7 : Objective
  {
    private readonly Random _random = 
      new MathNet.Numerics.Random.MersenneTwister();
    
    public override double Eval(Vector position)
    {
      return position.Select((x, i) => 
        i * Pow(x, 4)).Sum() + _random.NextDouble();
    }
  }

  /// <summary>
  /// Schwefel 2.26 function, minimum is 0.
  /// </summary>
  public class F8 : Objective
  {
    public override double Eval(Vector position)
    {
      return - position.Sum(x => x * Sin(Sqrt(Abs(x)))) 
             + position.Count * 418.98288727243369;
    }
  }

  /// <summary>
  /// Rastrigin function, minimum is 0.
  /// </summary>
  public class F9 : Objective
  {
    public override double Eval(Vector position)
    {
      return position.Sum(x => Pow(x, 2) - 10 * Cos(2 * PI * x) + 10);
    }
  }
  
  /// <summary>
  /// Ackley function, minimum is 0.
  /// </summary>
  public class F10 : Objective
  {
    public override double Eval(Vector position)
    {
      var d = position.Count;
      return -20
             * Exp(-0.2 * Sqrt(1.0 / d * position.Sum(x => x * x)))
             - Exp(1.0 / d * position.Sum(x => Cos(2 * PI * x)))
             + 20 + E;
    }
  }

  /// <summary>
  /// Griewank function, , minimum is 0.
  /// </summary>
  public class F11 : Objective
  {
    public override double Eval(Vector position)
    {
      var product = 1.0;
      
      for (var i = 0; i < position.Count; i++)
      {
        product *= Cos(position[i] / Sqrt(i + 1));
      }

      return 1.0 / 4000 * position.Sum(x => x * x) - product + 1;
    }
  }
  
  /// <summary>
  /// Penalized function 1, minimum is 0.
  /// </summary>
  public class F12 : Objective
  {
    public static double U(double x, double a, double k, double m)
    {
      if (x > a)
        return k * Pow(x - 1, m);
      
      if (-a <= x && x <= a)
        return 0;
      
      return k * Pow(-x - a, m);
    }

    private static double Y(double x)
    {
      return 1 + 0.25 * (x + 1);
    }

    public static double Sin2(double x)
    {
      return Pow(Sin(x), 2);
    }
    
    public override double Eval(Vector position)
    {
      var sum = 0.0;
      
      for (var i = 0; i < position.Count - 1; i++)
      {
        sum += Pow(Y(position[i]) - 1, 2)
               * (1 + 10 * Sin2(PI * Y(position[i + 1])));
      }

      return PI / position.Count
             * (10 * Sin2(PI * Y(position[0])) + sum + 
                Pow(Y(position.Last()) + 1, 2))
             + position.Sum(x => U(x, 10, 100, 4));
    }
  }

  /// <summary>
  /// Penalized function two, minimum is 0.
  /// </summary>
  public class F13 : Objective
  {
    public override double Eval(Vector position)
    {
      var last = position[position.Count - 1];
      var sum = 0.0;
      
      for (var i = 0; i < position.Count - 1; i++)
      {
        sum += Pow(position[i] - 1, 2) 
               * (1 + F12.Sin2(3 * PI * position[i + 1]));
      }

      return 0.1 * F12.Sin2(3 * PI * position[0])
             + sum
             + Pow(last - 1, 2) * (1 + F12.Sin2(2 * PI * last))
             + position.Sum(x => F12.U(x, 5, 100, 4));
    }
  }
}