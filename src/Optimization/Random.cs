using System;
using MersenneTwister;

namespace Optimization
{
  public static class RandomExtension
  {
    /// <summary>
    /// Generates a random number in [left, right] using a Mersenne Twister
    /// MT19937 with a 32 bit word.
    /// If left > right the number is generated inside [right, left].
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double Uniform(double left = 0.0, double right = 1.0)
    {
      return Uniform(MTRandom.Create(
        MTEdition.Original_19937), left, right);
    }

    /// <summary>
    /// Generates a random number in [left, right] using a provided instance of
    /// Random. If left > right the number is generated inside [right, left].
    /// </summary>
    /// <param name="random"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double Uniform(Random random, double left = 0, 
      double right = 1)
    {
      if (left < right) return left + random.NextDouble() * (right - left);
      
      return right + random.NextDouble() * (left - right);
    }
  }
}