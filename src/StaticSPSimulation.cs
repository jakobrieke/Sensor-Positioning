using System;
using System.Collections.Generic;
using System.Linq;
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
       * Fix 'sometimes worse best' in PSO implementation
       * Test standard PSO 2006 without neighbourhood -> did not work
       * Fix standard PSO 2007 & 2006 implementation
       * Render field centered to render area
       * Test behaviour if fitness does not return +infinity when two sensors
       *   overlap -> optimization with LibOptimization.PSO is better
       * Test behaviour if punishment factor is added to fitness
       *   -> sensors are getting drawn into the center
       */
      return "Author: Jakob Rieke; Version v1.3.3; Deterministic: No"; 
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
             "SizeTeamA = 1\n" +
             "SizeTeamB = 1\n" +
             "FieldHeight = 9\n" +
             "FieldWidth = 6\n" +
             "PlayerSensorRange = 12\n" +
             "PlayerSensorFOV = 56.3\n" +
             "PlayerSize = 0.1555\n" +
             "# The function used to optimize the problem,\n" +
             "# possible values are:\n" +
             "# Simplex, PSO, SPSO-2006, SPSO-2007, SPSO-2011,\n" +
             "# PSO-A-IW, PSO-LD-IW, PSO-Chaotic-IW, PSO-LD-IW,\n" +
             "# Evolution-Strategy, Differential-Evolution,\n" +
             "# Adaptive-Differential-Evolution\n" +
             "Optimizer = Adaptive-Differential-Evolution";
    }

    private Dictionary<string, Func<SspFct, absOptimization>> _optimizers =
      new Dictionary<string, Func<SspFct, absOptimization>>
      {
//        {"Cuckoo-Search", obj => new clsOptCS(obj)},
        {"Hill-Climbing", obj => new clsOptHillClimbing(obj)},
        {"Simulated-Annealing", obj => new clsOptSimulatedAnnealing(obj)},
        {"Pattern-Search", obj => new clsOptPatternSearch(obj)},
        {"Simplex", obj => new clsOptNelderMead(obj)},
        {"Simplex-Wikipedia", obj => new clsOptNelderMeadWiki(obj)},
        {"SPSO-2006", obj => new SPSO2006(obj)
        {
          Bounds = obj.Raw.Intervals()
        }},
        {"SPSO-2007", obj => new SPSO2007(obj)
        {
          Bounds = obj.Raw.Intervals()
        }},
        {
          "SPSO-2011", obj => new SPSO2011(obj)
          {
            Bounds = obj.Raw.Intervals()
          }
        },
        {"PSO", obj => new clsOptPSO(obj)
          {
            InitialPosition = obj.Raw.SearchSpace().RandPos()
          }
        },
        {"PSO-A-IW", obj => new clsOptPSOAIW(obj)},
        {"PSO-Chaotic-IW", obj => new clsOptPSOChaoticIW(obj)},
        {"PSO-LD-IW", obj => new clsOptPSOLDIW(obj)},
        {"Evolution-Strategy", obj => new clsOptES(obj)}, // no criterion
        {"Differential-Evolution", obj => new clsOptDE(obj)},
        {"Adaptive-Differential-Evolution", obj => new clsOptDEJADE(obj)
          {
            LowerBounds = obj.Raw.Intervals().Select(i => i[0]).ToArray(),
            UpperBounds = obj.Raw.Intervals().Select(i => i[1]).ToArray()
          }
        },
        {"Real-Coded-GA-BLX-Alpha+JGG", obj => new clsOptRealGABLX(obj)},
        {"Real-Coded-GA-PCX", obj => new clsOptRealGAPCX(obj)},
        {"Real-Coded-GA-REX", obj => new clsOptRealGAREX(obj)},
        {"Real-Coded-GA-SPX+JGG", obj => new clsOptRealGASPX(obj)
          {
            LowerBounds = obj.Raw.Intervals().Select(i => i[0]).ToArray(),
            UpperBounds = obj.Raw.Intervals().Select(i => i[1]).ToArray()
          }
        },
        {"Real-Coded-GA-UNDX", obj => new clsOptRealGAUNDX(obj)},
      };
    
    private absOptimization _optimizer;
    private SspFct _objective;
    private int _zoom;

    public override void Init(Dictionary<string, string> model)
    {
      _zoom = GetInt(model, "Zoom", 80);
      var sizeTeamA = GetInt(model, "SizeTeamA", 1);
      var sizeTeamB = GetInt(model, "SizeTeamB", 1);
      var fieldWidth = GetDouble(model, "FieldHeight", 9);
      var fieldHeight = GetDouble(model, "FieldWidth", 6);
      var playerSensorRange = GetDouble(model, "PlayerSensorRange", 12);
      var playerSensorFov = GetDouble(model, "PlayerSensorFOV", 56.3);
      var playerSize = GetDouble(model, "PlayerSize", 0.1555);

      var optimizerName = GetAnyOf(model, "Optimizer", 
        _optimizers.Keys.ToList(), "Adaptive-Differential-Evolution");
      
      _objective = new SspFct(sizeTeamA, sizeTeamB, fieldWidth, fieldHeight,
        playerSensorRange, playerSensorFov, playerSize);
      _optimizer = _optimizers[optimizerName](_objective);
      _optimizer.Init();
      SensorPositioningProblem.PlaceFromVector(
        _optimizer.Result.ToArray(), _objective.Raw.TeamA);
    }

    public override void Update(long deltaTime)
    {
      _optimizer.DoIteration(1);
      SensorPositioningProblem.PlaceFromVector(
        _optimizer.Result.ToArray(), _objective.Raw.TeamA);
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
    
    private void DrawSensors(Context cr)
    {
      cr.SetSourceRGBA(0, 0, 0, 0.7);
      foreach (var polygon in Sensor.Shadows(
        _objective.Raw.TeamA, _objective.Raw.Env))
      {
        if (polygon.Count == 0) continue;
        DrawPolygon(cr, polygon);
        cr.Fill();
      }
      
      foreach (var sensor in _objective.Raw.TeamA)
      {
        cr.SetSourceRGB(0, 0, 0);
        cr.SetDash(new double[]{10}, 0);
        cr.LineWidth = .2;
        DrawPolygon(cr, sensor.AreaOfActivity().ToPolygon());
        cr.Stroke();
        
        cr.SetSourceRGB(1, 0, 0);
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
      foreach (var obstacle in _objective.Raw.TeamB)
      {
        cr.SetSourceRGB(0.1, 0.1, 1);
        cr.Arc(
          obstacle.Position.X * _zoom,
          obstacle.Position.Y * _zoom,
          obstacle.Size * _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
      }

      cr.Fill();
    }

    public override void Render(Context ctx, int width, int height)
    {
      var fieldWidth = _objective.Raw.Env.Bounds.Max.X * _zoom;
      var fieldHeight = _objective.Raw.Env.Bounds.Max.Y * _zoom;
//      ctx.Scale(2, 2);
      ctx.Translate((width - fieldWidth) / 2, (width - fieldHeight) / 2);
      
      DrawCoordinateSystem(ctx);
      DrawSensors(ctx);
      DrawObstacles(ctx);
    }

    public override string Log()
    {
      var sensorPositions = "[";
      var i = 1;
      foreach (var sensor in _objective.Raw.TeamA)
      {
        sensorPositions += $"[{sensor.Position.X}," +
                           $"{sensor.Position.Y},{sensor.Rotation}]";

        if (i++ < _objective.Raw.TeamA.Count) sensorPositions += ", ";
      }
      sensorPositions += "]";
      
      var obstaclePositions = "[";
      i = 1;
      foreach (var obstacle in _objective.Raw.TeamB)
      {
        obstaclePositions += $"[{obstacle.Position.X}, {obstacle.Position.Y}]";

        if (i++ < _objective.Raw.TeamA.Count) obstaclePositions += ", ";
      }
      obstaclePositions += "]";

      return 
        $"<sensors>{sensorPositions}</sensors>\n" +
        $"<obstacles>{obstaclePositions}</obstacles>\n" +
        "<global-best>" + _optimizer.Result.Eval + "</global-best>";
    }
  }
}