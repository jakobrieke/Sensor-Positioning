using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cairo;
using Gdk;
using Geometry;
using GLib;
using LibOptimization.Optimization;
using LibOptimization.Util;
using Optimization;
using Gtk;
using Action = System.Action;
using Application = Gtk.Application;
using Key = Gdk.Key;
using Rectangle = Geometry.Rectangle;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;


namespace sensor_positioning
{
  public static class SensorPositioningTest
  {
    public static void TestAll()
    {
      for (var i = 0; i < 1000; i++) TestWithExternalPso();
      for (var i = 0; i < 500; i++) TestSSP();
      TestSSPStepByStep();
      TestWithSimplex();
      TestWithAdaptiveDifferentialEvolution();
    }

    public static void TestWithSimplex()
    {
      var objective = new SspFct(2, 5);
      var opt = new clsOptNelderMead(objective);
      opt.InitialPosition = objective.Raw.SearchSpace().RandPos();
      opt.IsUseCriterion = false;
      opt.Init();
      clsUtil.DebugValue(opt);
      while (opt.DoIteration(10) == false)
      {
        if (opt.Result.Eval < 5) break;
        clsUtil.DebugValue(opt, ai_isOutValue: false);
      }

      clsUtil.DebugValue(opt);
    }

    public static void TestWithAdaptiveDifferentialEvolution()
    {
      var objective = new SspFct(2, 5);
      var opt = new clsOptDEJADE(objective)
      {
        LowerBounds = objective.Raw.Intervals().Select(i => i[0]).ToArray(),
        UpperBounds = objective.Raw.Intervals().Select(i => i[1]).ToArray()
      };
      opt.Init();

      // Print debug
      Console.WriteLine("Sensors: " + objective.Raw.TeamA.Count);
      Console.WriteLine("Obstacles:");
      foreach (var sensor in objective.Raw.TeamB)
      {
        Console.WriteLine("- " + sensor.Position + " : " + sensor.Size);
      }

      clsUtil.DebugValue(opt);

      // Optimize
      while (opt.DoIteration(10) == false)
      {
        if (opt.Result.Eval < 5) break;
        clsUtil.DebugValue(opt, ai_isOutValue: false);
      }

      Console.WriteLine("Best: " + objective.Raw.Normalize(opt.Result.Eval) +
                        "%");
      clsUtil.DebugValue(opt);
    }

    public static void TestWithExternalPso()
    {
      var objective = new SspFct();
      var opt = new clsOptPSO(objective) {IsUseCriterion = false};
      opt.Init();

      Console.WriteLine("--- Fitness function setup");
      Console.WriteLine("Sensors: " + objective.Raw.TeamA.Count);
      Console.WriteLine("Obstacles: " + objective.Raw.TeamB.Count);
      foreach (var ob in objective.Raw.TeamB)
      {
        Console.Write("- " + ob.Position + " : " + ob.Size);
      }

      Console.WriteLine();
      Console.WriteLine("Field: " + objective.Raw.Env.Bounds);

      Console.WriteLine("\n--- Setup");
      clsUtil.DebugValue(opt);

      opt.DoIteration(500);
      Console.WriteLine("--- Results");
      Console.WriteLine("Best: " + objective.Raw.Normalize(opt.Result.Eval) +
                        "% shadow");
      clsUtil.DebugValue(opt);
    }

    public static void TestSSPStepByStep()
    {
      SSP prob = null;
      Swarm pso = null;

      var plotter = new TestPlotter
      {
        XScale = 20, YScale = 20,
        XOffset = -100, YOffset = 60
      };

      var plot = new Action(() =>
      {
        Console.WriteLine("Iteration: " + pso.Iteration +
                          ", Best: " + prob.Normalize(pso.GlobalBestValue) +
                          "% shadow");
        plotter.Clear();
        SensorPositioningProblem.PlaceFromVector(pso.GlobalBest, prob.TeamA);
        plotter.Plot(prob.Env.Bounds);
        plotter.Plot(Sensor.Shadows(prob.TeamA, prob.Env));
        plotter.Plot(prob.TeamA.Select(o =>
          (IGeometryObject) new Circle(o.Position, o.Size)));
        plotter.Plot(prob.TeamB.Select(o =>
          (IGeometryObject) new Circle(o.Position, o.Size)));
        pso.IterateOnce();
      });

      var reset = new Action(() =>
      {
        prob = new SSP(10, 10);
        pso = Pso.SwarmSpso2011(
          new SearchSpace(prob.Intervals()),
          prob.FitnessFct);

        Console.WriteLine("Reset");
        pso.Initialize();
        plot();
      });

      plotter.Window.KeyPressEvent += (o, args) =>
      {
        if (args.Event.Key == Key.n) plot();
        else if (args.Event.Key == Key.r) reset();
      };

      reset();
      plotter.Plot();
    }

    public static void TestSSP()
    {
      const int sizeTeamA = 6;
      const int sizeTeamB = 5;
      const int iterations = 10;
      const int swarmSize = 40;
      
      var prob = new SSP(sizeTeamA, sizeTeamB);
      var sw = Pso.SwarmSpso2011(prob.SearchSpace(), prob.FitnessFct);
      sw.Topology = Pso.RingTopology;
      sw.ShouldTopoUpdate = swarm => false;

      Console.WriteLine("--- Swarm");
      Console.WriteLine("Topology: " + sw.Topology.Method.Name);
      Console.WriteLine("Confinement: " + sw.Confinement.Method.Name);
      Console.WriteLine("Update Method: " + sw.Update.Method.Name);
      Console.WriteLine("Size: " + swarmSize);
      Console.WriteLine("W: " + Pso.W);
      Console.WriteLine("C1: " + Pso.C1);
      Console.WriteLine("C2: " + Pso.C2);
      
      Console.WriteLine("\n--- Static Sensor Problem");
      Console.WriteLine("Field: " + prob.Env.Bounds);
      Console.WriteLine("Sensor Range: " + prob.TeamA[0].Range);
      Console.WriteLine("Sensor FOV: " + prob.TeamA[0].Fov);
      Console.WriteLine("Obstacle Size: " + prob.TeamA[0].Size);
      Console.WriteLine("Sensors: " + prob.TeamA.Count);
      Console.WriteLine("Obstacles: " + prob.TeamB.Count);
      foreach (var ob in prob.TeamB)
      {
        Console.WriteLine("- " + ob.Position + ", " + ob.Size);
      }

      Console.WriteLine(
        "\n--- Iteration, Shadow area in %, Elapsed time in ms, Distance");
      var clock = new Stopwatch();
      
      clock.Start();
      sw.Initialize(swarmSize);
      clock.Stop();
      
      Console.WriteLine("- {0}, {1}, {2}, {3}",
        0, prob.Normalize(sw.GlobalBestValue), clock.ElapsedMilliseconds, 0);

      var last = new double[sw.SearchSpace.Dimensions];
      sw.GlobalBest.CopyTo(last, 0);
      var totalDistance = 0.0;
      double distance;
      
      for (var i = 0; i < 10; i++)
      {
        clock.Reset();
        clock.Start();
        sw.IterateMaxIterations(iterations);
        clock.Stop();
        distance = Vector.Distance(last, sw.GlobalBest);
        totalDistance += Vector.Distance(last, sw.GlobalBest);
        
        Console.WriteLine("- {0}, {1}, {2}, {3}",
          sw.Iteration, prob.Normalize(sw.GlobalBestValue), 
          clock.ElapsedMilliseconds, distance);
        
        sw.GlobalBest.CopyTo(last, 0);
      }

      Console.WriteLine("\n--- Results");
      Console.Write(string.Join(",\n", sw.GlobalBest));
      Console.WriteLine();
      Console.WriteLine("Collisions: " + prob.Collisions);
      Console.WriteLine("Total Distance: " + totalDistance);
      Console.WriteLine(
        "Shadow area: " + prob.Normalize(sw.GlobalBestValue) + "%");
    }
  }
}