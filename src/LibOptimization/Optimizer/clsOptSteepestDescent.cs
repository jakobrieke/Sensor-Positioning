using System;
using System.Collections.Generic;
using MathUtil;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Steepest descent method
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Use first derivertive.
    ///     '''  -First order conversion.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]http://dsl4.eee.u-ryukyu.ac.jp/DOCS/nlp/node4.html
    ///     ''' [2]金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' 
    ///     ''' Memo:
    ///     ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ///     ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ///     ''' </remarks>
    public class clsOptSteepestDescent : absOptimization
    {
        /// <summary>Max iteration count</summary>
        public override int Iteration { get; set; } = 10000;

        /// <summary>Epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.00000001;

        // -------------------------------------------------------------------
        // Coefficient of SteepestDescent
        // -------------------------------------------------------------------
        /// <summary>rate</summary>
        public double ALPHA { get; set; } = 0.3;

        // vector
        private clsEasyVector m_vect = null/* TODO Change to default(_) if this is not a reference type */;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptSteepestDescent(absObjectiveFunction ai_func)
        {
            this.m_func = ai_func;

            this.m_vect = new clsEasyVector(ai_func.NumberOfVariable());
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
                this.m_vect.Clear();
                this.m_error.Clear();

                // init initial position
                if (InitialPosition != null && InitialPosition.Length == m_func.NumberOfVariable())
                    this.m_vect = new clsEasyVector(InitialPosition);
                else
                {
                    var array = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_vect = new clsEasyVector(array);
                }
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
            clsEasyVector grad = new clsEasyVector(base.m_func.NumberOfVariable());
            if (this.Iteration <= m_iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - m_iteration - 1 : Math.Min(iteration, Iteration - m_iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                m_iteration += 1;

                // Calculate Gradient vector
                grad.RawVector = this.m_func.Gradient(this.m_vect);

                // Update
                this.m_vect = this.m_vect - this.ALPHA * grad;

                // Check conversion
                if (grad.NormL1() < EPS)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///         ''' Result
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override clsPoint Result
        {
            get
            {
                return new clsPoint(base.m_func, this.m_vect);
            }
        }

        /// <summary>
        ///         ''' Result for debug.(not implementation)
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
                List<clsPoint> ret = new List<clsPoint>();
                ret.Add(new clsPoint(base.m_func, this.m_vect));
                return ret;
            }
        }

        /// <summary>
        ///         ''' Get recent error infomation
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public clsError.clsErrorInfomation GetLastErrorInfomation()
        {
            return this.m_error.GetLastErrorInfomation();
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
    }
}

