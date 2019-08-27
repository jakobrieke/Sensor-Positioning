using System;
using System.Collections.Generic;
using charlie;
using Cairo;
using LinearAlgebra;
using MersenneTwister;
using Optimization;
using Optimization.LibOptimizationWrapper;
using MTRandom = MersenneTwister.MTRandom;

namespace SensorPositioning
{
  /* Recent Changes
   *
   * v2.1.1
   * - Fix a bug that LibOptimization Jade implementation might not use the
   *   specified Random instance
   * v2.1.0
   * - Use a Mersenne Twister 19937 with a 32 bit word as PRNG for all
   *   optimization algorithms
   * - Add an option to set the seed used by the optimization algorithms
   * - Restructure and unify code base
   * - Add a hidden experimental version of a new JADE implementation
   * v2.0.0
   * - Rename options
   *   PlayerSensorRange -> SensorRange,
   *   PlayerSensorFOV -> SensorFOV,
   *   PlayerSize -> ObstacleSize,
   *   ImpArea -> InterestingArea
   * v1.9.1
   * - Fix error in default configuration
   * - Split Update() into multiple methods
   * - v1.9.0
   * - Add option to hide grid
   * - Increase objective performance by removing unnecessary call to check
   *   if a sensor is inside the given boundaries
   * v1.8.0
   * - Disable usage of convergence criterion for PSO and ADE
   * - Add objective evaluation count to logging
   * - Add an option to use a dynamic local search space on SPSO-2006
   * - Fix that start position is not updated correctly 
   * v1.7.2
   * - Parallelize function SensorPositioning.Shadows(sensors)
   * v1.7.1
   * - Tiny code optimizations
   * v1.7.0
   * - Add option to initialize and iterate the optimizer every simulation
   * v1.6.0
   * - Change default Configuration
   * - Add experimental optimizer MinimalPSO
   * - Remove deprecated list of possible optimizers
   * - Add velocity to obstacles
   * - Remove sensor position option from config since it's not working
   * - Add option to draw start positions
   * - Add penalty for distance and rotation to sensor start position
   * v1.5.1
   * - Restructure SensorPositionObj class
   * v1.5.0
   * - Fix comma in obstacles not logged correctly
   * - Add option to toggle if changes should be logged or not
   * - Save iterations when global best changes
   * v1.4.1
   * - Translate LibOptimization to C#
   * v1.4.0
   * - Add option to hide sensor lines
   * - Change colors of sensors and obstacles
   * - Add functionality to mark important areas with polygons
   *   -> important areas reduce the fitness function value by their area
   * - Change config attribute names "SizeTeamA", "SizeTeamB"
   * - Add option to set fixed positions for obstacles
   * - Change rendering color to red for sensor area
   * - Fix 'sometimes worse best' in PSO implementation
   * - Test standard PSO 2006 without neighbourhood -> did not work
   * - Fix standard PSO 2007 & 2006 implementation
   * - Render field centered to render area
   * - Test behaviour if fitness does not return +infinity when two sensors
   *   overlap -> optimization with LibOptimization.PSO is better
   * - Add punishment factor is added to fitness so that sensors are
   *   getting drawn into the field
   */
  public class StaticSpSimulation : AbstractSimulation
  {
    public override string GetTitle()
    {
      return "Static Sensor Positioning";
    }

    public override string GetMeta()
    {
      return "Author: Jakob Rieke; Version v2.1.1; Deterministic: No"; 
    }
    
    public override string GetDescr()
    {
      return "A group of agents and obstacles are placed inside a plain, " +
             "now the agents have to be placed in a way that the area which " +
             "is monitored by them is maximized. The monitored area here is " +
             "white and the non monitored area is gray." +
             "All obstacles are circles with the same radius and all sensors " +
             "have the same field of view. Note that agents themselves are " +
             "obstacles since they have a body.\n" +
             "\n" +
             "Known bugs:\n" +
             "- Perceptible area is rendered black if area polygon is not open";
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
        "# InterestingArea02 = [[0, 6], [2, 6], [2, 5], [0, 5]]\n" +
        "# InterestingArea03 = [[9, 0], [7, 0], [7, 1], [9, 1]]\n" +
        "# InterestingArea04 = [[9, 6], [9, 5], [7, 5], [7, 6]]\n" +
        "StartPositionDistanceWeight = 0\n" +
        "StartPositionRotationWeight = 0\n" +
        "\n" +
        "# -- Optimizer configuration\n" +
        "# The random seed greater than zero\n" +
        "# Use -1 to use the current milliseconds as random seed\n" +
        "OptimizationRandomSeed = -1\n" +
        "# The function used to optimize the problem,\n" +
        "# possible values are:\n" +
        "# PSO, SPSO-2006, SPSO-2007, SPSO-2011, ADE\n" +
        "Optimizer = SPSO-2006\n" +
        "# If InitializeEachUpdate is not set, Updates\n" +
        "# per iteration is always one\n" +
        "# InitializeEachUpdate\n" +
        "UpdatesPerIteration = 30\n" +
        "# If enabled the search space for SPSO-2006 is\n" +
        "# restricted to a rectangle around each of the\n" +
        "# sensors last positions\n" +
        "# DynamicSearchSpaceRange = [0.1, 0.1]\n" +
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

    private Optimization.Optimization _optimizer;
    private SensorPositionObj _objective;
    private int _zoom;
    private bool _drawSensorLines;
    private bool _drawStartPositions;
    private bool _drawGrid;
    /// <summary>
    /// A list of all changes in the optimum found by the optimizer.
    /// </summary>
    private List<Tuple<int, double>> _changes;
    private bool _logChanges;
    private bool _logClearText;
    private bool _logEvaluations;
    private bool _logRoundedPositions;
    private List<Agent> _sensors;
    private Vector2 _obstacleStartVelocity;
    private List<Vector2> _obstacleVelocities;
    /// <summary>
    /// Indicates if the optimizer should be initialized again each update.
    /// </summary>
    private bool _initEachUpdate;
    private uint _updatesPerIteration;
    private Vector2 _dynamicSearchSpaceRange;

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

      var optimizerName = config.ContainsKey("Optimizer") ? 
        config["Optimizer"] : null;
      
      var sp = new SearchSpace(_objective.Intervals());
      
      var optSeed = GetInt(config, "OptimizationRandomSeed", -1);
      if (optSeed == -1) optSeed = DateTime.Now.Millisecond;
      
      if (optimizerName == "SPSO-2006")
      {
        _optimizer = new StandardPso2006(sp, _objective)
        {
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
        };
      }
      else if (optimizerName == "SPSO-2007")
      {
        _optimizer = new StandardPso2007(sp, _objective)
        {
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
        };
      }
      else if (optimizerName == "SPSO-2011")
      {
        _optimizer = new StandardPso2011(sp, _objective)
        {
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
        };
      }
      else if (optimizerName == "PSO")
      {
        _optimizer = new PsoWrapper(_objective, sp)
        {
          SwarmSize = 40,
          IsUseCriterion = false,
//          InitialPosition = _objective.SearchSpace().RandPos(),
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
        };
      }
      else if (optimizerName == "ADE")
      {
        _optimizer = new JadeWrapper(_objective, sp)
        {
          PopulationSize = 40,
          IsUseCriterion = false,
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
//          LowerBounds = _objective.Intervals().Select(i => i[0]).ToArray(),
//          UpperBounds = _objective.Intervals().Select(i => i[1]).ToArray()
        };
      }
      else if (optimizerName == "JADE")
      {
        _optimizer = new Jade(_objective, _objective.SearchSpace())
        {
          Random = MTRandom.Create(optSeed, MTEdition.Original_19937)
        };
      }
      else
      {
        throw new Exception($"Optimizer {optimizerName} is not supported");
      }
      
      _optimizer.Init();
      _sensors = _objective.ToAgents(_optimizer.Best().Position.ToArray());
      _objective.StartPosition = _sensors;

      _initEachUpdate = config.ContainsKey("InitializeEachUpdate");
      _updatesPerIteration = (uint) GetInt(config, 
        "UpdatesPerIteration", 30);
      _dynamicSearchSpaceRange =
        GetVector(config, "DynamicSearchSpaceRange") ?? Vector2.Zero;
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

    private void UpdateOptimizerStatic()
    {
      var lastBest = _optimizer.Best().Value;
      _optimizer.Update();
      
      if (_logChanges && lastBest > _optimizer.Best().Value) 
      {
        _changes.Add(new Tuple<int, double>(
          _optimizer.Iteration, lastBest - _optimizer.Best().Value));
      }
    }
    
    private void UpdateOptimizerDynamic()
    {
      var lastBest = _optimizer.Best().Value;
      _objective.StartPosition = _sensors;
      
      if (_optimizer.GetType() == typeof(StandardPso2006) 
          && _dynamicSearchSpaceRange != Vector2.Zero)
      {
        var pso = (StandardPso2006) _optimizer;
        var intervals = pso.SearchSpace.Intervals;
        for (var i = 0; i < intervals.Length; i += 3)
        {
          var pos = _sensors[i / 3].Position;
//            var rot = _sensors[i / 3].Rotation;
          intervals[i] = new []
          {
            pos.X - _dynamicSearchSpaceRange.X, 
            pos.X + _dynamicSearchSpaceRange.X
          }; // x
          intervals[i + 1] = new []
          {
            pos.Y - _dynamicSearchSpaceRange.Y, 
            pos.Y + _dynamicSearchSpaceRange.Y
          }; // y
          intervals[i + 2] = new []{0.0, 360}; // rotation
        }
      }
      
      _optimizer.Init();
      _optimizer.Iterate(_updatesPerIteration);

      if (_logChanges) 
      {
        _changes.Add(new Tuple<int, double>(
          _optimizer.Iteration, lastBest - _optimizer.Best().Value));
      }
    }
    
    public override void Update(long deltaTime)
    {
      UpdateObstacles();
      
      if (_initEachUpdate) UpdateOptimizerDynamic();
      else UpdateOptimizerStatic();

      _sensors = _objective.ToAgents(_optimizer.Best().Position.ToArray());
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

    private void DrawInterestingArea(Context cr)
    {
      cr.SetSourceRGBA(1, 1, 1, 0.5);
      foreach (var area in _objective.InterestingAreas)
      {
        DrawPolygon(cr, area);
        cr.Fill();
//        cr.LineWidth = 4;
//        cr.Stroke();
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
      DrawInterestingArea(cr);
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
          "global-best: " + _optimizer.Best().Value;
      }
      
      return 
        $"<sensors>{sensorPositions}</sensors>\n" +
        $"<obstacles>{obstaclePositions}</obstacles>\n" +
        (_logChanges ? $"<changes>{changes}</changes>\n" : "") + 
        (_logEvaluations ? 
          $"<objective-evaluations>{_objective.Evaluations}" +
          "</objective-evaluations>\n" : "") +
        "<global-best>" + _optimizer.Best().Value + "</global-best>";
    }
  }
}