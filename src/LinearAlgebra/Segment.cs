using System.Collections.Generic;

namespace LinearAlgebra
{
  public struct Segment : IGeometryObject 
  {
    public override int GetHashCode()
    {
      unchecked
      {
        return (Start.GetHashCode() * 397) ^ End.GetHashCode();
      }
    }

    public Vector2 Start;
    public Vector2 End;

    public Segment(Vector2 start, Vector2 end)
    {
      Start = start;
      End = end;
    }

    public Segment(double x1, double y1, double x2, double y2)
    {
      Start = new Vector2(x1, y1);
      End = new Vector2(x2, y2);
    }

    public void Scale(double value)
    {
      var d = Length();
      End = new Vector2(
        End.X + (End.X - Start.X) / d * value,
        End.Y + (End.Y - Start.Y) / d * value);
    }

    public void Inverse()
    {
      var buffer = End;
      End = Start;
      Start = buffer;
    }

    public double Length()
    {
      return Vector2.Distance(Start, End);
    }

    public static bool operator ==(Segment s1, Segment s2)
    {
      return s1.Start == s2.Start && s1.End == s2.End;
    }

    public static bool operator !=(Segment s1, Segment s2)
    {
      return !(s1 == s2);
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public List<double> ToList()
    {
      return new List<double>{Start.X, Start.Y, End.X, End.Y};
    }
    
    public override string ToString()
    {
      return Start + "; " + End;
    }
  }
}