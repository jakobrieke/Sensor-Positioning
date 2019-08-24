using System.Collections.Generic;

namespace LibOptimization.Optimization
{
    /// <summary>
    /// Abstarct objective function class
    /// </summary>
    /// <remarks></remarks>
    public abstract class AbsObjectiveFunction
    {
        /// <summary>
        /// Get number of variables
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract int Dimension();

        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="x"></param>
        /// <remarks></remarks>
        public abstract double F(List<double> x);

        /// <summary>
        /// Gradient vector (for Steepest descent method, newton method)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks>
        /// ex)
        /// f(x1,..,xn) = x1^2 + ... + xn^2
        /// del f =  [df/dx1 , ... , df/dxn]
        /// </remarks>
        public abstract List<double> Gradient(List<double> x);

        /// <summary>
        /// Hessian matrix (for newton method)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks>
        /// ex)
        /// f(x1,x2) = x1^2 + x2^2
        /// del f   =  [df/dx1 df/dx2]
        /// del^2 f = [d^2f/d^2x1     d^2f/dx1dx2]
        ///           [d^2f/d^2dx2dx1 d^2f/d^2x2]
        /// </remarks>
        public abstract List<List<double>> Hessian(List<double> x);

        /// <summary>
        /// Numerical derivative
        /// </summary>
        /// <param name="x"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public List<double> NumericDerivative(IEnumerable<double> x, 
            double h = 0.0000000001)
        {
            var df = new List<double>();
            for (var i = 0; i <= Dimension() - 1; i++)
            {
                // Center difference
                var tempX1 = new List<double>(x);
                var tempX2 = new List<double>(x);
                tempX1[i] = tempX1[i] + h;
                tempX2[i] = tempX2[i] - h;
                
                var tempDf = F(tempX1) - F(tempX2);
                tempDf /= 2.0 * h;
                df.Add(tempDf);
            }
            return df;
        }
    }
}

