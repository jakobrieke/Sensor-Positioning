using System;
using System.Collections.Generic;

namespace LOMath
{
    /// <summary>
    /// Matrix class
    /// </summary>
    /// <remarks>
    /// Inherits List(Of List(Of Double))
    /// 
    /// TODO:
    /// LU, Solve ,SVD
    /// </remarks>
    public class LoMatrix : List<List<double>>
    {
        public const double SAME_ZERO = 2.0E-50; // 2^-50
        public const double Epsiton = 0.0000000000000001; // 1.0^-16

        /// <summary>
        /// Default construcotr
        /// </summary>
        /// <remarks></remarks>
        public LoMatrix()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="ai_base"></param>
        /// <remarks></remarks>
        public LoMatrix(LoMatrix ai_base)
        {
            for (int i = 0; i <= ai_base.Count - 1; i++)
            {
                LoVector temp = new LoVector(ai_base[i]);
                this.Add(temp);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_dim"></param>
        /// <param name="ai_isIdentity">Make Identify matrix</param>
        /// <remarks></remarks>
        public LoMatrix(int ai_dim, bool ai_isIdentity = false)
        {
            for (int i = 0; i <= ai_dim - 1; i++)
            {
                LoVector temp = new LoVector(ai_dim);
                if (ai_isIdentity)
                    temp[i] = 1.0;
                this.Add(temp);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_rowSize"></param>
        /// <param name="ai_colSize"></param>
        /// <remarks></remarks>
        public LoMatrix(int ai_rowSize, int ai_colSize)
        {
            for (int i = 0; i <= ai_rowSize - 1; i++)
                this.Add(new LoVector(ai_colSize));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_val"></param>
        /// <remarks></remarks>
        public LoMatrix(List<List<double>> ai_val)
        {
            for (int i = 0; i <= ai_val.Count - 1; i++)
                this.Add(new LoVector(ai_val[i]));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_val"></param>
        /// <remarks></remarks>
        public LoMatrix(double[][] ai_val)
        {
            for (int i = 0; i <= ai_val.Length - 1; i++)
                this.Add(new LoVector(ai_val[i]));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ai_val"></param>
        /// <param name="ai_direction">Row or Col</param>
        /// <remarks></remarks>
        public LoMatrix(List<double> ai_val, LoVector.VectorDirection ai_direction)
        {
            if (ai_direction == LoVector.VectorDirection.Row)
            {
                LoVector temp = new LoVector(ai_val);
                this.Add(temp);
            }
            else
                for (int i = 0; i <= ai_val.Count - 1; i++)
                    this.Add(new LoVector(new[] { ai_val[i] }));
        }

        public LoMatrix(List<double> data, int order)
        {
            this.Clear();
            for (int i = 0; i <= order - 1; i++)
            {
                double[] temp = new double[order - 1 + 1];
                this.Add(new LoVector(temp));
            }
            int index = 0;
            for (int i = 0; i <= order - 1; i++)
            {
                for (int j = 0; j <= order - 1; j++)
                {
                    this[j][i] = data[index];
                    index += 1;
                }
            }
        }

        /// <summary>
        /// Add(Matrix + Matrix)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoMatrix operator +(LoMatrix ai_source, LoMatrix ai_dest)
        {
            if (IsSameDimension(ai_source, ai_dest) == false)
                throw new clsException(clsException.Series.DifferRowNumberAndCollumnNumber);
            LoMatrix ret = new LoMatrix(ai_source);
            for (int i = 0; i <= ret.RowCount - 1; i++)
            {
                ret.SetRow(i, ai_source.GetRow(i) + ai_dest.GetRow(i));
            }
            return ret;
        }

        /// <summary>s
        /// Add(Matrix + Vector)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoVector operator +(LoMatrix ai_source, LoVector ai_dest)
        {
            if (IsComputableMatrixVector(ai_source, ai_dest) == false)
                throw new clsException(clsException.Series.NotComputable);
            LoVector ret = new LoVector(ai_dest);
            for (int i = 0; i <= ai_dest.Count - 1; i++)
                ret[i] = ai_source[i][0] + ai_dest[i];
            return ret;
        }

        /// <summary>
        /// Add(Vector + Matrix)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoVector operator +(LoVector ai_source, LoMatrix ai_dest)
        {
            if (IsComputableMatrixVector(ai_dest, ai_source) == false)
                throw new clsException(clsException.Series.NotComputable);
            LoVector ret = new LoVector(ai_source);
            for (int i = 0; i <= ai_source.Count - 1; i++)
                ret[i] = ai_source[i] + ai_dest[i][0];
            return ret;
        }

        /// <summary>
        /// Diff
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoMatrix operator -(LoMatrix ai_source, LoMatrix ai_dest)
        {
            if (IsSameDimension(ai_source, ai_dest) == false)
                throw new clsException(clsException.Series.DifferRowNumberAndCollumnNumber);
            LoMatrix ret = new LoMatrix(ai_source);
            for (int i = 0; i <= ret.RowCount - 1; i++)
            {
                ret.SetRow(i, ai_source.GetRow(i) - ai_dest.GetRow(i));
            }
            return ret;
        }

        /// <summary>
        /// Diff(Matrix + Vector)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoVector operator -(LoMatrix ai_source, LoVector ai_dest)
        {
            if (IsComputableMatrixVector(ai_source, ai_dest) == false)
                throw new clsException(clsException.Series.NotComputable);
            LoVector ret = new LoVector(ai_dest);
            for (int i = 0; i <= ai_dest.Count - 1; i++)
                ret[i] = ai_source[i][0] - ai_dest[i];
            return ret;
        }

        /// <summary>
        /// Diff(Vector + Matrix)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoVector operator -(LoVector ai_source, LoMatrix ai_dest)
        {
            if (IsComputableMatrixVector(ai_dest, ai_source) == false)
                throw new clsException(clsException.Series.NotComputable);
            LoVector ret = new LoVector(ai_source);
            for (int i = 0; i <= ai_source.Count - 1; i++)
                ret[i] = ai_source[i] - ai_dest[i][0];
            return ret;
        }

        /// <summary>
        /// Product( Matrix * Matrix )
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static LoMatrix operator *(LoMatrix ai_source, LoMatrix ai_dest)
        {
            if (IsSameDimension(ai_source, ai_dest) == true)
            {
                // [M*M] X [M*M]
                var size = ai_source.RowCount;
                LoMatrix ret = new LoMatrix(size);

                // 並列化 .NET4
                // Threading.Tasks.Parallel.For(0, size - 1,
                // Sub[i]
                // For j As Integer = 0 To size - 1
                // For k As Integer = 0 To size - 1
                // Dim tempA = ai_source[i](k)
                // Dim tempB = ai_dest(k)[j]
                // ret[i][j] += tempA * tempB
                // Next
                // Next
                // End Sub)

                for (int i = 0; i <= size - 1; i++)
                {
                    for (int j = 0; j <= size - 1; j++)
                    {
                        for (int k = 0; k <= size - 1; k++)
                        {
                            var tempA = ai_source[i][k];
                            var tempB = ai_dest[k][j];
                            ret[i][j] += tempA * tempB;
                        }
                    }
                }

                return ret;
            }

            if (ai_source.ColCount == ai_dest.RowCount)
            {
                // [M*N] X [N*O]
                LoMatrix ret = new LoMatrix(ai_source.RowCount, ai_dest.ColCount);
                for (int i = 0; i <= ret.RowCount - 1; i++)
                {
                    for (int j = 0; j <= ret.ColCount - 1; j++)
                    {
                        double temp = 0.0;
                        for (int k = 0; k <= ai_source.ColCount - 1; k++)
                            temp += ai_source[i][k] * ai_dest[k][j];
                        ret[i][j] = temp;
                    }
                }
                return ret;
            }

            throw new clsException(clsException.Series.NotComputable, "Matrix * Matrix");
        }

        /// <summary>
        /// Product( Matrix * Vector )
        /// </summary>
        /// <param name="mat">Matrix</param>
        /// <param name="vec">Vector</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static LoVector operator *(LoMatrix mat, LoVector vec)
        {
            // ベクトルと行列のサイズ確認
            // OK
            // |a11 a12| * |v1| = cv1
            // |a21 a22|   |v2|   cv2
            // 
            // |a11 a12| * |v1| = cv1
            // |v2|
            // 
            // |a11 a12| * |v1| = cv1
            // |a21 a22|   |v2|   cv2
            // |a31 a32|          cv3
            // 
            if (mat.ColCount != vec.Count)
                throw new clsException(clsException.Series.NotComputable, "Matrix * Vector - size error");
            if (vec.Direction != LoVector.VectorDirection.Col)
                throw new clsException(clsException.Series.NotComputable, "Matrix * Vector - size error");

            // 計算
            int vSize = mat.RowCount; // 行列の行サイズ
            LoVector ret = new LoVector(vSize, LoVector.VectorDirection.Col);
            for (int i = 0; i <= vSize - 1; i++)
            {
                double sum = 0.0;
                for (int j = 0; j <= mat.ColCount - 1; j++)
                    sum += mat[i][j] * vec[j];
                ret[i] = sum;
            }
            return ret;
        }

        /// <summary>
        /// Product( Vector * Matrix)
        /// </summary>
        /// <param name="vec">Vector</param>
        /// <param name="mat">Matrix</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static LoVector operator *(LoVector vec, LoMatrix mat)
        {
            // ベクトルと行列のサイズ確認
            // OK
            // |v1 v2| * |a11| = |c1|
            // |a12|   
            // 
            // |v1 v2| * |a11 a12| = |c1 c2|
            // |a11 a22|   
            // 
            // |v1 v2| * |a11 a12 a13| = |c1 c2 c3|
            // |a21 a22 a23|   
            // 
            if (vec.Count != mat.RowCount)
                throw new clsException(clsException.Series.NotComputable, "Vector * Matrix - size error");

            // 計算
            int vSize = mat.ColCount; // 行列の行サイズ
            LoVector ret = new LoVector(vSize, LoVector.VectorDirection.Row);
            for (int j = 0; j <= vSize - 1; j++)
            {
                double sum = 0.0;
                for (int i = 0; i <= mat.RowCount - 1; i++)
                    sum += vec[i] * mat[i][j];
                ret[j] = sum;
            }
            return ret;
        }

        /// <summary>
        /// Product(value * Matrix)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoMatrix operator *(double ai_source, LoMatrix ai_dest)
        {
            LoMatrix ret = new LoMatrix(ai_dest);
            for (int i = 0; i <= ret.RowCount - 1; i++)
            {
                ret.SetRow(i, ai_source * ai_dest.GetRow(i));
            }
            return ret;
        }

        /// <summary>
        /// Product(Matrix * value)
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LoMatrix operator *(LoMatrix ai_source, double ai_dest)
        {
            LoMatrix ret = new LoMatrix(ai_source);
            for (int i = 0; i <= ret.RowCount - 1; i++)
            {
                ret.SetRow(i, ai_source.GetRow(i) * ai_dest);
            }
            return ret;
        }

        /// <summary>
        /// Transpose
        /// </summary>
        /// <remarks></remarks>
        public LoMatrix T()
        {
            LoMatrix ret = new LoMatrix(this.ColCount, this.RowCount);
            for (int rowIndex = 0; rowIndex <= ret.RowCount - 1; rowIndex++)
            {
                ret.SetRow(rowIndex, GetRow(rowIndex));
            }
            return ret;
        }

        /// <summary>
        /// Determinant
        /// </summary>
        /// <remarks></remarks>
        public double Det(bool ai_isDebug = false)
        {
            if (this.RowCount != this.ColCount)
                return 0;
            return CalcDeterminant(this, this.RowCount, ai_isDebug);
        }

        /// <summary>
        /// conversion Diagonal matrix
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public LoMatrix ToDiagonalMatrix()
        {
            if (this.RowCount != this.ColCount)
                throw new clsException(clsException.Series.NotComputable, "ToDiagonalMatrix()");
            LoMatrix ret = new LoMatrix(this.RowCount);
            for (int i = 0; i <= this.Count - 1; i++)
                ret[i][i] = this[i][i];
            return ret;
        }

        /// <summary>
        /// 対角成分をベクトルに変換
        /// </summary>
        /// <param name="direction">direction of vector</param>
        /// <returns></returns>
        public LoVector ToVectorFromDiagonal(LoVector.VectorDirection direction = LoVector.VectorDirection.Row)
        {
            if (this.RowCount != this.ColCount)
                throw new clsException(clsException.Series.NotComputable, "ToVectorFromDiagonal");
            LoVector ret = new LoVector(this.RowCount, direction);
            for (int i = 0; i <= this.Count - 1; i++)
                ret[i] = this[i][i];
            return ret;
        }

        /// <summary>
        /// Inverse
        /// </summary>
        /// <remarks></remarks>
        public LoMatrix Inverse()
        {
            if (this.RowCount != this.ColCount)
                return new LoMatrix(0);

            int n = this.RowCount;
            LoMatrix source = new LoMatrix(this);
            LoMatrix retInverse = new LoMatrix(n, true);
            if (n == 0)
            {
            }
            else if (n == 1)
                source[0][0] = 1.0 / source[0][0];
            else if (n == 2)
            {
                // Dim det As Double = source.Det()
                // If SAME_ZERO > det Then
                // Return New clsShoddyMatrix(n, True)
                // End If
                retInverse[0][0] = (1.0 / Det()) * this[1][1];
                retInverse[0][1] = (1.0 / Det()) * -this[0][1];
                retInverse[1][0] = (1.0 / Det()) * -this[1][0];
                retInverse[1][1] = (1.0 / Det()) * this[0][0];
            }
            else if (n == 3)
            {
                // Dim det As Double = source.Det()
                // If SAME_ZERO > det Then
                // Return New clsShoddyMatrix(n, True)
                // End If
                retInverse[0][0] = (1.0 / Det()) * (this[1][1] * this[2][2] - this[1][2] * this[2][1]);
                retInverse[0][1] = (1.0 / Det()) * (this[0][2] * this[2][1] - this[0][1] * this[2][2]);
                retInverse[0][2] = (1.0 / Det()) * (this[0][1] * this[1][2] - this[0][2] * this[1][1]);
                retInverse[1][0] = (1.0 / Det()) * (this[1][2] * this[2][0] - this[1][0] * this[2][2]);
                retInverse[1][1] = (1.0 / Det()) * (this[0][0] * this[2][2] - this[0][2] * this[2][0]);
                retInverse[1][2] = (1.0 / Det()) * (this[0][2] * this[1][0] - this[0][0] * this[1][2]);
                retInverse[2][0] = (1.0 / Det()) * (this[1][0] * this[2][1] - this[1][1] * this[2][0]);
                retInverse[2][1] = (1.0 / Det()) * (this[0][1] * this[2][0] - this[0][0] * this[2][1]);
                retInverse[2][2] = (1.0 / Det()) * (this[0][0] * this[1][1] - this[0][1] * this[1][0]);
            }
            else
                // Dim det As Double = source.Det() 'omoi...
                // If SAME_ZERO > det Then
                // Return New clsShoddyMatrix(n, True)
                // End If
                // Gauss elimination with pivot select
                for (int i = 0; i <= n - 1; i++)
                {
                    // diagonal element
                    int ip = i; // maxIndex
                    double amax = source[i][i]; // maxValue
                    for (int index = 0; index <= n - 1; index++)
                    {
                        double temp = System.Math.Abs(source[index][i]);
                        if (temp > amax)
                        {
                            amax = temp;
                            ip = index;
                        }
                    }

                    // check 正則性の判定
                    if (amax < SAME_ZERO)
                        return new LoMatrix(this.ColCount);

                    // change row
                    if (i != ip)
                    {
                        source.SwapRow(i, ip);
                        retInverse.SwapRow(i, ip);
                    }

                    // discharge calculation
                    double tempValue = 1.0 / source[i][i];
                    for (int j = 0; j <= n - 1; j++)
                    {
                        source[i][j] *= tempValue;
                        retInverse[i][j] *= tempValue;
                    }
                    for (int j = 0; j <= n - 1; j++)
                    {
                        if (i != j)
                        {
                            tempValue = source[j][i];
                            for (int k = 0; k <= n - 1; k++)
                            {
                                source[j][k] -= source[i][k] * tempValue;
                                retInverse[j][k] -= retInverse[i][k] * tempValue;
                            }
                        }
                    }
                }

            return retInverse;
        }

        /// <summary>
        /// Swap Row
        /// </summary>
        /// <param name="ai_sourceRowIndex"></param>
        /// <param name="ai_destRowIndex"></param>
        /// <remarks></remarks>
        public void SwapRow(int ai_sourceRowIndex, int ai_destRowIndex)
        {
            var temp = GetRow(ai_sourceRowIndex);
            SetRow(ai_sourceRowIndex, GetRow(ai_destRowIndex));
            SetRow(ai_destRowIndex, temp);
        }

        /// <summary>
        /// Swap Col
        /// </summary>
        /// <param name="ai_sourceColIndex"></param>
        /// <param name="ai_destColIndex"></param>
        /// <remarks></remarks>
        public void SwapCol(int ai_sourceColIndex, int ai_destColIndex)
        {
            var temp = GetColumn(ai_sourceColIndex);
            SetColumn(ai_sourceColIndex, GetRow(ai_destColIndex));
            SetColumn(ai_destColIndex, temp);
        }

        /// <summary>
        /// For Debug
        /// </summary>
        /// <param name="ai_preci"></param>
        /// <remarks></remarks>
        public void PrintValue(int ai_preci = 4, string name = "")
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            if (string.IsNullOrEmpty(name) == false)
                str.Append(string.Format("{0} =", name) + Environment.NewLine);
            else
                str.Append("Mat =" + Environment.NewLine);
            foreach (LoVector vec in this)
            {
                for (int i = 0; i <= vec.Count - 1; i++)
                    str.Append(vec[i].ToString("F" + ai_preci.ToString()) + "\t");
                str.Append(Environment.NewLine);
            }
            str.Append(Environment.NewLine);
            Console.Write(str.ToString());
        }

        /// <summary>
        /// To Vector
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public LoVector ToVector()
        {
            if (this.RowCount == 1)
                return this.GetRow(0);
            else if (this.ColCount == 1)
                return this.GetColumn(0);

            throw new clsException(clsException.Series.NotComputable, "Matrix");
        }

        /// <summary>
        /// 正方行列判定
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsSquare()
        {
            if (this.Count == 0)
                return false;

            if (this.Count == this[0].Count)
                return true;

            return false;
        }

        /// <summary>
        /// 対角成分で行列を作る
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static LoMatrix ToDiaglonalMatrix(LoVector vec)
        {
            var ret = new LoMatrix(vec.Count);
            for (int i = 0; i <= ret.Count - 1; i++)
                ret[i][i] = vec[i];
            return ret;
        }

        /// <summary>
        /// Cholesky decomposition A=LL^T
        /// </summary>
        /// <returns></returns>
        public LoMatrix Cholesky()
        {
            if (this.IsSquare() == false)
                throw new clsException(clsException.Series.NotComputable, "Cholesky() not Square");

            LoMatrix ret = new LoMatrix(this.RowCount);
            var n = System.Convert.ToInt32(System.Math.Sqrt(ret.RowCount * ret.ColCount));
            for (int i = 0; i <= n - 1; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (j == i)
                    {
                        var sum = 0.0;
                        for (int k = 0; k <= j; k++)
                            sum += ret[j][k] * ret[j][k];
                        ret[j][j] = System.Math.Sqrt(this[j][j] - sum);
                    }
                    else
                    {
                        var sum = 0.0;
                        for (int k = 0; k <= j; k++)
                            sum += ret[i][k] * ret[j][k];
                        ret[i][j] = 1.0 / ret[j][j] * (this[i][j] - sum);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// To Tridiagonal matrix using Householder transform
        /// </summary>
        /// <param name="sourceMat"></param>
        /// <returns></returns>
        public static LoMatrix ToTridiagonalMatrix(LoMatrix sourceMat)
        {
            // ref
            // ハウスホルダー変換
            // http://www.slis.tsukuba.ac.jp/~fujisawa.makoto.fu/cgi-bin/wiki/index.php?%B8%C7%CD%AD%C3%CD/%B8%C7%CD%AD%A5%D9%A5%AF%A5%C8%A5%EB

            var tempMat = new LoMatrix(sourceMat);
            int n = tempMat.Count;
            var u = new LoVector(n);
            var v = new LoVector(n);
            var q = new LoVector(n);

            for (int k = 0; k <= n - 2; k++)
            {
                // s
                double s = 0.0;
                for (var i = k + 1; i <= n - 1; i++)
                    s += tempMat[i][k] * tempMat[i][k];

                // | -
                // |a10 -
                // |   a21
                double temp = tempMat[k + 1][k];
                if (temp >= 0)
                    s = System.Math.Sqrt(s);
                else
                    s = -System.Math.Sqrt(s);

                // |x-y|
                var alpha = System.Math.Sqrt(2.0 * s * (s - temp));
                if (System.Math.Abs(alpha) < 0.0000000000000001)
                    continue;

                // u
                u[k + 1] = (temp - s) / alpha;
                for (var i = k + 2; i <= n - 1; i++)
                    u[i] = tempMat[i][k] / alpha;

                // Au
                q[k] = alpha / 2.0;
                for (var i = k + 1; i <= n - 1; i++)
                {
                    q[i] = 0.0;
                    for (var j = k + 1; j <= n - 1; j++)
                        q[i] += tempMat[i][j] * u[j];
                }

                // v=2(Au-uu^T(Au))
                alpha = 0.0;
                // uu^T
                for (var i = k + 1; i <= n - 1; i++)
                    alpha += u[i] * q[i];
                v[k] = 2.0 * q[k];
                for (var i = k + 1; i <= n - 1; i++)
                    v[i] = 2.0 * (q[i] - alpha * u[i]);

                // A = PAP
                // | -    a02
                // |    -
                // |a20    -
                tempMat[k][k + 1] = s;
                tempMat[k + 1][k] = s;
                for (var i = k + 2; i <= n - 1; i++)
                {
                    tempMat[k][i] = 0.0;
                    tempMat[i][k] = 0.0;
                }
                for (var i = k + 1; i <= n - 1; i++)
                {
                    tempMat[i][i] = tempMat[i][i] - 2.0 * u[i] * v[i];
                    for (var j = i + 1; j <= n - 1; j++)
                    {
                        var tempVal = tempMat[i][j] - u[i] * v[j] - v[i] * u[j];
                        tempMat[i][j] = tempVal;
                        tempMat[j][i] = tempVal;
                    }
                }
            }

            return tempMat;
        }

        /// <summary>
        /// Eigen decomposition using Jacobi Method.
        /// Memo: A = V*D*V−1, D is diag(eigen value1 ... eigen valueN), V is eigen vectors. V is orthogonal matrix.
        /// </summary>
        /// <param name="inMat">source matrix</param>
        /// <param name="eigenvalues">eigen vaues(Vector)</param>
        /// <param name="eigenVectors">eigen vectors(Matrix)</param>
        /// <param name="Iteration">default iteration:1000</param>
        /// <param name="Conversion">default conversion:1.0e-16</param>
        /// <returns>True:success conversion. False:not conversion</returns>
        public static bool Eigen(ref LoMatrix inMat, ref LoVector eigenvalues, ref LoMatrix eigenVectors, int Iteration = 1000, double Conversion = 0.0000000000000001, LoMatrix SuspendMat = null)
        {
            var size = inMat.ColCount;
            var retEigenMat = new LoMatrix(inMat);
            var rotate = new LoMatrix(size, true);

            var rowIdx = new int[size * 4 - 1 + 1];
            var colIdx = new int[size * 4 - 1 + 1];
            var value = new double[size * 4 - 1 + 1];

            // iteration
            bool isConversion = false;
            for (int itr = 0; itr <= Iteration - 1; itr++)
            {
                // find abs max value without diag
                var max = System.Math.Abs(retEigenMat[0][1]);
                int p = 0;
                int q = 1;
                for (int i = 0; i <= size - 1; i++)
                {
                    for (int j = i + 1; j <= size - 1; j++)
                    {
                        if (max < System.Math.Abs(retEigenMat[i][j]))
                        {
                            max = System.Math.Abs(retEigenMat[i][j]);
                            p = i;
                            q = j;
                        }
                    }
                }

                // check conversion
                if (max < Conversion)
                {
                    isConversion = true;
                    break;
                }

                // debug
                // Console.WriteLine("Itr = {0} Max = {1} p={2} q={3}", itr, max, p, q)
                // B.PrintValue(name:="B")
                // R.PrintValue(name:="R")

                var theta = 0.0;
                var diff = retEigenMat[p][p] - retEigenMat[q][q];
                if (System.Math.Abs(diff) == 0.0)
                    theta = System.Math.PI / 4.0;
                else
                    theta = System.Math.Atan(-2.0 * retEigenMat[p][q] / diff) * 0.5;

                var D = new LoMatrix(retEigenMat);
                var cosTheta = System.Math.Cos(theta);
                var sinTheta = System.Math.Sin(theta);
                for (int i = 0; i <= size - 1; i++)
                {
                    var temp = 0.0;
                    temp = retEigenMat[p][i] * cosTheta - retEigenMat[q][i] * sinTheta;
                    D[p][i] = temp;
                    D[i][p] = temp;
                    temp = retEigenMat[p][i] * sinTheta + retEigenMat[q][i] * cosTheta;
                    D[i][q] = temp;
                    D[q][i] = temp;
                }
                var cosThetasinTheta = cosTheta * cosTheta - sinTheta * sinTheta;
                var tempA = (retEigenMat[p][p] + retEigenMat[q][q]) / 2.0;
                var tempB = (retEigenMat[p][p] - retEigenMat[q][q]) / 2.0;
                var tempC = retEigenMat[p][q] * (sinTheta * cosTheta) * 2.0;
                D[p][p] = tempA + tempB * cosThetasinTheta - tempC;
                D[q][q] = tempA - tempB * cosThetasinTheta + tempC;
                D[p][q] = 0.0;
                D[q][p] = 0.0;
                retEigenMat = D;

                // expand
                // Dim cosTheta = Math.Cos(theta)
                // Dim sinTheta = Math.Sin(theta)
                // For k As Integer = 0 To size - 1
                // Dim idx As Integer = k * 4
                // 'store value
                // Dim temp = 0.0
                // temp = retEigenMat(p)(k) * cosTheta - retEigenMat(q)(k) * sinTheta
                // value(idx) = temp
                // value(idx + 1) = temp
                // rowIdx(idx) = p
                // colIdx(idx) = k
                // rowIdx(idx + 1) = k
                // colIdx(idx + 1) = p
                // temp = retEigenMat(p)(k) * sinTheta + retEigenMat(q)(k) * cosTheta
                // value(idx + 2) = temp
                // value(idx + 3) = temp
                // rowIdx(idx + 2) = k
                // colIdx(idx + 2) = q
                // rowIdx(idx + 3) = q
                // colIdx(idx + 3) = k
                // Next
                // Dim cosThetasinTheta = cosTheta * cosTheta - sinTheta * sinTheta
                // Dim tempA = (retEigenMat(p)(p) + retEigenMat(q)(q)) / 2.0
                // Dim tempB = (retEigenMat(p)(p) - retEigenMat(q)(q)) / 2.0
                // Dim tempC = retEigenMat(p)(q) * (sinTheta * cosTheta) * 2.0
                // For k As Integer = 0 To value.Length - 1
                // retEigenMat(rowIdx(k))(colIdx(k)) = value(k)
                // Next
                // retEigenMat(p)(p) = tempA + tempB * cosThetasinTheta - tempC
                // retEigenMat(q)(q) = tempA - tempB * cosThetasinTheta + tempC
                // retEigenMat(p)(q) = 0.0
                // retEigenMat(q)(p) = 0.0

                // rotate
                var rotateNew = new LoMatrix(size, true);
                rotateNew[p][p] = System.Math.Cos(theta);
                rotateNew[p][q] = System.Math.Sin(theta);
                rotateNew[q][p] = -System.Math.Sin(theta);
                rotateNew[q][q] = System.Math.Cos(theta);
                rotate = rotate * rotateNew;
            }

            // 途中結果
            SuspendMat = retEigenMat;

            // 値を代入
            eigenvalues = retEigenMat.ToVectorFromDiagonal();
            eigenVectors = rotate;

            return isConversion;
        }


        /// <summary>
        /// Get Row count
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int RowCount
        {
            get
            {
                return this.Count;
            }
        }

        /// <summary>
        /// Get Collumn count
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ColCount
        {
            get
            {
                return this[0].Count;
            }
        }

        public LoVector GetRow(int ai_rowIndex)
        {
            return new LoVector(this[ai_rowIndex]);
        }

        public void SetRow(int ai_rowIndex, LoVector value)
        {
            this[ai_rowIndex] = new LoVector(value);
        }

        public LoVector GetColumn(int ai_colIndex)
        {
            double[] temp = new double[this.RowCount - 1 + 1];
            for (int i = 0; i <= temp.Length - 1; i++)
                temp[i] = this.GetRow(i)[ai_colIndex];
            LoVector tempVector = new LoVector(temp);
            tempVector.Direction = LoVector.VectorDirection.Col;
            return tempVector;
        }
        
        public void SetColumn(int ai_colIndex, LoVector value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
                this[i][ai_colIndex] = value[i];
        }

        /// <summary>
        /// To List(Of List(Of Double))
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<List<double>> RawMatrix
        {
            get
            {
                return this;
            }
            set
            {
                this.Clear();
                this.AddRange(value);
            }
        }

        /// <summary>
        /// Check Dimension
        /// </summary>
        /// <param name="ai_source"></param>
        /// <param name="ai_dest"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool IsSameDimension(LoMatrix ai_source, LoMatrix ai_dest)
        {
            if (ai_source.RowCount != ai_dest.RowCount)
                return false;
            if (ai_source.ColCount != ai_dest.ColCount)
                return false;
            return true;
        }

        /// <summary>
        /// Check Matrix Vector
        /// </summary>
        /// <param name="ai_matrix"></param>
        /// <param name="ai_vector"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool IsComputableMatrixVector(LoMatrix ai_matrix, LoVector ai_vector)
        {
            if ((ai_matrix.ColCount == 1) && (ai_matrix.RowCount == ai_vector.Count))
                return true;
            else if ((ai_matrix.RowCount == 1) && (ai_matrix.ColCount == ai_vector.Count))
                return true;

            return false;
        }

        /// <summary>
        /// Determinant(Recursive)
        /// </summary>
        /// <param name="aiLoMatrix"></param>
        /// <param name="ai_dim"></param>
        /// <param name="ai_isDebug"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double CalcDeterminant(LoMatrix aiLoMatrix, int ai_dim, bool ai_isDebug = false)
        {
            if (ai_dim == 1)
                return aiLoMatrix[0][0];
            else if (ai_dim == 2)
                // 2 order
                // | a b |
                // | c d | = ad-bc
                return aiLoMatrix[0][0] * aiLoMatrix[1][1] - aiLoMatrix[0][1] * aiLoMatrix[1][0];
            else if (ai_dim == 3)
            {
                // 3 order using Sarrus rule
                double d = 0.0;
                d += aiLoMatrix[0][0] * aiLoMatrix[1][1] * aiLoMatrix[2][2];
                d += aiLoMatrix[1][0] * aiLoMatrix[2][1] * aiLoMatrix[0][2];
                d += aiLoMatrix[2][0] * aiLoMatrix[0][1] * aiLoMatrix[1][2];
                d -= aiLoMatrix[2][0] * aiLoMatrix[1][1] * aiLoMatrix[0][2];
                d -= aiLoMatrix[1][0] * aiLoMatrix[0][1] * aiLoMatrix[2][2];
                d -= aiLoMatrix[0][0] * aiLoMatrix[2][1] * aiLoMatrix[1][2];
                return d;
            }
            else
            {
                // over 4 order
                double detVal = 0.0;
                LoMatrix b = new LoMatrix(ai_dim - 1);
                int sign = 0;
                if (((ai_dim + 1) % (2)) == 0)
                    sign = 1;
                else
                    sign = -1;

                for (int k = 0; k <= ai_dim - 1; k++)
                {
                    for (int i = 0; i <= ai_dim - 2; i++)
                    {
                        for (int j = 0; j <= ai_dim - 1; j++)
                        {
                            if (j == k)
                                continue;
                            if (j > k)
                                b[i][j - 1] = aiLoMatrix[i][j];
                            else
                                b[i][j] = aiLoMatrix[i][j];
                        }
                    }
                    if (ai_isDebug == true)
                    {
                        Console.WriteLine(sign.ToString() + " " + aiLoMatrix[ai_dim - 1][k].ToString());
                        b.PrintValue();
                    }
                    detVal += sign * aiLoMatrix[ai_dim - 1][k] * CalcDeterminant(b, ai_dim - 1);
                    sign *= -1;
                }
                return detVal;
            }
        }
    }
}

