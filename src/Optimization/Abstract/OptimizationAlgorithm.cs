namespace Optimization
{
  public abstract class OptimizationAlgorithm
  {
    public ObjectiveFunction Fitness;
    
    public SearchSpace SearchSpace;

    public OptimizationAlgorithm(ObjectiveFunction fitness, 
      SearchSpace searchSpace)
    {
      Fitness = fitness;
      SearchSpace = searchSpace;
    }

    public abstract void Init();
    
    public abstract void Update();
  }
}