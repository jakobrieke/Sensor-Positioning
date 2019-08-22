using System;
using System.Linq;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public static class VectorNTest
  {
    [Test]
    public static void TestAdd()
    {
      var v1 = new[] {0.0, 0, 0};
      var v2 = new[] {1, 1.5, -10.1};

      Assert.True(VectorN.Add(v1, v2).SequenceEqual(
        new[] {1, 1.5, -10.1}));
    }

    [Test]
    public static void TestSubtract()
    {
      var v1 = new[] {0.0, 0, 0};
      var v2 = new[] {1, 1.5, -10.1};

      Assert.True(VectorN.Subtract(v1, v2).SequenceEqual(
        new[] {-1, -1.5, 10.1}));
    }

    [Test]
    public static void TestMultiply()
    {
      var v1 = new[] {-10, 0, 1.5};

      Assert.True(VectorN.Multiply(v1, 2).SequenceEqual(
        new[] {-20.0, 0, 3}));
    }

    [Test]
    public static void TestDivide()
    {
      var v1 = new[] {.75, -0.375, 1.5};

      Assert.True(VectorN.Divide(v1, 1.5).SequenceEqual(
        new[] {0.5, -0.25, 1}));
    }

    [Test]
    public static void TestDistance()
    {
      var v1 = new[] {0.0, 0, 0, 0};
      var v2 = new[] {1.0, 1, 1, 1};
      Assert.True(Math.Abs(VectorN.Distance(v1, v2) - 2.0) < Defs.Precision);
    }
  }
}