using System;
using LinearAlgebra;

namespace Optimization
{
  public struct Point : IComparable
  {
    public readonly Vector Position;
    public readonly double Value;

    public Point(Vector position, double value)
    {
      Position = position;
      Value = value;
    }

    public int CompareTo(object obj)
    {
      if (obj == null) return 1;
      
      if (GetType() != obj.GetType()) throw new ArgumentException(
        "A Point can only be compared to another point");
      
      var value2 = ((Point) obj).Value;
      return Value < value2 ? -1 : Value > value2 ? 1 : 0;
    }

    public override string ToString()
    {
      return $"({string.Join(", ", Position)}), {Value}";
    }
  }
}