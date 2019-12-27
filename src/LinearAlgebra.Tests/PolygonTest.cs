using System;
using System.Linq;
using ClipperLib;
using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public class PolygonTest
  {
    private readonly Polygon _a = new Polygon(new[]
    {
      new Vector2(0, 0), new Vector2(1, 0),
      new Vector2(1, 1), new Vector2(0, 1)
    });

    private readonly Polygon _b = new Polygon(new[]
    {
      new Vector2(0.1, 0.1), new Vector2(0.1, 0.5),
      new Vector2(0.5, 0.5), new Vector2(0.5, 0.1)
    });

    private readonly Polygon _c = new Polygon(new[]
    {
      new Vector2(0, 0), new Vector2(-1, 0),
      new Vector2(-1, -1), new Vector2(0, -1)
    });
    
    private readonly Polygon _d = new Polygon(new[]
    {
      new Vector2(0.5, 0.5), new Vector2(0.5, 0.9),
      new Vector2(0.9, 0.9), new Vector2(0.9, 0.5)
    });

    [Test]
    public void TestArea()
    {
      // The calculation for Polygon B is as follows:
      
      // (0,1*0,5 - 0,1*0,1
      // + 0,1*0,5 - 0,5*0,5
      // + 0,5*0,1 - 0,5*0,5
      // + 0,5*0,1 - 0,1*0,1) / 2
      
      Assert.AreEqual(Polygon.Area(_b), 0.16);
      Assert.AreEqual(Polygon.Area(_c), 1);
    }
  }
}