using System.Collections.Generic;
using LibOptimization.Optimization;

namespace LibOptimization.Optimization
{
  
  public abstract class Optimizer
  {
    public ObjectiveFunc Objective;

    protected Optimizer(ObjectiveFunc objective)
    {
      Objective = objective;
    }

    public abstract void Init();
    
    public abstract void Iterate();

    public void Iterate(int maxIterations)
    {
      for (var i = 0; i < maxIterations; i++) Iterate();
    }

    public void IterateUntil(double value)
    {
      while (GetBest() > value) Iterate();
    }

    public abstract double GetBest();
  }

  public abstract class ObjectiveFunc
  {
    public abstract double Evaluate(double[] position);

    public abstract int GetDimension();
  }

  public class LibOptObjective : AbsObjectiveFunction
  {
    private ObjectiveFunc _func;
    
    public LibOptObjective(ObjectiveFunc func)
    {
      _func = func;
    }

    public override int NumberOfVariable()
    {
      return _func.GetDimension();
    }

    public override double F(List<double> x)
    {
      return _func.Evaluate(x.ToArray());
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

  public class LibOptOptimizer : Optimizer
  {
    private readonly clsOptPSO _rawOpt;

    public LibOptOptimizer(ObjectiveFunc objective) : base(objective)
    {
      _rawOpt = new clsOptPSO(new LibOptObjective(objective));
    }

    public override void Init()
    {
      _rawOpt.Init();
    }

    public override void Iterate()
    {
      throw new System.NotImplementedException();
    }

    public override double GetBest()
    {
      throw new System.NotImplementedException();
    }
  }
  
  public class LibOptPso : Optimizer
  {
    private readonly clsOptPSO _rawOpt;

    public LibOptPso(ObjectiveFunc objective) : base(objective)
    {
      _rawOpt = new clsOptPSO(new LibOptObjective(objective));
    }

    public override void Init()
    {
      throw new System.NotImplementedException();
    }

    public override void Iterate()
    {
      throw new System.NotImplementedException();
    }

    public override double GetBest()
    {
      throw new System.NotImplementedException();
    }
  }
}