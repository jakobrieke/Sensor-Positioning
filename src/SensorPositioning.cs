using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using LibOptimization.Optimization;
using Shadows;

namespace sensor_positioning
{
  public class Sensor
  {
    public readonly double Range;
    public readonly double Fov;
    public double Rotation;
    public Vector2 Position;
    public double Size;

    public Sensor(double x, double y, double rotation, double range,
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

  public class SensorPositionObj : AbsObjectiveFunction
  {
    public readonly List<Circle> Obstacles = new List<Circle>();
    public readonly List<Polygon> ImportantAreas = new List<Polygon>();
    public readonly Rectangle Field;
    public readonly double FieldWidth;
    public readonly double FieldHeight;
    public readonly double SensorRange;
    public readonly double SensorFov;
    public readonly double ObjectSize;
    private readonly uint _numberOfSensors;
    /// <summary>
    /// If StartPosition is set to a value != null the distance is included
    /// into the result of the objective function.
    /// </summary>
    public List<Sensor> StartPosition;
    /// <summary>
    /// The weight by which the distance to the start position is taken into
    /// account.
    /// </summary>
    public double StartPositionDistanceWeight = 0.5;
    public double StartPositionRotationWeight = 0.5;

    public SensorPositionObj(
      uint numberOfSensors, uint numberOfObstacles, 
      double fieldWidth, double fieldHeight, 
      double sensorRange, double sensorFov,
      double objectSize)
    {
      _numberOfSensors = numberOfSensors;
      FieldWidth = fieldWidth;
      FieldHeight = fieldHeight;
      Field = new Rectangle(0, 0, 
        FieldWidth, FieldHeight);
      SensorRange = sensorRange;
      SensorFov = sensorFov;
      ObjectSize = objectSize;
      SetObstaclesRandom(numberOfObstacles);
    }

    /// <summary>
    /// Scatter a number of obstacles randomly over the field without collision.
    /// </summary>
    /// <param name="numberOfObstacles"></param>
    public void SetObstaclesRandom(double numberOfObstacles)
    {
      Obstacles.Clear();

      for (var i = 0; i < numberOfObstacles; i++)
      {
        var obstacle = new Circle(new Vector2(0, 0), ObjectSize);
        obstacle = PlaceWithoutCollision(obstacle, Obstacles);
        Obstacles.Add(obstacle);
      }
    }

    public void SetObstacles(double[][] positions)
    {
      Obstacles.Clear();

      foreach (var pos in positions)
      {
        Obstacles.Add(new Circle(new Vector2(pos[0], pos[1]), ObjectSize));
      }
    }

    /// <summary>
    /// Place an object randomly without collision inside the environment.
    /// </summary>
    /// <remarks>
    /// Warning, this function is not deterministic and might not
    /// terminate if Env.Field is to small.
    /// The object can also be placed on the edges of the field.
    /// </remarks>
    /// <param name="obstacle">The object to place.</param>
    /// <param name="obstacles"></param>
    private Circle PlaceWithoutCollision(Circle obstacle, 
      List<Circle> obstacles)
    {
      var collided = true;
      while (collided)
      {
        obstacle = PlaceRandom(obstacle);
        collided = CheckCollision(obstacle, obstacles);
      }

      return obstacle;
    }

    /// <summary>
    /// Place an object randomly inside the environment. This might
    /// cause a collision.
    /// </summary>
    /// <remarks>
    /// The object can also be placed on the edges of the field.
    /// </remarks>
    /// <param name="obstacle"></param>
    private Circle PlaceRandom(Circle obstacle)
    {
      obstacle.Position = new Vector2(
        MTRandom.Uniform(0, FieldWidth), 
        MTRandom.Uniform(0, FieldHeight));
      return obstacle;
    }

    /// <summary>
    /// Check if a given object collides with any object inside the
    /// problem environment. If the object already exists inside the environment
    /// it won't collide with itself.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="obstacles"></param>
    /// <returns></returns>
    public bool CheckCollision(Circle o, List<Circle> obstacles)
    {
      foreach (var o2 in obstacles)
      {
        if (o == o2) continue;

        var d = Vector2.Distance(o2.Position, o.Position);
        if (d < o2.Radius + o.Radius) return true;
      }

      return false;
    }

    /// <summary>
    /// Generate a list of sensors from a vector with length % 3 == 0. 
    /// </summary>
    /// <param name="vector"></param>
    public List<Sensor> ToSensors(double[] vector)
    {
      var sensors = new List<Sensor>();
      for (var i = 0; i < vector.Length; i += 3)
      {
        var sensor = new Sensor(
          vector[i], vector[i + 1], vector[i + 2], 
          SensorRange, SensorFov, ObjectSize);
        sensors.Add(sensor);
      }

      return sensors;
    }

    /// <summary>
    /// Get the bodies of a list of sensors.
    /// The reason to do so is that since agents carrying sensors also have a
    /// body and therefor represent an obstacle they have to be taken into
    /// account when calculating the shadow for a list of sensors.
    /// </summary>
    /// <returns>
    /// A list of circles where each circle represents an
    /// obstacle in the problem environment.
    /// </returns>
    public List<Circle> AllObstacles(List<Sensor> sensors)
    {
      var allObstacles = sensors.Select(s => s.ToCircle()).ToList();
      allObstacles.AddRange(Obstacles);
      return allObstacles;
    }
    
    /// <summary>
    /// Get the value of F(..) in percent.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public double Normalize(double value, bool round = true)
    {
      if (round) return Math.Round(value / Field.Area() * 100, 2);
      return value / Field.Area() * 100;
    }

    /// <summary>
    /// Get the search space of the problem instance.
    /// </summary>
    /// <returns></returns>
    public double[][] Intervals()
    {
      var intervals = new double[_numberOfSensors * 3][];
      for (var i = 0; i < intervals.Length; i += 3)
      {
        intervals[i] = new[] {Field.Min.X, Field.Max.X};
        intervals[i + 1] = new[] {Field.Min.Y, Field.Max.Y};
        intervals[i + 2] = new[] {0, 360.0};
      }

      return intervals;
    }

    public SearchSpace SearchSpace()
    {
      return new SearchSpace(Intervals());
    }

    public List<Circle> Others(List<Sensor> sensors, Sensor sensor)
    {
      var others = new List<Circle>();
      others.AddRange(Obstacles);
        
      foreach (var s in sensors)
      {
        if (s != sensor) others.Add(s.ToCircle());
      }

      return others;
    }
    
    public List<Polygon> Shadows(List<Sensor> sensors)
    {
      var shadows = sensors.Select(sensor => Shadows2D.Shadows(
        sensor.AreaOfActivity(), Others(sensors, sensor), Field)
      ).ToList();
      
      var result = shadows[0];
      for (var i = 1; i < shadows.Count; i++)
      {
        result = Polygon.Intersection(result, shadows[i]);
      }

      return result;
    }
    
    public override double F(List<double> vector)
    {
      var sensors = ToSensors(vector.ToArray());

      var penalty = 0.0;
      
      foreach (var sensor in sensors)
      {
        var x = Field.Min.X + FieldWidth / 2;
        var y = Field.Min.Y + FieldHeight / 2;
        var dist = Vector2.Distance(new Vector2(x, y), sensor.Position);
        
        if (!Field.Contains(sensor.Position, true)) 
          penalty += dist;

        foreach (var obstacle in Others(sensors, sensor))
        {
          var d = Vector2.Distance(obstacle.Position, sensor.Position);
          if (d < obstacle.Radius + sensor.Size) return Field.Area() + d;
        }
      }

      var shadows = Shadows(sensors);
      var shadowArea = Polygon.Area(shadows);

      var importantHidden = Polygon.Intersection(
        ImportantAreas, shadows);
      var importantArea = Polygon.Area(ImportantAreas) - 
                          Polygon.Area(importantHidden);

      if (StartPosition != null)
      {
        if (StartPosition.Count != sensors.Count) throw new Exception(
          "StartPosition and vector should have the same length " +
          $"{StartPosition.Count} != {sensors.Count}");

        for (var i = 0; i < StartPosition.Count; i++)
        {
          var d = Vector2.Distance(StartPosition[i].Position,
            sensors[i].Position);
          var rotDiff = Math.Abs(
            StartPosition[i].Rotation - sensors[i].Rotation);
          penalty += rotDiff * StartPositionRotationWeight + 
                     d * StartPositionDistanceWeight;
        }
      }
      
      return penalty + shadowArea - importantArea;
    }
    
    public override int Dimension()
    {
      return Intervals().Length;
    }
    
    public override List<double> Gradient(List<double> x)
    {
      throw new NotImplementedException();
    }

    public override List<List<double>> Hessian(List<double> x)
    {
      throw new NotImplementedException();
    }
  }
}