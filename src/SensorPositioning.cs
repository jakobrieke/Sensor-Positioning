using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Optimization;
using charlie;
using Shadows;
using Context = Cairo.Context;
using Rectangle = Geometry.Rectangle;
using Vector2 = Geometry.Vector2;


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

  public double Area()
  {
    return Bounds.Area();
  }
}

public class Obstacle
{
  public Vector2 Position;
  public readonly double Size;

  public Obstacle(Vector2 position, double size)
  {
    Position = position;
    Size = size;
  }

  public Obstacle(double x, double y, double size)
  {
    Position = new Vector2(x, y);
    Size = size;
  }
}

public class Sensor : Obstacle
{
  public double Rotation;
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
  public static List<Polygon> Shadows(List<Sensor> sensors,
    Environment env)
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

public class SensorPositioningProblem
{
  /// <summary>The problem environment.</summary>
  public Environment Env;

  /// <summary>A list of sensors.</summary>
  public List<Sensor> TeamA;

  /// <summary>A list of obstacles</summary>
  public List<Sensor> TeamB;


  public SensorPositioningProblem(
    int sizeTeamA = 1,
    int sizeTeamB = 1,
    double fieldWidth = 9f,
    double fieldHeight = 6f,
    double playerSensorRange = 12,
    double playerSensorFov = 56.3f,
    double playerSize = 0.1555f
  )
  {
    var field = new Rectangle(0, 0, fieldWidth, fieldHeight);
    Env = new Environment(field);

    TeamA = new List<Sensor>();
    TeamB = new List<Sensor>();

    for (var i = 0; i < sizeTeamA; i++)
    {
      var s = new Sensor(0, 0, 0, playerSensorRange,
        playerSensorFov, playerSize);

      PlaceWithoutCollision(s);
      TeamA.Add(s);
      Env.Obstacles.Add(s);
    }

    for (var i = 0; i < sizeTeamB; i++)
    {
      var s = new Sensor(0, 0, 0, playerSensorRange,
        playerSensorFov, playerSize);

      PlaceWithoutCollision(s);
      TeamB.Add(s);
      Env.Obstacles.Add(s);
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
    var x = Pso.UniformRand(Env.Bounds.Min.X,
      Env.Bounds.Max.X);
    var y = Pso.UniformRand(Env.Bounds.Min.Y,
      Env.Bounds.Max.Y);
    o.Position = new Vector2(x, y);
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
  
  public static void PlaceFromVector(double[] vector, double[] lastState, 
    List<Sensor> sensors, double moveWidth, double rotWidth)
  {
    for (var i = 0; i < vector.Length / 3; i += 3)
    {
      var v1 = new Vector2(lastState[i], lastState[i + 1]);
      var v2 = new Vector2(vector[i], vector[i + 1]);
      
      Console.WriteLine("Distance: " + Vector2.Distance(v1, v2));
      if (Vector2.Distance(v1, v2) > moveWidth)
      {
        var pos = v1.Move(Vector2.Gradient(v1, v2), moveWidth);
        vector[i] = pos.X;
        vector[i + 1] = pos.Y;
      }
      if (vector[i + 2] > rotWidth) vector[i + 2] = rotWidth;
      
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
    if (round) return Math.Round(value / Env.Area() * 100, 2);
    return value / Env.Area() * 100;
  }

  /// <summary>
  /// Get the search space of the problem instance.
  /// </summary>
  /// <returns></returns>
  public double[][] Intervals()
  {
    var region = Env.Bounds;
    var intervals = new double[TeamA.Count * 3][];
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
}

public class SSP : SensorPositioningProblem
{
  public SSP(
    int sizeTeamA = 1,
    int sizeTeamB = 1,
    double fieldWidth = 9,
    double fieldHeight = 6,
    double playerSensorRange = 12,
    double playerSensorFov = 56.3,
    double playerSize = 0.1555
  ) : base(sizeTeamA, sizeTeamB, fieldWidth, fieldHeight,
    playerSensorRange, playerSensorFov, playerSize)
  {}


  public double FitnessFct(double[] vector)
  {
    PlaceFromVector(vector, TeamA);
    foreach (var p in TeamA)
    {
      if (!CheckCollision(p)) continue;
      Collisions++;
      return double.PositiveInfinity;
    }
    return Sensor.ShadowArea(TeamA, Env);
  }

  public int Collisions { get; private set; }
}

public class SSPByStep : SensorPositioningProblem
{
  private double _moveWidth;
  private double _rotWidth;
  
  public SSPByStep(
    double moveWidth,
    int sizeTeamA = 1,
    int sizeTeamB = 1,
    double fieldWidth = 9,
    double fieldHeight = 6,
    double playerSensorRange = 12,
    double playerSensorFov = 56.3,
    double playerSize = 0.1555
  ) : base(sizeTeamA, sizeTeamB, fieldWidth, fieldHeight,
    playerSensorRange, playerSensorFov, playerSize)
  {
    _moveWidth = moveWidth;
  }

  public double FitnessFct(double[] vector, double[] lastState)
  {
    if (lastState == null) return double.PositiveInfinity;
    
    PlaceFromVector(vector, lastState, TeamA, _moveWidth, _rotWidth);
    foreach (var p in TeamA)
    {
      if (CheckCollision(p)) return double.PositiveInfinity;
    }
    return Sensor.ShadowArea(TeamA, Env);
  }
}

public class StaticSSP : AbstractSimulation
{
  private int _numberOfObstacles;
  private int _numberOfSensors;
  private double _fieldWidth;
  private double _fieldHeight;
  private double _sensorRange;
  private double _sensorFov;
  private double _sensorSize;
  private double _obstacleSize;
  private List<double[]> _sensors; // x, y, size, rotation, range, field of view
  private List<double[]> _obstacles; // x, y, size
  private Rectangle _field;
  private int _collisions;
  private Swarm _swarm;
  
  public override string GetTitle()
  {
    return "Static Sensor Positioning";
  }

  public override string GetMeta()
  {
    return "Author: Jakob Rieke; Version: 1.0.0";
  }

  public override string GetDescr()
  {
    return "Place a number of sensors into a field of obstacles and " +
           "optimize their position so that their area where they can sense" +
           "an input is maximized.";
  }

  public override string GetConfig()
  {
    return "FieldWidth = 9\n" +
           "FieldHeight = 6\n" +
           "NumberOfSensors = 1\n" +
           "SensorRange = 20\n" +
           "SensorFOV = 56.3\n" +
           "SensorSize = 0.1555\n" +
            "NumberOfObstacles = 1\n" +
           "ObstacleSize = 0.1555";
  }

  private bool CheckCollision(double[] x)
  {
    foreach (var y in _obstacles)
    {
      if (x == y) continue;
      var d = Vector2.Distance(
        new Vector2(x[0], x[1]), 
        new Vector2(y[0], y[1]));
      if (d < x[2] + y[2]) return true;
    }
    foreach (var y in _sensors)
    {
      if (x == y) continue;
      var d = Vector2.Distance(
        new Vector2(x[0], x[1]), 
        new Vector2(y[0], y[1]));
      if (d < x[2] + y[2]) return true;
    }
    return false;
  }
  
  private void PlaceRandom(double[] x)
  {
    var rand = new PcgRandom();
    x[0] = rand.GetDouble(_field.Min.X, _field.Max.X);
    x[1] = rand.GetDouble(_field.Min.Y, _field.Max.Y);
  }
  
  private void PlaceWithoutCollision(double[] x)
  {
    var collided = true;
    while (collided)
    {
      PlaceRandom(x);
      collided = CheckCollision(x);
    }
  }

  /// <summary>
  /// Calculate the core shadows from a vector x.
  /// </summary>
  /// <param name="x"></param>
  /// <returns></returns>
  private List<Polygon> CoreShadows(double[] x)
  {
    for (var i = 0; i < _numberOfSensors; i++)
    {
      _sensors[i][0] = x[i / 3];
      _sensors[i][1] = x[i / 3 + 1];
      _sensors[i][3] = x[i / 3 + 2];
    }
    
    var shadows = new List<List<Polygon>>();
    foreach (var sensor in _sensors)
    {
      var obstacles = new List<Circle>();
      foreach (var o in _obstacles)
      {
        obstacles.Add(new Circle(o[0], o[1], o[2]));
      }
      foreach (var s in _sensors)
      {
        if (sensor == s) continue;
        obstacles.Add(new Circle(s[0], s[1], s[2]));
      }
      shadows.Add(Shadows2D.Shadows(new Arc(sensor[0], sensor[1], sensor[4],
        sensor[5], sensor[3]), obstacles, _field));
    }
    return Shadows2D.CoreShadows(shadows);
  }

  public override void Init(Dictionary<string, string> config)
  {
    _fieldWidth = GetDouble(config, "FieldWidth", 9);
    _fieldHeight = GetDouble(config, "FieldHeight", 6);
    _numberOfObstacles = GetInt(config, "NumberOfObstacles", 1);
    _numberOfSensors = GetInt(config, "NumberOfSensors", 1);
    _sensorRange = GetDouble(config, "SensorRange", 20);
    _sensorFov = GetDouble(config, "SensorFOV", 56.3);
    _sensorSize = GetDouble(config, "SensorSize", 0.1555);
    _obstacleSize = GetDouble(config, "ObstacleSize", 0.1555);
    
    _sensors = new List<double[]>(_numberOfSensors);
    _obstacles = new List<double[]>(_numberOfObstacles);
    _field = new Rectangle(0, 0, _fieldWidth, _fieldHeight);
    _collisions = 0;

    for (var i = 0; i < _numberOfSensors; i++)
    {
      _sensors.Add(new[] { 0, 0, _sensorSize, 0, _sensorRange, _sensorFov });
      PlaceWithoutCollision(_sensors[i]);
    }

    for (var i = 0; i < _numberOfObstacles; i++)
    {
      _obstacles.Add(new [] { 0, 0, _obstacleSize });
      PlaceWithoutCollision(_obstacles[i]);
    }

    var intervals = new double[_numberOfSensors * 3][];
    for (var i = 0; i < _numberOfSensors; i++)
    {
      intervals[i * 3] = new[] {_field.Min.X, _field.Max.X};
      intervals[i * 3 + 1] = new[] {_field.Min.Y, _field.Max.Y};
      intervals[i * 3 + 2] = new[] {0, 360.0};
    }
    var fitness = new Func<double[], double>(x => 
      Polygon.Area(CoreShadows(x)));
    
    _swarm = Pso.SwarmSpso2011(new SearchSpace(intervals), fitness);
    _swarm.Initialize();
  }

  public override void Update(long deltaTime)
  {
    _swarm.IterateOnce();
  }

  public override string Log()
  {
    return "Global Best: " + _swarm.GlobalBestValue;
  }

  public override void Render(Context ctx, int width, int height)
  {
    const double scale = 44.44;
    const double gridSize = 44.44;
    
    ctx.SetSourceRGB(.1, .1, .1);
    ctx.Rectangle(_field.Min.X * scale, _field.Min.Y * scale,
      _fieldWidth * scale, _fieldHeight * scale);
    ctx.Fill();

    ctx.SetSourceRGB(1, 1, 1);
    ctx.LineWidth = .2;
    for (var i = 0; i < _fieldHeight * scale / gridSize; i++)
    {
      ctx.NewPath();
      ctx.MoveTo(_field.Min.X * scale, i * gridSize);
      ctx.LineTo(_field.Min.X * scale + _fieldWidth * scale, i * gridSize);
      ctx.ClosePath();
      ctx.Stroke();
    }
    for (var i = 0; i < _fieldWidth * scale / gridSize; i++)
    {
      ctx.NewPath();
      ctx.MoveTo(i * gridSize, _field.Min.Y * scale);
      ctx.LineTo(i * gridSize, _field.Min.Y * scale + _fieldHeight * scale);
      ctx.ClosePath();
      ctx.Stroke();
    }
    
    ctx.SetSourceRGB(.1, .1, .9);
    foreach (var x in _obstacles)
    {
      ctx.Arc(x[0] * scale, x[1] * scale, x[2] * scale, 0, Math.PI * 2);
      ctx.Fill();
    }

    // Draw sensors
    ctx.SetSourceRGB(.9, .1, .1);
    var best = _swarm.GlobalBest;
    for (var i = 0; i < best.Length / 3; i++)
    {
      ctx.Arc(best[0] * scale, best[1] * scale, _sensorSize * scale, 
        0, Math.PI * 2);
      ctx.Fill();
    }
    
    // Draw shadows
    ctx.SetSourceRGBA(.9, .1, .1, .3);
    var shadows = CoreShadows(best);
    foreach (var shadow in shadows)
    {
      ctx.NewPath();
      ctx.MoveTo(shadow[0].X * scale, shadow[0].Y * scale);
      for (var i = 1; i < shadow.Count; i++)
      {
        ctx.LineTo(shadow[i].X * scale, shadow[i].Y * scale);
      }
      if (shadow[0] != shadow[shadow.Count - 1])
      {
        ctx.MoveTo(shadow[0].X * scale, shadow[0].Y * scale);
      }
      ctx.ClosePath();
    }
    ctx.Fill();
  }
}