using System.Collections.Generic;

namespace LinearAlgebra
{
  public struct Rectangle : IGeometryObject 
  {
    public Vector2 Min;
    public Vector2 Max;

    public Rectangle(Vector2 min, Vector2 max)
    {
      Min = min;
      Max = max;
    }

    public Rectangle(double minX, double minY, double maxX, double maxY)
    {
      Min = new Vector2(minX, minY);
      Max = new Vector2(maxX, maxY);
    }

    public double Height()
    {
      return Max.Y - Min.Y;
    }

    public double Width()
    {
      return Max.X - Min.X;
    }

    public double Area()
    {
      return (Max.Y - Min.Y) * (Max.X - Min.X);
    }

    public double Diagonal()
    {
      return Vector2.Distance(Min, Max);
    }
    
    /// <summary>
    /// Test if a point is inside the rectangle.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="excludeBorders"></param>
    /// <returns></returns>
    public bool Contains(Vector2 point, bool excludeBorders = false)
    {
      if (excludeBorders)
      {
        return point.X > Min.X && point.Y > Min.Y &&
               point.X < Max.X && point.Y < Max.Y;
      }

      return point.X >= Min.X && point.Y >= Min.Y &&
             point.X <= Max.X && point.Y <= Max.Y;
    }

    /// <summary>
    /// Test if a point lays on the bounds of a rectangle.
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool OnBounds(Rectangle bounds, Vector2 point)
    {
      return bounds.Min.X.Equals(point.X) ||
             bounds.Min.Y.Equals(point.X) ||
             bounds.Max.X.Equals(point.X) ||
             bounds.Max.Y.Equals(point.X);
    }

    public override string ToString()
    {
      return Min + "; " + Max;
    }
    
    public Polygon ToPolygon()
    {
      return new Polygon
      {
        Min, new Vector2(Max.X, Min.Y), Max, new Vector2(Min.X, Max.Y)
      };
    }

    public List<double> ToList()
    {
      return new List<double>{Min.X, Min.Y, Max.X, Max.Y};
    }
  }
}