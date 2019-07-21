using System;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Particle class for PSO
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' for Swarm Particle Optimization
    ///     ''' </remarks>
    public class clsParticle : IComparable
    {
        private LoPoint m_point;
        private LoPoint m_bestPoint;
        private double[] m_velocity;

        /// <summary>
        ///         ''' Default construtor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        private clsParticle()
        {
        }

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_point"></param>
        ///         ''' <param name="ai_velocity"></param>
        ///         ''' <param name="ai_bestPoint"></param>
        ///         ''' <remarks></remarks>
        public clsParticle(LoPoint ai_point, double[] ai_velocity, LoPoint ai_bestPoint)
        {
            this.m_point = ai_point;
            this.m_velocity = ai_velocity;
            this.m_bestPoint = ai_bestPoint;
        }

        /// <summary>
        ///         ''' Copy Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_particle"></param>
        ///         ''' <remarks></remarks>
        public clsParticle(clsParticle ai_particle)
        {
            this.m_point = ai_particle.Point.Copy();
            
            m_velocity = new double[ai_particle.Velocity.Length];
            Array.Copy(ai_particle.Velocity, 
                m_velocity, ai_particle.Velocity.Length);
//            this.m_velocity = ai_particle.Velocity;

            this.m_bestPoint = ai_particle.BestPoint.Copy();
        }

        /// <summary>
        ///         ''' Point
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public LoPoint Point
        {
            get
            {
                return this.m_point;
            }
            set
            {
                this.m_point = value;
            }
        }

        /// <summary>
        ///         ''' Velocity
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public double[] Velocity
        {
            get
            {
                return this.m_velocity;
            }
            set
            {
                this.m_velocity = value;
            }
        }

        /// <summary>
        ///         ''' BestPoint
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public LoPoint BestPoint
        {
            get
            {
                return this.m_bestPoint;
            }
            set
            {
                this.m_bestPoint = value;
            }
        }

        /// <summary>
        ///         ''' for sort
        ///         ''' </summary>
        ///         ''' <param name="ai_obj"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public int CompareTo(object ai_obj)
        {
            // Nothing check
            if (ai_obj == null)
                return 1;

            // Type check
            if (this.GetType() != ai_obj.GetType())
                throw new ArgumentException("Different type", "obj");

            // Compare
            double mineValue = this.m_bestPoint.Eval;
            double compareValue = ((clsParticle)ai_obj).BestPoint.Eval;
            if (mineValue < compareValue)
                return -1;
            else if (mineValue > compareValue)
                return 1;
            else
                return 0;
        }
    }
}

