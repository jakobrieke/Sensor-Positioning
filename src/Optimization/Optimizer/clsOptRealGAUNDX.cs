using System;
using System.Collections.Generic;
using LOMath;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Real-coded Genetic Algorithm
    ///     ''' UNDX(Unimodal Normal Distribution Crossover)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Alternation of generation algorithm is MGG or JGG
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]小野功，佐藤浩，小林重信, "単峰性正規分布交叉UNDXを用いた実数値GAによる関数最適化"，人工知能学会誌，Vol. 14，No. 6，pp. 1146-1155 (1999)
    ///     ''' [2]北野 宏明 (編集), 遺伝的アルゴリズム 4, 産業図書出版株式会社, 2000年 初版, p261
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptRealGAUNDX : AbsOptimization
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
        /// <summary>Population Size(Default:n*33)</summary>
        public int PopulationSize { get; set; } = 100;

        /// <summary>Children size( = Number of CrossOver/2 )(Default:n*10)</summary>
        public int ChildrenSize { get; set; } = 100;

        /// <summary>Alpha(Default:0.5)</summary>
        public double ALPHA { get; set; } = 0.5;

        /// <summary>Beta(Default:0.35)</summary>
        public double BETA { get; set; } = 0.35;

        /// <summary>AlternationStrategy(Default:MGG)</summary>
        public EnumAlternatioType AlternationStrategy { get; set; } = EnumAlternatioType.MGG;

        /// <summary>alternation strategy</summary>
        public enum EnumAlternatioType
        {
            MGG,
            JGG
        }

        /// <summary>population</summary>
        private List<LoPoint> m_parents = new List<LoPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' "n" is function dimension.
        ///         ''' </remarks>
        public clsOptRealGAUNDX(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;

            this.PopulationSize = this._func.NumberOfVariable() * 33;
            this.ChildrenSize = this._func.NumberOfVariable() * 10;
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
        public override bool DoIteration(int iteration = 0)
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

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Eval, this.m_parents[this.HigherNPercentIndex].Eval))
                        return true;
                }

                // select parent
                List<int> randIndex = clsUtil.RandomPermutaion(this.m_parents.Count);
                int p1Index = randIndex[0];
                int p2Index = randIndex[1];
                var p1 = this.m_parents[p1Index];
                var p2 = this.m_parents[p2Index];
                var p3 = this.m_parents[randIndex[2]]; // for d2

                // UNDX
                var children = this.UNDX(p1, p2, p3);

                // AlternationStrategy
                if (this.AlternationStrategy == EnumAlternatioType.JGG)
                {
                    // JGG
                    children.Sort();
                    this.m_parents[p1Index] = children[0];
                    this.m_parents[p2Index] = children[1];
                }
                else
                {
                    // MGG
                    children.Add(p1);
                    children.Add(p2);
                    children.Sort();
                    this.m_parents[p1Index] = children[0]; // elite
                    children.RemoveAt(0);
                    var rIndex = this.SelectRoulette(children, true);
                    this.m_parents[p2Index] = children[rIndex];
                }
            }

            return false;
        }

        /// <summary>
        ///         ''' RouletteWheel Selection 
        ///         ''' </summary>
        ///         ''' <param name="ai_chidren"></param>
        ///         ''' <param name="isForMinimize"></param>
        ///         ''' <returns>index</returns>
        ///         ''' <remarks></remarks>
        private int SelectRoulette(List<LoPoint> ai_chidren, bool isForMinimize)
        {
            if (isForMinimize == true)
            {
                double tempSum = 0.0;
                foreach (var c in ai_chidren)
                    tempSum += Math.Abs(c.Eval);
                List<double> tempList = new List<double>(ai_chidren.Count);
                double newTempSum = 0.0;
                for (int i = 0; i <= ai_chidren.Count - 1; i++)
                {
                    var temp = tempSum - ai_chidren[i].Eval;
                    tempList.Add(temp);
                    newTempSum += temp;
                }
                // select
                var r = Random.NextDouble();
                double cumulativeRatio = 0.0;
                for (int i = 0; i <= ai_chidren.Count - 1; i++)
                {
                    cumulativeRatio += tempList[i] / newTempSum;
                    if (cumulativeRatio > r)
                        return i;
                }
            }
            else
            {
                double tempSum = 0.0;
                foreach (var c in ai_chidren)
                    tempSum += c.Eval;
                // select
                var r = Random.NextDouble();
                double cumulativeRatio = 0.0;
                for (int i = 0; i <= ai_chidren.Count - 1; i++)
                {
                    cumulativeRatio += ai_chidren[i].Eval / tempSum;
                    if (cumulativeRatio > r)
                        return i;
                }
            }

            return 0;
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

        /// <summary>
        ///         ''' Calc Triangle Area using Heron formula
        ///         ''' </summary>
        ///         ''' <param name="lengthA"></param>
        ///         ''' <param name="lengthB"></param>
        ///         ''' <param name="lengthC"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private double CalcTriangleArea(double lengthA, double lengthB, double lengthC)
        {
            // p3
            // /| 
            // / |
            // ----
            // p1  p2
            var s = (lengthA + lengthB + lengthC) / 2.0;
            var area = Math.Sqrt(s * (s - lengthA) * (s - lengthB) * (s - lengthC));
            return area;
        }

        /// <summary>
        ///         ''' UNDX CrossOver
        ///         ''' </summary>
        ///         ''' <param name="p1"></param>
        ///         ''' <param name="p2"></param>
        ///         ''' <param name="p3"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private List<LoPoint> UNDX(LoPoint p1, LoPoint p2, LoPoint p3)
        {
            // calc d
            var diffVectorP2P1 = p1 - p2;
            var length = diffVectorP2P1.NormL2();
            double areaTriangle = CalcTriangleArea(length, (p3 - p2).NormL2(), (p3 - p1).NormL2());
            var d2 = 2.0 * areaTriangle / length; // S=1/2 * h * a -> h = 2.0 * S / a

            // UNDX
            List<LoPoint> children = new List<LoPoint>(ChildrenSize);
            var g = (p1 + p2) / 2.0;
            var sd1 = Math.Pow((ALPHA * length), 2);
            var sd2 = Math.Pow((BETA * d2 / Math.Sqrt(ObjectiveFunction.NumberOfVariable())), 2);
            var e = diffVectorP2P1 / (double)length;
            var t = new LoVector(ObjectiveFunction.NumberOfVariable());
            for (int genChild = 0; genChild <= System.Convert.ToInt32(ChildrenSize / (double)2 - 1); genChild++)
            {
                for (int i = 0; i <= ObjectiveFunction.NumberOfVariable() - 1; i++)
                    t[i] = clsUtil.NormRand(0, sd2);
                t = t - (t.InnerProduct(e)) * e;

                // child
                double[] child1 = new double[ObjectiveFunction.NumberOfVariable() - 1 + 1];
                double[] child2 = new double[ObjectiveFunction.NumberOfVariable() - 1 + 1];
                var ndRand = clsUtil.NormRand(0, sd1);
                for (int i = 0; i <= ObjectiveFunction.NumberOfVariable() - 1; i++)
                {
                    var temp = t[i] + ndRand * e[i];
                    child1[i] = g[i] + temp;
                    child2[i] = g[i] - temp;
                }

                // overflow check
                var temp1 = new LoPoint(ObjectiveFunction, child1);
                if (clsUtil.CheckOverflow(temp1) == true)
                {
                    for (int i = 0; i <= ObjectiveFunction.NumberOfVariable() - 1; i++)
                        // temp1(i) = Util.Util.NormRand(g(i), 0.1)
                        temp1[i] = clsUtil.GenRandomRange(
                            this.InitialValueRangeLower, 
                            this.InitialValueRangeUpper);
                    temp1.ReEvaluate();
                }
                var temp2 = new LoPoint(ObjectiveFunction, child2);
                if (clsUtil.CheckOverflow(temp2) == true)
                {
                    for (int i = 0; i <= ObjectiveFunction.NumberOfVariable() - 1; i++)
                        temp2[i] = clsUtil.GenRandomRange(
                            this.InitialValueRangeLower, 
                            this.InitialValueRangeUpper);
                    temp2.ReEvaluate();
                }

                children.Add(temp1);
                children.Add(temp2);
            }
            return children;
        }
    }
}

