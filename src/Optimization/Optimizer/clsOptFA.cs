using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Firefly Algorithm
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -for Mulitimodal optimization
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]X. S. Yang, “Firefly algorithms for multimodal optimization,” in Proceedings of the 5th International Conference on Stochastic Algorithms: Foundation and Applications (SAGA '09), vol. 5792 of Lecture notes in Computer Science, pp. 169–178, 2009.
    ///     ''' [2]Firefly Algorithm - http://www.mathworks.com/matlabcentral/fileexchange/29693-firefly-algorithm
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptFA : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max iteration count(Default:5,000)</summary>
        public override int Iteration { get; set; } = 5000;

        /// <summary>
        ///         ''' epsilon(Default:1e-8) for Criterion
        ///         ''' </summary>
        public double EPS { get; set; } = 0.000000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        // ----------------------------------------------------------------
        // Peculiar parameter
        // ----------------------------------------------------------------
        /// <summary>
        ///         ''' Population Size(Default:50)
        ///         ''' </summary>
        public int PopulationSize { get; set; } = 50;

        /// <summary>
        ///         ''' Fire Fly
        ///         ''' </summary>
        private List<LoFireFly> m_fireflies = new List<LoFireFly>();

        /// <summary>
        ///         ''' attractiveness base
        ///         ''' </summary>
        public double Beta0 { get; set; } = 1.0;

        /// <summary>
        ///         ''' light absorption coefficient(Default:1.0)
        ///         ''' </summary>
        public double Gamma { get; set; } = 1.0;

        /// <summary>
        ///         ''' randomization parameter
        ///         ''' </summary>
        public double Alpha { get; set; } = 0.2;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptFA(AbsObjectiveFunction ai_func)
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
                // init meber varibles
                this._iteration = 0;
                this.m_fireflies.Clear();
                this._error.Clear();

                // initial position
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_fireflies.Add(new LoFireFly(base._func, array));
                    this.m_fireflies[i].ReEvaluate(); // with update intensity
                }

                // Sort Evaluate
                this.m_fireflies.Sort();

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this.m_fireflies.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this.m_fireflies.Count || this.HigherNPercentIndex >= this.m_fireflies.Count)
                    this.HigherNPercentIndex = this.m_fireflies.Count - 1;
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

                // Sort Evaluate
                this.m_fireflies.Sort();

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_fireflies[0].Eval, this.m_fireflies[this.HigherNPercentIndex].Eval))
                        return true;
                }

                // FireFly - original
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    for (int j = 0; j <= this.PopulationSize - 1; j++)
                    {
                        // Move firefly
                        if (this.m_fireflies[i].Intensity < this.m_fireflies[j].Intensity)
                        {
                            double r = (this.m_fireflies[i] - this.m_fireflies[j]).NormL1();
                            double beta = this.Beta0 * Math.Exp(-this.Gamma * r * r); // attractiveness
                            for (int k = 0; k <= this._func.Dimension() - 1; k++)
                            {
                                double newPos = this.m_fireflies[i][k];
                                newPos += beta * (this.m_fireflies[j][k] - this.m_fireflies[i][k]); // attraction
                                newPos += this.Alpha * (this._rand.NextDouble() - 0.5); // random search
                                this.m_fireflies[i][k] = newPos;
                            }
                            this.m_fireflies[i].ReEvaluate(); // with update intensity
                        }
                    }
                }
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
                // find best index
                int bestIndex = 0;
                var bestEval = this.m_fireflies[0].Eval;
                for (var i = 0; i <= this.m_fireflies.Count - 1; i++)
                {
                    if (this.m_fireflies[i].Eval < bestEval)
                    {
                        bestEval = this.m_fireflies[i].Eval;
                        bestIndex = i;
                    }
                }
                return new LoPoint(this._func, this.m_fireflies[bestIndex].ToArray());
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
                // create fireflies
                return clsOptFA.ToPoints(this._func, this.m_fireflies);
            }
        }


        /// <summary>
        ///         ''' create firefiles for Results()
        ///         ''' </summary>
        ///         ''' <param name="func"></param>
        ///         ''' <param name="fflies"></param>
        ///         ''' <returns></returns>
        private static List<LoPoint> ToPoints(AbsObjectiveFunction func, List<LoFireFly> fflies)
        {
            var ret = new List<LoPoint>();
            for (int i = 0; i <= fflies.Count - 1; i++)
                ret.Add(new LoPoint(func, fflies[i].ToArray()));
            return ret;
        }
    }
}

