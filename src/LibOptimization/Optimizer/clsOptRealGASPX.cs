using System;
using System.Collections.Generic;
using MathUtil;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Real-coded Genetic Algorithm
    ///     ''' SPX(Simplex Crossover) + JGG
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Cross over algorithm is SPX(Simplex Crossover).
    ///     '''  -Alternation of generation algorithm is JGG.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptRealGASPX : absOptimization
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

        /// <summary>Upper bound(limit solution space)</summary>
        public double[] UpperBounds { get; set; } = null;

        /// <summary>Lower bound(limit solution space)</summary>
        public double[] LowerBounds { get; set; } = null;

        // -------------------------------------------------------------------
        // Coefficient of GA
        // -------------------------------------------------------------------
        /// <summary>Population Size(Default:n*33)</summary>
        public int PopulationSize { get; set; } = 100;

        /// <summary>Children Size(Default:n*10)</summary>
        public int ChildrenSize { get; set; } = 100;

        /// <summary>population</summary>
        private List<clsPoint> m_parents = new List<clsPoint>(); // Parent

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptRealGASPX(absObjectiveFunction ai_func)
        {
            this.m_func = ai_func;

            this.PopulationSize = this.m_func.NumberOfVariable() * 33;
            this.ChildrenSize = this.m_func.NumberOfVariable() * 10;
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
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Eval, this.m_parents[this.HigherNPercentIndex].Eval))
                        return true;
                }

                // SPX with JGG
                // Parent is n+1
                List<KeyValuePair<int, clsPoint>> parents = this.SelectParent(this.m_parents, this.m_func.NumberOfVariable() + 1);

                // Crossover
                List<clsPoint> children = this.CrossOverSPX(this.ChildrenSize, parents);

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
        ///         ''' Simplex Crossover
        ///         ''' </summary>
        ///         ''' <param name="ai_childSize"></param>
        ///         ''' <param name="ai_parents"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private List<clsPoint> CrossOverSPX(int ai_childSize, List<KeyValuePair<int, clsPoint>> ai_parents)
        {
            // Calc Centroid
            clsEasyVector xg = new clsEasyVector(base.m_func.NumberOfVariable());
            foreach (KeyValuePair<int, clsPoint> p in ai_parents)
                xg += p.Value;
            xg /= (double)ai_parents.Count; // sum(xi)/(n+k)

            // SPX
            List<clsPoint> retChilds = new List<clsPoint>();
            double alpha = Math.Sqrt(base.m_func.NumberOfVariable() + 2); // expantion rate
            for (int i = 0; i <= ai_childSize - 1; i++)
            {
                List<clsEasyVector> cVector = new List<clsEasyVector>();
                List<clsEasyVector> pVector = new List<clsEasyVector>();
                int k = 0;
                foreach (KeyValuePair<int, clsPoint> xi in ai_parents)
                {
                    pVector.Add(xg + alpha * (xi.Value - xg));

                    if (k == 0)
                        cVector.Add(new clsEasyVector(base.m_func.NumberOfVariable())); // all zero
                    else
                    {
                        double rk = Math.Pow(m_rand.NextDouble(), (1 / (double)k));
                        var pos = rk * (pVector[k - 1] - pVector[k] + cVector[k - 1]);
                        cVector.Add(pos);
                    }
                    k += 1;
                }
                var tempChild = new clsPoint(base.m_func, pVector[pVector.Count - 1] + cVector[cVector.Count - 1]);

                // limit solution space
                clsUtil.LimitSolutionSpace(tempChild, this.LowerBounds, this.UpperBounds);

                retChilds.Add(tempChild);
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

