using System;
using System.Collections.Generic;
using MathUtil;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Real-coded Genetic Algorithm
    ///     ''' REX(Real-coded Ensemble Crossover) + JGG
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Cross over algorithm is REX(Real-coded Ensemble Cross over).
    ///     '''  -Alternation of generation algorithm is JGG.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptRealGAREX : absOptimization
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
        /// <summary>population</summary>
        public List<clsPoint> m_parents { get; set; } = new List<clsPoint>(); // Parent

        /// <summary>Population Size(Default:50*Log(n)+10)</summary>
        public int PopulationSize { get; set; } = 1000;

        /// <summary>Parent size for cross over(Default:n+1)</summary>
        public int ParentSize { get; set; } = 100; // REX(phi, n+k) -> n+1<n+k<npop

        /// <summary>Children Size(Default30*Log(n)+10)</summary>
        public int ChildrenSize { get; set; } = 100;

        /// <summary>REX randomo mode(Default:UNIFORM)</summary>
        public RexRandomMode RandomMode { get; set; } = RexRandomMode.UNIFORM;

        public enum RexRandomMode
        {
            UNIFORM,
            NORMAL_DIST
        }

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Target Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptRealGAREX(absObjectiveFunction ai_func)
        {
            this.m_func = ai_func;
            this.ParentSize = this.m_func.NumberOfVariable() + 1; // n+k

            // Me.PopulationSize = Me.m_func.NumberOfVariable() * 8
            // Me.ChildrenSize = Me.m_func.NumberOfVariable() * 6 '6-8 * n
            this.PopulationSize = System.Convert.ToInt32(50 * Math.Log(this.m_func.NumberOfVariable()) + 10);
            this.ChildrenSize = System.Convert.ToInt32(30 * Math.Log(this.m_func.NumberOfVariable()) + 10);
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
                this.m_parents.Clear();

                // initial position
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_parents.Add(new clsPoint(this.m_func, array));
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

            // Do Iterate
            if (this.Iteration <= m_iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - m_iteration - 1 : Math.Min(iteration, Iteration - m_iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                m_iteration += 1;

                // Sort Evaluate
                this.m_parents.Sort();

                // check criterion
                if (IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Eval, this.m_parents[this.HigherNPercentIndex].Eval))
                        return true;
                }

                // REX with JGG
                List<KeyValuePair<int, clsPoint>> parents = this.SelectParent(this.m_parents, this.ParentSize);

                // Crossover
                List<clsPoint> children = this.CrossOverREX(this.RandomMode, this.ChildrenSize, parents);

                // Replace
                int index = 0;
                foreach (KeyValuePair<int, clsPoint> p in parents)
                {
                    this.m_parents[p.Key] = children[index];
                    index += 1;
                }
            }

            return false;
        }

        /// <summary>
        ///         ''' Select Parent
        ///         ''' </summary>
        ///         ''' <param name="ai_population"></param>
        ///         ''' <param name="ai_parentSize"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private List<KeyValuePair<int, clsPoint>> SelectParent(List<clsPoint> ai_population, int ai_parentSize)
        {
            List<KeyValuePair<int, clsPoint>> ret = new List<KeyValuePair<int, clsPoint>>();

            // Index
            List<int> randIndex = clsUtil.RandomPermutaion(ai_population.Count);

            // PickParents
            for (int i = 0; i <= ai_parentSize - 1; i++)
                ret.Add(new KeyValuePair<int, clsPoint>(randIndex[i], ai_population[randIndex[i]]));

            return ret;
        }

        /// <summary>
        ///         ''' REX(Real-coded Ensemble Crossover)
        ///         ''' </summary>
        ///         ''' <param name="ai_randomMode"></param>
        ///         ''' <param name="ai_childNum">ChildNum</param>
        ///         ''' <param name="ai_parents"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' REX(U, n+k) -> U is UniformRandom
        ///         ''' REX(N, n+k) -> N is NormalDistribution
        ///         ''' "n+k" is parents size.
        ///         ''' </remarks>
        private List<clsPoint> CrossOverREX(RexRandomMode ai_randomMode, int ai_childNum, List<KeyValuePair<int, clsPoint>> ai_parents)
        {
            // Calc Centroid
            clsEasyVector xg = new clsEasyVector(base.m_func.NumberOfVariable());
            foreach (KeyValuePair<int, clsPoint> p in ai_parents)
                xg += p.Value;
            xg /= (double)ai_parents.Count; // sum(xi)/(n+k)

            // Range
            double range = (this.InitialValueRangeUpper - this.InitialValueRangeLower) / 2.0;

            // cross over
            List<clsPoint> retChilds = new List<clsPoint>();
            double uniformRandParam = Math.Sqrt(3 / (double)ai_parents.Count);
            double normalDistParam = 1 / (double)ai_parents.Count; // ???
            for (int i = 0; i <= ai_childNum; i++)
            {
                // cross over
                clsEasyVector childV = new clsEasyVector(base.m_func.NumberOfVariable());
                // sum( rand * (xi-xg) )
                foreach (KeyValuePair<int, clsPoint> xi in ai_parents)
                {
                    // rand parameter
                    double randVal = 0.0;
                    if (ai_randomMode == RexRandomMode.NORMAL_DIST)
                        randVal = clsUtil.NormRand(0, normalDistParam);
                    else
                        randVal = Math.Abs(2.0 * uniformRandParam) * m_rand.NextDouble() - range;
                    // rand * (xi-xg)
                    childV += randVal * (xi.Value - xg);
                }
                // xg + sum( rand * (xi-xg) )
                childV += xg;

                // convert clsPoint
                clsPoint child = new clsPoint(base.m_func, childV);
                child.ReEvaluate();
                retChilds.Add(child);
            }
            retChilds.Sort();

            return retChilds;
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
                return this.m_parents;
            }
        }
    }
}

