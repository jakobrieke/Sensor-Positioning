using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathUtil
{
  /// <summary>
  /// Vector class
  /// </summary>
  /// <remarks>
  /// Inherits List(of double)
  /// </remarks>
  public class clsEasyVector : List<double>
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
    public clsEasyVector()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="ai_base"></param>
    /// <remarks></remarks>
    public clsEasyVector(clsEasyVector ai_base)
    {
      AddRange(ai_base);
      Direction = ai_base.Direction;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ai_dim"></param>
    /// <remarks></remarks>
    public clsEasyVector(int ai_dim,
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
    public clsEasyVector(IReadOnlyCollection<double> ai_val,
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
    public clsEasyVector(IEnumerable<double> ai_val,
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
    public clsEasyMatrix ToMatrix()
    {
      return Direction == VectorDirection.Row
        ? new clsEasyMatrix(this, VectorDirection.Row)
        : new clsEasyMatrix(this, VectorDirection.Col);
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
        ret += Math.Abs(value);
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
      return Math.Sqrt(ret);
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
      foreach (var Val in this) ret.Add(Math.Abs(Val));
      ret.Sort();
      return ret[ret.Count - 1];
    }

    /// <summary>
    /// Transpose
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public clsEasyVector T()
    {
      var temp = new clsEasyVector(this);
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
    public clsEasyMatrix ToDiagonalMatrix()
    {
      var ret = new clsEasyMatrix(Count);
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
    /// <param name="ai_vec1"></param>
    /// <param name="ai_vec2"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private static bool IsSameDimension(clsEasyVector ai_vec1,
      clsEasyVector ai_vec2, bool isCheckDirection = false)
    {
      if (ai_vec1 == null ||
          ai_vec2 == null ||
          ai_vec1.Count != ai_vec2.Count)
        return false;
      if (!isCheckDirection) return true;

      return ai_vec1.Direction == ai_vec2.Direction;
    }

    public static clsEasyVector operator +(clsEasyVector ai_source,
      clsEasyVector ai_dest)
    {
      if (ai_source.Count != ai_dest.Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      var ret = new clsEasyVector(ai_source.Count);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = ai_source[i] + ai_dest[i];
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
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator -(clsEasyVector ai_source,
      clsEasyVector ai_dest)
    {
      if (ai_source.Count != ai_dest.Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      var ret = new clsEasyVector(ai_source.Count);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = ai_source[i] - ai_dest[i];
      }

      return ret;
    }

    /// <summary>
    /// Product(Inner product, dot product)
    /// </summary>
    /// <param name="ai_source"></param>
    /// <returns></returns>
    /// <remarks>
    /// a dot b = |a||b|cos(theta)
    /// </remarks>
    public double InnerProduct(clsEasyVector ai_source)
    {
      if (ai_source.Count != Count)
      {
        throw new clsException(clsException.Series.DifferElementNumber);
      }

      return ai_source.Select((t, i) => t * this[i]).Sum();
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator *(double ai_source,
      clsEasyVector ai_dest)
    {
      var ret = new clsEasyVector(ai_dest);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = ai_source * ai_dest[i];
      }

      return ret;
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator *(clsEasyVector ai_source,
      double ai_dest)
    {
      var ret = new clsEasyVector(ai_source);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = ai_source[i] * ai_dest;
      }

      return ret;
    }

    /// <summary>
    /// Product
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator *(clsEasyVector ai_source,
      clsEasyVector ai_dest)
    {
      // OK
      // |v1 v2| * |v3|
      //           |v4|
      if (ai_source.Direction == VectorDirection.Row
          && ai_dest.Direction == VectorDirection.Col)
      {
        throw new clsException(clsException.Series.NotComputable,
          "Vector * Vector - direction error");
      }

      var size = ai_source.Count;
      if (size != ai_dest.Count)
      {
        throw new clsException(clsException.Series.NotComputable,
          "Vector * Vector - size error");
      }

      var ret = 0.0;
      for (var i = 0; i < size; i++)
      {
        ret += ai_source[i] * ai_dest[i];
      }

      return new clsEasyVector(new[] {ret});
    }

    /// <summary>
    /// Divide
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator /(double ai_source,
      clsEasyVector ai_dest)
    {
      var ret = new clsEasyVector(ai_dest);
      for (var i = 0; i < ret.Count; i++) ret[i] = ai_source / ai_dest[i];
      return ret;
    }

    /// <summary>
    /// Divide
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator /(clsEasyVector ai_source,
      double ai_dest)
    {
      var ret = new clsEasyVector(ai_source);
      for (var i = 0; i < ret.Count; i++) ret[i] = ai_source[i] / ai_dest;
      return ret;
    }

    /// <summary>
    /// Power(exponentiation)
    /// </summary>
    /// <param name="ai_source"></param>
    /// <param name="ai_dest"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static clsEasyVector operator ^(clsEasyVector ai_source,
      double ai_dest)
    {
      var ret = new clsEasyVector(ai_source);
      for (var i = 0; i < ret.Count; i++)
      {
        ret[i] = Math.Pow(ai_source[i], ai_dest);
      }

      return ret;
    }

//    /// <summary>
//    /// Type convert
//    /// </summary>
//    /// <param name="ai_ar"></param>
//    /// <returns></returns>
//    /// <remarks>
//    /// double() -> clsShoddyVector
//    /// </remarks>
//    public clsEasyVector operator CType(ByVal ai_ar() As double)
//    {
//      return new clsEasyVector(ai_ar);
//    }
  }
}