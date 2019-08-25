using System;
using System.Collections.Generic;
using charlie;
using Cairo;

namespace Optimization
{ 
  public class PsoSimulation : AbstractSimulation
  {
    private ParticleSwarm _swarm;
    private int _xOffset;
    private int _yOffset;
    private double _size;
    private Color[][] _bg;
    private int _rasterSize;
    private double _gridSize;
    
    public override string GetTitle()
    {
      return "SWAAARM";
    }

    public override string GetDescr()
    {
      return "Present yourself with a particle swarm and never forget to be " +
             "amazing.";
    }

    public override string GetConfig()
    {
      return "XOffset = 200\n" +
             "YOffset = 200\n" +
             "# The level of detail the function is rasterized at\n" +
             "RasterSize = 100\n" +
             "# The examined optimization function:\n" +
             "# - SphereFct      (0)\n" +
             "# - McCormick      (-1.9133)\n" +
             "# - HimmelbauFct   (0)\n" +
             "# f(3, 2), f(-2.8, 3.1), f(-3.7, -3.2), f(3.5, -1.8) =~ 0\n" +
             "# - ThreeHumpCamel (0)\n" +
             "# - HoelderTable   (-19.2085)\n" +
             "Function = HimmelblauFct\n" +
             "SwarmSize = 40\n" +
             "SearchSpaceSize = 400";
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
      
      _bg = new Color[_rasterSize][];
      for (var i = 0; i < _bg.Length; i++)
      {
        _bg[i] = new Color[_rasterSize];
        for (var j = 0; j < _bg[i].Length; j++)
        {
          var value = (values[i][j] - min) / (max - min);
          _bg[i][j] = new Color(value / 1.5, value / 3, value / 3);
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
      _rasterSize = GetInt(config, "RasterSize", 100);
      _gridSize = _size / _rasterSize;
      
      var possibleValues = new Dictionary<string,
        Objective>
      {
        {"SphereFct", new SphereFunction()},
        {"F2", new F2()},
        {"F3", new F3()},
        {"Rosenbrock", new F5()},
        {"F6", new F6()},
        {"F7", new F7()},
      };
      var fct = GetAnyOf(config, "Function", possibleValues, "SphereFct");
      var swarmSize = GetInt(config, "SwarmSize", 40);
      
      var sp = new SearchSpace(2, _size / 2);
      _swarm = new StandardPso2006(sp, fct);
      _swarm.Init(swarmSize);

      RasterFunction();
    }

    public override void Update(long deltaTime)
    {
      _swarm.Update();
    }

    public override void Render(Context cr, int width, int height)
    {
      // Render background
      for (var i = 0; i < _bg.Length; i++)
      {
        for (var j = 0; j < _bg[i].Length; j++)
        {
          cr.SetSourceColor(_bg[i][j]);
          cr.Rectangle(
            _xOffset - _size / 2 + i * _gridSize, 
            _yOffset - _size / 2 + j * _gridSize, 
            _gridSize, _gridSize);
          cr.Fill();
        }
      }
      
      // Render particles
      foreach (var p in _swarm.Particles)
      {
        cr.SetSourceColor(new Color(0.769, 0.282, 0.295));
        cr.LineWidth = 1;
        cr.Arc(
          _xOffset + p.Position[0], 
          _yOffset + p.Position[1], 
          3, 0, 2 * Math.PI);
        cr.ClosePath();
        cr.Fill();
      }
      
      // Render global best status message
      var globalBest = "Best: (";
      for (var i = 0; i < _swarm.GlobalBest.Length; i++)
      {
        globalBest += Math.Round(_swarm.GlobalBest[i], 3);
        if (i < _swarm.GlobalBest.Length - 1) globalBest += " ";
      }

      globalBest += ")  " + Math.Round(_swarm.GlobalBestValue, 3);
      
      cr.SetSourceColor(new Color(.7, .7, .7));
      cr.SetFontSize(13);
      cr.MoveTo(12, 20);
      cr.ShowText(globalBest);
    }
  }
}