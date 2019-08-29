using System;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture] public class Spso2006Test
  {
    private static void RunTest(ParticleSwarm pso, double expected)
    {
      Console.WriteLine($"i = 0, best = {pso.GlobalBestValue}");
      
      pso.Iterate(500);
      
      Console.WriteLine($"i = 0, best = {pso.GlobalBestValue}");
      Console.WriteLine($"Expected: {expected}");
      Assert.True(pso.GlobalBestValue < expected);
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
  }
}