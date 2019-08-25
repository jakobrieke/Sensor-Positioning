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
    public class clsOptCS : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max iteration count</summary>
        public override int MaxIterations { get; set; } = 50000;

        /// <summary>
        ///         ''' epsilon(Default:1e-8) for Criterion
        ///         ''' </summary>
        public double EPS { get; set; } = 0.000000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex; // for IsCriterion())

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
        private List<LoPoint> m_nests = new List<LoPoint>();
        private LoPoint m_currentBest;


        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptCS(AbsObjectiveFunction ai_func)
        {
            _func = ai_func;
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
                _iteration = 0;
                m_nests.Clear();
                _error.Clear();

                // initial position
                for (int i = 0; i <= PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(_func, InitialPosition, InitialValueRangeLower, InitialValueRangeUpper);
                    m_nests.Add(new LoPoint(_func, array));
                }

                // Sort Evaluate
                m_nests.Sort();

                // Detect HigherNPercentIndex
                HigherNPercentIndex = Convert.ToInt32(m_nests.Count * HigherNPercent);
                if (HigherNPercentIndex == m_nests.Count || HigherNPercentIndex >= m_nests.Count)
                    HigherNPercentIndex = m_nests.Count - 1;
            }
            catch (Exception ex)
            {
                _error.SetError(true, clsError.ErrorType.ERR_INIT);
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
            if (IsRecentError())
                return true;

            // current best
            m_currentBest = m_nests[clsUtil.FindCurrentIndex(m_nests)];

            // levy flight parameter
            var sigma = Math.Pow((Gamma(1 + Beta) * Math.Sin(Math.PI * Beta / 2) / (Gamma((1 + Beta) / 2) * Beta * Math.Pow(2, ((Beta - 1) / 2)))), (1 / Beta));

            // Do Iterate
            if (MaxIterations <= _iteration)
                return true;
            iterations = iterations == 0 ? MaxIterations - _iteration - 1 : Math.Min(iterations, MaxIterations - _iteration) - 1;
            for (var iterate = 0; iterate <= iterations; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // Evaluate
                var sortedEvalList = clsUtil.GetSortedEvalList(m_nests);

                // check criterion
                if (IsUseCriterion)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    var bestEval = sortedEvalList[0].Eval;
                    var nPercentEval = sortedEvalList[HigherNPercentIndex].Eval;
                    if (clsUtil.IsCriterion(EPS, bestEval, nPercentEval))
                        return true;
                }

                // get cuckoo
                List<LoPoint> newNests = new List<LoPoint>(PopulationSize);
                for (var i = 0; i <= PopulationSize - 1; i++)
                {
                    var s = m_nests[i];
                    var newNest = new LoPoint(_func);
                    for (var j = 0; j <= _func.Dimension() - 1; j++)
                    {
                        var u = clsUtil.NormRand() * sigma;
                        var v = Math.Abs(clsUtil.NormRand());
                        var tempStep = u / Math.Pow(v, 1 / Beta);
                        var stepSize = 0.01 * tempStep * (s[j] - m_currentBest[j]);
                        var temp = s[j] + stepSize * clsUtil.NormRand();
                        newNest[j] = temp;
                    }
                    newNest.ReEvaluate();
                    newNests.Add(newNest); // new nests
                }

                // Find current best
                var candidateBest = newNests[clsUtil.FindCurrentIndex(newNests)];
                if (candidateBest.Value < m_currentBest.Value)
                    m_currentBest = candidateBest.Copy();

                // replace
                for (var i = 0; i <= PopulationSize - 1; i++)
                {
                    if (newNests[i].Value < m_nests[i].Value)
                        m_nests[i] = newNests[i];
                }

                // Discovery and randomization
                newNests.Clear();
                var randPerm1 = clsUtil.RandomPermutaion(PopulationSize);
                var randPerm2 = clsUtil.RandomPermutaion(PopulationSize);
                for (var i = 0; i <= PopulationSize - 1; i++)
                {
                    var newNest = new LoPoint(_func);
                    for (var j = 0; j <= _func.Dimension() - 1; j++)
                    {
                        if (_rand.NextDouble() > PA)
                            newNest[j] = m_nests[i][j] + _rand.NextDouble() * (m_nests[randPerm1[i]][j] - m_nests[randPerm2[i]][j]);
                        else
                            newNest[j] = m_nests[i][j];
                    }
                    newNest.ReEvaluate();
                    newNests.Add(newNest);
                }

                // Find current best
                candidateBest = newNests[clsUtil.FindCurrentIndex(newNests)];
                if (candidateBest.Value < m_currentBest.Value)
                    m_currentBest = candidateBest.Copy();

                // replace
                for (var i = 0; i <= PopulationSize - 1; i++)
                {
                    if (newNests[i].Value < m_nests[i].Value)
                        m_nests[i] = newNests[i];
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
                return clsUtil.GetBestPoint(m_nests, true);
            }
        }

        /// <summary>
        ///         ''' Get recent error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return _error.IsError();
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
                return m_nests;
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
            return (((((((B16 / (16 * 15) * w + B14 / (14 * 13)) * w
                        + B12 / (12 * 11)) * w + B10 / (10 * 9)) * w
                        + B8 / (8 * 7)) * w + B6 / (6 * 5)) * w
                        + B4 / (4 * 3)) * w + B2 / (2 * 1)) / x
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

