using System.Collections.Generic;
using LibOptimization.Optimization;

namespace Optimization.LibOptimizationWrapper
{
  public class ObjectiveWrapper : AbsObjectiveFunction
  {
    public Objective Objective;
    private int _dimension;

    public ObjectiveWrapper(Objective objective, int dimension)
    {
      Objective = objective;
      _dimension = dimension;
    }

    public override int Dimension()
    {
      return _dimension;
    }

    public override double F(List<double> x)
    {
      return Objective.Eval(x.ToArray());
    }

    public override List<double> Gradient(List<double> x)
    {
      throw new System.NotImplementedException();
    }

    public override List<List<double>> Hessian(List<double> x)
    {
      throw new System.NotImplementedException();
    }
  }
}