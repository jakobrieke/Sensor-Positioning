using System;
using System.Collections.Generic;
using charlie;
using Cairo;
using LinearAlgebra;
using SensorPositioning;

namespace Optimization
{
  public class PsoSimulation : AbstractSimulation
  {
    private ParticleSwarm _swarm;
    private int _xOffset;
    private int _yOffset;
    private double _size;
    private double _gridSize;
    private Color[][] _grid;
    private int _rasterSize;
    private double _zoom;
    
    public override string GetTitle()
    {
      return "Particle Swarm Optimization";
    }

    public override string GetDescr()
    {
      return
        "Shows a particle swarm searching for the minimum of a two " +
        "dimensional function. Points with lower (better) values are painted " +
        "darker.";
    }

    public override string GetConfig()
    {
      return
        "# The level of detail the function is rasterised with\n" + 
        "Resolution = 100\n" +
        "\n" +
        "# The examined optimization function:\n" +
        "# - Sphere\n" +
        "# - F2\n" +
        "# - F3\n" +
        "# - Rosenbrock\n" +
        "# - F6\n" +
        "# - F7\n" +
        "Function = Sphere\n" +
        "\n" +
        "SwarmSize = 40\n" +
        "SearchSpaceSize = 400" +
        "\n" +
        "XOffset = 0\n" +
        "YOffset = 0\n" +
        "Zoom = 2";
    }

    private void RasterFunction()
    {
      var values = new double[_rasterSize][];
      var min = double.PositiveInfinity;
      var max = double.NegativeInfinity;
      var distanceToCenter = 1.0 * _gridSize / 2;

      for (var i = 0; i < values.Length; i++)
      {
        values[i] = new double[_rasterSize];
        for (var j = 0; j < values[i].Length; j++)
        {
          var value = _swarm.Fitness.Eval(new[]
          {
            _gridSize * i + distanceToCenter - _size / 2,
            _gridSize * j + distanceToCenter - _size / 2
          });
          values[i][j] = value;
          if (value < min) min = value;
          if (value > max) max = value;
        }
      }
      
      _grid = new Color[_rasterSize][];
      for (var i = 0; i < _grid.Length; i++)
      {
        _grid[i] = new Color[_rasterSize];
        for (var j = 0; j < _grid[i].Length; j++)
        {
          var value = (values[i][j] - min) / (max - min);
          _grid[i][j] = new Color(value / 1.5, value / 3, value / 3);
        }
      }
    }

    public static Objective GetAnyOf(
      Dictionary<string, string> model, string key, 
      Dictionary<string, Objective> possibleValues, 
      string backup)
    {
      if (!model.TryGetValue(key, out var value)) return possibleValues[backup];
      return possibleValues.ContainsKey(value) ? 
        possibleValues[value] : possibleValues[backup];
    }
    
    public override void Init(Dictionary<string, string> config)
    {
      _xOffset = GetInt(config, "XOffset", 200);
      _yOffset = GetInt(config, "YOffset", 200);
      _size = GetInt(config, "SearchSpaceSize", 400);
      _rasterSize = GetInt(config, "Resolution", 100);
      _gridSize = _size / _rasterSize;
      _zoom = GetDouble(config, "Zoom", 0); 
      
      var possibleValues = new Dictionary<string,
        Objective>
      {
        {"Sphere", new SphereFunction()},
        {"F2", new F2()},
        {"F3", new F3()},
        {"Rosenbrock", new F5()},
        {"F6", new F6()},
        {"F7", new F7()},
      };
      var fct = GetAnyOf(config, "Function", possibleValues, "Sphere");
      var swarmSize = GetInt(config, "SwarmSize", 40);
      
      var sp = new SearchSpace(2, _size / 2);
      _swarm = new StandardPso2006(sp, fct);
      _swarm.Init(swarmSize);

      RasterFunction();
    }

    public override void Update(long deltaTime)
    {
      _swarm.Iterate();
    }

    public override void Render(Context cr, int width, int height)
    {
      cr.Save();
      cr.Scale(_zoom, _zoom);
      cr.Translate(200, 200);
      
      // -- Render background
      
      for (var i = 0; i < _grid.Length; i++)
      {
        for (var j = 0; j < _grid[i].Length; j++)
        {
          cr.SetSourceColor(_grid[i][j]);
          cr.Rectangle(
            _xOffset - _size / 2 + i * _gridSize, 
            _yOffset - _size / 2 + j * _gridSize, 
            _gridSize, _gridSize);
          cr.Fill();
        }
      }
      
      // -- Render particles
      
      foreach (var p in _swarm.Particles)
      {
        cr.SetSourceColor(new Color(0.769, 0.282, 0.295));
        cr.LineWidth = 1;
        cr.Arc(
          _xOffset + p.Position[0], 
          _yOffset + p.Position[1], 
          3 / _zoom, 0, 2 * Math.PI);
        cr.ClosePath();
        cr.Fill();
      }
      
      cr.Restore();
      RenderGui(cr);
    }

    public void RenderGui(Context cr)
    {
      cr.Scale(1.7, 1.7);
      cr.SetSourceRGB(.7, .7, .7);
      cr.SetFontSize(13);
      
      var globalBestText = "Best: (";
      
      for (var i = 0; i < _swarm.GlobalBest.Length; i++)
      {
        globalBestText += Math.Round(_swarm.GlobalBest[i], 3);
        if (i < _swarm.GlobalBest.Length - 1) globalBestText += " ";
      }

      globalBestText += ")  " + _swarm.GlobalBestValue;
      
      cr.MoveTo(12, 20);
      cr.ShowText(globalBestText);
    }
  }
}