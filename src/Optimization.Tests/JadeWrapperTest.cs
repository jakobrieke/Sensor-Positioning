using System;
using BenchmarkFunction;
using LibOptimization.Optimization;
using MersenneTwister;
using NUnit.Framework;
using Optimization.LibOptimizationWrapper;

namespace Optimization
{
  [TestFixture]
  public class LoJadeTest
  {
    [Test] public static void Test()
    {
      var obj = new clsBenchSphere(3);
      var jade = new clsJADE(obj)
      {
        Random = MersenneTwister.MTRandom.Create(674901, 
          MTEdition.Original_19937)
      };
      
      jade.Init();
      Console.WriteLine(jade.Result.Value);
      Assert.Less(jade.Result.Value, 1.4734);
      
      jade.Iterate(50);
      Console.WriteLine(jade.Result.Value);
      Assert.Less(jade.Result.Value, 1.21003E-06);
    }
  }
  
  [TestFixture]
  public class JadeWrapperTest
  {
    [Test] public static void Test()
    {
      var sp = new SearchSpace(2, 100);
      var opt = new JadeWrapper(new SphereFunction(), sp)
      {
        Random = MersenneTwister.MTRandom.Create(674901, 
          MTEdition.Original_19937)
      };

      opt.Init();
      Console.WriteLine(opt.Best());
      Assert.Less(opt.Best().Value, 0.711);
      
      opt.Iterate(100);
      Console.WriteLine(opt.Best());
      Assert.Less(opt.Best().Value, 1.7E-16);
    }
  }
}