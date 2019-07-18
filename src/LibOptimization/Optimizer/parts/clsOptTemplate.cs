using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' optimize template
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public class clsOptTemplate : absOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
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

        /// <summary>Max Iteration(Default:10,000)</summary>
        public override int Iteration { get; set; } = 10000;

        // ----------------------------------------------------------------
        // Peculiar parameter
        // ----------------------------------------------------------------
        /// <summary>
        ///         ''' Population Size(Default:100)
        ///         ''' </summary>
        public int PopulationSize { get; set; } = 100;

        // population
        private List<clsPoint> m_populations = new List<clsPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Objective Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptTemplate(absObjectiveFunction ai_func)
        {
            m_func = ai_func;
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public override void Init()
        {
            try
            {
                // init varibles
                m_iteration = 0;
                m_populations.Clear();
                m_error.Clear();

                // initial position
                for (int i = 0; i <= PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(m_func, InitialPosition, InitialValueRangeLower, InitialValueRangeUpper);
                    m_populations.Add(new clsPoint(m_func, array));
                }

                // Sort Evaluate
                m_populations.Sort();

                // Detect HigherNPercentIndex
                HigherNPercentIndex = System.Convert.ToInt32(m_populations.Count * HigherNPercent);
                if (HigherNPercentIndex == m_populations.Count || HigherNPercentIndex >= m_populations.Count)
                    HigherNPercentIndex = m_populations.Count - 1;
            }
            catch (Exception ex)
            {
                m_error.SetError(true, clsError.ErrorType.ERR_INIT);
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
            if (IsRecentError() == true)
                return true;

            // Do Iterate
            if (Iteration <= m_iteration)
                return true;
            else
                iteration = iteration == 0 ? Iteration - m_iteration - 1 : Math.Min(iteration, Iteration - m_iteration) - 1;
            
            for (int iterate = 0; iterate <= iteration; iterate++)
            {
                // Counting generation
                m_iteration += 1;

                // Sort Evaluate
                m_populations.Sort();

                // check criterion
                if (IsUseCriterion)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(EPS, m_populations[0].Eval, m_populations[HigherNPercentIndex].Eval))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        ///         ''' Best result
        ///         ''' </summary>
        ///         ''' <returns>Best point class</returns>
        ///         ''' <remarks></remarks>
        public override clsPoint Result => m_populations[0];

        /// <summary>
        ///         ''' Get recent error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return m_error.IsError();
        }

        /// <summary>
        ///         ''' All Result
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' for Debug
        ///         ''' </remarks>
        public override List<clsPoint> Results => m_populations;
    }
}

