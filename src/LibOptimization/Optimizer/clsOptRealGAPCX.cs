using System;
using System.Collections.Generic;
using LoMath;
using Util;

namespace LibOptimization.Optimization
{
    /// <summary>
    ///     ''' Real-coded Genetic Algorithm
    ///     ''' Parent Centric Recombination(PCX)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Features:
    ///     '''  -Derivative free optimization algorithm.
    ///     '''  -Cross over algorithm is PCX.
    ///     '''  -Alternation of generation algorithm is G3.
    ///     ''' 
    ///     ''' Refference:
    ///     ''' [1]Kalyanmoy Deb, Dhiraj Joshi and Ashish Anand, "Real-Coded Evolutionary Algorithms with Parent-Centric Recombination", KanGAL Report No. 2001003
    ///     ''' 
    ///     ''' Implment:
    ///     ''' N.Tomi(tomi.nori+github at gmail.com)
    ///     ''' </remarks>
    public class clsOptRealGAPCX : AbsOptimization
    {
        // ----------------------------------------------------------------
        // Common parameters
        // ----------------------------------------------------------------
        /// <summary>Max iteration count(Default:10,000)</summary>
        public override int MaxIterations { get; set; } = 10000;

        /// <summary>epsilon(Default:1e-8) for Criterion</summary>
        public double EPS { get; set; } = 0.000000001;

        /// <summary>
        ///         ''' higher N percentage particles are finished at the time of same evaluate value.
        ///         ''' This parameter is valid is when IsUseCriterion is true.
        ///         ''' </summary>
        public double HigherNPercent { get; set; } = 0.8; // for IsCriterion()
        private int HigherNPercentIndex = 0; // for IsCriterion())

        // ----------------------------------------------------------------
        // Coefficient of GA
        // ----------------------------------------------------------------
        /// <summary>Population Size(Default:100)</summary>
        public int PopulationSize { get; set; } = 100;

        /// <summary>Children size(Default:3)</summary>
        public int ChildrenSize { get; set; } = 3;

        /// <summary>Randomize parameter Eta(Default:0.1)</summary>
        public double Eta { get; set; } = 0.1;

        /// <summary>Randomize parameter Zeta(Default:0.1)</summary>
        public double Zeta { get; set; } = 0.1;

        /// <summary>population</summary>
        private List<LoPoint> m_parents = new List<LoPoint>();

        /// <summary>
        ///         ''' Constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_func">Optimize Function</param>
        ///         ''' <remarks>
        ///         ''' </remarks>
        public clsOptRealGAPCX(AbsObjectiveFunction ai_func)
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
                this.m_parents.Clear();

                // initial position
                for (int i = 0; i <= this.PopulationSize - 1; i++)
                {
                    var array = clsUtil.GenRandomPosition(this._func, InitialPosition, this.InitialValueRangeLower, this.InitialValueRangeUpper);
                    this.m_parents.Add(new LoPoint(this._func, array));
                }

                // Sort Evaluate
                this.m_parents.Sort();

                // Detect HigherNPercentIndex
                this.HigherNPercentIndex = System.Convert.ToInt32(this.m_parents.Count * this.HigherNPercent);
                if (this.HigherNPercentIndex == this.m_parents.Count || this.HigherNPercentIndex >= this.m_parents.Count)
                    this.HigherNPercentIndex = this.m_parents.Count - 1;
            }
            catch (Exception ex)
            {
                this._error.SetError(true, clsError.ErrorType.ERR_INIT);
            }
        }

        /// <summary>
        ///         ''' Do Iteration
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

                // Sort Evaluate
                this.m_parents.Sort();

                // check criterion
                if (this.IsUseCriterion == true)
                {
                    // higher N percentage particles are finished at the time of same evaluate value.
                    if (clsUtil.IsCriterion(this.EPS, this.m_parents[0].Value, this.m_parents[this.HigherNPercentIndex].Value))
                        return true;
                }

                // ----------------------------------------------------------------------------
                // Parent Centric Recombination
                // ----------------------------------------------------------------------------
                // Alternative Starategy - G3(Genelized Generation Gap)
                List<int> selectParentsIndex = null;
                List<LoPoint> selectParents = null;
                this.SelectParentsForG3(3, ref selectParentsIndex, ref selectParents);

                // PCX Crossover
                List<LoPoint> newPopulation = this.CrossoverPCX(selectParents, this.ChildrenSize, 3);

                // Replace(by G3)
                int replaceParent = 2;
                List<int> randIndex = clsUtil.RandomPermutaion(selectParentsIndex.Count);
                for (int i = 0; i <= replaceParent - 1; i++)
                {
                    int parentIndex = selectParentsIndex[randIndex[i]];
                    newPopulation.Add(this.m_parents[parentIndex]);
                }
                // sort by eval
                newPopulation.Sort();

                // replace
                for (int i = 0; i <= replaceParent - 1; i++)
                {
                    int parentIndex = selectParentsIndex[randIndex[i]];
                    this.m_parents[parentIndex] = newPopulation[i];
                }

                this.m_parents.Sort();
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
                return clsUtil.GetBestPoint(m_parents, true);
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
                return this.m_parents;
            }
        }

        /// <summary>
        ///         ''' Select parent for G3(Genelized Generation Gap)
        ///         ''' </summary>
        ///         ''' <param name="ai_pickN"></param>
        ///         ''' <param name="ao_parentIndex"></param>
        ///         ''' <param name="ao_retParents"></param>
        ///         ''' <remarks></remarks>
        private void SelectParentsForG3(int ai_pickN, ref List<int> ao_parentIndex, ref List<LoPoint> ao_retParents)
        {
            // generate random permutation array without best parent index
            List<int> randIndex = clsUtil.RandomPermutaion(this.m_parents.Count, 0);

            // generate random permutation with best parent index
            ao_parentIndex = new List<int>(ai_pickN);
            ao_retParents = new List<LoPoint>(ai_pickN);
            int insertBestParentPosition = this._rand.Next(0, ai_pickN);
            for (int i = 0; i <= ai_pickN - 1; i++)
            {
                if (i == insertBestParentPosition)
                {
                    ao_parentIndex.Add(0);
                    ao_retParents.Add(this.m_parents[0]);
                }
                else
                {
                    ao_parentIndex.Add(randIndex[i]);
                    ao_retParents.Add(this.m_parents[randIndex[i]]);
                }
            }
        }

        /// <summary>
        ///         ''' Crossover PCX
        ///         ''' </summary>
        ///         ''' <param name="ai_parents"></param>
        ///         ''' <param name="ai_childrenSize"></param>
        ///         ''' <param name="ai_pickParentNo"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        private List<LoPoint> CrossoverPCX(List<LoPoint> ai_parents, int ai_childrenSize, int ai_pickParentNo)
        {
            List<LoPoint> retPopulation = new List<LoPoint>();

            for (int pNo = 0; pNo <= ai_childrenSize - 1; pNo++)
            {
                List<int> randIndex = clsUtil.RandomPermutaion(ai_parents.Count);
                List<LoPoint> parentsPoint = new List<LoPoint>(ai_pickParentNo);
                for (int i = 0; i <= ai_pickParentNo - 1; i++)
                    parentsPoint.Add(ai_parents[randIndex[i]]);

                // calc g
                LoVector g = new LoVector(this._func.Dimension());
                for (int i = 0; i <= ai_pickParentNo - 1; i++)
                    g += parentsPoint[i];
                g /= (double)ai_pickParentNo;

                // calc D
                LoVector d = g - parentsPoint[0];
                double dist = d.NormL2();
                if (dist < this.EPS)
                {
                }

                List<LoVector> diff = new List<LoVector>();
                for (int i = 1; i <= ai_pickParentNo - 1; i++)
                {
                    diff.Add(new LoVector(this._func.Dimension()));
                    diff[i - 1] = parentsPoint[i] - parentsPoint[0];
                    if (diff[i - 1].NormL2() < EPS)
                    {
                    }
                }

                // orthogonal directions -> Vector D
                LoVector DD = new LoVector(this._func.Dimension());
                for (int i = 0; i <= ai_pickParentNo - 2; i++)
                {
                    double temp1 = diff[i].InnerProduct(d);
                    double temp2 = temp1 / (diff[i].NormL2() * dist);
                    var temp3 = 1.0 - Math.Pow(temp2, 2.0);
                    DD[i] = diff[i].NormL2() * Math.Sqrt(temp3);
                }

                // Average vector D
                double meanD = DD.Average();
                LoVector tempV1 = new LoVector(this._func.Dimension());
                for (int i = 0; i <= this._func.Dimension() - 1; i++)
                    tempV1[i] = clsUtil.NormRand(0.0, meanD * Eta);
                double tempInnerP = tempV1.InnerProduct(d);
                double tempNRand = clsUtil.NormRand(0.0, Zeta);
                for (int i = 0; i <= this._func.Dimension() - 1; i++)
                {
                    tempV1[i] = tempV1[i] - tempInnerP * DD[i] / Math.Pow(dist, 2.0);
                    tempV1[i] += tempNRand * d[i];
                }

                // add population
                LoVector tempChildVector = parentsPoint[0] + tempV1;
                retPopulation.Add(new LoPoint(this._func, tempChildVector));
            }

            return retPopulation;
        }
    }
}

