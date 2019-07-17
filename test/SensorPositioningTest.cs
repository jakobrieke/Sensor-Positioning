using System;
using System.Diagnostics;
using System.Linq;
using Geometry;
using Optimization;
using Action = System.Action;
using Key = Gdk.Key;

namespace sensor_positioning
{
  public static class SensorPositioningTest
  {
    public static void TestAll()
    {
      TestSSP();
      TestSSPStepByStep();
    }


    public static void TestSSPStepByStep()
    {
      StaticSensorPositioning prob = null;
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
        StaticSensorPositioning.PlaceFromVector(pso.GlobalBest, prob.Sensors);
        plotter.Plot(prob.Env.Bounds);
        plotter.Plot(Sensor.Shadows(prob.Sensors, prob.Env));
        plotter.Plot(prob.Sensors.Select(o =>
          (IGeometryObject) new Circle(o.Position, o.Size)));
        plotter.Plot(prob.Obstacles.Select(o =>
          (IGeometryObject) new Circle(o.Position, o.Size)));
        pso.Iterate();
      });

      var reset = new Action(() =>
      {
        prob = new StaticSensorPositioning();
        prob.Init(10, 10);
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
      
      var prob = new StaticSensorPositioning();
      prob.Init(sizeTeamA, sizeTeamB);
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
      Console.WriteLine("Sensor Range: " + prob.Sensors[0].Range);
      Console.WriteLine("Sensor FOV: " + prob.Sensors[0].Fov);
      Console.WriteLine("Obstacle Size: " + prob.Sensors[0].Size);
      Console.WriteLine("Sensors: " + prob.Sensors.Count);
      Console.WriteLine("Obstacles: " + prob.Obstacles.Count);
      foreach (var ob in prob.Obstacles)
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
        sw.Iterate(iterations);
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
//      Console.WriteLine("Collisions: " + prob.Collisions);
      Console.WriteLine("Total Distance: " + totalDistance);
      Console.WriteLine(
        "Shadow area: " + prob.Normalize(sw.GlobalBestValue) + "%");
    }
  }
}