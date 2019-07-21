using System;
using System.Collections.Generic;
using LOMath;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Newton Method
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Use second derivertive.
    ///     '''  -Second order conversion.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' 金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' 
    ///     ''' Memo:
    ///     ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ///     ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ///     ''' </remarks>
    public class clsOptNewtonMethod : AbsOptimization
    {
        /// <summary>Max iteration count(Default:5,000)</summary>
        public override int Iteration { get; set; } = 5000;


        /// <summary>Epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.00000001;

        /// <summary>hessian matrix coefficient(Default:1.0)</summary>
        public double ALPHA { get; set; } = 1.0;

        // vector
        private LoVector m_vect = null/* TODO Change to default(_) if this is not a reference type */;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptNewtonMethod(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;

            this.m_vect = new LoVector(ai_func.NumberOfVariable());
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
                this.m_vect.Clear();
                this._error.Clear();

                // init initial position
                if (InitialPosition != null && InitialPosition.Length == _func.NumberOfVariable())
                    this.m_vect = new LoVector(InitialPosition);
                else
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_vect = new LoVector(array);
                }
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
            LoVector grad = new LoVector(base._func.NumberOfVariable(), LoVector.VectorDirection.Col);
            LoMatrix h = new LoMatrix();
            if (this.Iteration <= _iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - _iteration - 1 : Math.Min(iteration, Iteration - _iteration) - 1;
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                _iteration += 1;

                // Calculate Gradient vector
                grad.RawVector = this._func.Gradient(this.m_vect);

                // Calculate Hessian matrix
                h.RawMatrix = this._func.Hessian(this.m_vect);

                // Update
                this.m_vect = this.m_vect - ALPHA * h.Inverse() * grad; // H^-1 calulate heavy...

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
        public override LoPoint Result
        {
            get
            {
                return new LoPoint(base._func, this.m_vect);
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
        public override List<LoPoint> Results
        {
            get
            {
                List<LoPoint> ret = new List<LoPoint>();
                ret.Add(new LoPoint(base._func, this.m_vect));
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
            return this._error.GetLastErrorInfomation();
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
    }
}

