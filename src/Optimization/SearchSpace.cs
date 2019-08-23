using System;
using System.Linq;

namespace Optimization
{
  /// <summary>
  /// A SearchSpace is a n-dimensional space with a left and right
  /// border for each dimension, used to limit the solution space of an
  /// objective function to a hypercube.
  /// </summary>
  public struct SearchSpace
  {
    public readonly int Dimension;
    public readonly double[][] Intervals;

    public SearchSpace(int dimension, double size) : this()
    {
      Dimension = dimension;
      Intervals = new double[dimension][];

      for (var i = 0; i < dimension; i++)
      {
        Intervals[i] = new[] {-size, size};
      }
    }

    public SearchSpace(int dimension, double left, double right)
    {
      if (left > right)
      {
        throw new ArgumentException(
          "Left border must be greater than right border.");
      }

      Dimension = dimension;
      Intervals = new double[dimension][];

      for (var i = 0; i < dimension; i++)
      {
        Intervals[i] = new[] {left, right};
      }
    }

    public SearchSpace(double[][] intervals)
    {
      Intervals = intervals;
      Dimension = intervals.Length;
    }

    /// <summary>
    /// Get a random position inside the SearchSpace.
    /// </summary>
    /// <returns>
    /// An array with the same dimension as the SearchSpace.
    /// </returns>
    public double[] RandPos()
    {
      return Intervals.Select(i => MTRandom.Uniform(i[0], i[1])).ToArray();
    }
  }
}