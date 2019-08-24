using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Optimization
{
    public static class OptimizationFcts
    {
        public const double SPHERE_FCT_OPT = 0; 
        public const double MC_CORMICK_FCT_OPT = -1.9133; 
        public const double HOELDER_TABLE_FCT_OPT = -19.2085;
        public const double THREE_HUMP_CAMEL_FCT_OPT = 0;
        public const double HIMMELBLAU_FCT_OPT = 0;
        
        public static double SphereFct(IEnumerable<double> x)
        {
            return x.Sum(t => Pow(t, 2));
        }


        public static double StyblinskiTangFct(IEnumerable<double> x)
        {
            return 0.5 * x.Sum(t => Pow(t, 4) - 16 * Pow(t, 2) + 5 * t);
        }
        
        
        public static double StyblinskiTangOpt(double dimension)
        {
            return -39.16599 * dimension;
        }

        
        public static double McCormickFct(double[] x)
        {
            if (x.Length != 2) throw new ArgumentException(
                "McCormick function is two dimensional");
            
            return Sin(x[0] + x[1]) + Pow(x[0] - x[1], 2)
                   - 1.5 * x[0] + 2.5 * x[1] + 1;
        }

        public static double HoelderTableFct(double[] x)
        {
            if (x.Length != 2) throw new ArgumentException(
                "Hoelder-Table function is two dimensional");
            
            return -Abs(Sin(x[0]) * Cos(x[1]) 
                   * Exp(Abs(1 - Sqrt(
                     Pow(x[0], 2) + Pow(x[1], 2)) / PI)));
        }

        public static double ThreeHumpCamelFct(double[] x)
        {
            if (x.Length != 2) throw new ArgumentException(
                "Three-Hump-Camel function is two dimensional");
            
            return 2 * Pow(x[0], 2) - 1.05 * Pow(x[0], 4) 
                   + Pow(x[0], 6) / 6 + x[0] * x[1] + Pow(x[1], 2);
        }

        public static double HimmelblauFct(double[] x)
        {
            if (x.Length != 2) throw new ArgumentException(
                "Himmelblau function is two dimensional");
            
            return Pow(Pow(x[0], 2) + x[1] - 11, 2)
                   + Pow(x[0] + Pow(x[1], 2) - 7, 2);
        }
    }
}