using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkFunction;
using LibOptimization.Optimization;
using LOMath;
using NUnit.Framework;
using Util;

/// <summary>
/// Unit test optimization
/// </summary>
public class TestOptimization
{
//  public TestOptimization()
//  {
//    // Fix random number generator
//    clsRandomXorshiftSingleton.GetInstance().SetDefaultSeed();
//  }

  [Test]
  public static void TestGenRandomRange()
  {
    XorShiftPRNGSingleton.GetInstance().SetDefaultSeed();
    
    var results = new List<double>();
    for (var i = 0; i < 100; i++)
    {
      results.Add(clsUtil.GenRandomRange(0, 100));
    }
    results.Sort();
    foreach (var i in results)
    {
      Console.Write(i + ", ");
    }
  }

  [Test]
  public static void TestGenRandomPosition()
  {
    var obj = new ShiftedSphereFct(1, 90);
    for (var i = 0; i < 100; i++)
    {
      var p = clsUtil.GenRandomPosition(obj, null, -5, 5);
    
      foreach (var d in p)
      {
        Assert.True(d > -5 && d < 5, "Generated value is outside bounds");
      }
    }
  }

  private class ShiftedSphereFct : clsBenchSphere
  {
    private int _shift;
    public ShiftedSphereFct(int dimensions, int Shift) : base(dimensions)
    {
      _shift = Shift;
    }

    public override double F(List<double> x)
    {
      for (var i = 0; i < x.Count; i++) x[i] += _shift;
      return base.F(x);
    }
  }

  [Test]
  public static void TestPsoWithShiftedSphere()
  {
    var obj = new ShiftedSphereFct(2, 90);

    var opt = new clsOptPSO(obj);
    opt.Init();
//    Console.WriteLine(opt);
    
    for (var i = 0; i < 1; i++)
    {
      opt.DoIteration();
    }
    
//    Console.WriteLine();
//    Console.WriteLine(opt);
//    Console.WriteLine(obj.F(new List<double>(){10, 10, 10}));
  }
  
  [Test]
  public static void TestPso()
  {
    const double eval = 0.0001;
    var obj = new clsBenchSphere(3);
    var opt = new clsOptPSO(obj)
    {
      InitialPosition = new double[] {10, 10}
    };

    opt.Init();
    var errorFlg = opt.IsRecentError();
    Assert.False(errorFlg);

    // Check iterate
    opt.DoIteration();
    errorFlg = opt.IsRecentError();
    Assert.False(errorFlg);

    // Eval
    Console.Write("Eval:{0} ", opt.Result.Eval);
    var isConversion = false;
    for (var retry = 0; retry <= 5; retry++)
    {
      if (Math.Abs(opt.Result.Eval) > eval)
      {
        Console.Write("{0}Retry! Eval:{1}", Environment.NewLine,
          opt.Result.Eval);

        opt.InitialPosition = opt.Result.ToArray();
        opt.InitialValueRangeLower /= 2;
        opt.InitialValueRangeUpper /= 2;
        opt.Init();
        opt.DoIteration();
      }
      else
      {
        isConversion = true;
        break;
      }
    }

    if (isConversion == false)
      throw new Exception(
        $"fail:{opt.GetType().Name} Eval:{opt.Result.Eval}");

    // Result
    if (Math.Abs(opt.Result[0]) > 0.1 || Math.Abs(opt.Result[1]) > 0.1)
      throw new Exception($"fail:{opt.GetType().Name} " +
                          $"Result:{opt.Result[0]} {opt.Result[1]}");

    Console.WriteLine("Result:{0} {1}", opt.Result[0], opt.Result[1]);
  }

  /// <summary>
  /// optimizer for UnitTest
  /// </summary>
  /// <param name="func"></param>
  /// <returns></returns>
  private static IEnumerable<AbsOptimization> GetOptimizersForUnitTest(
    AbsObjectiveFunction func)
  {
    List<AbsOptimization> optimizers = new List<AbsOptimization>();
    {
      var optCs = new clsOptCS(func);
      optimizers.Add(optCs);
    }
    {
      var optDE_b_1_b = new clsOptDE(func);
      optDE_b_1_b.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_1_bin;
      optimizers.Add(optDE_b_1_b);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_2_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_Best_1_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_Best_2_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_rand_1_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_1_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDE(func);
      opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_2_bin;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptDEJADE(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptES(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptFA(func);
      opt.Iteration = 300;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptHillClimbing(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptNelderMead(func);
      optimizers.Add(opt);
    }
    {
      var opt = new ClsOptNelderMeadWiki(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPatternSearch(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPSO(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPSOAIW(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPSOChaoticIW(func);
      opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CDIW;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPSOChaoticIW(func);
      opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CRIW;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptPSOLDIW(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptRealGABLX(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptRealGAPCX(func);
      opt.Iteration = 1000;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptRealGAREX(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptRealGASPX(func);
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptRealGAUNDX(func);
      opt.ALPHA = 0.6;
      opt.PopulationSize = 100;
      opt.ChildrenSize = 50;
      opt.Iteration = 700;
      optimizers.Add(opt);
    }
    {
      var opt = new clsOptSimulatedAnnealing(func);
      opt.NeighborRange = 0.1;
      opt.Iteration *= 2;
      optimizers.Add(opt);
    }

    // --------------------------------------------------------------
    // Need differentiation
    // --------------------------------------------------------------
    {
      if (func as clsBenchSphere != null)
        optimizers.Add(new clsOptNewtonMethod(func));
    }
    {
      if (func as clsBenchSphere != null)
        optimizers.Add(new clsOptSteepestDescent(func));
    }

    return optimizers;
  }

  /// <summary>
  /// Test Optimization
  /// </summary>
  [Test]
  public static void TestOptimizeSphere()
  {
    const double EVAL = 0.0001;
    for (var i = 2; i <= 3; i++)
    {
      Console.WriteLine("===sphere {0}D===", i);
      var optimizers = GetOptimizersForUnitTest(new clsBenchSphere(i));
      foreach (var opt in optimizers)
      {
        try
        {
          Console.Write("{0,-40}", opt.GetType().Name);

          // fix rng
          // opt.Random = New clsRandomXorshift()

          var sw = new Stopwatch();
          sw.Start();

          // check init
          opt.InitialPosition = new double[] {10, 10};
          opt.Init();
          var errorFlg = opt.IsRecentError();
          Assert.False(errorFlg);

          // check iterate
          opt.DoIteration();
          errorFlg = opt.IsRecentError();
          Assert.False(errorFlg);

          sw.Stop();
          Console.Write("ElapsedTime:{0}[ms] ", sw.ElapsedMilliseconds);

          // Eval
          Console.Write("Eval:{0} ", opt.Result.Eval);
          var isConversion = false;
          for (var retry = 0; retry <= 5; retry++)
          {
            if (Math.Abs(opt.Result.Eval) > EVAL)
            {
              Console.Write("{0}Retry! Eval:{1}", Environment.NewLine,
                opt.Result.Eval);
              opt.InitialPosition = opt.Result.ToArray();
              opt.InitialValueRangeLower = opt.InitialValueRangeLower / 2;
              opt.InitialValueRangeUpper = opt.InitialValueRangeUpper / 2;
              opt.Init();
              opt.DoIteration();
            }
            else
            {
              isConversion = true;
              break;
            }
          }

          if (isConversion == false)
            throw new Exception(
              $"fail:{opt.GetType().Name} Eval:{opt.Result.Eval}");

          // Result
          if (Math.Abs(opt.Result[0]) > 0.1 || Math.Abs(opt.Result[1]) > 0.1)
            throw new Exception(
              $"fail:{opt.GetType().Name} Result:{opt.Result[0]} {opt.Result[1]}");
          Console.WriteLine("Result:{0} {1}", opt.Result[0], opt.Result[1]);
        }
        catch (Exception ex)
        {
          Assert.Fail("Throw Exception! {0} {1}", opt.GetType().Name,
            ex.Message);
        }
      }
    }
  }

  /// <summary>
  /// Test Retry
  /// </summary>
  [Test]
  public static void TestRetryCheck()
  {
    var optimizers = GetOptimizersForUnitTest(new clsBenchSphere(2));
    foreach (var opt in optimizers)
    {
      try
      {
        Console.WriteLine("{0,-40}", opt.GetType().Name);
        // 1st
        opt.Init();
        opt.Iteration = 1;
        opt.DoIteration();

        // 2nd
        var recentCount = opt.IterationCount;
        var recentEval = opt.Result.Eval;
        opt.InitialPosition = opt.Result.ToArray();
        opt.Iteration = 100;
        opt.Init();

        // check iteration count
        if (opt.IterationCount != 0)
          Assert.Fail(
            $"fail {opt.GetType().Name} : 2nd try init iteration count");

        // do optimize
        opt.DoIteration();
        if (opt.IterationCount == 0)
          Assert.Fail(
            $"fail {opt.GetType().Name} : 2nd try DoIteration");
      }
      catch (Exception ex)
      {
        Assert.Fail(
          $"Throw Exception! {opt.GetType().Name} {ex.Message}");
      }
    }
  }

  /// <summary>
  /// Test initial position range
  /// </summary>
  [Test]
  public static void TestInitialPosition()
  {
    var optimizers = GetOptimizersForUnitTest(new clsBenchSphere(2));
    foreach (var opt in optimizers)
    {
      try
      {
        Console.WriteLine("Test optimize algorithm : {0}",
          opt.GetType().Name);

        // init
        const double range = 100.0;
        opt.InitialPosition = new[] {range, range};
        opt.InitialValueRangeLower = -range / 10;
        opt.InitialValueRangeUpper = range / 10;
        opt.Init();

        // initial position check
        var results = opt.Results.ToArray();
        LoMatrix resultMat = clsUtil.ToConvertMat(opt.Results);
        LoVector g = new LoVector();
        for (int i = 0; i <= results[0].Count - 1; i++)
        {
          var ar = resultMat.GetColumn(i);
          var tempVal = clsUtil.Average(ar);
          g.Add(clsUtil.Average(resultMat.GetColumn(i)));
        }

        g.PrintValue(name: "average initial position");

        if (Math.Abs(range - g[0]) > 10 || Math.Abs(range - g[1]) > 10)
          Assert.Fail($"fail {opt.GetType().Name}");
      }
      catch (Exception ex)
      {
        Assert.Fail(
          $"Throw {opt.GetType().Name} object");
      }
    }
  }

  /// <summary>
  /// bound check
  /// </summary>
  [Test]
  public static void TestOptimizationDEWithBound()
  {
    clsOptDE opt = new clsOptDE(new clsBenchTest2());
    // x1-> 0 to 5, x2-> 0 to 5
    opt.LowerBounds = new double[] {0, 0};
    opt.UpperBounds = new double[] {5, 5};
    opt.Init();
    var errorFlg = opt.IsRecentError();
    Assert.False(errorFlg);

    // check iterate
    opt.DoIteration();
    errorFlg = opt.IsRecentError();
    Assert.False(errorFlg);

    // Eval
    if (-78.99 < opt.Result.Eval && opt.Result.Eval < -78.98)
    {
    }
    else Assert.Fail($"fail Eval {opt.Result.Eval}");

    Console.WriteLine("Success Eval {0}", opt.Result.Eval);

    // Result
    if (2.8 < opt.Result[0] && opt.Result[0] < 2.9)
    {
    }
    else Assert.Fail(
      $"fail Result {opt.Result[0]} {opt.Result[1]}");

    Console.WriteLine("Success Result {0} {1}", opt.Result[0], opt.Result[1]);
  }

  private class NothingFunc : AbsObjectiveFunction
  {
    public override double F(List<double> x)
    {
      return XorShiftPRNGSingleton.GetInstance().NextDouble() * 10;
    }

    public override List<double> Gradient(List<double> x)
    {
      var aa = new List<double>
      {
        XorShiftPRNGSingleton.GetInstance().NextDouble() * 10,
        XorShiftPRNGSingleton.GetInstance().NextDouble() * 10
      };
      return aa;
    }

    public override List<List<double>> Hessian(List<double> x)
    {
      throw new NotImplementedException();
    }

    public override int NumberOfVariable()
    {
      return 2;
    }
  }

  /// <summary>
  /// Test Iteration count
  /// </summary>
  [Test]
  public static void TestIterationCount()
  {
    var optimizers = GetOptimizersForUnitTest(new NothingFunc());

    // all iteration
    {
      Console.WriteLine("=all=");
      foreach (var opt in optimizers)
      {
        opt.IsUseCriterion = false;
        opt.InitialPosition = new double[] {100, 100};
        opt.Iteration = 2;
        opt.Init();
        opt.DoIteration();
        if (opt.IterationCount != opt.Iteration)
          Assert.Fail(
            $"{opt.GetType().Name} : IterationCount : {opt.IterationCount}");

        Console.WriteLine("Success : {0}", opt.GetType().Name);
      }
    }

    // per 1
    {
      Console.WriteLine("=per 1=");
      foreach (var opt in optimizers)
      {
        opt.IsUseCriterion = false;
        opt.InitialPosition = new double[] {100, 100};
        opt.Iteration = 3;
        opt.Init();
        while ((opt.DoIteration(1) == false))
        {
        }

        if (opt.IterationCount != opt.Iteration)
          Assert.Fail($"Fail! {opt.GetType().Name} : IterationCount : {opt.IterationCount}");
        else
          Console.WriteLine("Success : {0}", opt.GetType().Name);
      }
    }

    // per 2
    {
      Console.WriteLine("=per 2=");
      foreach (var opt in optimizers)
      {
        opt.IsUseCriterion = false;
        opt.InitialPosition = new double[] {100, 100};
        opt.Iteration = 3;
        opt.Init();
        while ((opt.DoIteration(2) == false))
        {
        }

        if (opt.IterationCount != opt.Iteration)
          Assert.Fail("{0} : IterationCount : {1}", opt.GetType().Name,
            opt.IterationCount);
        else
          Console.WriteLine("Success : {0}", opt.GetType().Name);
      }
    }
  }
}