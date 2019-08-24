using System;
using System.Collections.Generic;

namespace LinearAlgebra
{
  public struct Circle : IGeometryObject
  {
    public Vector2 Position;
    public readonly double Radius;

    public Circle(double x, double y, double radius) : 
      this(new Vector2(x, y), radius) {}

    public Circle(Vector2 position, double radius)
    {
      if (radius <= 0) throw new ArgumentException("Radius must be greater 0");
      Position = position;
      Radius = radius;
    }

    /// <summary>
    /// Check if a point lies on the border of the circle. 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool OnBorder(Vector2 p)
    {
      return Math.Abs(Vector2.Distance(p, Position) - Radius) < Defs.Precision;
    }

    /// <summary>
    /// Check if a point lies inside the circle. This does not include the
    /// border of the circle.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool Contains(Vector2 p)
    {
      return Vector2.Distance(p, Position) < Radius - Defs.Precision;
    }

    /// <summary>
    /// Calculate the tangents of a point and a circle.
    /// </summary>
    /// <remarks>
    /// For more information see:
    /// https://en.wikipedia.org/wiki/Tangent_lines_to_circles
    /// </remarks>
    /// <param name="p"></param>
    /// <param name="c"></param>
    /// <returns>
    /// An array of two segments or an empty array if the point lies inside
    /// the circle.
    /// </returns>
    public static Segment[] ExternalTangents(Vector2 p, Circle c)
    {
      if (c.OnBorder(p))
      {
        return new[] {new Segment(p, p), new Segment(p, p)};
      }
      if (c.Contains(p)) return new Segment[]{};

      var tangent1 = ExternalTangent(c.Position.X, c.Position.Y, 
        c.Radius, p.X, p.Y, 0);
      var tangent2 = ExternalTangent(p.X, p.Y, 0, 
        c.Position.X, c.Position.Y, c.Radius);
      tangent1.Inverse();
      
      return new[] {tangent1, tangent2};
    }

    /// <summary>
    /// Calculate one outer tangent of two circles given by their position and
    /// radius.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="r1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    private static Segment ExternalTangent(double x1, double y1, double r1,
      double x2, double y2, double r2)
    {
      var d = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
      var α = Math.PI / 2  
              + Math.Atan2(y2 - y1, x2 - x1) - Math.Asin((r2 - r1) / d);

      return new Segment(
        new Vector2(x1 - r1 * Math.Cos(α), y1 - r1 * Math.Sin(α)), 
        new Vector2(x2 - r2 * Math.Cos(α), y2 - r2 * Math.Sin(α)));
    }
    
    public static bool operator ==(Circle c1, Circle c2)
    {
      return c1.Position == c2.Position && c1.Radius == c2.Radius;
    }

    public static bool operator !=(Circle c1, Circle c2)
    {
      return !(c1 == c2);
    }
    
    public override string ToString()
    {
      return Position + ", " + Radius;
    }
    
    public List<double> ToList()
    {
      return new List<double>{Position.X, Position.Y, Radius};
    }
    
    /// <summary>
    /// Convert the circle to a polygon.
    /// </summary>
    /// <param name="precision"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Polygon ToPolygon(int precision = 1, double offset = 0)
    {
      if (precision < 1)
        throw new ArgumentException(
          "Precision can't be smaller than 1");

      offset %= 360;

      var result = new Polygon();
      for (var i = 0; i < precision * 4; i++)
      {
        var angle = i * 360f / (precision * 4) + offset;
        result.Add(Position.Move(angle, Radius));
      }

      result.Add(result[0]);

      return result;
    }
  }
}