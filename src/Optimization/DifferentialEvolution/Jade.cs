using System;
using System.Collections.Generic;
using System.Linq;
using LinearAlgebra;
using static System.Math;

namespace Optimization
{
  /// <summary>
  /// Implementation of the Adaptive-Differential-Evolution algorithm JADE.
  /// For further information see:
  /// 
  /// Jingqiao Zhang, Arthur C. Sanderson.
  /// JADE: Self-Adaptive Differential Evolution with Fast and Reliable
  /// Convergence Performance.
  /// Center for Automation Technologies and Systems, 2007.
  /// </summary>
  public class Jade : DifferentialEvolution
  {
    public double C = 0.1;

    public double P;
    public double µCr { get; private set; }
    public double µF { get; private set; }

    public Jade(Objective fitness, SearchSpace searchSpace) :
      base(fitness, searchSpace)
    {
    }

    public override void Init(int populationSize)
    {
      base.Init(populationSize);
      µCr = 0.5;
      µF = 0.6;
      P = Max(0.05, 3.0 / populationSize);
      Population.Sort();
    }
    
    /// <summary>
    /// Create a uniform random number in [left, right].
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private double RandU(double left = 0.0, double right = 1.0)
    {
      return RandomExtension.Uniform(Random, left, right);
    }
    
    // Reminder: In previous implementation version Cauchy was used instead of
    // Normal distribution, the JADE then sometimes manipulated the FOV
    // attribute of sensors in the SensorPosition objective
    // -> That should never happen, so why did it?
    // -> Behaviour has not been noticed since moving to Normal distribution
    // See Git log for further reference 
    private double RandN(double mean, double standardDeviation = 0.1)
    {
      return MathNet.Numerics.Distributions.Normal.Sample(
        Random, mean, standardDeviation);
    }

    private static double Mean(List<double> values)
    {
      return values.Sum() / values.Count;
    }

    private static double LehmerMean(List<double> values)
    {
      return values.Sum(x => Pow(x, 2)) / values.Sum();
    }

    protected override void Update()
    {
      // Generate CR
      for (var i = 0; i < CR.Count; i++) CR[i] = RandN(µCr);

      // Generate F
      var third = F.Count / 3;
      for (var i = 0; i < F.Count; i++)
      {
        F[i] = i < third ? RandU(0, 1.2) : RandN(µF);
      }
      
      // Create SCR and SF
      var SCR = new List<double>(Population.Count);
      var SF = new List<double>(Population.Count);

      for (var i = 0; i < Population.Count; i++)
      {
        // -- Mutation
        
        // Choose x_best from top 100p% vectors
        var pBestIndex = (int) RandU(0, P) 
                         * Population.Count;
        var xBest = Population[pBestIndex].Position;

        // Choose r1 != r2 != i in {0, 1, ..., Population.Count - 1}
        var choices = new List<Point>(Population);
        
        choices.RemoveAt(i);
        var r1 = Random.Next(0, choices.Count);
        var xR1 = choices[r1].Position;
        
        choices.RemoveAt(r1);
        var r2 = Random.Next(0, choices.Count);
        var xR2 = choices[r2].Position;
        
        var x = Population[i].Position;
        var v = x + F[i] * (xBest - x) + F[i] * (xR1 - xR2);
        
        // -- Crossover

        var u = Vector.One(SearchSpace.Dimension);
        var jRand = Random.Next(SearchSpace.Dimension);
          
        for (var j = 0; j < SearchSpace.Dimension; j++)
        {
          u[j] = j == jRand || RandU() <= CR[i] ? v[j] : x[j];
        }

        // -- Selection
        
        var valueU = Fitness.Eval(u);
        if (valueU < Population[i].Value)
        {
          Population[i] = new Point(u, valueU);
          SCR.Add(CR[i]);
          SF.Add(F[i]);
          Population.Sort();
        }
      }
      
      µCr = (1 - C) * µCr + C * Mean(SCR);
      µF = (1 - C) * µF + C * LehmerMean(SF);
      
//      base.Update();
    }

    public override Point Best()
    {
      return Population[0];
    }

    /// <summary>
    /// Implementation of the DE/Current-to-p-best mutation strategy.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override Vector Mutation()
    {
      throw new NotImplementedException();
    }

    public override Vector Crossover()
    {
      throw new NotImplementedException();
    }

    public override Point Selection()
    {
      throw new NotImplementedException();
    }
  }
}