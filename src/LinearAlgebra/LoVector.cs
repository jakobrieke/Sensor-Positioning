using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOMath
{
  /// <summary>
  /// Vector class
  /// </summary>
  /// <remarks>
  /// Inherits List(of double)
  /// </remarks>
  public class LoVector : List<double>
  {
    /// <summary>
    /// Vector direction
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public VectorDirection Direction { get; set; } = VectorDirection.Row;

    /// <summary>
    /// Vector direction.
    /// Row vector or Column vector.
    /// </summary>
    /// <remarks></remarks>
    public enum VectorDirection { Row, Col }

    /// <summary>
    /// Default construcotr
    /// </summary>
    /// <remarks></remarks>
    public LoVector()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="ai_base"></param>
    /// <remarks></remarks>
    public LoVector(LoVector ai_base)
    {
      AddRange(ai_base);
      Direction = ai_base.Direction;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_dim"></param>
    /// <remarks></remarks>
    public LoVector(int ai_dim,
      VectorDirection ai_direction = VectorDirection.Row)
    {
      AddRange(new double[ai_dim - 1 + 1]);
      Direction = ai_direction;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_val"></param>
    /// <remarks></remarks>
    public LoVector(IReadOnlyCollection<double> ai_val,
      VectorDirection ai_direction = VectorDirection.Row)
    {
      AddRange(ai_val);
      Direction = ai_direction;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_val"></param>
    /// <remarks></remarks>
    public LoVector(IEnumerable<double> ai_val,
      VectorDirection ai_direction = VectorDirection.Row)
    {
      AddRange(ai_val);
      Direction = ai_direction;
    }

    /// <summary>
    /// Set Double()
    /// </summary>
    /// <param name="ai_list"></param>
    /// <remarks></remarks>
    public void Set(IEnumerable<double> ai_list)
    {
      Clear();
      AddRange(ai_list);
    }

    /// <summary>
    /// To Matrix
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public LoMatrix ToMatrix()
    {
      return Direction == VectorDirection.Row
        ? new LoMatrix(this, VectorDirection.Row)
        : new LoMatrix(this, VectorDirection.Col);
    }

    /// <summary>
    /// Norm L1 ( |x1| + |x2| ... )
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// |x|1
    /// Refference:
    /// Atsushi Minamoto, "Introduction to Numerical Computation in
    /// C-Solution, Algorithm, Program ...", 2008, pp28-32
    /// </remarks>
    public double NormL1()
    {
      double ret = 0.0;
      foreach (double value in this)
        ret += System.Math.Abs(value);
      return ret;
    }

    /// <summary>
    /// Norm L2 ( Sqrt( x1^2 + x2^2 ... ) )
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// ||x||
    /// Reference:
    /// Atsushi Minamoto, "Introduction to Numerical Computation in
    /// C-Solution, Algorithm, Program ...", 2008, pp28-32
    /// </remarks>
    public double NormL2()
    {
      var ret = 0.0;
      foreach (var val in this) ret += val * val;
      return System.Math.Sqrt(ret);
    }

    /// <summary>
    /// NormMax
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// |x|max
    /// Reference:
    /// Atsushi Minamoto, "Introduction to Numerical Computation in
    /// C-Solution, Algorithm, Program ...", 2008, pp28-32
    /// </remarks>
    public double NormMax()
    {
      var ret = new List<double>();
      foreach (var Val in this) ret.Add(System.Math.Abs(Val));
      ret.Sort();
      return ret[ret.Count - 1];
    }

    /// <summary>
    /// Transpose
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public LoVector T()
    {
      var temp = new LoVector(this);
      temp.Direction = temp.Direction == VectorDirection.Row
        ? VectorDirection.Col
        : VectorDirection.Row;
      return temp;
    }

    /// <summary>
    /// All scalars of the vector summed up.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public double Sum()
    {
      var ret = 0.0;
      foreach (var value in this) ret += value;
      return ret;
    }

    /// <summary>
    /// The sum of the vector divided by its length.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public double Average()
    {
      return Sum() / Count;
    }

    /// <summary>
    /// For Debug
    /// </summary>
    /// <param name="ai_preci"></param>
    /// <remarks></remarks>
    public void PrintValue(int ai_preci = 4, string name = "")
    {
      var str = new StringBuilder();
      if (string.IsNullOrEmpty(name) == false)
      {
        str.Append($"{name} =" + Environment.NewLine);
      }
      else
      {
        str.Append("Vec =" + Environment.NewLine);
      }
      if (Direction == VectorDirection.Row)
      {
        for (var i = 0; i <= Count - 1; i++)
        {
          str.Append(this[i].ToString("F" + ai_preci) + "\t");
        }
        str.AppendLine();
      }
      else
      {
        for (var i = 0; i <= Count - 1; i++)
        {
          str.Append(this[i].ToString("F" + ai_preci));
          str.AppendLine();
        }
      }

      str.Append(Environment.NewLine);
      Console.Write(str.ToString());
    }

    /// <summary>
    /// ベクトルを対角行列を作る
    /// </summary>
    /// <returns></returns>
    public LoMatrix ToDiagonalMatrix()
    {
      var ret = new LoMatrix(Count);
      for (var i = 0; i <= ret.Count - 1; i++)
        ret[i][i] = this[i];
      return ret;
    }

    /// <summary>
    /// Accessor 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<double> RawVector
    {
      get => this;
      set
      {
        Clear();
        AddRange(value);
      }
    }

    /// <summary>
    /// CheckDimension
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <param name="isCheckDirection"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private static bool IsSameDimension(LoVector vec1, LoVector vec2, 
      bool isCheckDirection = false)
    {
      if (vec1 == null ||
          vec2 == null ||
          vec1.Count != vec2.Count)
        return false;
      if (!isCheckDirection) return true;

      return vec1.Direction == vec2.Direction;
    }

    public static LoVector operator +(LoVector source, LoVector dest)
    {
      if (source.Count != dest.Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      var ret = new LoVector(source.Count);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = source[i] + dest[i];
      }

      return ret;
    }

    public override string ToString()
    {
      var str = new StringBuilder();
      str.Append(Direction == VectorDirection.Row ? "Row (" : "Column (");

      for (var i = 0; i < Count; i++)
      {
        str.Append(this[i]);
        if (i < Count - 1) str.Append(", ");
      }

      return str.Append(")").ToString();
    }

    /// <summary>
    /// Diff
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator -(LoVector source, LoVector dest)
    {
      if (source.Count != dest.Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      var ret = new LoVector(source.Count);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = source[i] - dest[i];
      }

      return ret;
    }

    /// <summary>
    /// Product(Inner product, dot product)
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// a dot b = |a||b|cos(theta)
    /// </remarks>
    public double InnerProduct(LoVector source)
    {
      if (source.Count != Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      return source.Select((t, i) => t * this[i]).Sum();
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator *(double source, LoVector dest)
    {
      var ret = new LoVector(dest);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = source * dest[i];
      }

      return ret;
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator *(LoVector source, double dest)
    {
      var ret = new LoVector(source);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = source[i] * dest;
      }

      return ret;
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator *(LoVector source, LoVector dest)
    {
      // |v1 v2| * |v3|
      //           |v4|
      if (source.Direction == VectorDirection.Row
          && dest.Direction == VectorDirection.Col)
      {
        throw new clsException(clsException.Series.NotComputable,
          "Vector * Vector - direction error");
      }

      var size = source.Count;
      if (size != dest.Count)
      {
        throw new clsException(clsException.Series.NotComputable,
          "Vector * Vector - size error");
      }

      var ret = 0.0;
      for (var i = 0; i < size; i++)
      {
        ret += source[i] * dest[i];
      }

      return new LoVector(new[] {ret});
    }

    /// <summary>
    /// Create a vector (source / dest[0], ..., source / dest[n])
    /// where n is the maximum length of dest.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator /(double source, LoVector dest)
    {
      var result = new LoVector(dest);
      for (var i = 0; i < result.Count; i++)
      {
        result[i] = source / dest[i];
      }
      return result;
    }

    /// <summary>
    /// Create a vector (source[0] / dest, ..., source[n] / dest) where
    /// n is the length of source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator /(LoVector source, double dest)
    {
      var result = new LoVector(source);
      for (var i = 0; i < result.Count; i++)
      {
        result[i] = source[i] / dest;
      }
      return result;
    }

    /// <summary>
    /// Power(exponentiation)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static LoVector operator ^(LoVector source, double dest)
    {
      var ret = new LoVector(source);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = Math.Pow(source[i], dest);
      }

      return ret;
    }

    /// <summary>
    /// Explicitly convert an array of doubles to a a LoVector. 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static explicit operator LoVector(double[] vector)
    {
      return new LoVector(vector);
    }
  }
}