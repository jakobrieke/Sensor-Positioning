using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Particle Swarm Optimization using Linear Decrease Inertia Weight(LDIW-PSO)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Swarm Intelligence algorithm.
    ///     '''  -Derivative free optimization algorithm.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]Y. Shi and Russell C. Eberhart, "Empirical Study of Particle Swarm Optimization, Proceeding Congress on Evolutionary Computation 1999, Piscataway, 1945-1949
    ///     ''' </remarks>
    public class clsOptPSOLDIW : AbsOptimization
    {
        /// <summary>Max Iteration(Default:20,000)</summary>
        public override int Iteration { get; set; } = 20000;

        /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
        public double EPS { get; set; } = 0.000001; // 1e-6

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        // particles
        private List<clsParticle> m_swarm = new List<clsParticle>();
        private LoPoint m_globalBest = null/* TODO Change to default(_) if this is not a reference type */;

        // -------------------------------------------------------------------
        // Coefficient of PSO
        // -------------------------------------------------------------------
        /// <summary>Swarm Size(Default:100)</summary>
        public int SwarmSize { get; set; } = 100;

        /// <summary>adaptive inertia weight(Default:1.0)</summary>
        public double Weight { get; set; } = 1.0;

        /// <summary>Weight max for adaptive weight(Default:0.9).</summary>
        public double WeightMax { get; set; } = 0.9;

        /// <summary>Weight min for adaptive weight(Default:0.4).</summary>
        public double WeightMin { get; set; } = 0.4;

        /// <summary>velocity coefficient(affected by personal best)(Default:1.49445)</summary>
        public double C1 { get; set; } = 1.49445;

        /// <summary>velocity coefficient(affected by global best)(Default:1.49445)</summary>
        public double C2 { get; set; } = 1.49445;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsOptPSOLDIW(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
        }

        /// <summary>
        ///         ''' Initialize
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public override void Init()
        {
            try
            {
                // init meber varibles
                this._iteration = 0;
                this.m_swarm.Clear();

                // init position
                for (int i = 0; i <= this.SwarmSize - 1; i++)
                {
                    // position
                    var tempPosition = new LoPoint(this._func);
                    var tempBestPosition = new LoPoint(this._func);
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    tempPosition = new LoPoint(this._func, array);
                    tempBestPosition = tempPosition.Copy();

                    // velocity
                    var tempVelocity = clsUtil.GenRandomPosition(this._func, null/* TODO Change to default(_) if this is not a reference type */, this.InitialValueRangeLower, this.InitialValueRangeUpper);

                    // create swarm
                    this.m_swarm.Add(new clsParticle(tempPosition, tempVelocity, tempBestPosition));
                }

                // Sort Evaluate
                this.m_swarm.Sort();
                this.m_globalBest = this.m_swarm[0].BestPoint.Copy();
                this.Weight = 1;

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this.m_swarm.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this.m_swarm.Count || this.HigherNPercentIndex >= this.m_swarm.Count)
                    this.HigherNPercentIndex = this.m_swarm.Count - 1;
            }
            catch (Exception ex)
            {
                this._error.SetError(true, Util.clsError.ErrorType.ERR_INIT);
            }
            finally
            {
                System.GC.Collect();
            }
        }

        /// <summary>
        ///         ''' Do optimize
        ///         ''' </summary>
        ///         ''' <param name="iteration"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool DoIteration(int iteration = 0)
        {
            // Check Last Error
            if (this.IsRecentError() == true)
                return true;

            // do iterate
            if (this.Iteration <= _iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - _iteration - 1 : Math.Min(iteration, Iteration - _iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_swarm[0].BestPoint, this.m_swarm[this.HigherNPercentIndex].BestPoint))
                        return true;
                }

                // PSO process
                foreach (var particle in this.m_swarm)
                {
                    // update a velocity 
                    for (int i = 0; i <= this._func.NumberOfVariable() - 1; i++)
                    {
                        var r1 = this._rand.NextDouble();
                        var r2 = this._rand.NextDouble();
                        var newV = this.Weight * particle.Velocity[i] + C1 * r1 * (particle.BestPoint[i] - particle.Point[i]) + C2 * r2 * (this.m_globalBest[i] - particle.Point[i]);
                        particle.Velocity[i] = newV;

                        // update a position using velocity
                        var newPos = particle.Point[i] + particle.Velocity[i];
                        particle.Point[i] = newPos;
                    }
                    particle.Point.ReEvaluate();

                    // replace personal best
                    if (particle.Point.Eval < particle.BestPoint.Eval)
                    {
                        particle.BestPoint = particle.Point.Copy();

                        // replace global best
                        if (particle.Point.Eval < this.m_globalBest.Eval)
                            this.m_globalBest = particle.Point.Copy();
                    }
                }

                // Inertia Weight Strategie - LDIW Linear Decreasing Inertia Weight
                this.Weight = (this.WeightMax - this.WeightMin) * (this.Iteration - this._iteration) / (double)this.Iteration + this.WeightMin;

                // sort by eval
                this.m_swarm.Sort();
            }

            return false;
        }

        /// <summary>
        ///         ''' Recent Error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return this._error.IsError();
        }

        /// <summary>
        ///         ''' Result
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override Optimization.LoPoint Result
        {
            get
            {
                // find best index
                int bestIndex = 0;
                var bestEval = this.m_swarm[0].BestPoint.Eval;
                for (var i = 0; i <= this.m_swarm.Count - 1; i++)
                {
                    if (this.m_swarm[i].BestPoint.Eval < bestEval)
                    {
                        bestEval = this.m_swarm[i].BestPoint.Eval;
                        bestIndex = i;
                    }
                }
                return this.m_swarm[0].BestPoint.Copy();
            }
        }

        /// <summary>
        ///         ''' for Debug
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override List<Optimization.LoPoint> Results
        {
            get
            {
                List<LoPoint> ret = new List<LoPoint>(this.m_swarm.Count - 1);
                foreach (var p in this.m_swarm)
                    ret.Add(p.BestPoint.Copy());
                return ret;
            }
        }
    }
}

