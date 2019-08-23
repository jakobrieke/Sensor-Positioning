using System;
using LinearAlgebra;

namespace Optimization
{
  public abstract class ObjectiveFunction
  {
    public abstract double Eval(Vector position);

    public static implicit operator Func<double[], double>(
      ObjectiveFunction func)
    {
      return position => func.Eval(position);
    }
  }
}