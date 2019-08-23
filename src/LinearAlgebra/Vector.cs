using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace LinearAlgebra
{
  /// <summary>
  /// Implementation of an N dimensional double vector.
  /// </summary>
  public class Vector : List<double>
  {
    public Vector(params double[] collection) : base(collection)
    {
    }

    private Vector(int length, double value) : base(length)
    {
      for (var i = 0; i < length; i++) Add(value);
    }

    public override string ToString()
    {
      return "(" + string.Join(", ", this) + ")";
    }

    public static Vector Zero(int length)
    {
      return new Vector(length, 0);
    }

    public static Vector One(int length)
    {
      return new Vector(length, 1);
    }

    private static void Check(Vector v1, Vector v2)
    {
      if (v1 == null) throw new ArgumentException("Vector v1 is null");
      if (v2 == null) throw new ArgumentException("Vector v2 is null");
      if (v1.Count != v2.Count)
        throw new ArgumentException(
          $"Vector length {v1} does not equal vector length {v2}");
    }

    public static Vector operator *(Vector v1, Vector v2)
    {
      Check(v1, v2);
      return new Vector(v1.Select((d, i) => d * v2[i]).ToArray());
    }

    public static Vector operator /(Vector v1, Vector v2)
    {
      Check(v1, v2);
      if (v2.Contains(0)) throw new DivideByZeroException();
      return new Vector(v1.Select((d, i) => d / v2[i]).ToArray());
    }
    
    public static Vector operator /(Vector v1, double x)
    {
      if (v1 == null) throw new ArgumentException("Vector 1 is null");
      if (Abs(x) < Defs.Precision) throw new DivideByZeroException();
      return new Vector(v1.Select((d, i) => d / x).ToArray());
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
      Check(v1, v2);
      return new Vector(v1.Select((d, i) => d + v2[i]).ToArray());
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
      Check(v1, v2);
      return new Vector(v1.Select((d, i) => d - v2[i]).ToArray());
    }

    public static Vector operator *(Vector v1, double x)
    {
      if (v1 == null) throw new ArgumentException("Vector 1 is null");
      return new Vector(v1.Select((d, i) => d * x).ToArray());
    }
    
    public static Vector operator *(double x, Vector v1)
    {
      return v1 * x;
    }

    public static implicit operator Vector(double[] array)
    {
      return new Vector(array);
    }

    public override bool Equals(object obj)
    {
      if (obj.GetType() != typeof(Vector)) return false;

      var v2 = (Vector) obj;
      if (Count != v2.Count) return false;
      
      for (var i = 0; i < Count; i++)
      {
        if (Abs(this[i] - v2[i]) >= Defs.Precision) return false;
      }

      return true;
    }

    public static double Dot(Vector v1, Vector v2)
    {
      Check(v1, v2);
      return v1.Select((d, i) => d * v2[i]).Sum();
    }
    
    /// <summary>
    /// Calculate the Euclidean distance between two vectors.
    /// </summary>
    /// <param name="v1">Vector 1</param>
    /// <param name="v2">Vector 2</param>
    /// <returns></returns>
    public static double Distance(Vector v1, Vector v2)
    {
      Check(v1, v2);
      return Sqrt(v1.Select((t, i) => Pow(t - v2[i], 2)).Sum());
    }
  }
}