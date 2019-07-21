// Author: Jakob Rieke

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using sensor_positioning;
using Opt = LibOptimization.Optimization.OptimizationFct;

namespace LibOptimization.Optimization
{
  public static class PsoTest
  {
    /// <summary>
    /// Test if ArgMin really does return the particle with the best
    /// position.
    /// </summary>
    [Test]
    public static void TestArgMin()
    {
      var updateValue = new Action<Particle>(p =>
        p.PositionValue = OptimizationFct.SphereFct(p.Position));

      var p1 = new Particle {Position = new[] {1.5, 1.5, 1.5}};
      var p2 = new Particle {Position = new[] {0.0, 0, 0}};
      var p3 = new Particle {Position = new[] {-1.5, -1.5, -1.5}};

      updateValue(p1);
      updateValue(p2);
      updateValue(p3);

      var particles = new List<Particle> {p1, p2, p3};

      var r1 = Swarm.ArgMin(particles).Position
        .SequenceEqual(new[] {0.0, 0, 0});

      p2.Position[2] = 5f;
      updateValue(p2);

      var r2 = Swarm.ArgMin(particles).Position
        .SequenceEqual(new[] {1.5, 1.5, 1.5});

      Assert.True(r1 && r2);
    }

    /// <summary>
    /// Test if the Swarm is initialized correctly. 
    /// </summary>
    [Test]
    public static void TestInitialize()
    {
      var swarm = Pso.SwarmSpso2006(
        new SearchSpace(2, 100f),
        OptimizationFct.SphereFct);
      swarm.Initialize();

      var dims = swarm.SearchSpace.Dimensions;

      var result = true;
      foreach (var particle in swarm.Particles)
      {
        if (particle.Position.Length == dims &&
            particle.PreviousBest.Length.Equals(dims) &&
            particle.Velocity.Length == dims &&
            particle.LocalBest.Length == dims &&
            particle.Neighbours.Count >= 1 &&
            particle.Neighbours.Count == 3) continue;

        result = false;
        break;
      }

      result = result &&
               swarm.Particles.Count == 40 &&
               swarm.Iteration == 0 &&
               swarm.Evaluations == 0;

      Assert.IsTrue(result, "Test Initialize failed");
    }

    private static void TestPso(string title, Swarm swarm,
      double expectation, int iterations = 100)
    {
      swarm.Initialize();
      swarm.Iterate(iterations);

      Console.Write("Best: " + swarm.GlobalBestValue);
      Console.Write(", Expected: " + expectation + " +- 1, ");

      var result = System.Math.Abs(swarm.GlobalBestValue - expectation) < 1;

      Assert.True(result, $"{title} passed");
    }

    [Test]
    public static void TestSpso2006WithSSP()
    {
      var ssp = new StaticSensorPositioning();
      ssp.Init();

      var sspFct = new SspObjectiveFct(ssp);
      var swarm = Pso.SwarmSpso2006(ssp.SearchSpace(), ssp.FitnessFct);
      var pso = new SPSO2006(swarm, sspFct);

      var positions = new[]
      {
        new[] {0.0, 0, 180}, new[] {3.3, 3, 0.0}
      };
      var initialPos = new double[positions.Length * 3];
      for (var i = 0; i < positions.Length; i++)
      {
        positions[i].CopyTo(initialPos, i * 3);
      }

      pso.InitialPosition = initialPos;

      pso.Init();
    }

    [Test]
    public static void TestHyperbolicConfinement()
    {
      var sp = new SearchSpace(3, -5, 5);
      var swarm = Pso.SwarmSpso2006(sp, OptimizationFct.SphereFct);
      swarm.Confinement = Confinement.Hyperbolic;
      swarm.Initialize();

      for (var i = 0; i < 100; i++)
      {
        swarm.Iterate();
        foreach (var p in swarm.Particles)
        {
          foreach (var x in p.Position)
          {
            Console.Write($"{x}, ");
            Assert.True(x > -5 && x < 5);
          }
          Console.WriteLine();
        }
      }
    }

    [Test]
    public static void TestWithSphereFct()
    {
      var sp = new SearchSpace(16, 100);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.SphereFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.SphereFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.SphereFct);

      Console.WriteLine("Sphere function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.SPHERE_FCT_OPT);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.SPHERE_FCT_OPT);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.SPHERE_FCT_OPT);
    }

    [Test]
    public static void TestWithStyblinskiTangFct()
    {
      var sp = new SearchSpace(2, 10);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.StyblinskiTangFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.StyblinskiTangFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.StyblinskiTangFct);

      Console.WriteLine("Styblinski-Tang function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.StyblinskiTangOpt(2),
        200);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.StyblinskiTangOpt(2),
        200);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.StyblinskiTangOpt(2),
        200);
    }

    [Test]
    public static void TestWithMcCormickFct()
    {
      var sp = new SearchSpace(2, 20);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.McCormickFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.McCormickFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.McCormickFct);

      Console.WriteLine("McCormick function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.MC_CORMICK_FCT_OPT);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.MC_CORMICK_FCT_OPT);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.MC_CORMICK_FCT_OPT);
    }

    [Test]
    public static void TestWithHoelderTableFct()
    {
      var sp = new SearchSpace(2, 10);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.HoelderTableFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.HoelderTableFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.HoelderTableFct);

      Console.WriteLine("Hoelder-Table function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.HOELDER_TABLE_FCT_OPT);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.HOELDER_TABLE_FCT_OPT);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.HOELDER_TABLE_FCT_OPT,
        200);
    }

    [Test]
    public static void TestWithThreeCamelHumpFct()
    {
      var sp = new SearchSpace(2, 100);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.ThreeHumpCamelFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.ThreeHumpCamelFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.ThreeHumpCamelFct);

      Console.WriteLine("Three-Hump-Camel function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.THREE_HUMP_CAMEL_FCT_OPT);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.THREE_HUMP_CAMEL_FCT_OPT);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.THREE_HUMP_CAMEL_FCT_OPT);
    }

    [Test]
    public static void TestWithHimmelblauFct()
    {
      var sp = new SearchSpace(2, 100);
      var swarm2006 = Pso.SwarmSpso2006(sp, OptimizationFct.HimmelblauFct);
      var swarm2007 = Pso.SwarmSpso2007(sp, OptimizationFct.HimmelblauFct);
      var swarm2011 = Pso.SwarmSpso2011(sp, OptimizationFct.HimmelblauFct);

      Console.WriteLine("Himmelblau function:");
      TestPso("SPSO 2006", swarm2006, OptimizationFct.HIMMELBLAU_FCT_OPT);
      TestPso("SPSO 2007", swarm2007, OptimizationFct.HIMMELBLAU_FCT_OPT);
      TestPso("SPSO 2011", swarm2011, OptimizationFct.HIMMELBLAU_FCT_OPT);
    }

    [Test]
    public static void TestFindsMultipleRoots()
    {
      var sp = new SearchSpace(2, 100);
      for (var i = 0; i < 10000; i++)
      {
        var swarm = Pso.SwarmSpso2011(sp, OptimizationFct.HimmelblauFct);
        swarm.Initialize();
        swarm.Iterate(100);

        foreach (var d in swarm.GlobalBest)
        {
          Console.Write(d + ", ");
        }

        Console.WriteLine();
      }
    }

    [Test]
    public static void DebugPso()
    {
      var sp = new SearchSpace(10, 100);
      var swarm =
        Pso.SwarmSpso2011(sp, OptimizationFct.StyblinskiTangFct);
      swarm.Initialize();

      Console.WriteLine("Optimum: " +
                        OptimizationFct.StyblinskiTangOpt(10));

      for (var i = 0; i < 10000; i++)
      {
        Console.Write(swarm.GlobalBestValue);
        swarm.Iterate();
        Console.Read();
      }
    }
  }
}