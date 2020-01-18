using System;
using NUnit.Framework;
using Poly2Tri.Triangulation.Polygon;
using Poly2TriePolygon = Poly2Tri.Triangulation.Polygon.Polygon;

namespace LinearAlgebra
{
  [TestFixture]
  public class Poly2TriTest
  {
    /// <summary>
    /// Test how the Poly2Tri API works.
    /// </summary>
    [Test]
    public void TestPoly2TriApi()
    {
      var polygon = new Poly2Tri.Triangulation.Polygon.Polygon(new[]
      {
        new PolygonPoint(0, 0), new PolygonPoint(1, 0),
        new PolygonPoint(1, 1), new PolygonPoint(0, 1)
      });
      var hole1 = new Poly2Tri.Triangulation.Polygon.Polygon(new []
      {
        new PolygonPoint(0.1, 0.1), new PolygonPoint(0.2, 0.1),
        new PolygonPoint(0.2, 0.2), new PolygonPoint(0.1, 0.2)
      });
      var hole2 = new Poly2Tri.Triangulation.Polygon.Polygon(new []
      {
        new PolygonPoint(0.4, 0.4), new PolygonPoint(0.5, 0.4),
        new PolygonPoint(0.5, 0.5), new PolygonPoint(0.4, 0.5)
      });
      polygon.AddHole(hole1);
      polygon.AddHole(hole2);
      Poly2Tri.P2T.Triangulate(polygon);
      
      foreach (var triangle in polygon.Triangles)
      {
        Console.WriteLine(triangle);
      }
    }

    /// <summary>
    /// Test what happens if all polygon points are aligned.
    /// </summary>
    [Test]
    public void TestSingleLine()
    {
      var polygon = new Poly2TriePolygon(new[]
      {
        new PolygonPoint(0, 0), new PolygonPoint(1, 0),
        new PolygonPoint(1, 1), new PolygonPoint(0, 1)
      });
      var hole1 = new Poly2TriePolygon(new []
      {
        new PolygonPoint(0.1, 0.1), new PolygonPoint(0.2, 0.1),
        new PolygonPoint(0.15, 0.1)
      });
      polygon.AddHole(hole1);
      Poly2Tri.P2T.Triangulate(polygon);
      
      foreach (var triangle in polygon.Triangles)
      {
        Console.WriteLine(triangle);
      }
    }
  }
}