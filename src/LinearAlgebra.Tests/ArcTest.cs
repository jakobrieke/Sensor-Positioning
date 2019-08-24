using System;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public static class ArcTest
  {
    [Test]
    public static void TestToPolygon()
    {
      Console.WriteLine("\nTest Arc.ToPolygon(..)");
      var r1 = new Arc(0, 0, 1, 90).ToPolygon() == new Polygon
      {
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0.866025403784439, 0.5),
        new Vector2(0.5, 0.866025403784439),
        new Vector2(6.12303176911189E-17, 1)
      };
      Console.WriteLine("01: " + r1);

      // Todo: Fix bug when arc radius is >= 180 degrees
      var arc2 = new Arc(0, 0, 1, 190).ToPolygon(0);
      Console.WriteLine("02: " + false);

      var r3 = new Arc(-1, -1, 1, 90, 180)
                 .ToPolygon(4) == new Polygon
               {
                 new Vector2(-1, -1), new Vector2(-2, -1),
                 new Vector2(-1.95105651629515, -1.30901699437495),
                 new Vector2(-1.80901699437495, -1.58778525229247),
                 new Vector2(-1.58778525229247, -1.80901699437495),
                 new Vector2(-1.30901699437495, -1.95105651629515),
                 new Vector2(-1, -2)
               };
      Console.WriteLine("03: " + r3);
    }
  }
}