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

    protected abstract void Update();

    /// <summary>
    /// Does n iterations
    /// </summary>
    /// <param name="n">The number of iterations to run. Defaults to 1.</param>
    public void Iterate(uint n)
    {
      for (var i = 0; i < n; i++) Iterate();
    }

    /// <summary>
    /// Run one iteration on 
    /// </summary>
    public void Iterate()
    {
      Update();
      Iteration++;
    }

    /// <summary>
    /// Get the best solution found so far.
    /// </summary>
    /// <returns></returns>
    public abstract Point Best();
  }
}