using System;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture]
  public class JadeTest
  {
    private static void RunTest(Jade jade, double expected)
    {
      Console.WriteLine($"i = 0, best = {jade.Best().Value}");
      
      for (var i = 0; i < 500; i++)
      {
        jade.Update();
      }
      
      Console.WriteLine($"i = 500, best = {jade.Best().Value}");
      Console.WriteLine($"Expected: {expected}");
      Assert.LessOrEqual(jade.Best().Value, expected);
    }
    
    [Test] public static void TestInit()
    {
      var obj = new SphereFunction();
      var sp = new SearchSpace(3, 5);
      
      var jade = new Jade(obj, sp);
      jade.Init(50);
      
      Assert.True(jade.Population.Count == 50);
      Assert.True(jade.µCr == 0.5);
      Assert.True(jade.µF == 0.6);
    }

    [Test] public static void TestOnSphereFunction()
    {
      var obj = new SphereFunction();
      var sp = new SearchSpace(5, 100);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);
      
      RunTest(jade, 1E-47);
    }

    [Test] public static void TestOnF2()
    {
      var obj = new F2();
      var sp = new SearchSpace(5, 10);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);
      
      RunTest(jade, 2E-25);
    }

    [Test] public static void TestOnF3()
    {
      var obj = new F3();
      var sp = new SearchSpace(5, 10);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);

      RunTest(jade, 8E-37);
    }
    
    [Test] public static void TestOnF5()
    {
      var obj = new F5();
      var sp = new SearchSpace(5, 10);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);

      RunTest(jade, 3.5E-1);
    }
    
    [Test] public static void TestOnF6()
    {
      var obj = new F6();
      var sp = new SearchSpace(5, 10);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);

      RunTest(jade, 0);
    }
    
    [Test] public static void TestOnF7()
    {
      var obj = new F7();
      var sp = new SearchSpace(5, 10);
      var jade = new Jade(obj, sp) {C = 0.1};
      jade.Init(20);

      RunTest(jade, 8.54E-4);
    }
  }
}