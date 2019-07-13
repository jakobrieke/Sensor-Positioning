using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using LibOptimization.Optimization;
using Optimization;
using Shadows;

namespace sensor_positioning
{
  public class Environment
  {
    public readonly Rectangle Bounds;
    public readonly List<Obstacle> Obstacles = new List<Obstacle>();

    public Environment(Rectangle bounds)
    {
      Bounds = bounds;
    }

    public Environment(double x, double y, double width, double height)
    {
      Bounds = new Rectangle(x, y, x + width, y + height);
    }
  }

  public class Obstacle
  {
    public Vector2 Position;
    public readonly double Size;
    public double Rotation;

    public Obstacle(Vector2 position, double size)
    {
      Position = position;
      Size = size;
    }

    public Obstacle(Vector2 position, double size, double rotation)
    {
      Position = position;
      Size = size;
      Rotation = rotation;
    }
  }

  public class Sensor : Obstacle
  {
    public readonly double Range;
    public readonly double Fov;

    public Sensor(Vector2 position, double rotation, double range,
      double fov, double size) : base(position, size)
    {
      Range = range;
      Fov = fov;
      Rotation = rotation;
    }

    public Sensor(double x, double y, double rotation, double range,
      double fov, double size) : base(new Vector2(x, y), size)
    {
      Range = range;
      Fov = fov;
      Rotation = rotation;
    }

    /// <summary>
    /// Get the area monitored by the sensor.
    /// </summary>
    /// <returns></returns>
    public Arc AreaOfActivity()
    {
      return new Arc(Position, Range, Fov, Rotation - Fov / 2);
    }

    /// <summary>
    /// Calculate the shadows for this sensor inside an environment.
    /// </summary>
    /// <param name="env"></param>
    /// <returns></returns>
    public List<Polygon> Shadows(Environment env)
    {
      var obstacles = new List<Circle>();
      env.Obstacles.ForEach(o =>
      {
        if (o != this)
          obstacles.Add(
            new Circle(o.Position.X, o.Position.Y, o.Size));
      });

      return Shadows2D.Shadows(AreaOfActivity(),
        obstacles, env.Bounds);
    }

    /// <summary>
    /// Calculate the shadows for a list of sensors in an environment.
    /// </summary>
    /// <param name="sensors"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    public static List<Polygon> Shadows(List<Sensor> sensors, Environment env)
    {
      var shadows = sensors.Select(sensor => sensor.Shadows(env)).ToList();
      var result = shadows[0];
      for (var i = 1; i < shadows.Count; i++)
      {
        result = Polygon.Intersection(result, shadows[i]);
      }

      return result;
    }

    /// <summary>
    /// Calculate the shadow area for a list of sensors in an environment.
    /// </summary>
    /// <param name="sensors"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    public static double ShadowArea(List<Sensor> sensors, Environment env)
    {
      return Polygon.Area(Shadows(sensors, env));
    }

    /// <summary>
    /// Calculate the shadow area for a list of shadows.
    /// </summary>
    /// <param name="shadows"></param>
    /// <returns></returns>
    public static double ShadowArea(List<Polygon> shadows)
    {
      return Polygon.Area(shadows);
    }

    public override string ToString()
    {
      return "Position: " + Position + ", Size: " + Size +
             ", Rotation: " + Rotation + ", Range: " + Range
             + ", Fov: " + Fov;
    }
  }

  public class SSP
  {
    /// <summary>The problem environment.</summary>
    public Environment Env;
    public readonly List<Sensor> Sensors = new List<Sensor>();
    public readonly List<Obstacle> Obstacles = new List<Obstacle>();
    public readonly List<Polygon> ImportantAreas = new List<Polygon>();
    public double FieldWidth = 9;
    public double FieldHeight = 6;
    public double SensorRange = 12;
    public double SensorFov = 56.3;
    public double ObstacleSize = 0.1555;

    public void Init(int numberOfSensors = 1, int numberOfObstacles = 1)
    {
      Env = new Environment(
        new Rectangle(0, 0, FieldWidth, FieldHeight));

      Obstacles.Clear();
      Sensors.Clear();

      for (var i = 0; i < numberOfObstacles; i++)
      {
        var s = new Sensor(0, 0, 0, SensorRange, 
          SensorFov, ObstacleSize);

        PlaceWithoutCollision(s);
        Obstacles.Add(s);
        Env.Obstacles.Add(s);
      }
      
      for (var i = 0; i < numberOfSensors; i++)
      {
        var s = new Sensor(0, 0, 0, SensorRange,
          SensorFov, ObstacleSize);

        PlaceWithoutCollision(s);
        Sensors.Add(s);
        Env.Obstacles.Add(s);
      }
    }

    public void SetObstacles(double[][] positions)
    {
      Obstacles.ForEach(o => Env.Obstacles.Remove(o));
      Obstacles.Clear();
        
      foreach (var pos in positions)
      {
        var o = new Sensor(pos[0], pos[1], 0, 0, 0, ObstacleSize);
        Obstacles.Add(o);
        Env.Obstacles.Add(o);
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
    /// <param name="o">The object to place.</param>
    public void PlaceWithoutCollision(Obstacle o)
    {
      var collided = true;
      while (collided)
      {
        PlaceRandom(o);
        collided = CheckCollision(o);
      }
    }

    /// <summary>
    /// Place an object randomly inside the environment. This might
    /// cause a collision.
    /// </summary>
    /// <remarks>
    /// The object can also be placed on the edges of the field.
    /// </remarks>
    /// <param name="o"></param>
    public void PlaceRandom(Obstacle o)
    {
      o.Position = new Vector2(
        Pso.UniformRand(Env.Bounds.Min.X, Env.Bounds.Max.X), 
        Pso.UniformRand(Env.Bounds.Min.Y,Env.Bounds.Max.Y));
    }

    /// <summary>
    /// Check if a given object collides with any object inside the
    /// problem environment. If the object already exists inside the environment
    /// it won't collide with itself.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public bool CheckCollision(Obstacle o)
    {
      foreach (var o2 in Env.Obstacles)
      {
        if (o == o2) continue;

        var d = Vector2.Distance(o2.Position, o.Position);
        if (d < o2.Size + o.Size) return true;
      }

      return false;
    }

    /// <summary>
    /// Place a list of sensors according 
    /// </summary>
    /// <remarks>
    /// This will only change the sensors position and rotation, sensors
    /// won't be added to the environment.
    /// </remarks>
    /// <param name="vector"></param>
    /// <param name="sensors"></param>
    public static void PlaceFromVector(double[] vector, List<Sensor> sensors)
    {
      for (var i = 0; i < vector.Length; i += 3)
      {
        sensors[i / 3].Position = new Vector2(vector[i], vector[i + 1]);
        sensors[i / 3].Rotation = vector[i + 2];
      }
    }
  
    /// <summary></summary>
    /// <param name="value"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public double Normalize(double value, bool round = true)
    {
      if (round) return Math.Round(value / Env.Bounds.Area() * 100, 2);
      return value / Env.Bounds.Area() * 100;
    }

    /// <summary>
    /// Get the search space of the problem instance.
    /// </summary>
    /// <returns></returns>
    public double[][] Intervals()
    {
      var region = Env.Bounds;
      var intervals = new double[Sensors.Count * 3][];
      for (var i = 0; i < intervals.Length; i += 3)
      {
        intervals[i] = new[] {region.Min.X, region.Max.X};
        intervals[i + 1] = new[] {region.Min.Y, region.Max.Y};
        intervals[i + 2] = new[] {0, 360.0};
      }

      return intervals;
    }

    public SearchSpace SearchSpace()
    {
      return new SearchSpace(Intervals());
    }

    public double FitnessFct(double[] vector)
    {
      PlaceFromVector(vector, Sensors);
      var penalty = 0.0;
      
      foreach (var p in Sensors)
      {
        var x = Env.Bounds.Min.X + Env.Bounds.Width() / 2;
        var y = Env.Bounds.Min.Y + Env.Bounds.Height() / 2;
        var dist = Vector2.Distance(new Vector2(x, y), p.Position);
        
        if (!Env.Bounds.Contains(p.Position, true)) 
          penalty += dist;
      
        foreach (var o2 in Env.Obstacles)
        {
          if (p == o2) continue;

          var d = Vector2.Distance(o2.Position, p.Position);
          if (d < o2.Size + p.Size) return Env.Bounds.Area() + d;
        }
      }

      var shadows = Sensor.Shadows(Sensors, Env);
      var shadowArea = Polygon.Area(shadows);

      var importantHidden = Polygon.Intersection(
        ImportantAreas, shadows);
      var importantArea = Polygon.Area(ImportantAreas) - 
                          Polygon.Area(importantHidden);
      
      return penalty + shadowArea - importantArea;
    }
  }

  public class SspObjectiveFct : absObjectiveFunction
  {
    public readonly Obstacle[] Obstacles;
    private readonly Sensor[] _sensors;
    public readonly double FieldWidth;
    public readonly double FieldHeight;
    public readonly double ObstacleSize;
    public readonly double SensorRange;
    public readonly double SensorFov;

    
    public SspObjectiveFct(int numberOfSensors, int numberOfObstacles)
    {
      _sensors = new Sensor[numberOfSensors];
      Obstacles = new Obstacle[numberOfObstacles];
    }

    public override int NumberOfVariable()
    {
      return _sensors.Length * 3;
    }

    public override double F(List<double> x)
    {
      for (var i = 0; i < _sensors.Length; i++)
      {
        _sensors[i] = new Sensor(x[i * 3], x[i * 3 + 1], x[i * 3 + 2], 
          SensorRange, SensorFov, ObstacleSize);
//        _sensors[i].Shadows()
      }
      
      var allObstacles = new Obstacle[Obstacles.Length + _sensors.Length];
      Obstacles.CopyTo(allObstacles, 0);
      _sensors.CopyTo(allObstacles, Obstacles.Length - 1);
      throw new NotImplementedException();
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
  
  public class SspFct : absObjectiveFunction
  {
    public readonly SSP Raw;

    public SspFct(SSP raw)
    {
      Raw = raw;
    }

    public override int NumberOfVariable()
    {
      return Raw.Intervals().Length;
    }

    public override double F(List<double> x)
    {
      return Raw.FitnessFct(x.ToArray());
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