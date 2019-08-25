using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    /// Hill-Climbing algorithm(山登り法)
    /// </summary>
    /// <remarks>
    /// Features:
    ///  -Randomized algorithm for optimization.
    /// 
    /// Reffrence:
    /// https://en.wikipedia.org/wiki/Hill_climbing
    /// 
    /// Implment:
    /// N.Tomi(tomi.nori+github at gmail.com)
    /// </remarks>
    public class clsOptHillClimbing : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>point</summary>
        private List<LoPoint> _populations = new List<LoPoint>();

        /// <summary>Max iteration count</summary>
        public override int MaxIterations { get; set; } = 10000;

        /// <summary>Upper bound(limit solution space)</summary>
        public double[] UpperBounds { get; set; } = null;

        /// <summary>Lower bound(limit solution space)</summary>
        public double[] LowerBounds { get; set; } = null;

        // ----------------------------------------------------------------
        // Hill-Climbing parameters
        // ----------------------------------------------------------------
        /// <summary>range of neighbor search</summary>
        public double NeighborRange { get; set; } = 0.1;

        /// <summary>range of neighbor search</summary>
        public int NeighborSize { get; set; } = 50;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_func">Objective Function</param>
        /// <remarks>
        /// </remarks>
        public clsOptHillClimbing(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <remarks></remarks>
        public override void Init()
        {
            try
            {
                // init meber varibles
                this._iteration = 0;
                this._populations.Clear();
                this._error.Clear();

                // init initial position
                if (InitialPosition != null && InitialPosition.Length == _func.Dimension())
                    this._populations.Add(new LoPoint(this._func, InitialPosition));
                else
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this._populations.Add(new LoPoint(this._func, array));
                }
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

        public int Count = 0;

        /// <summary>
        /// Do Iteration
        /// </summary>
        /// <param name="iterations">Iteration count. When you set zero, use the default value.</param>
        /// <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        /// <remarks></remarks>
        public override bool Iterate(int iterations = 0)
        {
            // Check Last Error
            if (this.IsRecentError() == true)
                return true;

            // Do Iterate
            if (this.MaxIterations <= _iteration)
                return true;
            else
                iterations = iterations == 0 ? MaxIterations - _iteration - 1 : Math.Min(iterations, MaxIterations - _iteration) - 1;
            for (var iterate = 0; iterate <= iterations; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // neighbor function
                var nextPoint = Neighbour(_populations[0]);

                // limit solution space
                clsUtil.LimitSolutionSpace(nextPoint, LowerBounds, UpperBounds);

                // evaluate
                if (_populations[0].Value > nextPoint.Value)
                    _populations[0] = nextPoint;
            }

            return false;
        }

        /// <summary>
        /// Best result
        /// </summary>
        /// <returns>Best point class</returns>
        /// <remarks></remarks>
        public override LoPoint Result
        {
            get
            {
                return _populations[0];
            }
        }

        /// <summary>
        /// Get recent error
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool IsRecentError()
        {
            return this._error.IsError();
        }

        /// <summary>
        /// All Result
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>
        /// for Debug
        /// </remarks>
        public override List<LoPoint> Results
        {
            get
            {
                return _populations;
            }
        }

        /// <summary>
        /// Neighbor function for local search
        /// </summary>
        /// <param name="base"></param>
        /// <returns></returns>
        private LoPoint Neighbour(LoPoint @base)
        {
            List<LoPoint> ret = new List<LoPoint>();
            for (int k = 0; k <= this.NeighborSize - 1; k++)
            {
                LoPoint temp = new LoPoint(@base);
                for (int i = 0; i <= temp.Count - 1; i++)
                {
                    var tempNeighbor = Math.Abs(2.0 * NeighborRange) * base.Random.NextDouble() - NeighborRange;
                    temp[i] += tempNeighbor;
                }
                temp.ReEvaluate();
                ret.Add(temp);
            }
            ret.Sort();

            return ret[0];
        }
    }
}

