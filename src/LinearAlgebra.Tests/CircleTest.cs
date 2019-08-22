using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public static class CircleTest
  {
    [Test]
    public static void TestToPolygon()
    {
      Console.WriteLine("\nTest Circle.ToPolygon(): ");

      var p1 = new Circle(9, 6, 3).ToPolygon();
      var tests = new List<bool>
      {
        p1.Count == 5,
        p1[0] == new Vector2(12, 6),
        p1[1] == new Vector2(9, 9),
        p1[2] == new Vector2(6, 6),
        p1[3] == new Vector2(9, 3)
      };

      var p2 = new Circle(9, 6, 3).ToPolygon(2);
  //        polygon.ForEach(vertex => Console.WriteLine(vertex));

      tests.Add(new Vector2(12, 6) == p2[0]);
      tests.Add(new Vector2(11.12132, 8.121321) == p2[1]);
      tests.Add(new Vector2(9, 9) == p2[2]);
      tests.Add(new Vector2(6.87868, 8.121321) == p2[3]);
      tests.Add(new Vector2(6, 6) == p2[4]);
      tests.Add(new Vector2(6.87868, 3.87868) == p2[5]);
      tests.Add(new Vector2(9, 3) == p2[6]);
      tests.Add(new Vector2(11.12132, 3.87868) == p2[7]);

      for (var i = 0; i < tests.Count; i++)
      {
        Console.WriteLine(i + ": " + tests[0]);
      }
    }

    [Test]
    public static void TestExternalTangents()
    {
      Console.WriteLine("\nTest Circle.ExternalTangents()");

      var s1 = new Vector2(0, 0);
      var s2 = new Vector2(.5, .6);
      var s3 = new Vector2(.5, .51);
      var o1 = new Circle(.5, .5, .1);
      var o2 = new Circle(-.3, -.3, .1);
      var o3 = new Circle(.5, 0, .1);
      var o4 = new Circle(-.15, 0, .1);

      var t1 = Circle.ExternalTangents(s1, o1);
      var t2 = Circle.ExternalTangents(s1, o2);
      var t3 = Circle.ExternalTangents(s2, o1);
      var t4 = Circle.ExternalTangents(s3, o1);
      var t5 = Circle.ExternalTangents(s1, o3);
      var t6 = Circle.ExternalTangents(s1, o4);

      var tests = new List<bool>
      {
        string.Join("; ", t1) == "0, 0; 0.42, 0.56; 0, 0; 0.56, 0.42",
        string.Join("; ", t2) == "0, 0; -0.214614906239706, " +
        "-0.352051760426961; 0, 0; -0.352051760426961, -0.214614906239706",
        string.Join("; ", t3) == "0.5, 0.6; 0.5, 0.6; 0.5, 0.6; 0.5, 0.6",
        string.Join("; ", t4) == "",
        string.Join("; ", t5) == "0, 0; 0.48, 0.0979795897113271; 0, 0; " +
        "0.48, -0.0979795897113271",
        string.Join("; ", t6) == "0, 0; -0.0833333333333333, " +
        "-0.074535599249993; 0, 0; -0.0833333333333333, 0.074535599249993"
      };

      for (var i = 0; i < tests.Count; i++)
      {
        Console.WriteLine(i + ": " + tests[i]);
      }
    }
  }
}