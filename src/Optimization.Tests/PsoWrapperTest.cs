using System;
using BenchmarkFunction;
using LibOptimization.Optimization;
using MersenneTwister;
using NUnit.Framework;
using Optimization.LibOptimizationWrapper;

namespace Optimization
{
  [TestFixture] public class LoPsoTest
  {
    [Test] public static void Test()
    {
      Console.WriteLine(1E-100 + 1E-60);
      Console.WriteLine(1E-60);
      Console.WriteLine(1E-30);
      Console.WriteLine(1E-15);
      Console.WriteLine(1E-7);
    
      var obj = new clsBenchSphere(3);
      var pso = new clsOptPSO(obj) 
      {
        SwarmSize = 40,
        Random = MersenneTwister.MTRandom.Create(674901,
          MTEdition.Original_19937)
      };
      
      pso.Init();
      Console.WriteLine(pso.Result.Value);
      Assert.Less(pso.Result.Value, 1.94);
      
      pso.Iterate(50);
      Console.WriteLine(pso.Result.Value);
      Assert.Less(pso.Result.Value, 3.9302E-05);
    }
  }
  
  [TestFixture] public class PsoWrapperTest
  {
    [Test] public static void Test()
    {
      var obj = new SphereFunction();
      var sp = new SearchSpace(3, 100);
      var opt = new PsoWrapper(obj, sp)
      {
        SwarmSize = 40,
        Random = MersenneTwister.MTRandom.Create(674901, 
          MTEdition.Original_19937)
      };

      opt.Init();
      Console.WriteLine(opt.Best().Value);
      Assert.Less(opt.Best().Value, 1.94);
      
      opt.Iterate(50);
      Console.WriteLine(opt.Best().Value);
      Assert.Less(opt.Best().Value, 3.9302E-05);
    }
  }
}