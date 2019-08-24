using System;
using System.Collections.Generic;
using System.Linq;
using LibOptimization.Optimization;
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

    public static implicit operator ObjectiveFunction(
      Func<double[], double> func)
    {
      return new SimpleObjective(position => func(position.ToArray()));
    }

    internal class AbsObjectiveFuncWrapper : AbsObjectiveFunction
    {
      public ObjectiveFunction Func;

      public AbsObjectiveFuncWrapper(ObjectiveFunction func)
      {
        Func = func;
      }

      public override int Dimension()
      {
        return -1;
      }

      public override double F(List<double> x)
      {
        return Func.Eval(x.ToArray());
      }

      public override List<double> Gradient(List<double> x)
      {
        throw new NotImplementedException();
      }

      public override List<List<double>> Hessian(List<double> x)
      {
        throw new NotImplementedException();
      }
    }
    
    public static implicit operator AbsObjectiveFunction(ObjectiveFunction func)
    {
      return new AbsObjectiveFuncWrapper(func);
    }

    public static implicit operator ObjectiveFunction(AbsObjectiveFunction func)
    {
      return new SimpleObjective(position => func.F(position.ToList()));
    }
  }

  public class SimpleObjective : ObjectiveFunction
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