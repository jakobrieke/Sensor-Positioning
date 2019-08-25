using System;
using LinearAlgebra;

namespace Optimization
{
  public class SimpleObjective : Objective
  {
    private Func<Vector, double> _func;

    public SimpleObjective(Func<Vector, double> func)
    {
      _func = func;
    }

    public override double Eval(Vector position)
    {
      return _func(position.ToArray());
    }
  }
}