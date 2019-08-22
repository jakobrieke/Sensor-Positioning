using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;

namespace LinearAlgebra
{
  public class Polygon : List<Vector2>, IGeometryObject 
  {
    public Polygon() {}

    public Polygon(IEnumerable<Vector2> collection) : base(collection) {}

    public static bool operator ==(Polygon p1, Polygon p2)
    {
      if (p1 is null && p2 is null) return true;
      if (p1 is null || p2 is null) return false;
      if (p1.Count != p2.Count) return false;
      return !p1.Where((t, i) => t != p2[i]).Any();
    }

    public static bool operator !=(Polygon p1, Polygon p2)
    {
      return !(p1 == p2);
    }

    /// <summary>
    /// Calculate the area of any polygon (Convex, Concave, Self-Intersecting).
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns>
    /// 0 if the polygon is invalid (consists of lesser than 3 points).
    /// </returns>
    public static double Area(Polygon polygon)
    {
      if (polygon.Count < 3) return 0;

      if (polygon[0] != polygon[polygon.Count - 1])
      {
        polygon.Add(polygon[0]);
      }

      var result = 0.0;

      for (var i = 0; i < polygon.Count - 1; i++)
      {
        result += polygon[i].X * polygon[i + 1].Y -
                  polygon[i + 1].X * polygon[i].Y;
      }

      return Math.Abs(result / 2);
    }

    /// <summary>
    /// Calculate the union of a list of any type of polygon.
    /// </summary>
    /// <param name="polygons"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Union(
      List<Polygon> polygons, bool closed = true)
    {
      var clipper = new Clipper();

      foreach (var polygon in polygons)
      {
        var shadowPath = ToIntPoint(polygon);

        clipper.AddPath(shadowPath, PolyType.ptSubject, closed);
      }

      var solution = new List<List<IntPoint>>();

      clipper.Execute(ClipType.ctUnion, solution,
        PolyFillType.pftPositive, PolyFillType.pftPositive);

      var result = new List<Polygon>();

      solution.ForEach(polygon =>
      {
        var shadowPolygon = FromIntPoint(polygon);
        shadowPolygon.Add(shadowPolygon[0]);
        result.Add(shadowPolygon);
      });

      return result;
    }

    /// <summary>
    /// Calculate the intersection between two polygons of any type.
    /// </summary>
    /// <param name="polygon1"></param>
    /// <param name="polygon2"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Intersection(
      Polygon polygon1,
      Polygon polygon2, bool closed = true)
    {
      var clipper = new Clipper();

      clipper.AddPath(ToIntPoint(polygon1),
        PolyType.ptSubject, closed);

      clipper.AddPath(ToIntPoint(polygon2),
        PolyType.ptClip, closed);

      var solution = new List<List<IntPoint>>();

      clipper.Execute(ClipType.ctIntersection, solution,
        PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

      var result = new List<Polygon>();
      foreach (var path in solution)
      {
        var polygon = FromIntPoint(path);
        polygon.Add(polygon[0]);
        result.Add(polygon);
      }

      return result;
    }

    /// <summary>
    /// Calculate the intersection of two lists of polygons.
    /// </summary>
    /// <param name="polygons1"></param>
    /// <param name="polygons2"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Intersection(
      List<Polygon> polygons1,
      List<Polygon> polygons2, bool closed = true)
    {
      var clipper = new Clipper();

      foreach (var shadow in polygons1)
      {
        clipper.AddPath(ToIntPoint(shadow),
          PolyType.ptSubject, closed);
      }

      foreach (var shadow in polygons2)
      {
        clipper.AddPath(ToIntPoint(shadow),
          PolyType.ptClip, closed);
      }

      var solution = new List<List<IntPoint>>();

      clipper.Execute(ClipType.ctIntersection, solution,
        PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

      var result = new List<Polygon>();
      foreach (var path in solution)
      {
        var polygon = FromIntPoint(path);
        polygon.Add(polygon[0]);
        result.Add(polygon);
      }

      return result;
    }

    public static List<Polygon> Difference(
      List<Polygon> polygons1,
      List<Polygon> polygons2, bool closed = true)
    {
      var clipper = new Clipper();

      foreach (var shadow in polygons1)
      {
        clipper.AddPath(ToIntPoint(shadow),
          PolyType.ptSubject, closed);
      }

      foreach (var shadow in polygons2)
      {
        clipper.AddPath(ToIntPoint(shadow),
          PolyType.ptClip, closed);
      }

      var solution = new List<List<IntPoint>>();

      clipper.Execute(ClipType.ctDifference, solution,
        PolyFillType.pftNonZero);

      var result = new List<Polygon>();
      foreach (var path in solution)
      {
        var polygon = FromIntPoint(path);
        polygon.Add(polygon[0]);
        result.Add(polygon);
      }

      return result;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="polygon1"></param>
    /// <param name="polygon2"></param>
    /// <returns></returns>
    public static List<Polygon> Difference(Polygon polygon1, Polygon polygon2)
    {
      var clipper = new Clipper();
      clipper.AddPath(ToIntPoint(polygon1),
        PolyType.ptSubject, true);
      clipper.AddPath(ToIntPoint(polygon2),
        PolyType.ptClip, true);

      var solution = new List<List<IntPoint>>();
      clipper.Execute(ClipType.ctDifference, solution,
        PolyFillType.pftNonZero);

      return solution.Select(FromIntPoint).ToList();
    }


    /// <summary>
    /// Calculate the area of a list of polygons.
    /// </summary>
    /// <param name="polygons"></param>
    /// <returns></returns>
    public static double Area(List<Polygon> polygons)
    {
      return polygons.Sum(Area);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
    private static List<IntPoint> ToIntPoint(Polygon polygon)
    {
      return polygon.Select(vertex => (IntPoint) vertex).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
    private static Polygon FromIntPoint(List<IntPoint> polygon)
    {
      return new Polygon(polygon.Select(vertex => 
        (Vector2) vertex).ToList());
    }

    public override string ToString()
    {
      return string.Join("; ", this);
    }
    
    public List<double> ToList()
    {
      var result = new List<double>(Count * 2);
      foreach (var vector in this)
      {
        result.Add(vector.X);
        result.Add(vector.Y);
      }

      return result;
    }
  }
}