using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Hill-Climbing algorithm(山登り法)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Randomized algorithm for optimization.
    ///     ''' 
    ///     ''' Reffrence:
    ///     ''' https://en.wikipedia.org/wiki/Hill_climbing
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptHillClimbing : absOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>point</summary>
        private List<clsPoint> _populations = new List<clsPoint>();

        /// <summary>Max iteration count</summary>
        public override int Iteration { get; set; } = 10000;

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
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptHillClimbing(absObjectiveFunction ai_func)
        {
            this.m_func = ai_func;
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public override void Init()
        {
            try
            {
                // init meber varibles
                this.m_iteration = 0;
                this._populations.Clear();
                this.m_error.Clear();

                // init initial position
                if (InitialPosition != null && InitialPosition.Length == m_func.NumberOfVariable())
                    this._populations.Add(new clsPoint(this.m_func, InitialPosition));
                else
                {
                    var array = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this._populations.Add(new clsPoint(this.m_func, array));
                }
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

        public int Count = 0;

        /// <summary>
        ///         ''' Do Iteration
        ///         ''' </summary>
        ///         ''' <param name="iteration">Iteration count. When you set zero, use the default value.</param>
        ///         ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ///         ''' <remarks></remarks>
        public override bool DoIteration(int iteration = 0)
        {
            // Check Last Error
            if (this.IsRecentError() == true)
                return true;

            // Do Iterate
            if (this.Iteration <= m_iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - m_iteration - 1 : Math.Min(iteration, Iteration - m_iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                m_iteration += 1;

                // neighbor function
                var nextPoint = Neighbor(_populations[0]);

                // limit solution space
                clsUtil.LimitSolutionSpace(nextPoint, LowerBounds, UpperBounds);

                // evaluate
                if (_populations[0].Eval > nextPoint.Eval)
                    _populations[0] = nextPoint;
            }

            return false;
        }

        /// <summary>
        ///         ''' Best result
        ///         ''' </summary>
        ///         ''' <returns>Best point class</returns>
        ///         ''' <remarks></remarks>
        public override clsPoint Result
        {
            get
            {
                return _populations[0];
            }
        }

        /// <summary>
        ///         ''' Get recent error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return this.m_error.IsError();
        }

        /// <summary>
        ///         ''' All Result
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' for Debug
        ///         ''' </remarks>
        public override List<clsPoint> Results
        {
            get
            {
                return _populations;
            }
        }

        /// <summary>
        ///         ''' Neighbor function for local search
        ///         ''' </summary>
        ///         ''' <param name="base"></param>
        ///         ''' <returns></returns>
        private clsPoint Neighbor(clsPoint @base)
        {
            List<clsPoint> ret = new List<clsPoint>();
            for (int k = 0; k <= this.NeighborSize - 1; k++)
            {
                clsPoint temp = new clsPoint(@base);
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

