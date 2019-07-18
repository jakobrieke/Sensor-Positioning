using System;
using System.Diagnostics;

namespace MathUtil
{
    /// <summary>
    ///     ''' Utility class for Math
    ///     ''' </summary>
    public class clsMathUtil
    {
        /// <summary>
        ///         ''' Create random symmetric matrix(for Debug)
        ///         ''' </summary>
        ///         ''' <param name="size"></param>
        ///         ''' <param name="rngSeed"></param>
        ///         ''' <returns></returns>
        public static clsEasyMatrix CreateRandomSymmetricMatrix(int size, int rngSeed = 123456)
        {
            var rng = new System.Random(rngSeed);
            var matTemp = new MathUtil.clsEasyMatrix(size);
            for (int i = 0; i <= matTemp.Count - 1; i++)
            {
                matTemp[i][i] = rng.Next(-10, 10);
                matTemp[i][i] = 1;
            }
            for (int i = 0; i <= matTemp.Count - 1; i++)
            {
                for (int j = 1 + i; j <= matTemp.Count - 1; j++)
                {
                    var r = rng.Next(-10, 10);
                    matTemp[i][j] = r;
                    matTemp[j][i] = r;
                }
            }

            return matTemp;
        }

        /// <summary>
        ///         ''' Create random Asymmetric matrix(for Debug)
        ///         ''' </summary>
        ///         ''' <param name="size"></param>
        ///         ''' <param name="rngSeed"></param>
        ///         ''' <returns></returns>
        public static clsEasyMatrix CreateRandomASymmetricMatrix(int size, int rngSeed = 123456)
        {
            var rng = new System.Random(rngSeed);
            var matTemp = new MathUtil.clsEasyMatrix(size);
            for (int i = 0; i <= matTemp.Count - 1; i++)
            {
                for (int j = 0; j <= matTemp.Count - 1; j++)
                {
                    var r = rng.Next(-10, 10);
                    matTemp[i][j] = r;
                }
            }

            return matTemp;
        }

        /// <summary>
        ///         ''' for Eigen() debug
        ///         ''' </summary>
        ///         ''' <param name="dimNum"></param>
        ///         ''' <param name="seed"></param>
        ///         ''' <returns></returns>
        public static bool IsCheckEigen(int dimNum, int seed = 123456)
        {
            Stopwatch sw = new Stopwatch();
            var mat2 = CreateRandomSymmetricMatrix(dimNum, seed);
            mat2.PrintValue(name: "Source");

            clsEasyMatrix retM = null/* TODO Change to default(_) if this is not a reference type */;
            clsEasyVector retV = null/* TODO Change to default(_) if this is not a reference type */;
            clsEasyMatrix suspend = null/* TODO Change to default(_) if this is not a reference type */;
            sw.Start();
            while (clsEasyMatrix.Eigen(ref mat2, ref retV, ref retM, SuspendMat: suspend) == false)
            {
                Console.WriteLine("Retry");
                mat2 = suspend;
            }
            sw.Stop();
            var retD = retV.ToDiagonalMatrix();

            retM.PrintValue(name: "Eigen V");
            retD.PrintValue(name: "D");
            retM.T().PrintValue(name: "Eigen V^T");

            var temp = retM * retV.ToDiagonalMatrix() * retM.T();
            temp.PrintValue();

            Console.WriteLine("Elapsed Time:{0}", sw.ElapsedMilliseconds);

            return true;
        }

        /// <summary>
        ///         ''' check eaual matrix
        ///         ''' </summary>
        ///         ''' <param name="matA"></param>
        ///         ''' <param name="matB"></param>
        ///         ''' <param name="eps">default:1E-8</param>
        ///         ''' <returns></returns>
        public static bool IsNearyEqualMatrix(clsEasyMatrix matA, clsEasyMatrix matB, double eps = 0.00000001)
        {
            try
            {
                for (int i = 0; i <= matA.RowCount - 1; i++)
                {
                    for (int j = 0; j <= matA.ColCount - 1; j++)
                    {
                        var tempValA = matA[i][j];
                        var tempValB = matB[i][j];
                        var diff = Math.Abs(tempValA) - Math.Abs(tempValB);
                        if (diff > eps)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

