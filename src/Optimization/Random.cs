namespace Optimization
{
  public static class MTRandom
  {
    /// <summary>
    /// Create a uniform random number inside the interval [left, right].
    /// The random number generation is uses a Mersenne Twister PRNG.
    /// If the left border is greater than the right border they are
    /// automatically switched.
    /// </summary>
    /// <param name="left">The left interval border. Default to 0.0</param>
    /// <param name="right">The right interval border. Default to 1.0</param>
    /// <returns></returns>
    public static double Uniform(double left = 0.0, double right = 1.0)
    {
      if (left > right)
      {
        var buffer = left;
        left = right;
        right = buffer;
      }
      
      var rand = MersenneTwister.MTRandom.Create();
      return left + rand.NextDouble() * (right - left);
    }
  }
}