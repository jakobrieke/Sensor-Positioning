using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Hooke-Jeeves Pattern Search Method
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     ''' 
    ///     ''' Reffrence:
    ///     ''' Hooke, R. and Jeeves, T.A., ""Direct search" solution of numerical and statistical problems", Journal of the Association for Computing Machinery (ACM) 8 (2), pp212–229.
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptPatternSearch : AbsOptimization
    {
        /// <summary>Max iteration count(Default:20,000)</summary>
        public override int Iteration { get; set; } = 20000;

        /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
        public double EPS { get; set; } = 0.000001;

        // -------------------------------------------------------------------
        // Coefficient of pattern search
        // -------------------------------------------------------------------
        /// <summary>step length(Default:0.6)</summary>
        private readonly double StepLength = 0.6;

        /// <summary>shrink parameter(Default:2.0)</summary>
        private readonly double Shrink = 2.0;

        /// <summary>current step length</summary>
        private double m_stepLength = 0.6;

        /// <summary>current base</summary>
        private LoPoint m_base = null/* TODO Change to default(_) if this is not a reference type */;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptPatternSearch(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public override void Init()
        {
            try
            {
                // Init meber varibles
                this._error.Clear();
                this._iteration = 0;
                this.m_stepLength = this.StepLength;
                this.m_base = null;

                // init position
                if (InitialPosition != null && InitialPosition.Length == _func.Dimension())
                    this.m_base = new LoPoint(this._func, InitialPosition);
                else
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_base = new LoPoint(this._func, array);
                }
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
            finally
            {
                System.GC.Collect();
            }
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public new void Init(double[] ai_initPoint)
        {
            try
            {
                // Init meber varibles
                this._error.Clear();
                this._iteration = 0;
                this.m_stepLength = this.StepLength;
                this.m_base = null;

                if (ai_initPoint.Length != this._func.Dimension())
                {
                    this._error.SetError(true, clsError.ErrorType.ERR_INIT, "");
                    return;
                }

                // Initialize
                this.m_base = new LoPoint(base._func, ai_initPoint);
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT, "");
            }
            finally
            {
                System.GC.Collect();
            }
        }

        /// <summary>
        ///         ''' Do optimization
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

                // MakeExploratoryMoves
                LoPoint exp = this.MakeExploratoryMoves(this.m_base, this.m_stepLength);

                if (exp.Eval < this.m_base.Eval)
                {
                    // Replace basepoint
                    LoPoint previousBasePoint = this.m_base;
                    this.m_base = exp;

                    // MakePatternMove and MakeExploratoryMoves
                    LoPoint temp = this.MakePatternMove(previousBasePoint, this.m_base);
                    var expUsingPatternMove = this.MakeExploratoryMoves(temp, this.m_stepLength);
                    if (expUsingPatternMove.Eval < this.m_base.Eval)
                        this.m_base = expUsingPatternMove;
                }
                else
                {
                    // Check conversion
                    if (this.m_stepLength < EPS)
                        return true;

                    // Shrink Step
                    this.m_stepLength /= this.Shrink;
                }
            }

            return false;
        }

        /// <summary>
        ///         ''' Exploratory Move
        ///         ''' </summary>
        ///         ''' <param name="ai_base">Base point</param>
        ///         ''' <param name="ai_stepLength">Step</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public LoPoint MakeExploratoryMoves(LoPoint ai_base, double ai_stepLength)
        {
            List<LoPoint> explorePoint = new List<LoPoint>();
            for (int i = 0; i <= this._func.Dimension() - 1; i++)
            {
                LoPoint tempPlus = new LoPoint(ai_base);
                tempPlus[i] += ai_stepLength;
                tempPlus.ReEvaluate();
                explorePoint.Add(tempPlus);

                LoPoint tempMinus = new LoPoint(ai_base);
                tempMinus[i] -= ai_stepLength;
                tempMinus.ReEvaluate();
                explorePoint.Add(tempMinus);
            }
            explorePoint.Sort();

            if (explorePoint[0].Eval < ai_base.Eval)
                return explorePoint[0];
            else
                return new LoPoint(ai_base);
        }

        /// <summary>
        ///         ''' Pattern Move
        ///         ''' </summary>
        ///         ''' <param name="ai_previousBasePoint"></param>
        ///         ''' <param name="ai_base"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private LoPoint MakePatternMove(LoPoint ai_previousBasePoint, LoPoint ai_base)
        {
            LoPoint ret = new LoPoint(ai_base);
            for (int i = 0; i <= ai_base.Count - 1; i++)
                ret[i] = 2.0 * ai_base[i] - ai_previousBasePoint[i];
            ret.ReEvaluate();

            return ret;
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
                return this.m_base.Copy();
            }
        }

        /// <summary>
        ///         ''' Base with length
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
                var ret = new List<LoPoint>();
                ret.Add(new LoPoint(this.m_base));
                for (int i = 0; i <= this.m_base.Count - 1; i++)
                {
                    LoPoint tempPlus = new LoPoint(this.m_base);
                    tempPlus[i] += this.m_stepLength;
                    ret.Add(tempPlus);
                    LoPoint tempMinus = new LoPoint(this.m_base);
                    tempMinus[i] -= this.m_stepLength;
                    ret.Add(tempMinus);
                }
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

