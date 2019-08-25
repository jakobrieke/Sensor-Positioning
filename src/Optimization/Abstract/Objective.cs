using System;
using LinearAlgebra;

namespace Optimization
{
  public abstract class Objective
  {
    public uint Evaluations;
    
    public abstract double Eval(Vector position);

    public static implicit operator Func<double[], double>(Objective func)
    {
      return position => func.Eval(position);
    }
  }
}