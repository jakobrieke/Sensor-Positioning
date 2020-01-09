using System.Collections.Generic;
using ClipperLib;
using static System.Math;

namespace LinearAlgebra
{
  public struct Vector2 : IGeometryObject
  {
    public readonly double X;
    public readonly double Y;
    public static readonly Vector2 Zero = new Vector2(0f, 0f);
    public static readonly Vector2 One = new Vector2(1f, 1f);


    public Vector2(double x, double y)
    {
      X = x;
      Y = y;
    }

    public static Vector2 operator *(Vector2 v, double factor)
    {
      return new Vector2(v.X * factor, v.Y * factor);
    }

    public static Vector2 operator *(Vector2 v1, Vector2 v2)
    {
      return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
      return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static bool operator ==(Vector2 v1, Vector2 v2)
    {
      return v1.X - v2.X <= Defs.Precision && v1.Y - v2.Y <= Defs.Precision;
    }

    public static bool operator !=(Vector2 v1, Vector2 v2)
    {
      return !(v1 == v2);
    }

    public override string ToString()
    {
      return X + ", " + Y;
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Calculates the Euclidean distance between two vectors.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static double Distance(Vector2 a, Vector2 b)
    {
      return Sqrt(Pow(a.X - b.X, 2) + Pow(a.Y - b.Y, 2));
    }

    /// <summary>
    /// Calculates the angle from a to b between two vectors.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>An angle in degrees</returns>
    public static double Angle(Vector2 a, Vector2 b)
    {
      if (b.Y < a.Y)
      {
        return Asin((b.X - a.X) / Distance(a, b)) * 180 / PI + 270;
      }
      return Asin((a.X - b.X) / Distance(a, b)) * 180 / PI + 90;
    }

    /// <summary>
    /// Rotates a vector (anticlockwise from east) and translates it by a given
    /// distance. 
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="distance"></param>
    /// <param name="useDegrees"></param>
    /// <returns></returns>
    public Vector2 Move(double angle, double distance, bool useDegrees = true)
    {
      if (useDegrees) angle *= PI / 180;

      return new Vector2(
        Cos(angle) * distance + X,
        Sin(angle) * distance + Y);
    }
    
    public static implicit operator double[](Vector2 v)
    {
      return new [] {v.X, v.Y};
    }

    public static implicit operator Vector2(double[] array)
    {
      return new Vector2(array[0], array[1]);
    }
    
    public static explicit operator IntPoint(Vector2 v)
    {
      return new IntPoint(
        Round(v.X / Defs.Precision), 
        Round(v.Y / Defs.Precision));
    }

    public static explicit operator Vector2(IntPoint p)
    {
      return new Vector2(p.X * Defs.Precision, p.Y * Defs.Precision);
    }
    
    public List<double> ToList()
    {
      return new List<double>{X, Y};
    }
  }
}