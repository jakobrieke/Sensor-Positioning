using System;
using System.Collections.Generic;

namespace LinearAlgebra
{
  public class Arc : IGeometryObject 
  {
    public Vector2 Position;
    public readonly double Radius;
    public readonly double Angle;
    public readonly double Rotation;


    public Arc(Vector2 position, double radius, double angle,
      double rotation = 0)
    {
      Position = position;
      Radius = radius;
      Angle = angle;
      Rotation = rotation;
    }

    public Arc(double x, double y, double radius,
      double angle, double rotation = 0)
    {
      Position = new Vector2(x, y);
      Radius = radius;
      Angle = angle;
      Rotation = rotation % 360;
    }
    
    public override string ToString()
    {
      return Position + "; " + Angle + "; " + Rotation + "; " + Radius;
    }

    public List<double> ToList()
    {
      return new List<double>{Position.X, Position.Y, Radius, Angle, Rotation};
    }
    
    /// <summary>
    /// Return a polygon representation of the arc.
    /// </summary>
    /// <param name="precision">
    /// The precision for the polygon representation, needs to be greater than
    /// 1. A higher number means a higher precision. Defaults to 1.
    /// </param>
    /// <param name="noCenter">
    /// Defines if the center point is included. If the arc has an angle of
    /// 360 degrees the center is always excluded. Defaults to false.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Polygon ToPolygon(int precision = 2, bool noCenter = false)
    {
      if (precision < 0)
        throw new ArgumentException(
          "Precision can't be smaller than 0");
      if (Angle > 360) throw new NotSupportedException(
        "Angles > 360° are not supported for conversion to polygon");
      precision += 1;

      var result = new Polygon();
      if (!noCenter && Angle < 360) result.Add(Position);

      for (var i = 0; i < precision + 1; i++)
      {
        var angle = i * Angle / precision + Rotation;
        result.Add(Position.Move(angle, Radius));
      }

      return result;
    }
  }
}