using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Differential Evolution Algorithm (DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -similar to GA algorithm.
    ///     ''' 
    ///     ''' Memo:
    ///     '''  Notation of DE
    ///     '''   DE/x/y/z
    ///     '''    x: pick parent strategy(rand or best)
    ///     '''    y: number of difference vector
    ///     '''    z: crossover scheme
    ///     '''       ex.Binomial -> bin
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]Storn, R., Price, K., "Differential Evolution – A Simple and Efficient Heuristic for Global Optimization over Continuous Spaces", Journal of Global Optimization 11: 341–359.
    ///     ''' [2]Price, K. and Storn, R., "Minimizing the Real Functions of the ICEC’96 contest by Differential Evolution", IEEE International Conference on Evolutionary Computation (ICEC’96), may 1996, pp. 842–844.
    ///     ''' [3]Sk. Minhazul Islam, Swagatam Das, "An Adaptive Differential Evolution Algorithm With Novel Mutation and Crossover Strategies for Global Numerical Optimization", IEEE TRANSACTIONS ON SYSTEMS, MAN, AND CYBERNETICS—PART B: CYBERNETICS, VOL. 42, NO. 2, APRIL 2012, pp482-500.
    ///     ''' [4]田邊遼司, and 福永Alex. "自動チューナーを用いた異なる最大評価回数における Differential Evolution アルゴリズムのパラメータ設定の調査." 進化計算学会論文誌 6.2 (2015): 67-81.
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptDE : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max iteration count</summary>
        public override int MaxIterations { get; set; } = 20000;

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
        // DE parameters
        // ----------------------------------------------------------------
        /// <summary>
        ///         ''' Population Size(Default:100)
        ///         ''' </summary>
        public int PopulationSize { get; set; } = 100;

        /// <summary>
        ///         ''' Differential weight(Scaling factor)(Default:0.5)
        ///         ''' </summary>
        public double F { get; set; } = 0.5;

        /// <summary>
        ///         ''' Cross over ratio(Default:0.9)
        ///         ''' </summary>
        public double CrossOverRatio { get; set; } = 0.9;

        /// <summary>
        ///         ''' Differential Evolution Strategy
        ///         ''' </summary>
        public EnumDEStrategyType DEStrategy { get; set; } = EnumDEStrategyType.DE_rand_1_bin;

        /// <summary>
        ///         ''' Enum Differential Evolution Strategy[3][4]
        ///         ''' </summary>
        public enum EnumDEStrategyType
        {
            /// <summary>DE/rand/1/bin - global searchability(大域検索)</summary>
            DE_rand_1_bin,
            /// <summary>DE/rand/2/bin - Strong global searchability(強い大域検索)</summary>
            DE_rand_2_bin,
            /// <summary>DE/best/1/bin - local searchability(局所検索)</summary>
            DE_best_1_bin,
            /// <summary>DE/best/2/bin - Strong local searchability(強い局所検索)</summary>
            DE_best_2_bin,
            /// <summary>DE/currentToRand/1/bin</summary>
            DE_current_to_rand_1_bin,
            /// <summary>DE/currentToBest/1/bin - local searchability(局所検索)</summary>
            DE_current_to_Best_1_bin,
            /// <summary>DE/currentToBest/2/bin - local searchability(局所検索)</summary>
            DE_current_to_Best_2_bin
        }

        /// <summary>population</summary>
        private List<LoPoint> m_parents = new List<LoPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptDE(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
        }

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <param name="ai_destrategy">DE Strategt(e.g. DE/rand/1/bin)</param>
        ///         ''' <remarks></remarks>
        public clsOptDE(AbsObjectiveFunction ai_func, EnumDEStrategyType ai_destrategy)
        {
            this._func = ai_func;
            this.DEStrategy = ai_destrategy;
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
                this.m_parents.Clear();
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
                    // initialize
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);

                    // bound check
                    var tempPoint = new LoPoint(base._func, array);
                    if (UpperBounds != null && LowerBounds != null)
                        clsUtil.LimitSolutionSpace(tempPoint, this.LowerBounds, this.UpperBounds);

                    // save point
                    this.m_parents.Add(tempPoint);
                }

                // Sort Evaluate
                this.m_parents.Sort();

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this.m_parents.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this.m_parents.Count || this.HigherNPercentIndex >= this.m_parents.Count)
                    this.HigherNPercentIndex = this.m_parents.Count - 1;
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
                this.m_parents.Sort();

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Value, this.m_parents[this.HigherNPercentIndex].Value))
                        return true;
                }

                // reserve best value
                var best = this.m_parents[0].Copy();

                // DE
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    // pick different parent without i
                    List<int> randIndex = clsUtil.RandomPermutaion(this.m_parents.Count, i);
                    var xi = this.m_parents[i];
                    LoPoint p1 = this.m_parents[randIndex[0]];
                    LoPoint p2 = this.m_parents[randIndex[1]];
                    LoPoint p3 = this.m_parents[randIndex[2]];
                    LoPoint p4 = this.m_parents[randIndex[3]];
                    LoPoint p5 = this.m_parents[randIndex[4]];

                    // Mutation and Crossover
                    var child = new LoPoint(this._func);
                    var j = this._rand.Next() % this._func.Dimension();
                    var D = this._func.Dimension() - 1;

                    switch (this.DEStrategy)
                    {
                        case EnumDEStrategyType.DE_best_1_bin:
                            {
                                // DE/best/1/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = best[j] + this.F * (p1[j] - p2[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_best_2_bin:
                            {
                                // DE/best/2/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = best[j] + this.F * (p1[j] + p2[j] - p3[j] - p4[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_current_to_Best_1_bin:
                            {
                                // DE/current-to-best/1/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = xi[j] + this.F * (best[j] - p1[j]) + this.F * (p2[j] - p3[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_current_to_Best_2_bin:
                            {
                                // DE/current-to-best/2/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = xi[j] + this.F * (best[j] - p1[j]) + this.F * (p2[j] - p3[j]) + this.F * (p4[j] - p5[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_current_to_rand_1_bin:
                            {
                                // DE/current-to-rand/1/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = xi[j] + this.F * (p2[j] - p3[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_rand_1_bin:
                            {
                                // DE/rand/1/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = p1[j] + this.F * (p2[j] - p3[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }

                        case EnumDEStrategyType.DE_rand_2_bin:
                            {
                                // DE/rand/2/bin
                                for (var k = 0; k <= this._func.Dimension() - 1; k++)
                                {
                                    if (this._rand.NextDouble() < this.CrossOverRatio || k == D)
                                        child[j] = p1[j] + this.F * (p2[j] + p3[j] - p4[j] - p5[j]);
                                    else
                                        child[j] = xi[k];
                                    j = (j + 1) % this._func.Dimension(); // next
                                }

                                break;
                            }
                    }
                    child.ReEvaluate(); // Evaluate child

                    // Limit solution space
                    clsUtil.LimitSolutionSpace(child, this.LowerBounds, this.UpperBounds);

                    // Survive
                    if (child.Value < this.m_parents[i].Value)
                        this.m_parents[i] = child;

                    // Current best
                    if (child.Value < best.Value)
                        best = child;
                }
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
                return clsUtil.GetBestPoint(m_parents, true);
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
                return this.m_parents;
            }
        }
    }
}

