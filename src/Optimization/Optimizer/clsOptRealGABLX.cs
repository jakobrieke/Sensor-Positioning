using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Real-coded Genetic Algorithm
    ///     ''' BLX-Alpha + JGG
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Alternation of generation algorithm is JGG.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' 北野宏明 (編集), "遺伝的アルゴリズム 4", 産業図書出版株式会社, 2000年初版
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptRealGABLX : AbsOptimization
    {
        /// <summary>Max iteration count(Default:20,000)</summary>
        public override int Iteration { get; set; } = 20000;

        /// <summary>Epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.00000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        // -------------------------------------------------------------------
        // Coefficient of GA
        // -------------------------------------------------------------------
        /// <summary>Population Size(Default:dimension*50)</summary>
        public int PopulationSize { get; set; } = 100;

        /// <summary>Children Size(Default:dimension*20)</summary>
        public int ChildrenSize { get; set; } = 100;

        /// <summary>Alpha is expantion ratio(Default:0.5)</summary>
        public double Alpha { get; set; } = 0.5;

        /// <summary>population</summary>
        private List<LoPoint> m_parents = new List<LoPoint>(); // Parent

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptRealGABLX(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;

            this.PopulationSize = this._func.Dimension() * 50;
            this.ChildrenSize = this._func.Dimension() * 20;
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

                // initial position
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_parents.Add(new LoPoint(this._func, array));
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
        ///         ''' <param name="iteration">Iteration count. When you set zero, use the default value.</param>
        ///         ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ///         ''' <remarks></remarks>
        public override bool Iterate(int iteration = 0)
        {
            // Check Last Error
            if (this.IsRecentError() == true)
                return true;

            // Do Iterate
            if (this.Iteration <= _iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - _iteration - 1 : Math.Min(iteration, Iteration - _iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // Sort Evaluate
                this.m_parents.Sort();

                // Check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Eval, this.m_parents[this.HigherNPercentIndex].Eval))
                        return true;
                }

                // BLX-alpha cross-over
                // Pick parent
                List<int> randIndex = clsUtil.RandomPermutaion(this.m_parents.Count);
                int p1Index = randIndex[0];
                int p2Index = randIndex[1];
                var p1 = this.m_parents[p1Index];
                var p2 = this.m_parents[p2Index];

                // cross over
                List<LoPoint> children = new List<LoPoint>();
                for (int numChild = 0; numChild <= this.ChildrenSize - 1; numChild++)
                {
                    children.Add(new LoPoint(this._func));
                    for (int i = 0; i <= this._func.Dimension() - 1; i++)
                    {
                        double range = Math.Abs(p1[i] - p2[i]);
                        double min = 0;
                        double max = 0;
                        if (p1[i] > p2[i])
                        {
                            min = p2[i];
                            max = p1[i];
                        }
                        else
                        {
                            min = p1[i];
                            max = p2[i];
                        }
                        children[numChild][i] = clsUtil.GenRandomRange(min - this.Alpha * range, max + this.Alpha * range);
                    }
                    children[numChild].ReEvaluate();
                }

                // replace(JGG)
                children.Sort();
                this.m_parents[p1Index] = children[0];
                this.m_parents[p2Index] = children[1];
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

