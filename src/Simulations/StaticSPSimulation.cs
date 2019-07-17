using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using charlie;
using Cairo;
using Geometry;
using LibOptimization.Optimization;
using Optimization;

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
      return "Author: Jakob Rieke; Version v1.4.0; Deterministic: No"; 
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
      return "Zoom = 80\n" +
             "# DrawSensorLines\n" +
             "NumberOfSensors = 1\n" +
             "NumberOfObstacles = 1\n" +
             "FieldHeight = 9\n" +
             "FieldWidth = 6\n" +
             "# If SensorPositions is set NumberOfSensors is\n" +
             "# ignored\n" +
             "# SensorPositions = [[0, 0, 0.0], [9, 6, 128]]\n" +
             "# If ObstaclePositions is set NumberOfObstacles is\n" +
             "# ignored\n" +
             "# ObstaclePositions = []\n" +
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

    private absOptimization _optimizer;
    private SspObjectiveFct _objective;
    private int _zoom;
    private bool _drawSensorLines;

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
    
    private double[][] GetDoubleMatrix(Dictionary<string, string> model, 
      string key)
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
      _drawSensorLines = model.ContainsKey("DrawSensorLines");
      
      var possibleOptimizers = new[]
      {
        "SPSO-2006", "SPSO-2007", "SPSO-2011", "PSO", "ADE"
      };
      var numberOfSensors = GetInt(model, "NumberOfSensors", 1);
      var numberOfObstacles = GetInt(model, "NumberOfObstacles", 1);
      var sensorPositions = GetDoubleMatrix(model, "SensorPositions");
      var obstaclePositions = GetDoubleMatrix(model, "ObstaclePositions");
      var optimizerName = GetAnyOf(model, "Optimizer", 
        possibleOptimizers.ToList(), "SPSO-2006");

      var ssp = new StaticSensorPositioning
      {
        FieldWidth = GetDouble(model, "FieldHeight", 9),
        FieldHeight = GetDouble(model, "FieldWidth", 6),
        SensorRange = GetDouble(model, "PlayerSensorRange", 12),
        SensorFov = GetDouble(model, "PlayerSensorFOV", 56.3),
        ObstacleSize = GetDouble(model, "PlayerSize", 0.1555)
      };
      
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
        ssp.ImportantAreas.Add(impArea);
      }

      ssp.Init(numberOfSensors, numberOfObstacles);

      if (obstaclePositions != null)
      {
        Console.WriteLine("Setting up obstacles from fixed values");
        ssp.SetObstacles(obstaclePositions);
      }
      _objective = new SspObjectiveFct(ssp);
      
      // -- Initialize optimizer

      if (optimizerName == "SPSO-2006")
      {
        var swarm = Pso.SwarmSpso2006(ssp.SearchSpace(), ssp.FitnessFct);
        _optimizer = new SPSO2006(swarm, _objective);

        if (sensorPositions != null)
        {
          var initialPos = new double[sensorPositions.Length * 3];
          for (var i = 0; i < sensorPositions.Length; i++)
          {
            sensorPositions[i].CopyTo(initialPos, i * 3);
          }
          _optimizer.InitialPosition = initialPos;
        }
//        _optimizer = new SPSO2006(_objective)
//        {
//          Bounds = _objective.Raw.Intervals()
//        };
      }
      else if (optimizerName == "SPSO-2007")
        _optimizer = new SPSO2007(_objective)
        {
          Bounds = _objective.Raw.Intervals()
        };
      else if (optimizerName == "SPSO-2011")
        _optimizer =  new SPSO2011(_objective)
        {
          Bounds = _objective.Raw.Intervals()
        };
      else if (optimizerName == "PSO")
        _optimizer = new clsOptPSO(_objective)
        {
          InitialPosition = _objective.Raw.SearchSpace().RandPos()
        };
      else if (optimizerName == "ADE")
        _optimizer = new clsOptDEJADE(_objective)
        {
          LowerBounds = _objective.Raw.Intervals().Select(i => i[0]).ToArray(),
          UpperBounds = _objective.Raw.Intervals().Select(i => i[1]).ToArray()
        };

      _optimizer.Init();
      StaticSensorPositioning.PlaceFromVector(_optimizer.Result.ToArray(), _objective.Raw.Sensors);
    }

    public override void Update(long deltaTime)
    {
      _optimizer.DoIteration(1);
      StaticSensorPositioning.PlaceFromVector(_optimizer.Result.ToArray(), _objective.Raw.Sensors);
    }

    private void DrawCoordinateSystem(Context cr)
    {
      var width = _objective.Raw.Env.Bounds.Max.X * _zoom;
      var height = _objective.Raw.Env.Bounds.Max.Y * _zoom;
      
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
      foreach (var area in _objective.Raw.ImportantAreas)
      {
        DrawPolygon(cr, area);
        cr.LineWidth = 4;
        cr.Stroke();
      }
    }
    
    private void DrawSensors(Context cr)
    {
      var shadows = Sensor.Shadows(_objective.Raw.Sensors, _objective.Raw.Env);
      cr.SetSourceRGBA(0, 0, 0, 0.7);
      
      foreach (var polygon in shadows)
      {
        if (polygon.Count == 0) continue;
        DrawPolygon(cr, polygon);
        cr.Fill();
      }
      
      foreach (var sensor in _objective.Raw.Sensors)
      {
        if (_drawSensorLines)
        {
          cr.SetSourceRGB(1, 0, 0);
          cr.SetDash(new double[]{10}, 0);
          cr.LineWidth = .4;
          DrawPolygon(cr, sensor.AreaOfActivity().ToPolygon());
          cr.Stroke();
        }
        
//        cr.SetSourceRGB(0.495, 0.118, 0.248);
//        cr.LineWidth = 4;
//        var rot = (sensor.Rotation - 28) * Math.PI / 180;
//        cr.Arc(sensor.Position.X * _zoom,
//          sensor.Position.Y * _zoom, 
//          sensor.Size * _zoom,
//          rot,
//          rot + sensor.Fov * Math.PI / 180);
//        cr.Stroke();
        
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
      foreach (var obstacle in _objective.Raw.Obstacles)
      {
        cr.SetSourceRGB(0.159, 0.695, 0.775);
        cr.Arc(
          obstacle.Position.X * _zoom,
          obstacle.Position.Y * _zoom,
          obstacle.Size * _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
      }

      cr.Fill();
    }

    public override void Render(Context cr, int width, int height)
    {
      cr.Translate(
        (width - _objective.Raw.FieldWidth * _zoom) / 2, 
        (width - _objective.Raw.FieldHeight * _zoom) / 2);
      
      DrawCoordinateSystem(cr);
      DrawImportantArea(cr);
      DrawSensors(cr);
      DrawObstacles(cr);
    }

    public override string Log()
    {
      var sensorPositions = "[";
      var i = 1;
      foreach (var sensor in _objective.Raw.Sensors)
      {
        sensorPositions += $"[{sensor.Position.X}," +
                           $"{sensor.Position.Y},{sensor.Rotation}]";

        if (i++ < _objective.Raw.Sensors.Count) sensorPositions += ", ";
      }
      sensorPositions += "]";
      
      var obstaclePositions = "[";
      i = 1;
      foreach (var obstacle in _objective.Raw.Obstacles)
      {
        obstaclePositions += $"[{obstacle.Position.X}, {obstacle.Position.Y}]";

        if (i++ < _objective.Raw.Sensors.Count) obstaclePositions += ", ";
      }
      obstaclePositions += "]";

      return 
        $"<sensors>{sensorPositions}</sensors>\n" +
        $"<obstacles>{obstaclePositions}</obstacles>\n" +
        "<global-best>" + _optimizer.Result.Eval + "</global-best>";
    }
  }
}