using System;

namespace Util
{
  /// <summary>
  /// XORShift pseudo random number generator.
  /// Inherits System.Random
  /// </summary>
  /// <remarks>
  /// Reference:
  /// George Marsaglia, "XorShift RNGs", Journal of Statistical Software
  /// Vol. 8, Issue 14, Jul 2003
  /// </remarks>
  public class XorShiftPRNG : Random
  {
    public static readonly XorShiftPRNG Random = new XorShiftPRNG();
    
    // DefaultParameter
    private uint _x = 123456789;
    private uint _y = 362436069;
    private uint _z = 521288629;
    private uint _w = 88675123;
    private uint _t;

    /// <summary>
    /// Constructor with reference seed
    /// </summary>
    /// <remarks></remarks>
    public XorShiftPRNG()
    {
    }

    /// <summary>
    /// Constructor with seed
    /// </summary>
    /// <param name="seed">seed for random algorithm</param>
    /// <remarks></remarks>
    public XorShiftPRNG(uint seed)
    {
      SetSeed(seed);
    }

    /// <summary>
    /// Initializes the random generator with a default seed.
    /// </summary>
    /// <remarks></remarks>
    public void SetDefaultSeed()
    {
      _x = 123456789;
      _y = 362436069;
      _z = 521288629;
      _w = 88675123;
      _t = 0;
    }

    /// <summary>
    /// Set random seed with itinitialize
    /// </summary>
    /// <param name="x">random parameter x</param>
    /// <param name="y">random parameter y</param>
    /// <param name="z">random parameter z</param>
    /// <param name="w">random parameter w</param>
    public void SetSeed(uint x, uint y, uint z, uint w)
    {
      // "The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by
      // reference
      if (x == 0 && y == 0 && z == 0 && w == 0) SetDefaultSeed();
      else
      {
        _x = x;
        _y = y;
        _z = z;
        _w = w;
      }
    }

    /// <summary>
    /// Set simple random seed with itinitialize
    /// </summary>
    /// <param name="seed">seed for random algorithm</param>
    /// <remarks></remarks>
    public void SetSeed(uint seed)
    {
      // Init parameter and rotate seed.
      // 全パラメータにseedの影響を与えないと初期の乱数が同じ傾向になる。8bitずつ回転左シフト
      SetDefaultSeed();
      
      _x ^= RotateLeftShiftForUInteger(seed, 8);
      _y ^= RotateLeftShiftForUInteger(seed, 16);
      _z ^= RotateLeftShiftForUInteger(seed, 24);
      _w ^= seed; // Set seed
      _t = 0;

      // "The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by
      // reference
      if (_x == 0 && _y == 0 && _z == 0 && _w == 0) SetDefaultSeed();
    }

    /// <summary>
    /// Override Next
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int Next()
    {
      return Convert.ToInt32(Xor128() & 0x7FFFFFFF);
    }

    /// <summary>
    /// Override Next
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int Next(int maxValue)
    {
      return Next(0, maxValue);
    }

    /// <summary>
    /// Override Next
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int Next(int minValue, int maxValue)
    {
      return Convert.ToInt32(minValue + Xor128() % (maxValue - minValue));
    }

    /// <summary>
    /// Override NextDouble
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override double NextDouble()
    {
      return Xor128() / (double) uint.MaxValue;
    }

    /// <summary>
    /// Random double with range
    /// </summary>
    /// <param name="ai_min"></param>
    /// <param name="ai_max"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public double NextDouble(double ai_min, double ai_max)
    {
      return Math.Abs(ai_max - ai_min) * base.NextDouble() + ai_min;
    }

    /// <summary>
    /// for random seed
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public static uint GetTimeSeed()
    {
      return Convert.ToUInt32(
        DateTime.Now.Millisecond * DateTime.Now.Minute * DateTime.Now.Second);
    }

    /// <summary>
    /// Xor128
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// C Source by reference
    ///  t=(xˆ(x left shift 11));
    ///  x=y;
    ///  y=z;
    ///  z=w;
    ///  return( w=(wˆ(w right shift 19))ˆ(tˆ(t right shift 8)) )
    /// </remarks>
    private uint Xor128()
    {
      _t = _x ^ (_x << 11);
      _x = _y;
      _y = _z;
      _z = _w;
      _w = _w ^ (_w >> 19) ^ _t ^ (_t >> 8);

      return _w;
    }

    /// <summary>
    /// Rotate Shift
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    private static uint RotateLeftShiftForUInteger(uint value, int leftShit)
    {
      if (32 - leftShit <= 0) return value;

      // keeping upper bits
      uint maskBit = 0;
      for (var i = 32 - leftShit; i <= 32 - 1; i++)
      {
        maskBit = Convert.ToUInt32(maskBit + (Math.Pow(2, i)));
      }
      
      var upperBit = value & maskBit;
      upperBit >>= 32 - leftShit;

      // left shift
      var temp = value << leftShit;

      // rotate upper bits
      temp |= upperBit;
      return temp;
    }
  }

  /// <summary>
  /// XorShift random algorithm singleton
  /// </summary>
  /// <remarks>
  /// </remarks>
  public static class XorShiftPRNGSingleton
  {
    private static readonly XorShiftPRNG Random = new XorShiftPRNG();
    
    [Obsolete("Please use XorShiftPRNG.Random")]
    public static XorShiftPRNG GetInstance()
    {
      return Random;
    }
  }
}