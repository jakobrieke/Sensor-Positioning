using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Nelder Mead Method
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Also known as "Down hill simplex" or "simplex method".
    ///     '''  -Implementation according to the original paper.
    ///     ''' 
    ///     ''' Reffrence:
    ///     ''' J.A.Nelder and R.Mead, "A Simplex Method for Function Minimization", The Computer Journal vol.7, 308–313 (1965)
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptNelderMead : AbsOptimization
    {
        /// <summary>Max iteration count(Default:5,000)</summary>
        public override int MaxIterations { get; set; } = 5000;

        /// <summary>Epsilon(Default:0.000001) for Criterion</summary>
        public double EPS { get; set; } = 0.000001;

        // -------------------------------------------------------------------
        // Coefficient of Simplex Operator
        // -------------------------------------------------------------------
        /// <summary>Refrection coeffcient(default:1.0)</summary>
        public readonly double Refrection = 1.0;

        /// <summary>Expantion coeffcient(default:2.0)</summary>
        public readonly double Expantion = 2.0;

        /// <summary>Contraction coeffcient(default:0.5)</summary>
        public readonly double Contraction = 0.5;

        /// <summary>Shrink coeffcient(default:2.0)</summary>
        public readonly double Shrink = 2.0;

        private List<LoPoint> m_points = new List<LoPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks></remarks>
        public clsOptNelderMead(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
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
                this._error.Clear();
                this._iteration = 0;
                this.m_points.Clear();

                // initial position
                double[][] tempSimplex = null;
                tempSimplex = new double[base._func.Dimension() + 1][];
                for (int i = 0; i <= tempSimplex.Length - 1; i++)
                    tempSimplex[i] = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);

                this.Init(tempSimplex);
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT, "");
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
                this._error.Clear();
                this._iteration = 0;
                this.m_points.Clear();

                // Check number of variable
                if (this._func.Dimension() < 2)
                {
                    this._error.SetError(true, clsError.ErrorType.ERR_INIT);
                    return;
                }

                // Check Simplex
                // MEMO:Target function variable is the same as vertex of simplex.
                if (ai_initPoint.Length != (base._func.Dimension() + 1))
                {
                    this._error.SetError(true, clsError.ErrorType.ERR_INIT);
                    return;
                }

                // Generate vertex
                for (int i = 0; i <= _func.Dimension(); i++)
                    this.m_points.Add(new LoPoint(base._func, new List<double>(ai_initPoint[i])));

                // Sort Evaluate
                this.m_points.Sort();
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

        /// <summary>
        ///         ''' Do optimization
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

                // Check criterion
                this.m_points.Sort();
                if (clsUtil.IsCriterion(this.EPS, this.m_points[0].Value, this.m_points[this.m_points.Count - 1].Value))
                    return true;

                // -----------------------------------------------------
                // The following is optimization by Nelder-Mead Method.
                // -----------------------------------------------------
                // Calc centroid
                LoPoint centroid = this.GetCentroid(this.m_points);

                // 1st Refrection
                LoPoint refrection = this.CalcRefrection(WorstPoint, centroid, this.Refrection);

                // Simplex Operators - Refrection, Expantion, Constratction, (Shrink)
                if (refrection.Value < BestPoint.Value)
                {
                    LoPoint expantion = this.CalcExpantion(refrection, centroid, this.Expantion); // Fig. 1 Flow diagram is constratction??
                    if (expantion.Value < BestPoint.Value)
                        WorstPoint = expantion;
                    else
                        WorstPoint = refrection;
                }
                else if (refrection.Value > Worst2ndPoint.Value)
                {
                    if (refrection.Value > WorstPoint.Value)
                    {
                    }
                    else
                        WorstPoint = refrection;
                    // Contraction
                    LoPoint contraction = this.CalcContraction(WorstPoint, centroid, this.Contraction);
                    if (contraction.Value > WorstPoint.Value)
                        WorstPoint = contraction;
                    else
                        // Shrink
                        this.CalcShrink(this.Shrink);
                }
                else
                    WorstPoint = refrection;
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
                return this.m_points;
            }
        }

        /// <summary>
        ///         ''' Calc Centroid
        ///         ''' </summary>
        ///         ''' <param name="ai_vertexs"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private LoPoint GetCentroid(List<LoPoint> ai_vertexs)
        {
            LoPoint ret = new LoPoint(ai_vertexs[0]);

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
        ///         ''' Refrection
        ///         ''' </summary>
        ///         ''' <param name="ai_tgt">Target vertex</param>
        ///         ''' <param name="ai_base">Base vertex</param>
        ///         ''' <param name="ai_coeff">Expantion coeffcient. Recommned value 1.0</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' xr = (1 + alpha)¯x - p
        ///         ''' </remarks>
        private LoPoint CalcRefrection(LoPoint ai_tgt, LoPoint ai_base, double ai_coeff = 1.0)
        {
            LoPoint ret = new LoPoint(base._func);

            int numVar = ai_base.Count;
            for (int i = 0; i <= numVar - 1; i++)
            {
                double temp = -ai_coeff * ai_tgt[i] + (1 + ai_coeff) * ai_base[i];
                ret[i] = temp;
            }

            ret.ReEvaluate();
            return ret;
        }

        /// <summary>
        ///         ''' Expantion
        ///         ''' </summary>
        ///         ''' <param name="ai_tgt">Target vertex</param>
        ///         ''' <param name="ai_base">Base vertex</param>
        ///         ''' <param name="ai_coeff">Expantion coeffcient. Recommned value 2.0</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' xe = gamma * p + (1 - gamma)¯x
        ///         ''' </remarks>
        private LoPoint CalcExpantion(LoPoint ai_tgt, LoPoint ai_base, double ai_coeff = 2.0)
        {
            LoPoint ret = new LoPoint(base._func);

            int numVar = ai_base.Count;
            for (int i = 0; i <= numVar - 1; i++)
            {
                double temp = ai_coeff * ai_tgt[i] + (1 - ai_coeff) * ai_base[i];
                ret[i] = temp;
            }

            ret.ReEvaluate();
            return ret;
        }

        /// <summary>
        ///         ''' Contraction
        ///         ''' </summary>
        ///         ''' <param name="ai_tgt">Target vertex</param>
        ///         ''' <param name="ai_base">Base vertex</param>
        ///         ''' <param name="ai_coeff">Constraction coeffcient. Recommned value 0.5</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' xc = beta * p + (1 - beta)¯x
        ///         ''' </remarks>
        private LoPoint CalcContraction(LoPoint ai_tgt, LoPoint ai_base, double ai_coeff = 0.5)
        {
            LoPoint ret = new LoPoint(base._func);

            int numVar = ai_base.Count;
            for (int i = 0; i <= numVar - 1; i++)
            {
                double temp = -ai_coeff * ai_tgt[i] + (1 + ai_coeff) * ai_base[i];
                ret[i] = temp;
            }

            ret.ReEvaluate();
            return ret;
        }

        /// <summary>
        ///         ''' Shrink(All point replace)
        ///         ''' </summary>
        ///         ''' <param name="ai_coeff">Shrink coeffcient.</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        private void CalcShrink(double ai_coeff = 2.0)
        {
            int numVar = this.m_points[0].Count;

            LoPoint tempBestPoint = new LoPoint(BestPoint);
            for (int i = 0; i <= m_points.Count - 1; i++)
            {
                for (int j = 0; j <= numVar - 1; j++)
                {
                    double temp = (tempBestPoint[j] + m_points[i][j]) / ai_coeff;
                    m_points[i][j] = temp;
                }
                m_points[i].ReEvaluate();
            }
        }

        private LoPoint BestPoint
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

        private LoPoint WorstPoint
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

        private LoPoint Worst2ndPoint
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

