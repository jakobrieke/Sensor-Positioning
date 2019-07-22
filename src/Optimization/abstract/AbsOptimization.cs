using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    /// Abstract optimization Class
    /// </summary>
    /// <remarks></remarks>
    public abstract class AbsOptimization
    {
        /* TODO Change to default(_) if this is not a reference type */
        /// <summary>Objective function</summary>
        protected AbsObjectiveFunction _func;

        /// <summary>Iteration count</summary>
        protected int _iteration;

        /// <summary>Random object</summary>
        protected Random _rand = new XorShiftPRNG(
            BitConverter.ToUInt32(
                BitConverter.GetBytes(Environment.TickCount), 
                0));

        /// <summary>Error manage class</summary>
        protected readonly clsError _error = new clsError();

        /// <summary>Initial position</summary>
        public double[] InitialPosition;

        /// <summary>Upper range of initial value</summary>
        /// <remarks>This parameters to use when generate a variable</remarks>
        public double InitialValueRangeUpper { get; set; } = 5;

        /// <summary>Lower range of initial value</summary>
        /// <remarks>This parameters to use when generate a variable</remarks>
        public double InitialValueRangeLower { get; set; } = -5;

        /// <summary>Use criterion</summary>
        public bool IsUseCriterion { get; set; } = true;

        /// <summary>
        /// Objective function Property
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public AbsObjectiveFunction ObjectiveFunction
        {
            get => _func;
            set => _func = value;
        }

        /// <summary>
        /// Random object Property
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Random Random
        {
            get => _rand;
            set => _rand = value;
        }

        /// <summary>
        /// Initialize parameter
        /// </summary>
        /// <remarks></remarks>
        public abstract void Init();

        /// <summary>
        /// Do Iteration
        /// </summary>
        /// <param name="iteration">
        /// Iteration count. When you set zero, use the default value.
        /// </param>
        /// <returns>
        /// True:Stopping Criterion.
        /// False:Do not Stopping Criterion
        /// </returns>
        /// <remarks></remarks>
        public abstract bool Iterate(int iteration = 0);

        /// <summary>
        /// Result
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract LoPoint Result { get; }

        /// <summary>
        /// Results
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Get all result.
        /// Do not need to implement this method.
        /// e.g)Throw New NotImplementedException
        /// </remarks>
        public abstract List<LoPoint> Results { get; }

        /// <summary>
        /// Recent Error
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract bool IsRecentError();

        /// <summary>
        /// Max Iteration
        /// </summary>
        /// <returns></returns>
        public abstract int Iteration { get; set; }

        /// <summary>
        /// Iteration count 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int IterationCount => _iteration;

        /// <summary>
        /// Reset Iteration count
        /// </summary>
        /// <remarks></remarks>
        public void ResetIterationCount()
        {
            _iteration = 0;
        }
    }
}

