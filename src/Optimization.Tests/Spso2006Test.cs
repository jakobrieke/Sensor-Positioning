using System;
using System.Linq;
using LinearAlgebra;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture] public class Spso2006Test
  {
    private static void RunTest(ParticleSwarm pso, double expected, 
      uint iterations = 500)
    {
      Console.WriteLine($"i = 0, best = {pso.GlobalBestValue}");
      
      pso.Iterate(iterations);
      
      Console.WriteLine($"i = 500, best = {pso.Best().Value}");
      Console.WriteLine($"Expected: {expected}");
      
      var value = Math.Abs(pso.Best().Value);
      Assert.LessOrEqual(value, expected);
    }
    
    [Test] public static void TestOnSphereFunction()
    {
      var obj = new SphereFunction();
      var sp = new SearchSpace(5, 100);

      var pso = new StandardPso2006(sp, obj);
      pso.Init(40);
      
      RunTest(pso, 9E-17);
    }
    
    [Test] public static void TestOnF2()
    {
      var obj = new F2();
      var sp = new SearchSpace(5, 10);
      
      var pso = new StandardPso2006(sp, obj);
      pso.Init(40);
      
      RunTest(pso, 3E-09);
    }

    [Test] public static void TestOnF3()
    {
      var obj = new F3();
      var sp = new SearchSpace(5, 100);
      
      var pso = new StandardPso2006(sp, obj);
      pso.Init(40);
      
      RunTest(pso, 2E-12);
    }
    
    [Test] public static void TestOnF4()
    {
      var obj = new F4();
      var sp = new SearchSpace(5, 100);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 7.67E-22);
    }
    
    [Test] public static void TestOnF5()
    {
      var obj = new F5();
      var sp = new SearchSpace(5, 10);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 3.5E-1);
    }
    
    [Test] public static void TestOnF6()
    {
      var obj = new F6();
      var sp = new SearchSpace(5, 10);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 0);
    }
    
    [Test] public static void TestOnF7()
    {
      var obj = new F7();
      var sp = new SearchSpace(5, 10);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 8.54E-4);
    }
    
    [Test] public static void TestOnF8()
    {
      var obj = new F8();
      var sp = new SearchSpace(5, 500);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 2.37E+1);
    }
    
    [Test] public static void TestOnF9()
    {
      var obj = new F9();
      var sp = new SearchSpace(5, 5.12);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 0);
    }
    
    [Test] public static void TestOnF10()
    {
      var obj = new F10();
      var sp = new SearchSpace(5, 32);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 2.81E-15);
    }
    
    [Test] public static void TestOnF11()
    {
      var obj = new F11();
      var sp = new SearchSpace(5, 600);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 1.48E-4, 1000);
    }
    
    [Test] public static void TestOnF12()
    {
      var obj = new F12();
      var sp = new SearchSpace(5, 50);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 9.424E-32);
    }
    
    [Test] public static void TestOnF13()
    {
      var obj = new F13();
      var sp = new SearchSpace(5, 50);
      var pso = new StandardPso2006(sp, obj);
      pso.Init(20);

      RunTest(pso, 1.35E-32);
    }

    [Test]
    public static void TestAxisBias()
    {
      var center = new Vector(4.5, 6);
      var sp = new SearchSpace(new[]
      {
        new double[] {0, 9}, new double[] {0, 6}
      });
      var obj = new SimpleObjective(vector => -Vector.Distance(center, vector));
      var pso = new StandardPso2006(sp, obj);

      const int runs = 1000;
      var ll = 0;
      var lr = 0;
      var ur = 0;
      var ul = 0;
      for (var i = 0; i < runs; i++)
      {
        pso.Init(40);
        pso.Iterate(200);
        if (pso.GlobalBest[0] == 0 && pso.GlobalBest[1] == 0) ll++;
        else if (pso.GlobalBest[0] == 9 && pso.GlobalBest[1] == 0) lr++;
        else if (pso.GlobalBest[0] == 0 && pso.GlobalBest[1] == 6) ul++;
        else if (pso.GlobalBest[0] == 9 && pso.GlobalBest[1] == 6) ur++;
        else Console.WriteLine((Vector) pso.GlobalBest);
      }
      Console.WriteLine("Lower left: " + (double) ll / runs * 100 + "%");
      Console.WriteLine("Lower right: " + (double) lr / runs * 100 + "%");
      Console.WriteLine("Upper left: " + (double) ul / runs * 100 + "%");
      Console.WriteLine("Upper right: " + (double) ur / runs * 100 + "%");
    }
  }
}