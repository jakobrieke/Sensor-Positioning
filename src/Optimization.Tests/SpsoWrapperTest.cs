using NUnit.Framework;

namespace Optimization
{
  [TestFixture]
  public class SpsoWrapperTest
  {
    [Test] public static void TestConstructor()
    {
      var obj = new SphereFunction();
      var bounds = new SearchSpace(3, 5).Intervals;
      var pso = new Spso2006Wrapper(obj, bounds);
      Assert.Pass("Construction ran without exception.");
    }

    [Test] public static void TestInit()
    {
      var obj = new SphereFunction();
      var bounds = new SearchSpace(3, 5).Intervals;
      var pso = new Spso2006Wrapper(obj, bounds);
      pso.Init();
      Assert.Pass("Initialization ran without exception.");
    }
    
    [Test] public static void TestUpdate()
    {
      var obj = new SphereFunction();
      var bounds = new SearchSpace(3, 5).Intervals;
      var pso = new Spso2006Wrapper(obj, bounds);
      pso.Init();
      pso.Iterate();
      Assert.Pass("Update ran without exception.");
    }
  }
}