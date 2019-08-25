using System;
using System.Collections.Generic;
using LoMath;
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
    public class clsOptRealGASPX : AbsOptimization
    {
        /// <summary>Max iteration count(Default:20,000)</summary>
        public override int MaxIterations { get; set; } = 20000;

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
        private List<LoPoint> m_parents = new List<LoPoint>(); // Parent

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptRealGASPX(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;

            this.PopulationSize = this._func.Dimension() * 33;
            this.ChildrenSize = this._func.Dimension() * 10;
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

                // SPX with JGG
                // Parent is n+1
                List<KeyValuePair<int, LoPoint>> parents = this.SelectParent(this.m_parents, this._func.Dimension() + 1);

                // Crossover
                List<LoPoint> children = this.CrossOverSPX(this.ChildrenSize, parents);

                // Replace
                int index = 0;
                foreach (KeyValuePair<int, LoPoint> p in parents)
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
        private List<KeyValuePair<int, LoPoint>> SelectParent(List<LoPoint> ai_population, int ai_parentSize)
        {
            List<KeyValuePair<int, LoPoint>> ret = new List<KeyValuePair<int, LoPoint>>();

            // Index
            List<int> randIndex = clsUtil.RandomPermutaion(ai_population.Count);

            // PickParents
            for (int i = 0; i <= ai_parentSize - 1; i++)
                ret.Add(new KeyValuePair<int, LoPoint>(randIndex[i], ai_population[randIndex[i]]));

            return ret;
        }

        /// <summary>
        ///         ''' Simplex Crossover
        ///         ''' </summary>
        ///         ''' <param name="ai_childSize"></param>
        ///         ''' <param name="ai_parents"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private List<LoPoint> CrossOverSPX(int ai_childSize, List<KeyValuePair<int, LoPoint>> ai_parents)
        {
            // Calc Centroid
            var xg = new LoVector(base._func.Dimension());
            foreach (var p in ai_parents)
                xg += p.Value;
            xg /= (double)ai_parents.Count; // sum(xi)/(n+k)

            // SPX
            var retChilds = new List<LoPoint>();
            var alpha = Math.Sqrt(base._func.Dimension() + 2); // expantion rate
            for (var i = 0; i <= ai_childSize - 1; i++)
            {
                var cVector = new List<LoVector>();
                var pVector = new List<LoVector>();
                var k = 0;
                foreach (var xi in ai_parents)
                {
                    pVector.Add(xg + alpha * (xi.Value - xg));

                    if (k == 0)
                        cVector.Add(new LoVector(base._func.Dimension())); // all zero
                    else
                    {
                        var rk = Math.Pow(_rand.NextDouble(), (1 / (double)k));
                        var pos = rk * (pVector[k - 1] - pVector[k] + cVector[k - 1]);
                        cVector.Add(pos);
                    }
                    k += 1;
                }
                var tempChild = new LoPoint(base._func, pVector[pVector.Count - 1] + cVector[cVector.Count - 1]);

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

