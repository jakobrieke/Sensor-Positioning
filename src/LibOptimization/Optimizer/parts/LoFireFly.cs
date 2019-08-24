using System;
using System.Collections.Generic;
using LoMath;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Firefly class
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public class LoFireFly : LoVector, IComparable
    {
        private AbsObjectiveFunction m_func = null/* TODO Change to default(_) if this is not a reference type */;
        private double m_evaluateValue = 0.0;

        public double Intensity { get; set; } = 0.0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        private LoFireFly()
        {
        }

        /// <summary>
        ///         ''' copy constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_vertex"></param>
        ///         ''' <remarks></remarks>
        public LoFireFly(LoFireFly ai_vertex)
        {
            this.m_func = ai_vertex.GetFunc();
            this.AddRange(ai_vertex); // ok
            this.m_evaluateValue = ai_vertex.Eval;
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

        /// <summary>
        ///         ''' constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func"></param>
        ///         ''' <remarks></remarks>
        public LoFireFly(AbsObjectiveFunction ai_func)
        {
            this.m_func = ai_func;
            this.AddRange(new double[ai_func.Dimension() - 1 + 1]); // ok
            this.m_evaluateValue = this.m_func.F(this);
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

        /// <summary>
        ///         ''' constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func"></param>
        ///         ''' <param name="ai_vars"></param>
        ///         ''' <remarks></remarks>
        public LoFireFly(AbsObjectiveFunction ai_func, List<double> ai_vars)
        {
            this.m_func = ai_func;
            this.AddRange(ai_vars); // ok
            this.m_evaluateValue = this.m_func.F(this);
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

        /// <summary>
        ///         ''' constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func"></param>
        ///         ''' <param name="ai_vars"></param>
        ///         ''' <remarks></remarks>
        public LoFireFly(AbsObjectiveFunction ai_func, double[] ai_vars)
        {
            this.m_func = ai_func;
            this.AddRange(ai_vars); // ok
            this.m_evaluateValue = this.m_func.F(this);
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

        /// <summary>
        ///         ''' constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func"></param>
        ///         ''' <param name="ai_dim"></param>
        ///         ''' <remarks></remarks>
        public LoFireFly(AbsObjectiveFunction ai_func, int ai_dim)
        {
            this.m_func = ai_func;
            this.AddRange(new double[ai_dim - 1 + 1]); // ok
            this.m_evaluateValue = this.m_func.F(this);
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

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
            double mineValue = this.Eval;
            double compareValue = ((LoFireFly)ai_obj).Eval;
            if (mineValue < compareValue)
                return -1;
            else if (mineValue > compareValue)
                return 1;
            else
                return 0;
        }

        /// <summary>
        ///         ''' Re Evaluate
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public void ReEvaluate()
        {
            this.m_evaluateValue = this.m_func.F(this);
            this.Intensity = 1 / (this.m_evaluateValue + 0.0001);
        }

        /// <summary>
        ///         ''' Get Function
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public AbsObjectiveFunction GetFunc()
        {
            return this.m_func;
        }

        /// <summary>
        ///         ''' EvaluateValue
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double Eval
        {
            get
            {
                return this.m_evaluateValue;
            }
        }

        /// <summary>
        ///         ''' Init
        ///         ''' </summary>
        ///         ''' <param name="ai_range">-ai_range to ai_range</param>
        ///         ''' <param name="ai_rand">Random object</param>
        ///         ''' <remarks></remarks>
        public void InitValue(double ai_range, System.Random ai_rand)
        {
            for (int i = 0; i <= this.m_func.Dimension() - 1; i++)
                this.Add(Math.Abs(2.0 * ai_range) * ai_rand.NextDouble() - ai_range);
        }

        /// <summary>
        ///         ''' Copy
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public LoFireFly Copy()
        {
            return new LoFireFly(this);
        }

        /// <summary>
        ///         ''' Copy
        ///         ''' </summary>
        ///         ''' <param name="ai_point"></param>
        ///         ''' <remarks></remarks>
        public void Copy(LoFireFly ai_point)
        {
            for (int i = 0; i <= ai_point.Count - 1; i++)
                this[i] = ai_point[i];
            this.m_evaluateValue = ai_point.Eval;
            this.Intensity = ai_point.Intensity;
        }
    }
}

