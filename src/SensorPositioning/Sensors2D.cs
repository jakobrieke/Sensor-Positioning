using System.Collections.Generic;
using LinearAlgebra;
using Vector2 = LinearAlgebra.Vector2;


namespace SensorPositioning
{
  public static class Sensors2D
  {
    /// <summary>
    /// The precision with which a sensor arc is converted into a polygon.
    /// </summary>
    public static int SensorArcPrecision = 10;
    
    /// <summary>
    /// Calculate the area that is blocked trough an obstacle.
    /// </summary>
    /// <remarks>The position has to be inside the bounds.</remarks>
    /// <param name="position"></param>
    /// <param name="obstacle"></param>
    /// <param name="bounds"></param>
    /// <returns>A quadrilateral.</returns>
    private static Polygon CalcBlockedArea(
      Vector2 position, Circle obstacle, Rectangle bounds)
    {
      var tangs = Circle.ExternalTangents(position, obstacle);
      if (tangs.Length == 0) return bounds.ToPolygon();
      
      var s1 = new Segment(position, tangs[0].End);
      var s2 = new Segment(position, tangs[1].End);
      var d = bounds.Diagonal();
      
      s1.Scale(d);
      s2.Scale(d);
      s1 = new Segment(tangs[0].End, s1.End);
      s2 = new Segment(tangs[1].End, s2.End);

      // Since segment (s1.End, s2.End) might intersect with bounds, make
      // sure that area "behind" that intersection is also included.
      // Therefore two extra points are included:
      // -> s1.End.Move(angle, d), s2.End.Move(angle, d),
      
      var angle = Vector2.Angle(position, obstacle.Position);
      var result = new Polygon {
        s1.Start, s1.End,
        s1.End.Move(angle, d), s2.End.Move(angle, d),
        s2.End, s2.Start};
      result = Polygon.Intersection(bounds.ToPolygon(), result)[0];
      return result;
    }
    
    /// <summary>
    /// Calculate the area that is blocked trough an obstacle.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="obstacle"></param>
    /// <param name="bounds"></param>
    /// <returns>
    /// The bounds if the sensor is positioned inside the obstacle or the
    /// sensor is outside the bounds otherwise a quadrilateral.
    /// </returns>
    public static Polygon BlockedArea(
      Vector2 position, Circle obstacle, Rectangle bounds)
    {
      return !bounds.Contains(position, true) 
        ? bounds.ToPolygon() 
        : CalcBlockedArea(position, obstacle, bounds);
    }

    /// <summary>
    /// Calculate the area perceptible by a sensor.
    /// </summary>
    /// <param name="sensor"></param>
    /// <param name="obstacles"></param>
    /// <returns></returns>
    public static Polygon Perceptible(Arc sensor, List<Circle> obstacles)
    {
      var result = sensor.ToPolygon(SensorArcPrecision);
      var bounds = new Rectangle(
        sensor.Position.X - sensor.Radius,
        sensor.Position.Y - sensor.Radius,
        sensor.Position.X + sensor.Radius, 
        sensor.Position.Y + sensor.Radius);
      
      foreach (var obstacle in obstacles)
      {
        if (!bounds.Contains(obstacle.Position)) continue;
        
        // The perceptible area is always a non self intersecting polygon
        result = Polygon.Difference(
          result, CalcBlockedArea(sensor.Position, obstacle, bounds))[0];
      }

      return result;
    }

    /// <summary>
    /// Examines whether an arc or a circle are intersecting.
    /// !!! Warning: This function currently only checks if the two radii of arc
    /// and circle are intersecting and ignores the rotation of the arc.
    /// </summary>
    /// <param name="arc"></param>
    /// <param name="circle"></param>
    /// <returns></returns>
    public static bool Intersecting(Arc arc, Circle circle)
    {
      if (Vector2.Distance(arc.Position, circle.Position) >=
          arc.Radius) return false;
      return true;
      // Todo: Implement check for arc rotation
      // Todo: Fix arc rotation is clockwise, angle anticlockwise
      // -> use unit circle as reference
//      var sensorHalfAngle = sensor.Angle / 2;
//        var angle = 360 - Vector2.Angle(sensor.Position, obstacle.Position);
//        Console.WriteLine("Rotation: " + sensor.Rotation);
//        Console.WriteLine("Alpha: " + (sensor.Rotation + sensorHalfAngle));
//        Console.WriteLine("Beta: " + (sensor.Rotation - sensorHalfAngle));
//        Console.WriteLine("Angle: " + angle);
//        if (sensor.Rotation + sensorHalfAngle < angle
//            && sensor.Rotation - sensorHalfAngle > angle) continue;
    }
    
    /// <summary>
    /// Calculate the area that is not perceptible by sensor.
    /// </summary>
    /// <param name="sensor"></param>
    /// <param name="obstacles"></param>
    /// <param name="bounds"></param>
    /// <returns>
    /// An intersection of not perceptible areas of all sensors.
    /// </returns>
    public static List<Polygon> Imperceptible(
      Arc sensor, List<Circle> obstacles, Rectangle bounds)
    {
      if (!bounds.Contains(sensor.Position, true))
        return new List<Polygon> {bounds.ToPolygon()};

      var polygons = new List<Polygon>();
      foreach (var obstacle in obstacles)
      {
        if (!bounds.Contains(obstacle.Position)) continue;
        if (!Intersecting(sensor, obstacle)) continue;
        
        polygons.Add(CalcBlockedArea(sensor.Position, obstacle, bounds));
      }

      // Add the area outside of the sensors area of activity
      polygons.AddRange(Polygon.Difference(
        bounds.ToPolygon(), 
        sensor.ToPolygon(SensorArcPrecision)));

      return Polygon.Union(polygons);
    }
  }
}