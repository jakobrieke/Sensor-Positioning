using LinearAlgebra;

namespace Optimization
{
  public abstract class SwarmOptimization : StochisticOptimization
  {
    protected SwarmOptimization(Objective fitness, SearchSpace searchSpace) : 
      base(fitness, searchSpace)
    {
    }

    public abstract void Init(Vector[] startPositions);

    /// <summary>
    /// Initializes the swarm optimization algorithm with n particles.
    /// </summary>
    /// <param name="swarmSize">The size of the swarm "n".</param>
    public void Init(int swarmSize)
    {
      var startPositions = new Vector[swarmSize];
      for (var i = 0; i < swarmSize; i++)
      {
        startPositions[i] = SearchSpace.RandPos(Random);
      }
      Init(startPositions);
    }

    /// <summary>
    /// Initializes the swarm optimization algorithm with a swarm size of 40.
    /// </summary>
    public override void Init()
    {
      Init(40);
    }
  }
}