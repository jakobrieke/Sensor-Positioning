using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Nelder Mead Method wikipedia ver
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Also known as "Down hill simplex" or "simplex method".
    ///     ''' 
    ///     ''' Reffrence:
    ///     ''' http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptNelderMeadWiki : absOptimization
    {
        /// <summary>Max iteration count(Default:5,000)</summary>
        public override int Iteration { get; set; } = 5000;

        /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
        public double EPS { get; set; } = 0.000001;

        // -------------------------------------------------------------------
        // Coefficient of Simplex Operator
        // -------------------------------------------------------------------
        /// <summary>Refrection coeffcient(default:1.0)</summary>
        public readonly double Refrection = 1.0;

        /// <summary>Expantion coeffcient(default:2.0)</summary>
        public readonly double Expantion = 2.0;

        /// <summary>Contraction coeffcient(default:-0.5)</summary>
        public readonly double Contraction = -0.5;

        /// <summary>Shrink coeffcient(default:0.5)</summary>
        public readonly double Shrink = 0.5;

        private List<clsPoint> m_points = new List<clsPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptNelderMeadWiki(absObjectiveFunction ai_func)
        {
            this.m_func = ai_func;
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' All vertexs are made at random.
        ///         ''' </remarks>
        public override void Init()
        {
            try
            {
                // init meber varibles
                this.m_error.Clear();
                this.m_iteration = 0;
                this.m_points.Clear();

                // initial position
                double[][] tempSimplex = null;
                tempSimplex = new double[base.m_func.NumberOfVariable() + 1][];
                for (int i = 0; i <= tempSimplex.Length - 1; i++)
                    tempSimplex[i] = clsUtil.GenRandomPosition(this.m_func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);

                this.Init(tempSimplex);
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT, "");
            }
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <param name="ai_initPoint"></param>
        ///         ''' <remarks>
        ///         ''' Set simplex
        ///         ''' </remarks>
        public new void Init(double[][] ai_initPoint)
        {
            try
            {
                // init meber varibles
                this.m_error.Clear();
                this.m_iteration = 0;
                this.m_points.Clear();

                // Check number of variable
                if (this.m_func.NumberOfVariable() < 2)
                {
                    this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
                    return;
                }

                // Check Simplex
                // MEMO:Target function variable is the same as vertex of simplex.
                if (ai_initPoint.Length != (base.m_func.NumberOfVariable() + 1))
                {
                    this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
                    return;
                }

                // Generate vertex
                for (int i = 0; i <= m_func.NumberOfVariable(); i++)
                    this.m_points.Add(new clsPoint(base.m_func, new List<double>(ai_initPoint[i])));

                // Sort Evaluate
                this.m_points.Sort();
            }
            catch (Exception ex)
            {
                this.m_error.SetError(true, clsError.ErrorType.ERR_INIT);
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

                // Check criterion
                this.m_points.Sort();
                if (clsUtil.IsCriterion(this.EPS, this.m_points[0].Eval, this.m_points[this.m_points.Count - 1].Eval))
                    return true;

                // -----------------------------------------------------
                // The following is optimization by Nelder-Mead Method.
                // -----------------------------------------------------
                // Calc centroid
                var centroid = this.GetCentroid(m_points);

                // Reflection
                var refrection = this.ModifySimplex(this.WorstPoint, centroid, this.Refrection);
                if (BestPoint.Eval <= refrection.Eval && refrection.Eval < Worst2ndPoint.Eval)
                    WorstPoint = refrection;
                else if (refrection.Eval < BestPoint.Eval)
                {
                    // Expantion
                    var expantion = this.ModifySimplex(this.WorstPoint, centroid, this.Expantion);
                    if (expantion.Eval < refrection.Eval)
                        WorstPoint = expantion;
                    else
                        WorstPoint = refrection;
                }
                else
                {
                    // Contraction
                    var contraction = this.ModifySimplex(WorstPoint, centroid, this.Contraction);
                    if (contraction.Eval < WorstPoint.Eval)
                        WorstPoint = contraction;
                    else
                        // Reduction(Shrink) BestPoint以外を縮小
                        this.CalcShrink(this.Shrink);
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
                return clsUtil.GetBestPoint(this.m_points, true);
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
                return this.m_points;
            }
        }

        /// <summary>
        ///         ''' Calc Centroid
        ///         ''' </summary>
        ///         ''' <param name="ai_vertexs"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private clsPoint GetCentroid(List<clsPoint> ai_vertexs)
        {
            clsPoint ret = new clsPoint(ai_vertexs[0]);

            int numVar = ai_vertexs[0].Count;
            for (int i = 0; i <= numVar - 1; i++)
            {
                double temp = 0.0;
                for (int numVertex = 0; numVertex <= ai_vertexs.Count - 2; numVertex++) // Except Worst
                    temp += ai_vertexs[numVertex][i];
                ret[i] = temp / (ai_vertexs.Count - 1);
            }

            ret.ReEvaluate();
            return ret;
        }

        /// <summary>
        ///         ''' Simplex
        ///         ''' </summary>
        ///         ''' <param name="ai_tgt">Target vertex</param>
        ///         ''' <param name="ai_base">Base vertex</param>
        ///         ''' <param name="ai_coeff">Coeffcient</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' </remarks>
        private clsPoint ModifySimplex(clsPoint ai_tgt, clsPoint ai_base, double ai_coeff)
        {
            clsPoint ret = new clsPoint(this.m_func);
            for (int i = 0; i <= ret.Count - 1; i++)
            {
                double temp = ai_base[i] + ai_coeff * (ai_base[i] - ai_tgt[i]);
                ret[i] = temp;
            }
            ret.ReEvaluate();
            return ret;
        }

        /// <summary>
        ///         ''' Shrink(Except best point)
        ///         ''' </summary>
        ///         ''' <param name="ai_coeff">Shrink coeffcient</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        private void CalcShrink(double ai_coeff)
        {
            for (int i = 1; i <= m_points.Count - 1; i++) // expect BestPoint
            {
                for (int j = 0; j <= this.m_points[0].Count - 1; j++)
                {
                    var temp = BestPoint[j] +
                               ai_coeff * (m_points[i][j] - BestPoint[j]);
                    m_points[i][j] = temp;
                }
                m_points[i].ReEvaluate();
            }
        }

        private clsPoint BestPoint
        {
            get
            {
                return this.m_points[0];
            }
            set
            {
                this.m_points[0] = value;
            }
        }

        private clsPoint WorstPoint
        {
            get
            {
                return this.m_points[this.m_points.Count - 1];
            }
            set
            {
                this.m_points[this.m_points.Count - 1] = value;
            }
        }

        private clsPoint Worst2ndPoint
        {
            get
            {
                return this.m_points[this.m_points.Count - 2];
            }
            set
            {
                this.m_points[this.m_points.Count - 2] = value;
            }
        }
    }
}

