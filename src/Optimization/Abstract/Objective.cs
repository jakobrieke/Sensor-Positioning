using System;
using LinearAlgebra;

namespace Optimization
{
  public abstract class Objective
  {
    /// <summary>
    /// Evaluates the objective function at a given position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public abstract double Eval(Vector position);

    /// <summary>
    /// Converts an objective function into a lambda function (function
    /// delegate). 
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static implicit operator Func<double[], double>(Objective func)
    {
      return position => func.Eval(position);
    }
  }
}