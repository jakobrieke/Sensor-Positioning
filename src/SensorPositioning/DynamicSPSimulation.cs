using System;
using System.Collections.Generic;
using System.Linq;
using charlie;
using Cairo;
using LinearAlgebra;
using Optimization;

namespace SensorPositioning
{
  public class DynamicSPSimulation : AbstractSimulation
  {
    public override string GetTitle()
    {
      return "Static Sensor Positioning";
    }

    public override string GetMeta()
    {
      return "Author: Jakob Rieke; Version v0.0.0; Deterministic: No"; 
    }
    
    public override string GetDescr()
    {
      return "A group of sensors and obstacles are placed inside a plain, " +
             "now the sensors have to be placed in a way that the area which " +
             "is monitored by them is maximized." +
             "All obstacles are circles with the same radius and all sensors " +
             "have the same field of view. Note that sensor themselves are " +
             "obstacles since they have a body.";
    }
    
    public override string GetConfig()
    {
      return
        "# -- Problem configuration\n" +
        "NumberOfSensors = 1\n" +
        "NumberOfObstacles = 1\n" +
        "FieldHeight = 6\n" +
        "FieldWidth = 9\n" +
        "SensorRange = 12\n" +
        "SensorFOV = 56.3\n" +
        "ObjectSize = 0.1555\n" +
        "# If ObstaclePositions is set NumberOfObstacles is\n" +
        "# ignored\n" +
        "# ObstaclePositions = [[2, 1]]\n" +
        "# ObstacleVelocity = [0.1, 0.1]\n" +
        "# InterestingArea01 = [[0, 0], [2, 0], [2, 1], [0, 1]]\n" +
        "StartPositionDistanceWeight = 0\n" +
        "StartPositionRotationWeight = 0\n" +
        "OptimizeObjectsIndividually\n" +
        "\n" +
        "# -- Optimizer configuration\n" +
        "UpdatesPerIteration = 30\n" +
        "\n" +
        "# -- Rendering configuration\n" +
        "Zoom = 80\n" +
        "DrawGrid\n" +
        "DrawSensorLines\n" +
        "# Draws the start position for the optimization\n" +
        "# Changes over time if InitializeEachUpdate is set\n" +
        "# DrawStartPositions\n" +
        "\n" +
        "# -- Logging configuration\n" +
        "# LogChanges\n" +
        "# LogClearText\n" +
        "# LogEvaluations\n" +
        "LogRoundedPositions";
    }

    private int _zoom;
    private bool _drawSensorLines;
    private bool _drawStartPositions;
    private bool _drawGrid;
    private bool _logClearText;
    private bool _logChanges;
    private bool _logEvaluations;
    private bool _logRoundedPositions;
    
    private SPSO2006 _optimizer;
    private uint _updatesPerIteration;
    private SensorPositionObj _objective;
    private List<Agent> _sensors;
    private Vector2 _obstacleStartVelocity;
    private List<Vector2> _obstacleVelocities;
    private List<Tuple<int, double>> _changes;

    public override void Init(Dictionary<string, string> config)
    {
      Sensors2D.SensorArcPrecision = 2;
      
      _changes = new List<Tuple<int, double>>();
      _sensors = new List<Agent>();
      _logChanges = config.ContainsKey("LogChanges");
      _logClearText = config.ContainsKey("LogClearText");
      _logRoundedPositions = config.ContainsKey("LogRoundedPositions");
      _logEvaluations = config.ContainsKey("LogEvaluations");
      _zoom = GetInt(config, "Zoom", 80);
      _drawSensorLines = config.ContainsKey("DrawSensorLines");
      _drawStartPositions = config.ContainsKey("DrawStartPositions");
      _drawGrid = config.ContainsKey("DrawGrid");
      
      // -- Initialize objective
      
      var numberOfSensors = GetInt(config, "NumberOfSensors", 1);
      var numberOfObstacles = GetInt(config, "NumberOfObstacles", 1);
      
      _objective = new SensorPositionObj(
        (uint)numberOfSensors,
        (uint)numberOfObstacles, 
        GetDouble(config, "FieldWidth", 9),
        GetDouble(config, "FieldHeight", 6),
        GetDouble(config, "SensorRange", 12),
        GetDouble(config, "SensorFOV", 56.3),
        GetDouble(config, "ObjectSize", 0.1555)
        );
      
      // -- Initialize interesting areas
      
      foreach (var key in config.Keys)
      {
        if (!key.StartsWith("InterestingArea")) continue;

        var matrix = ParseMatrix(config[key]);
        if (matrix == null || matrix.Length < 3) continue;

        var impArea = new Polygon();
        foreach (var row in matrix)
        {
          impArea.Add(new Vector2(row[0], row[1]));
        }
        _objective.InterestingAreas.Add(impArea);
      }
      
      // -- Initialize obstacles from positions
      
      var obstaclePositions = GetMatrix(config, "ObstaclePositions");
      if (obstaclePositions != null)
      {
        _objective.SetObstacles(obstaclePositions);
      }
      
      _objective.StartPositionDistanceWeight = GetDouble(config, 
        "StartPositionDistanceWeight", 0);
      _objective.StartPositionRotationWeight = GetDouble(config, 
        "StartPositionRotationWeight", 0);
      
      _obstacleStartVelocity = GetVector(config, "ObstacleVelocity") ?? 
                          Vector2.Zero;
      _obstacleVelocities = new List<Vector2>(_objective.Obstacles.Count);
      for (var i = 0; i < _objective.Obstacles.Count; i++)
      {
        _obstacleVelocities.Add(_obstacleStartVelocity);
      }
      
      // -- Initialize optimizer

      var swarm = Pso.SwarmSpso2006(_objective.SearchSpace(),
        x => _objective.F(x.ToList()));
      _optimizer = new SPSO2006(swarm, _objective);
      
      _optimizer.Init();
      _sensors = _objective.ToAgents(_optimizer.Result.ToArray());
      _objective.StartPosition = _sensors;

      _updatesPerIteration = (uint) GetInt(config, 
        "UpdatesPerIteration", 30);
    }

    private void UpdateObstacles()
    {
      for (var i = 0; i < _objective.Obstacles.Count; i++)
      {
        var o = _objective.Obstacles[i];
        var p = new Particle
        {
          LastPosition = o.Position, 
          Position = o.Position + _obstacleVelocities[i], 
          Velocity = _obstacleVelocities[i]
        };
        var sp = new SearchSpace(new[]
        {
          new[] {0, _objective.FieldWidth}, new[] {0, _objective.FieldHeight}
        });
        Confinement.Bounce(p, sp);
        
        _obstacleVelocities[i] = p.Velocity;
        _objective.Obstacles[i] = new Circle(p.Position, o.Radius);
      }
    }

    private void UpdateOptimizerDynamic()
    {
      var lastBest = _optimizer.Result.Eval;
      _objective.StartPosition = _sensors;
      
      _optimizer.Init();
      _optimizer.Iterate((int) _updatesPerIteration);

      if (_logChanges) 
      {
        _changes.Add(new Tuple<int, double>(
          _optimizer.IterationCount, lastBest - _optimizer.Result.Eval));
      }
    }
    
    public override void Update(long deltaTime)
    {
      // Note:
      // ~50ms - 1 Sensor,
      // ~150ms - 2 Sensors (MacBook Pro Mid 2014)
      
      const int i = 0;
      var o = _objective.Obstacles[i];
      var p = new Particle
      {
        LastPosition = o.Position, 
        Position = o.Position + _obstacleVelocities[i], 
        Velocity = _obstacleVelocities[i]
      };
      var sp = new SearchSpace(new[]
      {
        new[] {0, _objective.FieldWidth}, new[] {0, _objective.FieldHeight}
      });
      Confinement.Bounce(p, sp);
        
      _obstacleVelocities[i] = p.Velocity;
      _objective.Obstacles[i] = new Circle(p.Position, o.Radius);
        
      // -- Update sensor
        
      _optimizer.Init();
      _optimizer.Iterate((int) _updatesPerIteration);
      
      UpdateObstacles();
      
      _sensors = _objective.ToAgents(_optimizer.Result.ToArray());
    }

    private void DrawCoordinateSystem(Context cr)
    {
      var width = _objective.FieldWidth * _zoom;
      var height = _objective.FieldHeight * _zoom;
      
      cr.SetSourceRGB(.8, .8, .8);
      cr.Rectangle(0, 0, width, height);
      cr.ClosePath();
      cr.Fill();

      if (!_drawGrid) return;
      
      cr.SetSourceRGBA(0, 0, 0, 0.2);
      cr.LineWidth = .5;
      for (var i = 0.0; i < width; i += width / 10)
      {
        cr.NewPath();
        cr.MoveTo(i, 0);
        cr.LineTo(i, height);
        cr.ClosePath();
        cr.Stroke();
      }

      for (var i = 0.0; i < height; i += height / 10)
      {
        cr.NewPath();
        cr.MoveTo(0, i);
        cr.LineTo(width, i);
        cr.ClosePath();
        cr.Stroke();
      }
    }

    private void DrawPolygon(Context cr, Polygon polygon)
    {
      cr.NewPath();
      cr.MoveTo(polygon[0].X * _zoom, polygon[0].Y * _zoom);
      for (var i = 1; i < polygon.Count; i++)
      {
        cr.LineTo(polygon[i].X * _zoom, polygon[i].Y * _zoom);
      }
      cr.ClosePath();
    }

    /// <summary>
    /// Draw interesting and known areas. 
    /// </summary>
    /// <param name="cr"></param>
    private void DrawAreas(Context cr)
    {
      cr.SetSourceRGBA(0, 0, 0, 0.3);
      foreach (var area in _objective.InterestingAreas)
      {
        DrawPolygon(cr, area);
        cr.LineWidth = 4;
        cr.Stroke();
      }
      
      cr.SetSourceRGBA(0, 0, 0, 0.3);
      foreach (var area in _objective.KnownAreas)
      {
        DrawPolygon(cr, area);
        cr.LineWidth = 4;
        cr.Stroke();
      }
    }
    
    private void DrawSensors(Context cr)
    {
      var shadows = _objective.NotPerceptible(_sensors);
      cr.SetSourceRGBA(0, 0, 0, 0.7);
      
      foreach (var polygon in shadows)
      {
        if (polygon.Count == 0) continue;
        DrawPolygon(cr, polygon);
        cr.Fill();
      }
      
      foreach (var sensor in _sensors)
      {
        if (_drawSensorLines)
        {
          cr.SetSourceRGB(1, 0, 0);
          cr.SetDash(new double[]{10}, 0);
          cr.LineWidth = .4;
          DrawPolygon(cr, sensor.AreaOfActivity().ToPolygon());
          cr.Stroke();
        }

        cr.SetSourceRGB(0.753, 0.274, 0.275);
        cr.Arc(
          sensor.Position.X * _zoom,
          sensor.Position.Y * _zoom,
          sensor.Size * _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
        cr.Fill();
      }
    }

    private void DrawStartPosition(Context cr)
    {
      foreach (var sensor in _objective.StartPosition)
      {
        cr.SetSourceRGB(0.753, 0.274, 0.275);
        cr.Arc(
          sensor.Position.X * _zoom,
          sensor.Position.Y * _zoom,
          sensor.Size * _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
        cr.SetDash(new double[]{4}, 0);
        cr.LineWidth = 1;
        cr.Stroke();
      }
    }
    
    private void DrawObstacles(Context cr)
    {
      foreach (var obstacle in _objective.Obstacles)
      {
        cr.SetSourceRGB(0.159, 0.695, 0.775);
        cr.Arc(
          obstacle.Position.X * _zoom,
          obstacle.Position.Y * _zoom,
          obstacle.Radius * _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
      }

      cr.Fill();
    }

    public override void Render(Context cr, int width, int height)
    {
      cr.Translate(
        (width - _objective.FieldWidth * _zoom) / 2, 
        (width - _objective.FieldHeight * _zoom) / 2);
      
      DrawCoordinateSystem(cr);
      DrawAreas(cr);
      DrawSensors(cr);
      if (_drawStartPositions) DrawStartPosition(cr);
      DrawObstacles(cr);
    }

    public override string Log()
    {
      var sensorPositions = "[";
      var i = 1;
      foreach (var sensor in _sensors)
      {
        sensorPositions += _logRoundedPositions ?
          $"[{Math.Round(sensor.Position.X, 2)}, " +
          $"{Math.Round(sensor.Position.Y, 2)}, " +
          $"{Math.Round(sensor.Rotation, 2)}]" :
          $"[{sensor.Position.X}, {sensor.Position.Y}, {sensor.Rotation}]";
        
        if (i++ < _sensors.Count) sensorPositions += ", ";
      }
      sensorPositions += "]";
      
      var obstaclePositions = "[";
      i = 1;
      foreach (var obstacle in _objective.Obstacles)
      {
        obstaclePositions += _logRoundedPositions ?
          $"[{Math.Round(obstacle.Position.X, 2)}, " +
          $"{Math.Round(obstacle.Position.Y, 2)}]" :
          $"[{obstacle.Position.X}, {obstacle.Position.Y}]";
        
        if (i++ < _objective.Obstacles.Count) obstaclePositions += ", ";
      }
      obstaclePositions += "]";

      var changes = "[";
      i = 1;
      foreach (var (iteration, improvement) in _changes)
      {
        changes += $"[{iteration}, {improvement}]";
        if (i++ < _changes.Count) changes += ", ";
      }
      changes += "]";

      if (_logClearText)
      {
        return
          $"sensors: {sensorPositions}\n" +
          $"obstacles: {obstaclePositions}\n" +
          (_logChanges ? $"changes: {changes}\n" : "") + 
          "global-best: " + _optimizer.Result.Eval;
      }
      
      return 
        $"<sensors>{sensorPositions}</sensors>\n" +
        $"<obstacles>{obstaclePositions}</obstacles>\n" +
        (_logChanges ? $"<changes>{changes}</changes>\n" : "") + 
        (_logEvaluations ? 
          $"<objective-evaluations>{_objective.Evaluations}" +
          "</objective-evaluations>\n" : "") +
        "<global-best>" + _optimizer.Result.Eval + "</global-best>";
    }
  }
}