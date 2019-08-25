using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Evolution Strategy (1+1)-ES without Criterion
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]進化戦略 https://ja.wikipedia.org/wiki/%E9%80%B2%E5%8C%96%E6%88%A6%E7%95%A5
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptES : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max Iteration</summary>
        public override int MaxIterations { get; set; } = 50000;

        /// <summary>epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.000000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        /// <summary>Upper bound(limit solution space)</summary>
        public double[] UpperBounds { get; set; } = null;

        /// <summary>Lower bound(limit solution space)</summary>
        public double[] LowerBounds { get; set; } = null;

        // ----------------------------------------------------------------
        // parameters
        // ----------------------------------------------------------------
        /// <summary>population</summary>
        private List<LoPoint> _populations = new List<LoPoint>();

        /// <summary>Population Size</summary>
        public int PopulationSize { get; set; } = 1;

        /// <summary>update ratio C(Schwefel 0.85)</summary>
        public double C { get; set; } = 0.85;

        /// <summary>recent result for Criterion</summary>
        private double _recentResult = 0.0;

        /// <summary>variance</summary>
        private double _variance = 0.0;

        /// <summary>recent Mutate success history for 1/5 rule</summary>
        private Queue<int> _successMutate = new Queue<int>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptES(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
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
                this._iteration = 0;
                this._populations.Clear();
                this._error.Clear();

                // bound check
                if (UpperBounds != null && LowerBounds != null)
                {
                    if (UpperBounds.Length != this._func.Dimension())
                        throw new Exception("UpperBounds.Length is different");
                    if (LowerBounds.Length != this._func.Dimension())
                        throw new Exception("LowerBounds.Length is different");
                }

                // generate population
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    // initial position
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);

                    // bound check
                    var tempPoint = new LoPoint(base._func, array);
                    if (UpperBounds != null && LowerBounds != null)
                        clsUtil.LimitSolutionSpace(tempPoint, this.LowerBounds, this.UpperBounds);

                    // save point
                    this._populations.Add(tempPoint);
                }

                // Sort Evaluate
                this._populations.Sort();

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this._populations.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this._populations.Count || this.HigherNPercentIndex >= this._populations.Count)
                    this.HigherNPercentIndex = this._populations.Count - 1;

                // specify ES parameters
                _successMutate.Clear();
                for (int i = 0; i <= (this._func.Dimension() * 10) - 1; i++)
                    _successMutate.Enqueue(0);
                // init variance
                _variance = clsUtil.GenRandomRange(0.1, 5);
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

        /// <summary>
        ///         ''' Do Iteration
        ///         ''' </summary>
        ///         ''' <param name="iterations">Iteration count. When you set zero, use the default value.</param>
        ///         ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ///         ''' <remarks></remarks>
        public override bool Iterate(int iterations = 0)
        {
            // Check Last Error
            if (this.IsRecentError() == true)
                return true;

            // for 1/5 rule
            var n = this._func.Dimension() * 10.0;

            // Do Iterate
            if (this.MaxIterations <= _iteration)
                return true;
            else
                iterations = iterations == 0 ? MaxIterations - _iteration - 1 : Math.Min(iterations, MaxIterations - _iteration) - 1;
            for (int iterate = 0; iterate <= iterations; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // Sort Evaluate
                this._populations.Sort();

                // check criterion
                // If Me.IsUseCriterion = True AndAlso m_iteration <> 0 Then
                // 'higher N percentage particles are finished at the time of same evaluate value.
                // If clsUtil.IsCriterion(Me.EPS, Me._populations(0).Eval, _recentResult) Then
                // Return True
                // End If
                // End If

                // -------------------------------------------------
                // ES
                // -------------------------------------------------
                // mutate
                var child = _populations[0].Copy();
                for (int i = 0; i <= _populations[0].Count - 1; i++)
                    child[i] = _populations[0][i] + clsUtil.NormRand(_rand, 0, _variance);
                child.ReEvaluate();

                // check best
                if (child.Value < _populations[0].Value)
                {
                    _recentResult = _populations[0].Value;
                    _populations[0] = child;
                    _successMutate.Enqueue(1);
                }
                else
                    _successMutate.Enqueue(0);
                _successMutate.Dequeue();

                // 1/5 rule
                int successCount = 0;
                foreach (var tempVal in _successMutate)
                    successCount += tempVal;
                var updateSuccessRatio = successCount / (double)n;
                if (updateSuccessRatio < 0.2)
                    _variance = _variance * C;
                else
                    _variance = _variance / C;
            }

            return false;
        }

        /// <summary>
        ///         ''' Best result
        ///         ''' </summary>
        ///         ''' <returns>Best point class</returns>
        ///         ''' <remarks></remarks>
        public override LoPoint Result
        {
            get
            {
                return clsUtil.GetBestPoint(this._populations, true);
            }
        }

        /// <summary>
        ///         ''' Get recent error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return this._error.IsError();
        }

        /// <summary>
        ///         ''' All Result
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' for Debug
        ///         ''' </remarks>
        public override List<LoPoint> Results
        {
            get
            {
                return this._populations;
            }
        }
    }
}

