using System;
using NUnit.Framework;
using Util;

/// <summary>
/// Unit test optimization, other than linear algebra
/// </summary>
// [TestClass()]
public class TestOther
{
  /// <summary>
  /// Check random library
  /// </summary>
  [Test]
  public static void Other_Random()
  {
    System.Random rand = null;
    try
    {
      rand = new XorShiftPRNG();
    }
    catch (Exception ex)
    {
      Assert.Fail("clsRandomXorshift error");
    }

    try
    {
      int temp = 123456;
      rand = new XorShiftPRNG(BitConverter.ToUInt32(
        BitConverter.GetBytes(temp), 0));
    }
    catch (Exception ex)
    {
      Assert.Fail("clsRandomXorshift seed error using positive value.");
    }

    try
    {
      int temp = -123456;
      rand = new XorShiftPRNG(BitConverter.ToUInt32(
        BitConverter.GetBytes(temp), 0));
    }
    catch (Exception ex)
    {
      Assert.Fail("clsRandomXorshift seed error using negative value.");
    }
  }
}