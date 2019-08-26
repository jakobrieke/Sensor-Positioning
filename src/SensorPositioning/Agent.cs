using LinearAlgebra;

namespace SensorPositioning
{
  public class Agent
  {
    public readonly double Range;
    public readonly double Fov;
    public double Rotation;
    public Vector2 Position;
    public double Size;

    public Agent(double x, double y, double rotation, double range,
      double fov, double size)
    {
      Range = range;
      Fov = fov;
      Rotation = rotation;
      Position = new Vector2(x, y);
      Size = size;
    }

    /// <summary>
    /// Get a circle of the physical representation of the agent as a circle.
    /// </summary>
    /// <returns></returns>
    public Circle ToCircle()
    {
      return new Circle(Position, Size);
    }

    /// <summary>
    /// Get the area monitored by the sensor.
    /// </summary>
    /// <returns></returns>
    public Arc AreaOfActivity()
    {
      return new Arc(Position, Range, Fov, Rotation - Fov / 2);
    }

    public override string ToString()
    {
      return "Position: " + Position + ", Size: " + Size +
             ", Rotation: " + Rotation + ", Range: " + Range
             + ", Fov: " + Fov;
    }
  }
}