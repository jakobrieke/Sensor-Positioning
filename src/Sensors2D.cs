using System.Collections.Generic;
using Geometry;
using Vector2 = Geometry.Vector2;


namespace sensor_positioning
{
  public static class Sensors2D
  {
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

      var angle = Vector2.Gradient(position, obstacle.Position);
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

//    public static List<Polygon> Perceptible(
//      Arc sensor, List<Circle> obstacles, Rectangle bounds)
//    {
//      
//    }
    
    /// <summary>
    /// Calculate the area not perceptible to a sensor.
    /// </summary>
    /// <param name="sensor"></param>
    /// <param name="obstacles"></param>
    /// <param name="bounds"></param>
    /// <returns>
    /// An intersection of not perceptible areas of all sensors.
    /// </returns>
    public static List<Polygon> NotPerceptible(
      Arc sensor, List<Circle> obstacles, Rectangle bounds)
    {
      if (!bounds.Contains(sensor.Position, true))
        return new List<Polygon> {bounds.ToPolygon()};

      var polygons = new List<Polygon>();
      foreach (var obstacle in obstacles)
      {
        if (!bounds.Contains(obstacle.Position)) continue;

        polygons.Add(CalcBlockedArea(sensor.Position, obstacle, bounds));
      }

      // Add the area outside of the sensors area of activity
      polygons.AddRange(
        Polygon.Difference(bounds.ToPolygon(), sensor.ToPolygon()));

      return Polygon.Union(polygons);
    }


    /// <summary>
    /// Calculate the area not perceptible to a group of sensors.
    /// </summary>
    /// <param name="sensors"></param>
    /// <param name="obstacles"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static List<List<Polygon>> NotPerceptible(
      List<Arc> sensors, List<Circle> obstacles, Rectangle bounds)
    {
      var baseShadows = new List<List<Polygon>>();

      sensors.ForEach(light => baseShadows.Add(
        NotPerceptible(light, obstacles, bounds)));

      return baseShadows;
    }
  }
}