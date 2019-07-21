using System;
using System.Diagnostics;
using System.Linq;
using Geometry;
using LibOptimization.Optimization;
using NUnit.Framework;
using Action = System.Action;
using Key = Gdk.Key;

namespace sensor_positioning
{
  public static class SensorPositioningTest
  {
    [Test]
    public static void TestSSP()
    {
      const int numberOfSensors = 6;
      const int numberOfObstacles = 5;
      const int iterations = 10;
      const int swarmSize = 40;
      
      var obj = new SensorPositionObj(numberOfSensors, numberOfObstacles, 
      9, 6, 12, 56, 0.1555);
      var sw = Pso.SwarmSpso2011(obj.SearchSpace(), 
        x => obj.F(x.ToList()));
      sw.Topology = Topology.RingTopology;
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
      Console.WriteLine("Field: " + obj.Field);
      Console.WriteLine("Sensor Range: " + obj.SensorRange);
      Console.WriteLine("Sensor FOV: " + obj.SensorFov);
      Console.WriteLine("Object Size: " + obj.ObjectSize);
      Console.WriteLine("Sensors: " + obj.NumberOfVariable() / 3);
      Console.WriteLine("Obstacles: " + obj.Obstacles.Count);
      foreach (var ob in obj.Obstacles)
      {
        Console.WriteLine("- " + ob.Position + ", " + ob.Radius);
      }

      Console.WriteLine(
        "\n--- Iteration, Shadow area in %, Elapsed time in ms, Distance");
      var clock = new Stopwatch();
      
      clock.Start();
      sw.Initialize(swarmSize);
      clock.Stop();
      
      Console.WriteLine("- {0}, {1}, {2}, {3}",
        0, obj.Normalize(sw.GlobalBestValue), clock.ElapsedMilliseconds, 0);

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
          sw.Iteration, obj.Normalize(sw.GlobalBestValue), 
          clock.ElapsedMilliseconds, distance);
        
        sw.GlobalBest.CopyTo(last, 0);
      }

      Console.WriteLine("\n--- Results");
      Console.Write(string.Join(",\n", sw.GlobalBest));
      Console.WriteLine();
//      Console.WriteLine("Collisions: " + prob.Collisions);
      Console.WriteLine("Total Distance: " + totalDistance);
      Console.WriteLine(
        "Shadow area: " + obj.Normalize(sw.GlobalBestValue) + "%");
    }
  }
}