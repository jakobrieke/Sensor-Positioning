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
      for (var i = 0; i < NumberOfVariable(); i++)
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

    public override int NumberOfVariable()
    {
      return 5;
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      {
        var func = new clsBenchRosenblock(2);
        var opt = new clsOptHillClimbing(func);
        opt.InitialPosition = new[] {10.0, -10.0};
        opt.Iteration = 10;
        //opt.IsUseCriterion = false;
        opt.Init();
        opt.Count = 0;
        clsUtil.DebugValue(opt);
        opt.DoIteration(3);
        clsUtil.DebugValue(opt);
        opt.DoIteration(3);
        clsUtil.DebugValue(opt);
        opt.DoIteration();
        clsUtil.DebugValue(opt);
        //
        Console.WriteLine("=========================");
        opt.Init();
        opt.Count = 0;
        clsUtil.DebugValue(opt);
        opt.DoIteration();
        clsUtil.DebugValue(opt);
        //return;
      }

      //Typical use
      {
        //Target Function
        var func = new clsBenchRosenblock(2);

        //Set Function
        var opt = new clsOptNelderMead(func);
        opt.Init();

        //Optimization
        opt.DoIteration();

        //Check Error
        if (opt.IsRecentError())
        {
          return;
        }

        //Get Result
        clsUtil.DebugValue(opt);
      }

      //Evaluate optimization result per 100 iteration
      {
        var opt =
          new clsOptDEJADE(new clsBenchRosenblock(10));
        opt.Init();
        clsUtil.DebugValue(opt);

        while (opt.DoIteration(100) == false)
        {
          clsUtil.DebugValue(opt, ai_isOutValue: false);
        }

        clsUtil.DebugValue(opt);
      }

      //Evaluate optimization result per 100 iteration with check my criterion.
      {
        var opt = new clsOptDEJADE(new clsBenchRosenblock(10))
        {
          IsUseCriterion = false
        };
        //Disable Internal criterion

        //Init
        opt.Init();
        clsUtil.DebugValue(opt);

        //do optimization!
        while (opt.DoIteration(100) == false)
        {
          var eval = opt.Result.Eval;

          //my criterion
          if (eval < 0.01)
          {
            break;
          }

          clsUtil.DebugValue(opt, ai_isOutValue: false);
        }

        clsUtil.DebugValue(opt);
      }

      //Set boundary variable.
      //-20<x1<-1, -15<x2<0
      {
        var opt = new clsOptDEJADE(new clsBenchRosenblock(2))
        {
          InitialPosition = new double[] {-10, -10},
          LowerBounds = new double[] {-20, -15},
          UpperBounds = new double[] {-1, 0}
        };

        //Init
        opt.Init();
        clsUtil.DebugValue(opt);

        //do optimization!
        while (opt.DoIteration(100) == false)
        {
          var eval = opt.Result.Eval;

          //my criterion
          if (eval < 0.01)
          {
            break;
          }

          clsUtil.DebugValue(opt, ai_isOutValue: false);
        }

        clsUtil.DebugValue(opt);
      }

      // Optimization problem using MyObjectiveFunction
      // min f(x)
      // s.t. x>0, 170<=x1<=200, 200<=x2<=300, 250<=x3<=400, 370<=x4<=580,
      // 380<=x5<=600
      {
        var func = new MyObjectiveFunction();
        var opt = new clsOptDEJADE(func)
        {
          LowerBounds = new double[] {170, 200, 250, 370, 380},
          UpperBounds = new double[] {200, 300, 400, 580, 600}
        };

        // Move initial position
        double[] initialPosition = {0, 0, 0, 0, 0};
        for (var i = 0; i < initialPosition.Length; i++)
        {
          // Center of boundary range.
          initialPosition[i] = (opt.LowerBounds[i] + opt.UpperBounds[i]) / 2.0;
        }

        opt.InitialPosition = initialPosition;

        //Init
        opt.Init();
        clsUtil.DebugValue(opt);

        // Do optimization!
        while (opt.DoIteration(100) == false)
        {
          var eval = opt.Result.Eval;

          // My criterion
          if (eval < 0.01)
          {
            break;
          }

          Console.WriteLine("Eval:{0}", opt.Result.Eval);
        }

        clsUtil.DebugValue(opt);
      }
    }
  }
}