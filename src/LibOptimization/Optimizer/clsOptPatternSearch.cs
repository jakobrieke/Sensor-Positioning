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
    public class clsOptPatternSearch : absOptimization
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
        private clsPoint m_base = null/* TODO Change to default(_) if this is not a reference type */;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptPatternSearch(absObjectiveFunction ai_func)
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
                // Init meber varibles
                this.m_error.Clear();
                this.m_iteration = 0;
                this.m_stepLength = this.StepLength;
                this.m_base = null;

                // init position
                if (InitialPosition != null && InitialPosition.Length == m_func.NumberOfVariable())
                    this.m_base = new clsPoint(this.m_func, InitialPosition);
                else
                {
                    var array = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_base = new clsPoint(this.m_func, array);
                }
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
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
                this.m_error.Clear();
                this.m_iteration = 0;
                this.m_stepLength = this.StepLength;
                this.m_base = null;

                if (ai_initPoint.Length != this.m_func.NumberOfVariable())
                {
                    this.m_error.SetError(true, clsError.ErrorType.ERR_INIT, "");
                    return;
                }

                // Initialize
                this.m_base = new clsPoint(base.m_func, ai_initPoint);
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT, "");
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

                // MakeExploratoryMoves
                clsPoint exp = this.MakeExploratoryMoves(this.m_base, this.m_stepLength);

                if (exp.Eval < this.m_base.Eval)
                {
                    // Replace basepoint
                    clsPoint previousBasePoint = this.m_base;
                    this.m_base = exp;

                    // MakePatternMove and MakeExploratoryMoves
                    clsPoint temp = this.MakePatternMove(previousBasePoint, this.m_base);
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
        public clsPoint MakeExploratoryMoves(clsPoint ai_base, double ai_stepLength)
        {
            List<clsPoint> explorePoint = new List<clsPoint>();
            for (int i = 0; i <= this.m_func.NumberOfVariable() - 1; i++)
            {
                clsPoint tempPlus = new clsPoint(ai_base);
                tempPlus[i] += ai_stepLength;
                tempPlus.ReEvaluate();
                explorePoint.Add(tempPlus);

                clsPoint tempMinus = new clsPoint(ai_base);
                tempMinus[i] -= ai_stepLength;
                tempMinus.ReEvaluate();
                explorePoint.Add(tempMinus);
            }
            explorePoint.Sort();

            if (explorePoint[0].Eval < ai_base.Eval)
                return explorePoint[0];
            else
                return new clsPoint(ai_base);
        }

        /// <summary>
        ///         ''' Pattern Move
        ///         ''' </summary>
        ///         ''' <param name="ai_previousBasePoint"></param>
        ///         ''' <param name="ai_base"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private clsPoint MakePatternMove(clsPoint ai_previousBasePoint, clsPoint ai_base)
        {
            clsPoint ret = new clsPoint(ai_base);
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
        public override clsPoint Result
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
        public override List<clsPoint> Results
        {
            get
            {
                var ret = new List<clsPoint>();
                ret.Add(new clsPoint(this.m_base));
                for (int i = 0; i <= this.m_base.Count - 1; i++)
                {
                    clsPoint tempPlus = new clsPoint(this.m_base);
                    tempPlus[i] += this.m_stepLength;
                    ret.Add(tempPlus);
                    clsPoint tempMinus = new clsPoint(this.m_base);
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

