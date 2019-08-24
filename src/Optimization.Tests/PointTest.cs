using System;
using System.Collections.Generic;
using LinearAlgebra;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture]
  public class PointTest
  {
    [Test]
    public static void TestCompareTo()
    {
      var sp = new SearchSpace(3, 5);
      var fct = new SphereFunction();
      var points = new List<Point>(10);
      
      for (var i = 0; i < 10; i++)
      {
        var pos = new Vector(sp.RandPos());
        points.Add(new Point(pos, fct.Eval(pos)));
      }
      
      points.Sort();

      var previousValue = points[0].Value;
      foreach (var point in points)
      {
        Console.WriteLine(point.Value);
        Assert.True(previousValue <= point.Value);
        previousValue = point.Value;
      }
    }
  }
}