using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public static class Vector2Test
  {
    [Test]
    public static void TestAngle()
    {
      var v1 = new Vector2(0, 0);
      var tests = new Dictionary<Vector2, double>
      {
        {new Vector2(1, 0), 0},
        {new Vector2(0, 1), 90},
        {new Vector2(-1, 0), 180},
        {new Vector2(0, -1), 270},
        {new Vector2(1, 1), 45},
        {new Vector2(-1, 1), 135},
        {new Vector2(-1, -1), 225},
        {new Vector2(1, -1), 315},
        {new Vector2(.5, Math.Sqrt(3) / 2), 60},
        {new Vector2(-.5, -Math.Sqrt(3) / 2), 240},
        {new Vector2(Math.Sqrt(3) / 2, -.5), 330}
      };

      Assert.IsTrue(double.IsNaN(Vector2.Angle(v1, Vector2.Zero)));
      
      var i = 0;
      foreach (var value in tests.Keys)
      { 
        var result = Vector2.Angle(v1, value);
        Console.WriteLine($"Test {i++}: ({v1}) -> ({value}) = {result}°");
        Assert.IsTrue(Math.Abs(result - tests[value]) < 0.0000000001);
      }
    }
  }
}