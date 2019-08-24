using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public class TestVector
  {
    private static void TestCases<T>(IReadOnlyList<T> cases)
    {
      for (var i = 0; i < cases.Count; i += 2)
      {
        Console.WriteLine($"{cases[i]} == {cases[i + 1]}");
        Assert.True(cases[i].Equals(cases[i + 1]));
      }
    }
    
    [Test]
    public static void TestAddition()
    {
      var v1 = new Vector(1, 2, 3);
      TestCases(new[]
      {
        Vector.Zero(3) + v1, new Vector(1, 2, 3),
        Vector.One(3) + v1, new Vector(2, 3, 4)
      });
    }
    
    [Test]
    public static void TestSubtraction()
    {
      var v1 = new Vector(1, 2, 3);
      TestCases(new[]
      {
        Vector.Zero(3) - v1, new Vector(-1, -2, -3),
        Vector.One(3) - v1, new Vector(0, -1, -2)
      });
    }
    
    
    [Test]
    public static void TestMultiplication()
    {
      var v1 = new Vector(1, 2, 3);
      TestCases(new[]
      {
        Vector.Zero(3) * v1, new Vector(0, 0, 0),
        Vector.One(3) * v1, new Vector(1, 2, 3),
        v1 * 3, new Vector(3, 6, 9),
        0.5 * v1, new Vector(0.5, 1, 1.5)
      });
    }
    
    [Test]
    public static void TestDivision()
    {
      var v1 = new Vector(1, 2, 3.0);
      TestCases(new[]
      {
        Vector.One(3) / v1, new Vector(1, 0.5, 1 / 3.0)
      });

      // -- Test division by zero
      
      Assert.Throws<DivideByZeroException>(() =>
      {
        var r = v1 / Vector.Zero(3);
      });
      Assert.Throws<DivideByZeroException>(() =>
      {
        var r = v1 / 0;
      });
    }
    
    [Test]
    public static void TestDot()
    {
      var v1 = new Vector(1, 2, 3);
      TestCases(new[]
      {
        Vector.Dot(Vector.Zero(3), v1), 0,
        Vector.Dot(Vector.One(3), v1), 6
      });
    }

    [Test]
    public static void TestDistance()
    {
      var v1 = new Vector(1, 2, 3);
      TestCases(new[]
      {
        Vector.Distance(Vector.Zero(3), Vector.Zero(3)), 0,
        Vector.Distance(Vector.Zero(3), Vector.One(3)), Math.Sqrt(3),
        Vector.Distance(Vector.One(3), Vector.Zero(3)), Math.Sqrt(3),
//        Vector.Distance(Vector.Zero(3), v1), 3.74165738677394,
//        Vector.Distance(Vector.One(3), v1), 6
      });
    }
  }
}