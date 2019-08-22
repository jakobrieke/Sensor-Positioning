using System;
using System.Collections.Generic;
using LibOptimization.Optimization;

namespace BenchmarkFunction
{
    /// <summary>
    ///     ''' Benchmark function
    ///     ''' Rosenblock function(Banana function)
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Minimum:
    ///     '''  F(0,...,0) = 0
    ///     ''' </remarks>
    public class clsBenchRosenblock : AbsObjectiveFunction
    {
        private int dimension = 0;

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <param name="ai_dim">Set dimension</param>
        ///         ''' <remarks></remarks>
        public clsBenchRosenblock(int ai_dim)
        {
            if (ai_dim <= 1)
                throw new NotImplementedException();
            this.dimension = ai_dim;
        }

        /// <summary>
        ///         ''' Target Function
        ///         ''' </summary>
        ///         ''' <param name="x"></param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public override double F(List<double> x)
        {
            if (x == null)
                return 0;

            if (this.dimension != x.Count)
                return 0;

            double ret = 0.0;
            for (int i = 0; i <= this.dimension - 2; i++)
                ret += 100 * Math.Pow((x[i + 1] - Math.Pow(x[i], 2)), 2) + Math.Pow((x[i] - 1), 2);

            return ret;
        }

        public override List<double> Gradient(List<double> x)
        {
            List<double> gradVec = new List<double>();

            // i0
            double grad = 0;
            grad = -400 * x[0] * (x[0] - Math.Pow(x[1], 2)) - 2 * (x[0] - 1);
            gradVec.Add(grad);

            // i1 ~ in-1
            for (int i = 1; i <= this.dimension - 2; i++)
            {
                grad = -400 * x[i] * (x[i + 1] - Math.Pow(x[i], 2)) + 200 * (x[i] - Math.Pow(x[i - 1], 2)) - 2;
                gradVec.Add(grad);
            }

            // in
            grad = 200 * (x[this.dimension - 1] - Math.Pow(x[this.dimension - 2], 2));
            gradVec.Add(grad);

            return gradVec;
        }

        public override List<List<double>> Hessian(List<double> x)
        {
            List<List<double>> hesse = new List<List<double>>();
            double[] tempVect = new double[this.dimension - 1 + 1];
            for (int i = 0; i <= this.dimension - 1; i++)
                hesse.Add(new List<double>(tempVect));

            if (this.dimension == 2)
            {
                for (int i = 0; i <= this.dimension - 1; i++)
                {
                    for (int j = 0; j <= this.dimension - 1; j++)
                    {
                        if (i == j)
                        {
                            if (i != this.dimension - 1)
                                hesse[i][j] = -400 * (x[i + 1] - Math.Pow(x[i], 2)) + 800 * Math.Pow(x[i], 2) - 2;
                            else
                                hesse[i][j] = 200;
                        }
                        else
                            hesse[i][j] = -400 * x[0];
                    }
                }
            }
            else
                for (int i = 0; i <= this.dimension - 1; i++)
                {
                    for (int j = 0; j <= this.dimension - 1; j++)
                    {
                        if (i == j)
                        {
                            if (i == 0)
                                hesse[i][j] = -400 * (x[i + 1] - Math.Pow(x[i], 2)) + 800 * Math.Pow(x[i], 2) - 2;
                            else if (i == this.dimension - 1)
                                hesse[i][j] = 200;
                            else
                                hesse[i][j] = -400 * (x[i + 1] - Math.Pow(x[i], 2)) + 800 * Math.Pow(x[i], 2) + 198;
                        }
                        if (i == j - 1)
                            hesse[i][j] = -400 * x[i];
                        if (i - 1 == j)
                            hesse[i][j] = -400 * x[j];
                    }
                }

            return hesse;
        }

        public override int Dimension()
        {
            return this.dimension;
        }
    }
}

