using System.Collections.Generic;

namespace Optimization
{
  /// <summary>
  /// A Particle in terms of Particle Swarm Optimization is a point inside a
  /// n-dimensional space with a velocity applied to it which indicates where
  /// and with what speed the particle is currently moving inside the space.
  /// A particle is used to evaluate the search space at a given position
  /// with a given fitness function which is to be optimized.
  /// </summary>
  public class Particle
  {
    /// <summary>
    /// The current position of the particle.
    /// </summary>
    public double[] Position;
    /// <summary>
    /// The position the particle had before the last update.
    /// </summary>
    public double[] LastPosition;
    /// <summary>
    /// The best position which the particle found so far.
    /// </summary>
    public double[] PreviousBest;
    /// <summary>
    /// The current velocity with which the particle moves through the search
    /// space.
    /// </summary>
    public double[] Velocity;
    /// <summary>
    /// The best position inside the neighbourhood. 
    /// </summary>
    public double[] LocalBest;
    /// <summary>
    /// The evaluated value of the current position.
    /// </summary>
    public double PositionValue;
    /// <summary>
    /// The evaluated value of the previously best value.
    /// </summary>
    public double PreviousBestValue;
    /// <summary>
    /// The evaluated value of the best position inside the neighbourhood.
    /// </summary>
    public double LocalBestValue;
    /// <summary>
    /// A list of neighbours of the particle, the particle knows all values
    /// of its neighbours.
    /// </summary>
    public List<Particle> Neighbours;
  }
}