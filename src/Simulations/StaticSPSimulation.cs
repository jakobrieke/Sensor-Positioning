using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using charlie;
using Cairo;
using Geometry;
using LibOptimization.Optimization;

namespace sensor_positioning
{
  public class StaticSpSimulation : AbstractSimulation
  {
    public override string GetTitle()
    {
      return "Static Sensor Positioning";
    }

    public override string GetMeta()
    {
      /* Recent Changes
       *
       * Restructure SensorPositionObj class
       * Fix comma in obstacles not logged correctly
       * Add option to toggle if changes should be logged or not
       * Save iterations when global best changes
       * Translate LibOptimization to C#
       * Add option to hide sensor lines
       * Change colors of sensors and obstacles
       * Add functionality to mark important areas with polygons
       *   -> important areas reduce the fitness function value by their area
       * Change config attribute names "SizeTeamA", "SizeTeamB"
       * Add option to set fixed positions for obstacles
       * Change rendering color to red for sensor area
       * Fix 'sometimes worse best' in PSO implementation
       * Test standard PSO 2006 without neighbourhood -> did not work
       * Fix standard PSO 2007 & 2006 implementation
       * Render field centered to render area
       * Test behaviour if fitness does not return +infinity when two sensors
       *   overlap -> optimization with LibOptimization.PSO is better
       * Test behaviour if punishment factor is added to fitness
       *   -> sensors are getting drawn into the center
       */
      return "Author: Jakob Rieke; Version v1.5.1; Deterministic: No"; 
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
      // Todo: API breaking changes, rename
      // PlayerSensorRange -> SensorRange
      // PlayerSensorFOV -> SensorFOV
      // PlayerSize -> ObstacleSize
      return "Zoom = 80\n" +
             "DrawSensorLines\n" +
             "LogChanges\n" +
             "# LogClearText\n" +
             "# LogRoundedPositions\n" +
             "NumberOfSensors = 1\n" +
             "NumberOfObstacles = 1\n" +
             "FieldHeight = 9\n" +
             "FieldWidth = 6\n" +
             "# If SensorPositions is set NumberOfSensors is\n" +
             "# ignored\n" +
             "# SensorPositions = [[0, 0, 0.0], [9, 6, 128]]\n" +
             "# If ObstaclePositions is set NumberOfObstacles is\n" +
             "# ignored\n" +
             "# ObstaclePositions = [[2, 1]]\n" +
             "# ImpArea01 = [[0, 0], [2, 0], [2, 1], [0, 1]]\n" +
             "# ImpArea02 = [[0, 6], [2, 6], [2, 5], [0, 5]]\n" +
             "# ImpArea03 = [[9, 0], [7, 0], [7, 1], [9, 1]]\n" +
             "# ImpArea04 = [[9, 6], [9, 5], [7, 5], [7, 6]]\n" +
             "PlayerSensorRange = 12\n" +
             "PlayerSensorFOV = 56.3\n" +
             "PlayerSize = 0.1555\n" +
             "# The function used to optimize the problem,\n" +
             "# possible values are:\n" +
             "# PSO, SPSO-2006, SPSO-2007, SPSO-2011, ADE\n" +
             "Optimizer = SPSO-2006";
    }

    private AbsOptimization _optimizer;
    private SensorPositionObj _objective;
    private int _zoom;
    private bool _drawSensorLines;
    private List<Tuple<int, double>> _changesInOptimum;
    private bool _logChanges;
    private bool _logClearText;
    private bool _logRoundedPositions;
    private List<Sensor> _sensors;

    private static double[][] ParseDoubleMatrix(string source)
    {
      if (string.IsNullOrEmpty(source)) return null;
      
      source = source.Replace(" ", "");
      source = source.Substring(1, source.Length - 2);

      if (source == "") return new double[0][];
      
      var positions = Regex.Split(source, "],");
      var result = new double[positions.Length][];
      
      for (var i = 0; i < positions.Length; i++)
      {
        var pos = positions[i]
          .Replace("]", "")
          .Replace("[", "")
          .Split(',');
        result[i] = new double[pos.Length];
        
        for (var j = 0; j < pos.Length; j++)
        {
          if (float.TryParse(pos[j], out var number)) result[i][j] = number;
          else return null;
        }
      }
      return result;
    }
    
    private double[][] GetDoubleMatrix(
      IReadOnlyDictionary<string, string> model, string key)
    {
      return !model.ContainsKey(key) ? null : ParseDoubleMatrix(model[key]);
    }

    public static void TestParseDoubleMatrix()
    {
      const string text = "[[0.0, 0, 0], [0.0, 0, 0], [0.0, 0, 0, 0.0, 0, 0]]";
      var matrix = ParseDoubleMatrix(text);
      var matrixString = "";
      foreach (var row in matrix)
      {
        foreach(var cell in row)
        {
          matrixString += cell + ", ";
        }
        matrixString = matrixString.Substring(0, 
                         matrixString.Length - 2) + "\n";
      }
      Console.Write(matrixString);
    }
    
    public override void Init(Dictionary<string, string> model)
    {
      // -- Initialize objective
      
      _zoom = GetInt(model, "Zoom", 80);
      _changesInOptimum = new List<Tuple<int, double>>();
      _sensors = new List<Sensor>();
      _logChanges = model.ContainsKey("LogChanges");
      _logClearText = model.ContainsKey("LogClearText");
      _logRoundedPositions = model.ContainsKey("LogRoundedPositions");
      _drawSensorLines = model.ContainsKey("DrawSensorLines");
      
      var possibleOptimizers = new[]
      {
        "SPSO-2006", "SPSO-2007", "SPSO-2011", "PSO", "ADE"
      };
      var numberOfSensors = GetInt(model, "NumberOfSensors", 1);
      var numberOfObstacles = GetInt(model, "NumberOfObstacles", 1);
      
      _objective = new SensorPositionObj(
        (uint)numberOfSensors,
        (uint)numberOfObstacles, 
        GetDouble(model, "FieldHeight", 9),
        GetDouble(model, "FieldWidth", 6),
        GetDouble(model, "PlayerSensorRange", 12),
        GetDouble(model, "PlayerSensorFOV", 56.3),
        GetDouble(model, "PlayerSize", 0.1555)
        );
      
      foreach (var key in model.Keys)
      {
        if (!key.StartsWith("ImpArea")) continue;

        var matrix = ParseDoubleMatrix(model[key]);
        if (matrix == null || matrix.Length < 3) continue;

        var impArea = new Polygon();
        foreach (var row in matrix)
        {
          impArea.Add(new Vector2(row[0], row[1]));
        }
        _objective.ImportantAreas.Add(impArea);
      }
      
      var obstaclePositions = GetDoubleMatrix(model, "ObstaclePositions");

      if (obstaclePositions != null)
      {
        _objective.SetObstacles(obstaclePositions);
      }
      
      // -- Initialize optimizer

      var optStartPosition = GetDoubleMatrix(model, "SensorPositions");
      var optimizerName = GetAnyOf(model, "Optimizer", 
        possibleOptimizers.ToList(), "SPSO-2006");
      
      if (optimizerName == "SPSO-2006")
      {
        var swarm = Pso.SwarmSpso2006(_objective.SearchSpace(),
          x => _objective.F(x.ToList()));
        _optimizer = new SPSO2006(swarm, _objective);

        if (optStartPosition != null)
        {
          var initialPos = new double[optStartPosition.Length * 3];
          for (var i = 0; i < optStartPosition.Length; i++)
          {
            optStartPosition[i].CopyTo(initialPos, i * 3);
          }
          _optimizer.InitialPosition = initialPos;
        }
//        _optimizer = new SPSO2006(_objective)
//        {
//          Bounds = _objective.Intervals()
//        };
      }
      else if (optimizerName == "SPSO-2007")
        _optimizer = new SPSO2007(_objective)
        {
          Bounds = _objective.Intervals()
        };
      else if (optimizerName == "SPSO-2011")
        _optimizer =  new SPSO2011(_objective)
        {
          Bounds = _objective.Intervals()
        };
      else if (optimizerName == "PSO")
        _optimizer = new clsOptPSO(_objective)
        {
          InitialPosition = _objective.SearchSpace().RandPos()
        };
      else if (optimizerName == "ADE")
        _optimizer = new clsOptDEJADE(_objective)
        {
          LowerBounds = _objective.Intervals().Select(i => i[0]).ToArray(),
          UpperBounds = _objective.Intervals().Select(i => i[1]).ToArray()
        };

      _optimizer.Init();
      _sensors = _objective.ToSensors(_optimizer.Result.ToArray());
    }

    public override void Update(long deltaTime)
    {
      var lastBest = _optimizer.Result.Eval;
      _optimizer.Iterate(1);
      
      if (lastBest > _optimizer.Result.Eval) 
        _changesInOptimum.Add(new Tuple<int, double>(
          _optimizer.IterationCount, 
          lastBest - _optimizer.Result.Eval));
      
      _sensors = _objective.ToSensors(_optimizer.Result.ToArray());
    }

    private void DrawCoordinateSystem(Context cr)
    {
      var width = _objective.FieldWidth * _zoom;
      var height = _objective.FieldHeight * _zoom;
      
      cr.SetSourceRGB(.8, .8, .8);
      cr.Rectangle(0, 0, width, height);
      cr.ClosePath();
      cr.Fill();

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

    private void DrawImportantArea(Context cr)
    {
      // Pattern types: Linear, Radial, Solid, Surface
      cr.SetSourceRGBA(0, 0, 0, 0.3);
      foreach (var area in _objective.ImportantAreas)
      {
        DrawPolygon(cr, area);
        cr.LineWidth = 4;
        cr.Stroke();
      }
    }
    
    private void DrawSensors(Context cr)
    {
      var shadows = _objective.Shadows(_sensors);
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
      DrawImportantArea(cr);
      DrawSensors(cr);
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
      foreach (var (iteration, improvement) in _changesInOptimum)
      {
        changes += $"[{iteration}, {improvement}]";
        if (i++ < _changesInOptimum.Count) changes += ", ";
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
        "<global-best>" + _optimizer.Result.Eval + "</global-best>";
    }
  }
}