using System;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Eval
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public class clsEval : IComparable
    {
        private int m_index = 0;
        private double m_eval = 0;

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_index"></param>
        ///         ''' <param name="ai_eval"></param>
        ///         ''' <remarks></remarks>
        public clsEval(int ai_index, double ai_eval)
        {
            this.m_index = ai_index;
            this.m_eval = ai_eval;
        }

        /// <summary>
        ///         ''' Eval
        ///         ''' </summary>
        ///         ''' <param name="ai_index"></param>
        ///         ''' <param name="ai_eval"></param>
        ///         ''' <remarks></remarks>
        public void SetEval(int ai_index, double ai_eval)
        {
            this.m_index = ai_index;
            this.m_eval = ai_eval;
        }

        /// <summary>
        ///         ''' get evaluate value
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double Eval => m_eval;

        /// <summary>
        ///         ''' get index
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public int Index => m_index;

        /// <summary>
        ///         ''' Compare(ICompareble)
        ///         ''' </summary>
        ///         ''' <param name="ai_obj"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks>
        ///         ''' larger Me than obj is -1. smaller Me than obj is 1.
        ///         ''' Equal is return to Zero
        ///         ''' </remarks>
        public int CompareTo(object ai_obj)
        {
            // Nothing check
            if (ai_obj == null)
                return 1;

            // Type check
            if (this.GetType() != ai_obj.GetType())
                throw new ArgumentException("Different type", "obj");

            // Compare
            double mineValue = this.m_eval;
            double compareValue = ((clsEval)ai_obj).Eval;
            if (mineValue == compareValue)
                return 0;
            else if (mineValue < compareValue)
                return -1;
            else
                return 1;
        }
    }
}

