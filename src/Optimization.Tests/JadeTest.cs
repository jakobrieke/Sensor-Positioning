using System;
using NUnit.Framework;

namespace Optimization
{
  [TestFixture]
  public class JadeTest
  {
    /// <summary>
    /// Test if after 500 iterations the optimum of JADE is in
    /// interval [0, expected].
    /// </summary>
    /// <param name="jade"></param>
    /// <param name="expected"></param>
    private static void RunTest(Jade jade, double expected, 
      uint iterations = 500)
    {
      Console.WriteLine($"i = 0, best = {jade.Best().Value}");
      
      jade.Iterate(iterations);
      
      Console.WriteLine($"i = 500, best = {jade.Best().Value}");
      Console.WriteLine($"Expected: {expected}");
      
      var value = Math.Abs(jade.Best().Value);
      Assert.LessOrEqual(value, expected);
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

    [Test] public static void TestOnF4()
    {
      var obj = new F4();
      var sp = new SearchSpace(5, 100);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 7.67E-22);
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
    
    [Test] public static void TestOnF8()
    {
      var obj = new F8();
      var sp = new SearchSpace(5, 500);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 2.37E+1);
    }
    
    [Test] public static void TestOnF9()
    {
      var obj = new F9();
      var sp = new SearchSpace(5, 5.12);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 0);
    }
    
    [Test] public static void TestOnF10()
    {
      var obj = new F10();
      var sp = new SearchSpace(5, 32);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 2.81E-15);
    }
    
    [Test] public static void TestOnF11()
    {
      var obj = new F11();
      var sp = new SearchSpace(5, 600);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 1.48E-4, 1000);
    }
    
    [Test] public static void TestOnF12()
    {
      var obj = new F12();
      var sp = new SearchSpace(5, 50);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 9.424E-32);
    }
    
    [Test] public static void TestOnF13()
    {
      var obj = new F13();
      var sp = new SearchSpace(5, 50);
      var jade = new Jade(obj, sp);
      jade.Init(20);

      RunTest(jade, 1.35E-32);
    }
  }
}