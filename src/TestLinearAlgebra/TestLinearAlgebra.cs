using System;
using LOMath;
using NUnit.Framework;

public static class TestVector
{
  [Test]
  public static void TestCreateVector()
  {
    var v1 = new LoVector(3);
    for (var i = 0; i <= 3 - 1; i++)
    {
      Assert.AreEqual(v1[i], Convert.ToDouble(0));
    }

    var v2 = new LoVector(new double[] {1, 2, 3});
    for (var i = 0; i <= 3 - 1; i++)
    {
      Assert.AreEqual(v2[i], Convert.ToDouble(i + 1));
    }
  }

  [Test]
  public static void TestCopyVector()
  {
    var v1 = new LoVector(3);
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
      var mat = new LoMatrix(new[]
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
      var mat = new LoMatrix(3, true);

      if (mat.RowCount != 3) Assert.True(false, "Row Error");

      if (mat.ColCount != 3) Assert.True(false, "Col Error");

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
      var v = new LoVector(new double[] {1, 1, 1});
      var matV = new LoMatrix(new[]
      {
        new double[] {1}, 
        new double[] {2}, 
        new double[] {3}
      });
      try
      {
        var result = v + matV;
        Assert.True(
          result[0] == 2.0 && result[1] == 3.0 && result[2] == 4.0,
          "Error");
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    // ------------------------
    // Bad
    // ------------------------
    {
      var v = new LoVector(new double[] {1, 1, 1});
      var matV = new LoMatrix(new[] {new double[] {1, 2, 3}});
      try
      {
        v.PrintValue(name: "v");
        matV.PrintValue(name: "matV");

        v.Direction = LoVector.VectorDirection.Col;
        var result = v + matV;

        // error
        Assert.True(false, "Error");
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
        var v1 = new LoMatrix(4, true);
        var v2 = new LoMatrix(5, true);
        var temp = v1 + v2;

        Assert.True(false, "Error");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    {
      try
      {
        var v1 = new LoMatrix(5, true);
        var v2 = new LoMatrix(5, true);
        var temp = v1 + v2;
        var diagonal = temp.ToVectorFromDiagonal();
        
        foreach (var val in diagonal) Assert.AreEqual(2.0, val);
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }
  }

  /// <summary>
  /// Matrix + vector
  /// </summary>
  [Test]
  public static void Add_MatrixVector()
  {
    LoVector v = new LoVector(new double[] {1, 1, 1});
    LoMatrix matV = new LoMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = matV + v;
      if (v[0] == 2.0 && v[1] == 3.0 && v[2] == 4.0)
      {
      }
      else
        Assert.True(false, "Error");
    }
    catch (Exception ex)
    {
      Assert.True(false, "Error");
    }
  }

  /// <summary>
  /// Vector - Matrix
  /// </summary>
  [Test]
  public static void Sub_VectorMatrix()
  {
    LoVector v = new LoVector(new double[] {1, 1, 1});
    LoMatrix matV = new LoMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = v - matV;
      if (v[0] == 0 && v[1] == -1.0 && v[2] == -2.0)
      {
      }
      else
        Assert.True(false, "Error");
    }
    catch (Exception ex)
    {
      Assert.True(false, "Error");
    }
  }

  /// <summary>
  /// Matrix - Vector
  /// </summary>
  [Test]
  public static void Sub_MatrixVector()
  {
    LoVector v = new LoVector(new double[] {1, 1, 1});
    LoMatrix matV = new LoMatrix(new[] {new double[] {1}, new double[] {2}, new double[] {3}});
    try
    {
      v = matV - v;
      if (v[0] == 0.0 && v[1] == 1.0 && v[2] == 2.0)
      {
      }
      else
        Assert.True(false, "Error");
    }
    catch (Exception ex)
    {
      Assert.True(false, "Error");
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
        var v1 = new LoMatrix(4, true);
        var v2 = new LoMatrix(5, true);
        var temp = v1 - v2;

        Assert.True(false, "Error");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    {
      try
      {
        var v1 = new LoMatrix(5, true);
        var v2 = new LoMatrix(5, true);
        var temp = v1 - v2;
        var diag = temp.ToVectorFromDiagonal();
        foreach (double val in diag)
          Assert.AreEqual(val, 0.0);
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
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
      var mat = new LoMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      var v = new LoVector(new double[] {2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // bad
    {
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 0}, new double[] {0, 1}, new double[] {0, 0}});
      LoVector v = new LoVector(new double[] {2, 2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // bad
    {
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 0, 0}, new double[] {0, 0, 1}});
      LoVector v = new LoVector(new double[] {2, 2, 2});
      try
      {
        var temp = mat * v;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // ----------------
    // OK
    // ----------------
    {
      LoMatrix mat = new LoMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      LoVector v = new LoVector(new double[] {2, 2, 2},
        LoVector.VectorDirection.Col);
      try
      {
        mat.PrintValue();
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction == LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoMatrix mat =
        new LoMatrix(new[] {new double[] {1, 2, 3}});
      LoVector v = new LoVector(new double[] {2, 2, 2},
        LoVector.VectorDirection.Col);
      try
      {
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction == LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 12)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 0}, new double[] {1, 0}, new double[] {1, 0}});
      LoVector v = new LoVector(new double[] {2, 2},
        LoVector.VectorDirection.Col);
      try
      {
        var temp = mat * v;

        // size check
        if (temp.Count != mat.RowCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction == LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }
  }

  [Test]
  public static void Determinant()
  {
    LoMatrix detMat = new LoMatrix(new[]
    {
      new double[] {3, 1, 1, 2}, new double[] {5, 1, 3, 4},
      new double[] {2, 0, 1, 0}, new double[] {1, 3, 2, 1}
    });
    detMat.PrintValue();

    double d = detMat.Det();
    Console.WriteLine("Determinant:{0}", d);
    if (d != -22)
      Assert.True(false, "Error");
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
      LoMatrix mat = new LoMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      LoVector v = new LoVector(new double[] {2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // bad
    {
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 0}, new double[] {0, 1}, new double[] {0, 0}});
      LoVector v = new LoVector(new double[] {2, 2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // bad
    {
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 0, 0}, new double[] {0, 0, 1}});
      LoVector v = new LoVector(new double[] {2, 2, 2});
      try
      {
        var temp = v * mat;
      }
      catch (clsException myex)
      {
        // OK
        return;
      }

      Assert.True(false, "Error");
    }

    // ----------------
    // OK
    // ----------------
    {
      LoVector v = new LoVector(new double[] {2, 2, 2});
      LoMatrix mat = new LoMatrix(new[]
      {
        new double[] {1, 0, 0}, new double[] {0, 1, 0}, new double[] {0, 0, 1}
      });
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction != LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 2.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoVector v = new LoVector(new double[] {2, 2, 2});
      LoMatrix mat = new LoMatrix(new[] {new double[] {1}, new double[] {1}, new double[] {1}});

      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction != LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 6.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoVector v = new LoVector(new double[] {2, 2, 2});
      LoMatrix mat = new LoMatrix(new[] {new double[] {1, 1}, new double[] {1, 1}, new double[] {1, 1}});
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction != LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 6.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoVector v = new LoVector(new double[] {2, 2});
      LoMatrix mat = new LoMatrix(new[] {new double[] {2, 2, 2}, new double[] {0, 0, 0}});
      try
      {
        var temp = v * mat;

        // size check
        if (temp.Count != mat.ColCount)
          Assert.True(false, "Error");

        // dirction check
        if (temp.Direction != LoVector.VectorDirection.Row)
          Assert.True(false, "Error");

        // value check
        foreach (double val in temp)
        {
          if (val != 4.0)
            Assert.True(false, "Error");
        }
      }
      catch (Exception ex)
      {
        Assert.True(false, "Error");
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
      var v = new LoVector(3);
      try
      {
        for (int i = 0; i <= v.Count - 1; i++) v[i] = 1;

        var temp = v * 2;

        foreach (double val in v)
        {
          if (val != 2) 
            Assert.True(false, "Error");
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
      var v = new LoVector(3);
      try
      {
        for (int i = 0; i <= v.Count - 1; i++) v[i] = 1;

        var temp = v * 2;

        foreach (double val in v)
        {
          if (val != 2)
            Assert.True(false, "Error");
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
              Assert.True(false, "Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.True(false, "Error");
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
              Assert.True(false, "Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.True(false, "Error");
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
      var matB = new LoMatrix(dimNum - 1, true);
      try
      {
        var temp = matA * matB;

        Assert.True(false, "Error");
      }
      catch (clsException myex)
      {
      }
    }

    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum - 1);
      var matB = new LoMatrix(dimNum, true);
      try
      {
        var temp = matA * matB;

        Assert.True(false, "Error");
      }
      catch (clsException myex)
      {
      }
    }

    {
      LoMatrix matA = new LoMatrix(3, true);
      LoMatrix matB = new LoMatrix(new[] {new double[] {1, 1, 1}, new double[] {2, 2, 2}});
      try
      {
        var temp = matA * matB;

        Assert.True(false, "Error");
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
      var matB = new LoMatrix(dimNum, true);
      try
      {
        var temp = matA * matB;
        temp.PrintValue();

        if (clsMathUtil.IsNearyEqualMatrix(temp, matA) == false)
          Assert.True(false, "Error");
      }
      catch (clsException myex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      var dimNum = 3;
      var matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum);
      var matB = new LoMatrix(dimNum);
      try
      {
        var temp = matA * matB;
        temp.PrintValue();

        if (clsMathUtil.IsNearyEqualMatrix(temp, matB) == false)
          Assert.True(false, "Error");
      }
      catch (clsException myex)
      {
        Assert.True(false, "Error");
      }
    }

    {
      LoMatrix matA = new LoMatrix(new[] {new double[] {1, 1, 1}, new double[] {2, 2, 2}});
      LoMatrix matB = new LoMatrix(3, true);
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
                Assert.True(false, "Error");
            }
            else if (temp[i][j] != 2)
              Assert.True(false, "Error");
          }
        }
      }
      catch (clsException myex)
      {
        Assert.True(false, "Error");
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
      LoMatrix mat = new LoMatrix(new[]
      {
        new double[] {3, 1, 1}, new double[] {5, 1, 3}, new double[] {2, 0, 1}
      });
      // Inverse
      // 0.5	-0.5	1
      // 0.5	 0.5	-2
      // -1      1  	-1
      LoMatrix matInv = mat.Inverse();

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
              Assert.True(false, "Error");
          }
          else if (productMat[i][j] < -0.0001 || productMat[i][j] > 0.0001)
            Assert.True(false, "Error");
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
          new LoMatrix(dimNum, true)))
        {
        }
        else
          Assert.True(false, "Error");
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
      var result = clsMathUtil.IsNearyEqualMatrix(product,
        new LoMatrix(dimNum, true));
      
      Assert.True(result);
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

    Assert.True(clsMathUtil.IsNearyEqualMatrix(tempMat, check));
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

    LoMatrix
      retM = null /* TODO Change to default(_) if this is not a reference type */;
    LoVector
      retV = null /* TODO Change to default(_) if this is not a reference type */;
    LoMatrix
      suspend =
        null /* TODO Change to default(_) if this is not a reference type */;
    LoMatrix.Eigen(ref tempMat, ref retV, ref retM, 10000);
    var retD = retV.ToDiagonalMatrix();

    retM.PrintValue(name: "Eigen V");
    retD.PrintValue(name: "D");
    retM.T().PrintValue(name: "Eigen V^T");
    retM.Inverse().PrintValue(name: "Eigen V^-1");

    // check
    LoMatrix
      temp = null /* TODO Change to default(_) if this is not a reference type */;
    temp = retM * retV.ToDiagonalMatrix() * retM.T();
    temp.PrintValue(name: "V*D*V^T");
    if (clsMathUtil.IsNearyEqualMatrix(tempMat, temp) == false)
      Assert.True(false, "Error");

    // check
    temp = retM * retV.ToDiagonalMatrix() * retM.Inverse();
    temp.PrintValue(name: "V*D*V^-1");
    if (clsMathUtil.IsNearyEqualMatrix(tempMat, temp) == false)
      Assert.True(false, "Error");
  }
}