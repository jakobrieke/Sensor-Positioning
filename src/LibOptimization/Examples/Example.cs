using System;
using System.Collections.Generic;
using BenchmarkFunction;
using LibOptimization.Optimization;
using Util;

namespace LibOptimization.Examples
{
  internal class MyObjectiveFunction : AbsObjectiveFunction
  {
    private double _maxEval;
    private bool _oneShot = true;

    /// <summary>
    /// eval
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public override double F(List<double> x)
    {
      var ret = 0.0;
      for (var i = 0; i < Dimension(); i++)
      {
        // When even one variable has a negative value, use the
        // recent large evaluation value
        if (x[i] < 0 && _oneShot == false)
        {
          ret = _maxEval;
          break;
        }

        // Model(sphere)
        ret += x[i] * x[i];
      }

      // Update max eval
      if (_oneShot)
      {
        _maxEval = ret; //one shot
        _oneShot = false;
      }

      if (_maxEval < ret)
      {
        _maxEval = ret;
      }

      return ret;
    }

    public override List<double> Gradient(List<double> aa)
    {
      return null;
    }

    public override List<List<double>> Hessian(List<double> aa)
    {
      return null;
    }

    public override int Dimension()
    {
      return 5;
    }
  }

  public class Example
  {
    public static void Test(string[] args)
    {
      var func = new clsBenchRosenblock(2);
      var opt = new clsOptHillClimbing(func);
      opt.InitialPosition = new[] {10.0, -10.0};
      opt.MaxIterations = 10;
      //opt.IsUseCriterion = false;
      opt.Init();
      opt.Count = 0;
      clsUtil.DebugValue(opt);
      opt.Iterate(3);
      clsUtil.DebugValue(opt);
      opt.Iterate(3);
      clsUtil.DebugValue(opt);
      opt.Iterate();
      clsUtil.DebugValue(opt);
      //
      Console.WriteLine("=========================");
      opt.Init();
      opt.Count = 0;
      clsUtil.DebugValue(opt);
      opt.Iterate();
      clsUtil.DebugValue(opt);
      //return;

      // -- Typical use
      
      // Target Function
      var func2 = new clsBenchRosenblock(2);

      // Set Function
      var opt2 = new clsOptNelderMead(func2);
      opt2.Init();

      // Optimization
      opt2.Iterate();

      // Check Error
      if (opt2.IsRecentError())
      {
        return;
      }

      // Get Result
      clsUtil.DebugValue(opt2);

      // Evaluate optimization result per 100 iteration
      
      var opt3 = new clsJADE(new clsBenchRosenblock(10));
      opt3.Init();
      clsUtil.DebugValue(opt3);

      while (opt3.Iterate(100) == false)
      {
        clsUtil.DebugValue(opt3, ai_isOutValue: false);
      }

      clsUtil.DebugValue(opt3);

      // -- Evaluate optimization result per 100 iteration with check my criterion.
      
      var opt4 = new clsJADE(new clsBenchRosenblock(10))
      {
        IsUseCriterion = false
      };
      //Disable Internal criterion

      //Init
      opt4.Init();
      clsUtil.DebugValue(opt4);

      //do optimization!
      while (opt4.Iterate(100) == false)
      {
        var eval = opt4.Result.Value;

        //my criterion
        if (eval < 0.01)
        {
          break;
        }

        clsUtil.DebugValue(opt4, ai_isOutValue: false);
      }

      clsUtil.DebugValue(opt4);

      // -- Test set boundary variable.
      
      // -20<x1<-1, -15<x2<0
      var opt5 = new clsJADE(new clsBenchRosenblock(2))
      {
        InitialPosition = new double[] {-10, -10},
        LowerBounds = new double[] {-20, -15},
        UpperBounds = new double[] {-1, 0}
      };

      //Init
      opt5.Init();
      clsUtil.DebugValue(opt5);

      //do optimization!
      while (opt5.Iterate(100) == false)
      {
        var eval = opt5.Result.Value;

        //my criterion
        if (eval < 0.01)
        {
          break;
        }

        clsUtil.DebugValue(opt5, ai_isOutValue: false);
      }

      clsUtil.DebugValue(opt5);

      // -- Test with optimization problem using MyObjectiveFunction
      
      // min f(x)
      // s.t. x>0, 170<=x1<=200, 200<=x2<=300, 250<=x3<=400, 370<=x4<=580,
      // 380<=x5<=600
      
      var func3 = new MyObjectiveFunction();
      var opt6 = new clsJADE(func3)
      {
        LowerBounds = new double[] {170, 200, 250, 370, 380},
        UpperBounds = new double[] {200, 300, 400, 580, 600}
      };

      // Move initial position
      double[] initialPosition = {0, 0, 0, 0, 0};
      for (var i = 0; i < initialPosition.Length; i++)
      {
        // Center of boundary range.
        initialPosition[i] = (opt6.LowerBounds[i] + opt6.UpperBounds[i]) / 2.0;
      }

      opt6.InitialPosition = initialPosition;

      //Init
      opt6.Init();
      clsUtil.DebugValue(opt6);

      // Do optimization!
      while (opt6.Iterate(100) == false)
      {
        var eval = opt6.Result.Value;

        // My criterion
        if (eval < 0.01)
        {
          break;
        }

        Console.WriteLine("Eval:{0}", opt6.Result.Value);
      }

      clsUtil.DebugValue(opt6);
    }
  }
}