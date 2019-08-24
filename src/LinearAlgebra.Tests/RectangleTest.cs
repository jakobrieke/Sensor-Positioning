using NUnit.Framework;

namespace LinearAlgebra
{
  [TestFixture]
  public static class RectangleTest
  {
    [Test]
    public static void TestOnBounds()
    {
      var bounds = new Rectangle(Vector2.One * 2, Vector2.One);

      Assert.True(Rectangle.OnBounds(bounds, 
        new Vector2(bounds.Min.X, 10)));
      
      Assert.False(Rectangle.OnBounds(bounds, 
        new Vector2(bounds.Min.X + 0.1, 10)));
    }
  }
}