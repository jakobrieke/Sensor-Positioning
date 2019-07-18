using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Standard Cuckoo Search
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]Xin-She Yang, Suash Deb, "Cuckoo search via Lévy flights.", World Congress on Nature and Biologically Inspired Computing (NaBIC 2009). IEEE Publications. pp. 210–214. arXiv:1003.1594v1.
    ///     ''' [2]Cuckoo Search (CS) Algorithm
    ///     '''    http://www.mathworks.com/matlabcentral/fileexchange/29809-cuckoo-search--cs--algorithm
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptCS : absOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max iteration count</summary>
        public override int Iteration { get; set; } = 50000;

        /// <summary>
        ///         ''' epsilon(Default:1e-8) for Criterion
        ///         ''' </summary>
        public double EPS { get; set; } = 0.000000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        // ----------------------------------------------------------------
        // Parameters
        // ----------------------------------------------------------------
        /// <summary>
        ///         ''' Population Size(Default:25)
        ///         ''' </summary>
        public int PopulationSize { get; set; } = 25;

        /// <summary>
        ///         ''' Discovery rate(Default:0.25)
        ///         ''' </summary>
        public double PA { get; set; } = 0.25;

        /// <summary>
        ///         ''' Levy flight parameter(Default:1.5)
        ///         ''' </summary>
        public double Beta { get; set; } = 1.5;

        // nest
        private List<clsPoint> m_nests = new List<clsPoint>();
        private clsPoint m_currentBest;


        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptCS(absObjectiveFunction ai_func)
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
                this.m_nests.Clear();
                this.m_error.Clear();

                // initial position
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_nests.Add(new clsPoint(this.m_func, array));
                }

                // Sort Evaluate
                this.m_nests.Sort();

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this.m_nests.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this.m_nests.Count || this.HigherNPercentIndex >= this.m_nests.Count)
                    this.HigherNPercentIndex = this.m_nests.Count - 1;
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

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

            // current best
            this.m_currentBest = this.m_nests[clsUtil.FindCurrentIndex(this.m_nests)];

            // levy flight parameter
            var sigma = Math.Pow((this.Gamma(1 + Beta) * Math.Sin(Math.PI * Beta / 2) / (this.Gamma((1 + Beta) / 2) * Beta * Math.Pow(2, ((Beta - 1) / 2)))), (1 / Beta));

            // Do Iterate
            if (this.Iteration <= m_iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - m_iteration - 1 : Math.Min(iteration, Iteration - m_iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                m_iteration += 1;

                // Evaluate
                var sortedEvalList = clsUtil.GetSortedEvalList(this.m_nests);

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    var bestEval = sortedEvalList[0].Eval;
                    var nPercentEval = sortedEvalList[this.HigherNPercentIndex].Eval;
                    if (clsUtil.IsCriterion(this.EPS, bestEval, nPercentEval))
                        return true;
                }

                // get cuckoo
                List<clsPoint> newNests = new List<clsPoint>(this.PopulationSize);
                for (var i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var s = this.m_nests[i];
                    var newNest = new clsPoint(this.m_func);
                    for (var j = 0; j <= this.m_func.NumberOfVariable() - 1; j++)
                    {
                        var u = clsUtil.NormRand(0) * sigma;
                        var v = Math.Abs(clsUtil.NormRand(0));
                        var tempStep = u / Math.Pow(v, 1 / Beta);
                        var stepSize = 0.01 * tempStep * (s[j] - this.m_currentBest[j]);
                        var temp = s[j] + stepSize * clsUtil.NormRand(0);
                        newNest[j] = temp;
                    }
                    newNest.ReEvaluate();
                    newNests.Add(newNest); // new nests
                }

                // Find current best
                var candidateBest = newNests[clsUtil.FindCurrentIndex(newNests)];
                if (candidateBest.Eval < this.m_currentBest.Eval)
                    this.m_currentBest = candidateBest.Copy();

                // replace
                for (var i = 0; i <= this.PopulationSize - 1; i++)
                {
                    if (newNests[i].Eval < this.m_nests[i].Eval)
                        this.m_nests[i] = newNests[i];
                }

                // Discovery and randomization
                newNests.Clear();
                var randPerm1 = clsUtil.RandomPermutaion(this.PopulationSize);
                var randPerm2 = clsUtil.RandomPermutaion(this.PopulationSize);
                for (var i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var newNest = new clsPoint(this.m_func);
                    for (var j = 0; j <= this.m_func.NumberOfVariable() - 1; j++)
                    {
                        if (this.m_rand.NextDouble() > this.PA)
                            newNest[j] = this.m_nests[i][j] + this.m_rand.NextDouble() * (this.m_nests[randPerm1[i]][j] - this.m_nests[randPerm2[i]][j]);
                        else
                            newNest[j] = this.m_nests[i][j];
                    }
                    newNest.ReEvaluate();
                    newNests.Add(newNest);
                }

                // Find current best
                candidateBest = newNests[clsUtil.FindCurrentIndex(newNests)];
                if (candidateBest.Eval < this.m_currentBest.Eval)
                    this.m_currentBest = candidateBest.Copy();

                // replace
                for (var i = 0; i <= this.PopulationSize - 1; i++)
                {
                    if (newNests[i].Eval < this.m_nests[i].Eval)
                        this.m_nests[i] = newNests[i];
                }
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
                return clsUtil.GetBestPoint(this.m_nests, true);
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
                return this.m_nests;
            }
        }

        /// <summary>
        ///         ''' Log Gamma function
        ///         ''' </summary>
        ///         ''' <param name="x"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' Refference:
        ///         ''' C言語による最新アルゴリズム事典
        ///         ''' </remarks>
        private double LogGamma(double x)
        {
            var LOG_2PI = 1.8378770664093456;
            var N = 8;

            var B0 = 1;              // /* 以下はBernoulli数 */
            var B1 = (-1.0 / 2.0);
            var B2 = (1.0 / 6.0);
            var B4 = (-1.0 / 30.0);
            var B6 = (1.0 / 42.0);
            var B8 = (-1.0 / 30.0);
            var B10 = (5.0 / 66.0);
            var B12 = (-691.0 / 2730.0);
            var B14 = (7.0 / 6.0);
            var B16 = (-3617.0 / 510.0);

            double v = 1;
            double w;

            while (x < N)
            {
                v *= x;
                x += 1;
            }
            w = 1 / (x * x);
            return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w
                        + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w
                        + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w
                        + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x
                        + 0.5 * LOG_2PI - Math.Log(v) - x + (x - 0.5) * Math.Log(x);
        }

        /// <summary>
        ///         ''' Gamma function
        ///         ''' </summary>
        ///         ''' <param name="x"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' Refference:
        ///         ''' C言語による最新アルゴリズム事典
        ///         ''' </remarks>
        private double Gamma(double x)
        {
            if ((x < 0))
                return Math.PI / (Math.Sin(Math.PI * x) * Math.Exp(LogGamma(1 - x)));
            return Math.Exp(LogGamma(x));
        }
    }
}

