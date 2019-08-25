namespace Optimization
{
  /// <summary>
  /// Base class for an iterative optimization algorithm.
  /// </summary>
  public abstract class Optimization
  {
    public Objective Fitness;
    
    public SearchSpace SearchSpace;

    public int Iteration { get; private set; }

    public Optimization(Objective fitness, 
      SearchSpace searchSpace)
    {
      Fitness = fitness;
      SearchSpace = searchSpace;
      Iteration = -1;
    }

    protected void ResetIterations()
    {
      Iteration = 0;
    }

    public abstract void Init();

    public abstract void Update();

    /// <summary>
    /// Run n iterations on an initialized swarm.
    /// </summary>
    /// <param name="n">The number of iterations to run. Defaults to 1.</param>
    public void Iterate(uint n = 1)
    {
      for (var i = 0; i < n; i++) Update();
    }

    /// <summary>
    /// Get the best solution found so far.
    /// </summary>
    /// <returns></returns>
    public abstract Point Best();
  }
}