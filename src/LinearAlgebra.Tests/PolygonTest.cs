using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using NUnit.Framework;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation.Polygon;

namespace LinearAlgebra
{
  [TestFixture]
  public class PolygonTest
  {
    private readonly Polygon _a = new Polygon(new[]
    {
      new Vector2(0, 0), new Vector2(1, 0),
      new Vector2(1, 1), new Vector2(0, 1)
    });

    private readonly Polygon _b = new Polygon(new[]
    {
      new Vector2(0.1, 0.1), new Vector2(0.1, 0.5),
      new Vector2(0.5, 0.5), new Vector2(0.5, 0.1)
    });

    private readonly Polygon _c = new Polygon(new[]
    {
      new Vector2(0, 0), new Vector2(-1, 0),
      new Vector2(-1, -1), new Vector2(0, -1)
    });
    
    private readonly Polygon _d = new Polygon(new[]
    {
      new Vector2(0.5, 0.5), new Vector2(0.5, 0.9),
      new Vector2(0.9, 0.9), new Vector2(0.9, 0.5)
    });

    [Test]
    public void TestArea()
    {
      // The calculation for Polygon B is as follows:
      
      // (0,1*0,5 - 0,1*0,1
      // + 0,1*0,5 - 0,5*0,5
      // + 0,5*0,1 - 0,5*0,5
      // + 0,5*0,1 - 0,1*0,1) / 2
      
      // -- Test regular polygons
      
      Assert.AreEqual(Polygon.Area(_b), 0.16);
      Assert.AreEqual(Polygon.Area(_c), 1);
      
      // -- Test area with holes
      
      var clipper = new Clipper();
      clipper.AddPath(Polygon.ToIntPoint(_a), PolyType.ptSubject, true);
      clipper.AddPath(Polygon.ToIntPoint(_c), PolyType.ptSubject, true);
      clipper.AddPath(Polygon.ToIntPoint(_b), PolyType.ptClip, true);
      clipper.AddPath(Polygon.ToIntPoint(_d), PolyType.ptClip, true);

      var solution = new PolyTree();
      clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero);
      Assert.AreEqual(Polygon.Area(solution), 1.68);
    }

    [Test]
    public void TestDifference()
    {
      var difference = Polygon.Difference(new[] {_a, _c}, new[] {_b});
      Assert.AreEqual(difference.Count, 9);

      foreach (var polygon in difference) Console.WriteLine(polygon);

      Assert.AreEqual(
        Polygon.FromList(new []{1, 1, 0, 1, 0.5, 0.5}.ToList()),
        difference[0]);
      Assert.AreEqual(
        Polygon.FromList(new []{0, 1, 0.1, 0.5, 0.5, 0.5}.ToList()),
        difference[1]);
      Assert.AreEqual(
        Polygon.FromList(new []{0.1, 0.5, 0, 1, 0, 0}.ToList()),
        difference[2]);
      Assert.AreEqual(
        Polygon.FromList(new []{0, 0, 0.1, 0.1, 0.1, 0.5}.ToList()),
        difference[3]);
      Assert.AreEqual(
        Polygon.FromList(new []{0.5, 0.1, 0.1, 0.1, 0, 0}.ToList()),
        difference[4]);
      Assert.AreEqual(
        Polygon.FromList(new []{1, 0, 0.5, 0.1, 0, 0}.ToList()),
        difference[5]);
      Assert.AreEqual(
        Polygon.FromList(new []{0.5, 0.5, 0.5, 0.1, 1, 0}.ToList()),
        difference[6]);
      Assert.AreEqual(
        Polygon.FromList(new []{0.5, 0.5, 1, 0, 1, 1}.ToList()),
        difference[7]);
      Assert.AreEqual(
        Polygon.FromList(new []{0.0, 0, -1, 0, -1, -1, 0, -1}.ToList()),
        difference[8]);
    }

    /// <summary>
    /// Test how the Poly2Trie API works.
    /// </summary>
    [Test]
    public void TestPoly2Trie()
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
  }
}