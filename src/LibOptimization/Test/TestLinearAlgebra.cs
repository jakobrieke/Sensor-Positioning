using System;
using System.Collections.Generic;
using MathUtil;
using NUnit.Framework;
using Util;

public static class TestVector
{
  [Test]
  public static void TestCreateVector()
  {
    var v1 = new clsEasyVector(3);
    for (var i = 0; i <= 3 - 1; i++)
    {
      Assert.AreEqual(v1[i], Convert.ToDouble(0));
    }

    var v2 = new clsEasyVector(new double[] {1, 2, 3});
    for (var i = 0; i <= 3 - 1; i++)
    {
      Assert.AreEqual(v2[i], Convert.ToDouble(i + 1));
    }
  }

  [Test]
  public static void TestCopyVector()
  {
    var v1 = new clsEasyVector(3);
//    var v2 = v1.Copy
    throw new NotImplementedException();
    v1.PrintValue();
    Console.WriteLine(v1.ToString());
  }
}

/// <summary>
/// Unit test for Linear Algebra library
/// </summary>
public static class TestMatrix
{
  /// <summary>
  /// Test Create Matrix
  /// </summary>
  [Test]
  public static void CreateMatrix()
  {
    {
      var mat = new clsEasyMatrix(new[]
      {
        new double[] {1, 0, 0},
        new double[] {0, 1, 0},
        new double[] {0, 0, 1}
      });

      if (mat.RowCount != 3)
        Console.WriteLine("Row Error");

      if (mat.ColCount != 3)
        Console.WriteLine("Col Error");

      for (var i = 0; i <= 3 - 1; i++)
      {
        for (var j = 0; j <= 3 - 1; j++)
        {
          Assert.AreEqual(mat[i][j], i == j ? 1.0 : 0.0);
        }
      }
    }

    {
      var mat = new clsEasyMatrix(3, true);

      if (mat.RowCount != 3) Assert.Fail("Row Error");

      if (mat.ColCount != 3) Assert.Fail("Col Error");

      for (var i = 0; i <= 3 - 1; i++)
      {
        for (var j = 0; j <= 3 - 1; j++)
        {
          Assert.AreEqual(mat[i][j], i == j ? 1.0 : 0.0);
        }
      }
    }
  }

  /// <summary>
  /// Vector + Matrix
  /// </summary>
  [Test]
  public static void Add_VectorMatrix()
  {
    // ------------------------
    // OK
    // ------------------------
    {
      var v = new clsEasyVector(new double[] {1, 1, 1});
      var matV = new clsEasyMatrix(new[]
      {
        new double[] {1}, 
        new double[] {2}, 
        new double[] {3}
      });
      try
      {
        var result = v + matV;
        Assert.IsTrue(
          result[0] == 2.0 && result[1] == 3.0 && result[2] == 4.0,
          "Error");
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    // ------------------------
    // Bad
    // ------------------------
    {
      var v = new clsEasyVector(new double[] {1, 1, 1});
      var matV = new clsEasyMatrix(new[] {new double[] {1, 2, 3}});
      try
      {
        v.PrintValue(name: "v");
        matV.PrintValue(name: "matV");

        v.Direction = clsEasyVector.VectorDirection.Col;
        var result = v + matV;

        // error
        Assert.Fail("Error");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
  }

  /// <summary>
  /// Matrix + Matrix
  /// </summary>
  [Test]
  public static void Add_MatrixMatrix()
  {
    {
      try
      {
        var v1 = new clsEasyMatrix(4, true);
        var v2 = new clsEasyMatrix(5, true);
        var temp = v1 + v2;

        Assert.Fail("Error");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    {
      try
      {
        var v1 = new clsEasyMatrix(5, true);
        var v2 = new clsEasyMatrix(5, true);
        var temp = v1 + v2;
        var diag = temp.ToVectorFromDiagonal();
        foreach (double val in diag)
          Assert.AreEqual(val, 2.0);
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }
  }

  /// <summary>
  /// Matrix + vector
  /// </summary>
  [Test]
  public static void Add_MatrixVector()
  {
    clsEasyVector v = new clsEasyVector(new double[] {1, 1, 1});
    clsEasyMatrix matV = new clsEasyMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = matV + v;
      if (v[0] == 2.0 && v[1] == 3.0 && v[2] == 4.0)
      {
      }
      else
        Assert.Fail("Error");
    }
    catch (Exception ex)
    {
      Assert.Fail("Error");
    }
  }

  /// <summary>
  /// Vector - Matrix
  /// </summary>
  [Test]
  public static void Sub_VectorMatrix()
  {
    clsEasyVector v = new clsEasyVector(new double[] {1, 1, 1});
    clsEasyMatrix matV = new clsEasyMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = v - matV;
      if (v[0] == 0 && v[1] == -1.0 && v[2] == -2.0)
      {
      }
      else
        Assert.Fail("Error");
    }
    catch (Exception ex)
    {
      Assert.Fail("Error");
    }
  }

  /// <summary>
  /// Matrix - Vector
  /// </summary>
  [Test]
  public static void Sub_MatrixVector()
  {
    clsEasyVector v = new clsEasyVector(new double[] {1, 1, 1});
    clsEasyMatrix matV = new clsEasyMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = matV - v;
      if (v[0] == 0.0 && v[1] == 1.0 && v[2] == 2.0)
      {
      }
      else
        Assert.Fail("Error");
    }
    catch (Exception ex)
    {
      Assert.Fail("Error");
    }
  }

  /// <summary>
  /// Matrix - Matrix
  /// </summary>
  [Test]
  public static void Sub_MatrixMatrix()
  {
    {
      try
      {
        var v1 = new clsEasyMatrix(4, true);
        var v2 = new clsEasyMatrix(5, true);
        var temp = v1 - v2;

        Assert.Fail("Error");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    {
      try
      {
        var v1 = new clsEasyMatrix(5, true);
        var v2 = new clsEasyMatrix(5, true);
        var temp = v1 - v2;
        var diag = temp.ToVectorFromDiagonal();
        foreach (double val in diag)
          Assert.AreEqual(val, 0.0);
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }
  }

  /// <summary>
  /// Matrix x Vector
  /// </summary>
  [Test]
  public static void Product_MatrixVector()
  {
    // ----------------
    // bad
    // ----------------
    {
      var mat = new clsEasyMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      var v = new clsEasyVector(new double[] {2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // bad
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 0}, new double[] {0, 1}, new double[] {0, 0}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // bad
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 0, 0}, new double[] {0, 0, 1}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // ----------------
    // OK
    // ----------------
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2},
        clsEasyVector.VectorDirection.Col);
      try
      {
        mat.PrintValue();
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction == clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyMatrix mat =
        new clsEasyMatrix(new[] {new double[] {1, 2, 3}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2},
        clsEasyVector.VectorDirection.Col);
      try
      {
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction == clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 12)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 0}, new double[] {1, 0}, new double[] {1, 0}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2},
        clsEasyVector.VectorDirection.Col);
      try
      {
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction == clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }
  }

  [Test]
  public static void Determinant()
  {
    clsEasyMatrix detMat = new clsEasyMatrix(new[]
    {
      new double[] {3, 1, 1, 2}, new double[] {5, 1, 3, 4},
      new double[] {2, 0, 1, 0}, new double[] {1, 3, 2, 1}
    });
    detMat.PrintValue();

    double d = detMat.Det();
    Console.WriteLine("Determinant:{0}", d);
    if (d != -22)
      Assert.Fail("Error");
  }

  /// <summary>
  /// Vector * Matrix
  /// </summary>
  [Test]
  public static void Product_VectorMatrix()
  {
    // ----------------
    // bad
    // ----------------
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      clsEasyVector v = new clsEasyVector(new double[] {2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // bad
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 0}, new double[] {0, 1}, new double[] {0, 0}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // bad
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 0, 0}, new double[] {0, 0, 1}});
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.Fail("Error");
    }

    // ----------------
    // OK
    // ----------------
    {
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      clsEasyMatrix mat = new clsEasyMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction != clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1}, new double[] {1}, new double[] {1}});

      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction != clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 6.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyVector v = new clsEasyVector(new double[] {2, 2, 2});
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {1, 1}, new double[] {1, 1}, new double[] {1, 1}});
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction != clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 6.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyVector v = new clsEasyVector(new double[] {2, 2});
      clsEasyMatrix mat = new clsEasyMatrix(new[] {new double[] {2, 2, 2}, new double[] {0, 0, 0}});
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.Fail("Error");

        // dirction check
        if (temp.Direction != clsEasyVector.VectorDirection.Row)
          Assert.Fail("Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 4.0)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Assert.Fail("Error");
      }
    }
  }

  /// <summary>
  /// Vector * Scalar
  /// </summary>
  [Test]
  public static void Product_VectorScalar()
  {
    {
      var v = new clsEasyVector(3);
      try
      {
        for (int i = 0; i <= v.Count - 1; i++) v[i] = 1;

        var temp = v * 2;

        foreach (double val in v)
        {
          if (val != 2) 
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
  }

  /// <summary>
  /// Scalar * Vector
  /// </summary>
  [Test]
  public static void Product_ScalarVector()
  {
    {
      var v = new clsEasyVector(3);
      try
      {
        for (int i = 0; i <= v.Count - 1; i++) v[i] = 1;

        var temp = v * 2;

        foreach (double val in v)
        {
          if (val != 2)
            Assert.Fail("Error");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
  }

  /// <summary>
  /// Matrix x scalar
  /// </summary>
  [Test]
  public static void Product_MatrixScalar()
  {
    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      try
      {
        for (int i = 0; i <= matA.RowCount - 1; i++)
        {
          for (int j = 0; j <= matA.ColCount - 1; j++)
            matA[i][j] = 1;
        }

        matA.PrintValue();

        var temp = matA * 2.0;
        temp.PrintValue();

        for (int i = 0; i <= matA.RowCount - 1; i++)
        {
          for (int j = 0; j <= matA.ColCount - 1; j++)
          {
            if (temp[i][j] != 2.0)
              Assert.Fail("Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.Fail("Error");
      }
    }
  }

  /// <summary>
  /// Scalar x Matrix
  /// </summary>
  [Test]
  public static void Product_ScalarMatrix()
  {
    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      try
      {
        for (int i = 0; i <= matA.RowCount - 1; i++)
        {
          for (int j = 0; j <= matA.ColCount - 1; j++) matA[i][j] = 1;
        }

        matA.PrintValue();

        var temp = 2.0 * matA;
        temp.PrintValue();

        for (int i = 0; i <= matA.RowCount - 1; i++)
        {
          for (int j = 0; j <= matA.ColCount - 1; j++)
          {
            if (temp[i][j] != 2.0)
              Assert.Fail("Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.Fail("Error");
      }
    }
  }


  /// <summary>
  /// Test Matrix x Matrix
  /// </summary>
  [Test]
  public static void Product_MatrixMatrix()
  {
    // ---------
    // bad
    // ---------
    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      var matB = new clsEasyMatrix(dimNum - 1, true);
      try
      {
        var temp = matA * matB;

        Assert.Fail("Error");
      }
      catch (clsException myex)
      {
      }
    }

    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum - 1);
      var matB = new clsEasyMatrix(dimNum, true);
      try
      {
        var temp = matA * matB;

        Assert.Fail("Error");
      }
      catch (clsException myex)
      {
      }
    }

    {
      clsEasyMatrix matA = new clsEasyMatrix(3, true);
      clsEasyMatrix matB = new clsEasyMatrix(new[] {new double[] {1, 1, 1}, new double[] {2, 2, 2}});
      try
      {
        var temp = matA * matB;

        Assert.Fail("Error");
      }
      catch (clsException myex)
      {
      }
    }

    // ---------
    // OK
    // ---------
    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      var matB = new clsEasyMatrix(dimNum, true);
      try
      {
        var temp = matA * matB;
        temp.PrintValue();

        if (clsMathUtil.IsNearyEqualMatrix(temp, matA) == false)
          Assert.Fail("Error");
      }
      catch (clsException myex)
      {
        Assert.Fail("Error");
      }
    }

    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      var matB = new clsEasyMatrix(dimNum);
      try
      {
        var temp = matA * matB;
        temp.PrintValue();

        if (clsMathUtil.IsNearyEqualMatrix(temp, matB) == false)
          Assert.Fail("Error");
      }
      catch (clsException myex)
      {
        Assert.Fail("Error");
      }
    }

    {
      clsEasyMatrix matA = new clsEasyMatrix(new[] {new double[] {1, 1, 1}, new double[] {2, 2, 2}});
      clsEasyMatrix matB = new clsEasyMatrix(3, true);
      try
      {
        var temp = matA * matB;
        temp.PrintValue();

        if (temp.RowCount != 2)
          Console.WriteLine("row error");

        if (temp.ColCount != 3)
          Console.WriteLine("col error");

        // check value
        for (int i = 0; i <= temp.RowCount - 1; i++)
        {
          for (int j = 0; j <= temp.ColCount - 1; j++)
          {
            if (i == 0)
            {
              if (temp[i][j] != 1)
                Assert.Fail("Error");
            }
            else if (temp[i][j] != 2)
              Assert.Fail("Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.Fail("Error");
      }
    }
  }

  /// <summary>
  /// Test Inverse
  /// </summary>
  [Test]
  public static void InverseMatrix()
  {
    {
      clsEasyMatrix mat = new clsEasyMatrix(new[]
      {
        new double[] {3, 1, 1}, new double[] {5, 1, 3}, new double[] {2, 0, 1}
      });
      // Inverse
      // 0.5	-0.5	1
      // 0.5	 0.5	-2
      // -1      1  	-1
      clsEasyMatrix matInv = mat.Inverse();

      // check Identy matrix
      // I = A * A^-1
      var productMat = mat * matInv;
      for (int i = 0; i <= productMat.RowCount - 1; i++)
      {
        for (int j = 0; j <= productMat.ColCount - 1; j++)
        {
          if (i == j)
          {
            if (productMat[i][j] < 0.9999 || productMat[i][j] > 1.0001)
              Assert.Fail("Error");
          }
          else if (productMat[i][j] < -0.0001 || productMat[i][j] > 0.0001)
            Assert.Fail("Error");
        }
      }
    }

    // 1*1 - 5*5
    {
      for (int dimNum = 1; dimNum <= 5; dimNum++)
      {
        var source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, 12345);
        source.PrintValue(name: "Source matrix");
        source.Inverse().PrintValue(name: "Inverse matrix");

        var product = source * source.Inverse();
        product.PrintValue(name: "S*S^-1");

        // check
        if (clsMathUtil.IsNearyEqualMatrix(product,
          new clsEasyMatrix(dimNum, true)))
        {
        }
        else
          Assert.Fail("Error");
      }
    }

    {
      var dimNum = 10;
      var source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, 12345);
      source.PrintValue(name: "Source matrix");
      source.Inverse().PrintValue(name: "Inverse matrix");

      var product = source * source.Inverse();
      product.PrintValue(name: "S*S^-1");

      // check
      if (clsMathUtil.IsNearyEqualMatrix(product,
        new clsEasyMatrix(dimNum, true)))
      {
      }
      else
        Assert.Fail("Error");
    }
  }

  /// <summary>
  /// Test Cholesky decomposition
  /// </summary>
  [Test]
  public static void Cholesky()
  {
    var tempMat = clsMathUtil.CreateRandomSymmetricMatrix(5);
    tempMat.PrintValue(name: "Source");

    var c = tempMat.Cholesky();
    tempMat.PrintValue(name: "Cholesky decomposition");

    // A = LL^T
    var check = c * c.T();

    if (clsMathUtil.IsNearyEqualMatrix(tempMat, check) == false)
      Assert.Fail("Error");
  }

  /// <summary>
  /// Test eigen()
  /// </summary>
  [Test]
  public static void Eigen()
  {
    var tempMat =
      clsMathUtil
        .CreateRandomSymmetricMatrix(5); // 8, 12345 is not calc?
    tempMat.PrintValue(name: "Source");

    clsEasyMatrix
      retM = null /* TODO Change to default(_) if this is not a reference type */;
    clsEasyVector
      retV = null /* TODO Change to default(_) if this is not a reference type */;
    clsEasyMatrix
      suspend =
        null /* TODO Change to default(_) if this is not a reference type */;
    clsEasyMatrix.Eigen(ref tempMat, ref retV, ref retM, 10000);
    var retD = retV.ToDiagonalMatrix();

    retM.PrintValue(name: "Eigen V");
    retD.PrintValue(name: "D");
    retM.T().PrintValue(name: "Eigen V^T");
    retM.Inverse().PrintValue(name: "Eigen V^-1");

    // check
    clsEasyMatrix
      temp = null /* TODO Change to default(_) if this is not a reference type */;
    temp = retM * retV.ToDiagonalMatrix() * retM.T();
    temp.PrintValue(name: "V*D*V^T");
    if (clsMathUtil.IsNearyEqualMatrix(tempMat, temp) == false)
      Assert.Fail("Error");

    // check
    temp = retM * retV.ToDiagonalMatrix() * retM.Inverse();
    temp.PrintValue(name: "V*D*V^-1");
    if (clsMathUtil.IsNearyEqualMatrix(tempMat, temp) == false)
      Assert.Fail("Error");
  }
}