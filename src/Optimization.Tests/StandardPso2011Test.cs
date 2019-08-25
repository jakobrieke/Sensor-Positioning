using System;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture]
  public class StandardPso2011Test
  {
    [Test] public static void TestOnSphereFunction()
    {
      var sp = new SearchSpace(3, 10);
      var obj = new SphereFunction();
      var opt = new StandardPso2011(sp, obj);
      
      opt.Init();
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) != 0);

      
      opt.Iterate(50);
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) == 0);

    }

    [Test] public static void TestOnF6()
    {
      var sp = new SearchSpace(3, 10);
      var obj = new F6();
      var opt = new StandardPso2011(sp, obj);

      opt.Init();
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) != 0);
      
      opt.Iterate(50);
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) == 0);
    }
    
    [Test] public static void TestOnF7()
    {
      var sp = new SearchSpace(3, 10);
      var obj = new F7();
      var opt = new StandardPso2011(sp, obj);
      
      opt.Init();
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) != 0);
      
      opt.Iterate(50);
      Console.WriteLine(opt.Best());
      Assert.True(Math.Round(opt.Best().Value) == 0);
    }
  }
}