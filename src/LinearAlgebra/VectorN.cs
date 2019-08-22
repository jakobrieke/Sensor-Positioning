using System;
using System.Linq;

namespace LinearAlgebra
{
  public class VectorN
  {
    public static double[] Multiply(double[] vector, double x)
    {
      var result = new double[vector.Length];
      for (var i = 0; i < result.Length; i++) result[i] = vector[i] * x;
      return result;
    }
    
    public static double[] Divide(double[] vector, double x)
    {
      if (x.Equals(0f))
      {
        throw new ArgumentException("Can't divide through zero.");
      }

      var result = new double[vector.Length];
      for (var i = 0; i < result.Length; i++) result[i] = vector[i] / x;
      return result;
    }
    
    public static double[] Add(double[] vector1, double[] vector2)
    {
      if (vector1.Length != vector2.Length)
      {
        throw new ArgumentException(
          "Length of vector 1 is not equal to length of vector 2");
      }

      var result = new double[vector1.Length];
      for (var i = 0; i < result.Length; i++)
      {
        result[i] = vector1[i] + vector2[i];
      }
      return result;
    }
    
    public static double[] Subtract(double[] vector1, double[] vector2)
    {
      if (vector1.Length != vector2.Length)
      {
        throw new ArgumentException(
          "Length of vector 1 is not equal to length of vector 2");
      }

      var result = new double[vector1.Length];
      for (var i = 0; i < result.Length; i++)
      {
        result[i] = vector1[i] - vector2[i];
      }

      return result;
    }

    /// <summary>
    /// Calculate the Euclidean distance between two n dimensional vectors.
    /// </summary>
    /// <param name="v1">Vector 1</param>
    /// <param name="v2">Vector 2</param>
    /// <returns></returns>
    public static double Distance(double[] v1, double[] v2)
    {
      if (v1.Length != v2.Length)
      {
        throw new ArgumentException(
          "Length of vector 1 is not equal to length of vector 2");
      }
      
      return Math.Sqrt(v1.Select((t, i) => Math.Pow(t - v2[i], 2)).Sum());
    }
  }
}