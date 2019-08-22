using System;
using System.Collections.Generic;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Simulated Annealing
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Randomized algorithm for optimization.
    ///     ''' 
    ///     ''' Reffrence:
    ///     ''' http://ja.wikipedia.org/wiki/%E7%84%BC%E3%81%8D%E3%81%AA%E3%81%BE%E3%81%97%E6%B3%95
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptSimulatedAnnealing : AbsOptimization
    {
        /// <summary>Max iteration count(Default:20,000)</summary>
        public override int Iteration { get; set; } = 20000;

        /// <summary>Epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.00000001;

        // -------------------------------------------------------------------
        // Coefficient of SA(Simulated Annealing)
        // -------------------------------------------------------------------
        /// <summary>cooling ratio</summary>
        public double CoolingRatio { get; set; } = 0.99965;

        /// <summary>range of neighbor search</summary>
        public double NeighborRange { get; set; } = 0.1;

        /// <summary>start temperature</summary>
        public double Temperature { get; set; } = 1000.0;

        /// <summary>stop temperature</summary>
        public double StopTemperature { get; set; } = 0.00000001;

        private LoPoint m_point = null/* TODO Change to default(_) if this is not a reference type */;
        private LoPoint m_Bestpoint = null/* TODO Change to default(_) if this is not a reference type */;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsOptSimulatedAnnealing(AbsObjectiveFunction ai_func)
        {
            this._func = ai_func;
            this.m_point = new LoPoint(ai_func);
        }

        /// <summary>
        ///         ''' Initialize
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public override void Init()
        {
            try
            {
                this._iteration = 0;
                this.m_point.Clear();

                // init initial position
                if (InitialPosition != null && InitialPosition.Length == _func.Dimension())
                    this.m_point = new LoPoint(this._func, InitialPosition);
                else
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_point = new LoPoint(this._func, array);
                }

                this.m_Bestpoint = this.m_point.Copy();
            }
            catch (Exception ex)
            {
                this._error.SetError(true, Util.clsError.ErrorType.ERR_INIT);
            }
            finally
            {
                System.GC.Collect();
            }
        }

        /// <summary>
        ///         ''' Do optimization
        ///         ''' </summary>
        ///         ''' <param name="iteration"></param>
        ///         ''' <returns></returns>
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

                // neighbor function
                LoPoint temp = Neighbor(this.m_point);

                // transition
                double evalNow = this.m_point.Eval;
                double evalNew = temp.Eval;
                double r1 = 0.0;
                var r2 = base.Random.NextDouble();
                if (evalNew < evalNow)
                    r1 = 1.0;
                else
                {
                    var delta = evalNow - evalNew;
                    r1 = Math.Exp(delta / Temperature);
                }
                if (r1 >= r2)
                    this.m_point = temp;// exchange

                // cooling
                Temperature *= CoolingRatio;
                if (Temperature < StopTemperature)
                    return true;// stop iteration

                // reserve best
                if (this.m_point.Eval < this.m_Bestpoint.Eval)
                    this.m_Bestpoint = this.m_point.Copy();
            }

            return false;
        }

        /// <summary>
        ///         ''' Recent Error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override bool IsRecentError()
        {
            return this._error.IsError();
        }

        /// <summary>
        ///         ''' Result
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override Optimization.LoPoint Result
        {
            get
            {
                return this.m_Bestpoint;
            }
        }

        /// <summary>
        ///         ''' for Debug
        ///         ''' </summary>
        ///         ''' <value></value>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override List<Optimization.LoPoint> Results => 
            new List<LoPoint> {m_Bestpoint};

        /// <summary>
        ///         ''' Neighbor function for local search
        ///         ''' </summary>
        ///         ''' <param name="base"></param>
        ///         ''' <returns></returns>
        private LoPoint Neighbor(LoPoint @base)
        {
            LoPoint temp = new LoPoint(@base);
            for (int i = 0; i <= temp.Count - 1; i++)
            {
                var tempNeighbor = Math.Abs(2.0 * NeighborRange) * base.Random.NextDouble() - NeighborRange;
                temp[i] += tempNeighbor;
            }
            temp.ReEvaluate();

            return temp;
        }
    }
}

