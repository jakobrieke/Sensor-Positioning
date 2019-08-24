using System.Collections.Generic;
using LibOptimization.Optimization;

namespace Optimization
{
  /// <summary>
  /// A wrapper class to use the standard particle swarm optimization algorithms
  /// with LibOptimization.
  /// </summary>
  public abstract class SpsoWrapper : AbsOptimization
  {
    public ParticleSwarm Swarm;

    public override void Init()
    {
      Swarm.Init();
    }
    
    public override bool Iterate(int iteration = 0)
    {
      Swarm.Iterate(iteration);
      _iteration += iteration;
      return true;
    }

    public override bool IsRecentError()
    {
      return false;
    }

    public override LoPoint Result => 
      new LoPoint(ObjectiveFunction, Swarm.GlobalBest);
    
    public override List<LoPoint> Results { get; }
    
    public override int Iteration 
    {
      get => Swarm.Iteration;
      set {}
    }
  }
  
  public class Spso2006Wrapper : SpsoWrapper
  {
    public Spso2006Wrapper(AbsObjectiveFunction objective, double[][] bounds)
    {
      ObjectiveFunction = objective;

      var obj = new SimpleObjective(v => ObjectiveFunction.F(v));
      Swarm = new StandardPso2006(new SearchSpace(bounds), obj);
    }
  }
  
  public class Spso2007Wrapper : SpsoWrapper
  {
    public Spso2007Wrapper(AbsObjectiveFunction objective, double[][] bounds)
    {
      ObjectiveFunction = objective;
      
      var obj = new SimpleObjective(v => ObjectiveFunction.F(v));
      Swarm = new StandardPso2007(new SearchSpace(bounds), obj);
    }
  }
  
  public class Spso2011Wrapper : SpsoWrapper
  {
    public Spso2011Wrapper(AbsObjectiveFunction objective, double[][] bounds)
    {
      ObjectiveFunction = objective;
      
      var obj = new SimpleObjective(v => ObjectiveFunction.F(v));
      Swarm = new StandardPso2011(new SearchSpace(bounds), obj);
    }
  }
}