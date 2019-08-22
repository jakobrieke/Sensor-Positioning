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
    public readonly int Dimensions;
    public readonly double[][] Intervals;

    public SearchSpace(int dimensions, double size) : this()
    {
      Dimensions = dimensions;
      Intervals = new double[dimensions][];

      for (var i = 0; i < dimensions; i++)
      {
        Intervals[i] = new[] {-size, size};
      }
    }

    public SearchSpace(int dimensions, double left, double right)
    {
      if (left > right)
      {
        throw new ArgumentException(
          "Left border must be greater than right border.");
      }

      Dimensions = dimensions;
      Intervals = new double[dimensions][];

      for (var i = 0; i < dimensions; i++)
      {
        Intervals[i] = new[] {left, right};
      }
    }

    public SearchSpace(double[][] intervals)
    {
      Intervals = intervals;
      Dimensions = intervals.Length;
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