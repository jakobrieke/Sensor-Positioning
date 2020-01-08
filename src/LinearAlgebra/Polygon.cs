using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using Poly2Tri.Triangulation.Polygon;
using Poly2TriePolygon = Poly2Tri.Triangulation.Polygon.Polygon;

namespace LinearAlgebra
{
  public class Polygon : List<Vector2>, IGeometryObject
  {
    public Polygon()
    {
    }

    public Polygon(IEnumerable<Vector2> collection) : base(collection)
    {
    }

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

      var result = 0.0;
      for (var i = 0; i < polygon.Count - 1; i++)
      {
        result += polygon[i].X * polygon[i + 1].Y -
                  polygon[i + 1].X * polygon[i].Y;
      }
      
      result += polygon.Last().X * polygon.First().Y -
                polygon.First().X * polygon.Last().Y;
      
      return Math.Abs(result / 2);
    }
    
    public static double Area(PolyNode node)
    {
      var area = Area(FromIntPoint(node.Contour));
      if (node.IsHole) area *= -1;

      area += node.Childs.Sum(Area);

      return area;
    }
    
    /// <summary>
    /// Calculate the area of a list of polygons.
    /// </summary>
    /// <param name="polygons"></param>
    /// <returns></returns>
    public static double Area(IEnumerable<Polygon> polygons)
    {
      return polygons.Sum(Area);
    }

    /// <summary>
    /// Calculate the union of a list of any type of polygon.
    /// </summary>
    /// <param name="polygons"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Union(
      IEnumerable<Polygon> polygons, bool closed = true)
    {
      var clipper = new Clipper();

      foreach (var polygon in polygons)
      {
        var shadowPath = ToIntPoint(polygon);

        clipper.AddPath(shadowPath, PolyType.ptSubject, closed);
      }

      var solution = new PolyTree();

      clipper.Execute(ClipType.ctUnion, solution,
        PolyFillType.pftPositive, PolyFillType.pftPositive);

      var result = new List<Polygon>();
      foreach (var child in solution.Childs) RemoveHoles(child, ref result);
      return result;
    }

    /// <summary>
    /// Calculate the intersection between two polygons
    /// </summary>
    /// <remarks>The polygons are not supposed to contain holes.</remarks>
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
    /// <remarks>The polygons are not supposed to contain holes.</remarks>
    /// <param name="polygons1"></param>
    /// <param name="polygons2"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Intersection(
      IEnumerable<Polygon> polygons1,
      IEnumerable<Polygon> polygons2, bool closed = true)
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

    /// <summary>
    /// Calculate the polygon difference between a list of subject polygons
    /// and a list of clip polygons.
    /// </summary>
    /// <remarks>The clips polygons shall not be overlapping.</remarks>
    /// <param name="subjects"></param>
    /// <param name="clips"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static List<Polygon> Difference(
      IEnumerable<Polygon> subjects,
      IEnumerable<Polygon> clips, bool closed = true)
    {
      var clipper = new Clipper();

      foreach (var polygon in subjects)
      {
        clipper.AddPath(ToIntPoint(polygon), PolyType.ptSubject, closed);
      }

      foreach (var polygon in clips)
      {
        clipper.AddPath(ToIntPoint(polygon), PolyType.ptClip, closed);
      }

      var solution = new PolyTree();
      clipper.Execute(ClipType.ctDifference, solution,
        PolyFillType.pftNonZero);

      var result = new List<Polygon>();
      foreach (var child in solution.Childs) RemoveHoles(child, ref result);
      return result;
    }

    /// <summary>
    /// Calculate the difference between a subject polygon and a clip polygon
    /// (subject - clip).
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="clip"></param>
    /// <returns></returns>
    public static List<Polygon> Difference(Polygon subject, Polygon clip)
    {
      var clipper = new Clipper();
      clipper.AddPath(ToIntPoint(subject), PolyType.ptSubject, true);
      clipper.AddPath(ToIntPoint(clip), PolyType.ptClip, true);

      var solution = new PolyTree();
      clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero);

      var result = new List<Polygon>();
      foreach (var child in solution.Childs) RemoveHoles(child, ref result);
      return result;
    }

    /// <summary>
    /// A recursive function to remove holes from a PolyTree by triangulating
    /// the polygon.
    /// The result is stored inside a flat polygon list.
    /// </summary>
    /// <param name="node">The PolyNode to remove holes from.</param>
    /// <param name="result">A list to store the result in.</param>
    private static void RemoveHoles(PolyNode node, ref List<Polygon> result)
    {
      // -- Check if the polygon contains holes
      
      if (node.ChildCount == 0)
      {
        result.Add(FromIntPoint(node.Contour));
        return;
      }

      // -- Triangulate the polygon if it contains holes
      
      var poly2TriPolygon = (Poly2TriePolygon) FromIntPoint(node.Contour);
      foreach (var hole in node.Childs)
      {
        if (!hole.IsHole)
        {
          RemoveHoles(hole, ref result);
        }
        poly2TriPolygon.AddHole(
          (Poly2TriePolygon) FromIntPoint(hole.Contour));
          
        foreach (var child in hole.Childs)
        {
          RemoveHoles(child, ref result);
        }
      }
      Poly2Tri.P2T.Triangulate(poly2TriPolygon);
      
      // -- Add the generated triangles to the result 
      
      foreach (var triangle in poly2TriPolygon.Triangles)
      {
        result.Add(new Polygon(new []
        {
          new Vector2(triangle.Points.Item0.X, triangle.Points.Item0.Y), 
          new Vector2(triangle.Points.Item1.X, triangle.Points.Item1.Y), 
          new Vector2(triangle.Points.Item2.X, triangle.Points.Item2.Y) 
        }));
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static List<IntPoint> ToIntPoint(Polygon polygon)
    {
      return polygon.Select(vertex => (IntPoint) vertex).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static Polygon FromIntPoint(List<IntPoint> polygon)
    {
      return new Polygon(polygon.Select(vertex =>
        (Vector2) vertex).ToList());
    }
    
    public static explicit operator Poly2TriePolygon(Polygon polygon)
    {
      return new Poly2TriePolygon(
        polygon.Select(vertex => new PolygonPoint(vertex.X, vertex.Y)));
    }
    
    public static explicit operator Polygon(Poly2TriePolygon polygon)
    {
      var result = new Polygon();
      for (var i = 0; i < polygon.Count; i++)
      {
        result.Add(new Vector2(polygon[i].X, polygon[i].Y));
      }
      return result;
    }

    public static void PrintPolyNode(PolyNode node, uint depth = 0)
    {
      for (var i = 0; i < depth; i++) Console.Write("  ");
      Console.Write(node.IsHole ? "Hole" : "Solid");
      Console.Write(", Children: " + node.ChildCount);
      Console.Write(", Area: " + Area(node));
      
      Console.WriteLine();
      
      foreach (var child in node.Childs)
      {
        PrintPolyNode(child, depth + 1);
      }
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

    public static Polygon FromList(List<double> xyList)
    {
      if (xyList.Count < 6)
      {
        throw new ArgumentException(
          "xyList must contain at least values for three points.");
      }
      if (xyList.Count % 2 == 1)
      {
        throw new ArgumentException(
          "xyList must contain pairs of x and y.");
      }

      var result = new Polygon();
      for (var i = 0; i < xyList.Count; i += 2)
      {
        result.Add(new Vector2(xyList[i], xyList[i + 1]));
      }
      return result;
    }
  }
}