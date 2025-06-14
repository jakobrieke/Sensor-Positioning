using System;
using System.Collections.Generic;
using LinearAlgebra;
using NUnit.Framework;
using Rectangle = LinearAlgebra.Rectangle;

namespace SensorPositioning
{
  [TestFixture]
  public static class Sensors2DTest
  {
    [Test]
    public static void TestBlockedArea()
    {
      Console.WriteLine("\nTest Shadows2D.HiddenArea()");
      
      var b1 = new Rectangle(-1, -1, 1, 1);
      var b2 = new Rectangle(0, 0, 9, 6);
      var o1 = new Circle(1.05481918857192, 0.629068343261754, 0.1555);

      var tests = new Dictionary<Polygon, string>
      {
        {
          Sensors2D.BlockedArea(
            new Vector2(0, 0), new Circle(0.5, .5, .1), b1),
          "1, 0.75; 1, 1; 0.75, 1; 0.42, 0.56; 0.56, 0.42; 1, 0.75"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-1, 0), new Circle(0.5, .5, .1), b1),
          "1, 0.5287523764459; 1, 0.81053333783982; 0.46244053232388, 0.5926" +
          "7840302836; 0.52555946767612, 0.40332159697164; 1, 0.5287523764459"
        },
        {
          // Test what happens if circle is in corner of bounds
          Sensors2D.BlockedArea(
            new Vector2(-1, -1), new Circle(0.5, .5, .1), b1),
          "1, 0.81973553481771; 1, 1; 0.81973553481771, 1; 0.42603459966528," +
          " 0.56729873366806; 0.56729873366806, 0.42603459966528; 1, 0.81973" +
          "553481771"
        },
        {
          // Test what happens if sensor is inside obstacle
          Sensors2D.BlockedArea(
            new Vector2(-0.9, -0.85), new Circle(-0.9, -0.9, .1), b1),
          b1.ToPolygon().ToString()
        },
        {
          // Test what happens if obstacle is on borders of bounds
          Sensors2D.BlockedArea(
            new Vector2(.1, .1), new Circle(-1, 0, .1), b1),
          "-0.98196721311475, -0.0983606557377; -1, 0.1; -1, -0.101666666666" +
          "66; -0.98196721311475, -0.0983606557377"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-1, -1), new Circle(1, 1, .1), b1),
          "1, 1; 0.995, 1; 1, 0.995; 1, 1"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-1, -1), new Circle(-1, 0, .1), b1),
          "-0.79899243694816, 1; -1, 1; -1, -0.01; -0.90050125628934, -0.01;" +
          " -0.79899243694816, 1"
        },
        {
          // Test what happens if sensor is inside 2x radius of circle
          Sensors2D.BlockedArea(
            new Vector2(-.90001, 0), new Circle(-.8, 0, .1), b1),
          "1, 1; -0.88586751082734, 1; -0.8999900009999, 0.00141410750651; -" +
          "0.8999900009999, -0.00141410750651; -0.88586751082734, -1; 1, -1;" +
          " 1, 1"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-.90001, -.05), new Circle(-.8, 0, .1), b1),
          "1, 1; -0.89980004198321, 1; -0.8999999980008, 1.99960012E-05; -0." +
          "85999040225524, -0.08000719740908; 0.36697329534852, -1; 1, -1; 1, 1"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.5, .61), new Circle(.5, .5, .1), b1),
          "1, 0.38087121525221; 0.54165977904505, 0.59090909090909; 0.458340" +
          "22095495, 0.59090909090909; -1, -0.07738635424338; -1, -1; 1, -1;" +
          " 1, 0.38087121525221"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.5, .39), new Circle(.5, .5, .1), b1),
          "1, 0.61912878474779; 1, 1; -0.83112913043955, 1; 0.45834022095495" +
          ", 0.40909090909091; 0.54165977904505, 0.40909090909091; 1, 0.6191" +
          "2878474779"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.61, .5), new Circle(.5, .5, .1), b1),
          "0.59090909090909, 0.45834022095495; 0.59090909090909, 0.541659779" +
          "04505; 0.38087121525221, 1; -1, 1; -1, -1; -0.07738635424338, -1;" +
          " 0.59090909090909, 0.45834022095495"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.39, .5), new Circle(.5, .5, .1), b1),
          "1, 1; 0.61912878474779, 1; 0.40909090909091, 0.54165977904505; 0." +
          "40909090909091, 0.45834022095495; 1, -0.83112913043955; 1, 1"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.61, .61), new Circle(.5, .5, .1), b1),
          "0.59961988767188, 0.49128920323721; 0.49128920323721, 0.599619887" +
          "67188; -1, 0.46922105398996; -1, -1; 0.46922105398996, -1; 0.5996" +
          "1988767188, 0.49128920323721"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(.65, .6), new Circle(.5, .5, .1), b1),
          "0.59230769230769, 0.46153846153846; 0.5, 0.6; -1, 0.6; -1, -1; -0" +
          ".01666666666667, -1; 0.59230769230769, 0.46153846153846"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-.90001, -.1), new Circle(.5, .5, .1), b1),
          "1, 0.57037064649587; 1, 0.86654923360652; 0.45465884136027, 0.589" +
          "13012584534; 0.53327225257532, 0.40569752278671; 1, 0.57037064649587"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-.8, -.15), new Circle(.5, .5, .1), b1),
          "1, 0.6; 1, 0.91071428571429; 0.44923076923077, 0.58615384615385; " +
          "0.53846153846154, 0.40769230769231; 1, 0.6"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(-.78, -.15), new Circle(.5, .5, .1), b1),
          "1, 0.60291503742684; 1, 0.91599923425088; 0.44862134807331, 0.585" +
          "79180687103; 0.5389568944421, 0.40790026940633; 1, 0.60291503742684"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(0.549550101745069, 6), o1, b2),
          "1.20915148573974, 0.64808917528625; 0.89964726200985, 0.618972640" +
          "8584; 0.9399184870721, 0; 1.28902585789796, 0; 1.20915148573974, " +
          "0.64808917528625"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(0.549550101745069, 5), o1, b2),
          "1.20856297484783, 0.65237281024876; 0.89981328266531, 0.616682095" +
          "48166; 0.94909125818183, 0; 1.30744957695337, 0; 1.20856297484783" +
          ", 0.65237281024876"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(0, 1), o1, b2),
          "1.08550270971766, 0.7815110260317; 0.98333403274372, 0.4909737289" +
          "7062; 1.93179426821167, 0; 4.96822649675292, 0; 1.08550270971766," +
          " 0.7815110260317"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(2, 1), o1, b2),
          "1.13312477163057, 0.49472369073756; 1.02085048276346, 0.780812789" +
          "7028; 0, 0.55229062275225; 0, 0; 0.28435409996794, 0; 1.133124771" +
          "63057, 0.49472369073756"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(0, 0), o1, b2),
          "9, 3.92021175789384; 9, 6; 7.65629401123085, 6; 0.95890279552899," +
          " 0.75146236086733; 1.11691658069126, 0.48650550135694; 9, 3.92021" +
          "175789384"
        },
        {
          Sensors2D.BlockedArea(
            new Vector2(0.549550101745069, 0), o1, b2),
          "7.67054143891798, 6; 3.6942538580168, 6; 0.91708981201415, 0.7012" +
          "5469123012; 1.15501527598559, 0.51015243151319; 7.67054143891798, 6"
        },
//        {
//          // Test what happens if sensor is on the edge of the obstacle
//          // Not implemented
//          Shadows2D.HiddenArea(new Vector2(.1, 0), new Circle(0, 0, .1), b1), 
//          ""
//        }
      };

      
      var i = 0;
      foreach (var test in tests.Keys)
      {
        i++;
        Console.WriteLine(i + ": " + (test.ToString() == tests[test]));
      }
    }
    
    [Test]
    public static void TestIntersecting()
    {
      var arc = new Arc(0, 0, 1, 90, 0);
      
      var c1 = new Circle(-.7, 0, 1);
      Console.WriteLine(Sensors2D.Intersecting(arc, c1));
      Assert.True(Sensors2D.Intersecting(arc, c1));

      var c2 = new Circle(-1, 0, 1);
      Console.WriteLine(Sensors2D.Intersecting(arc, c2));
      Assert.True(Sensors2D.Intersecting(arc, c2));
      
      var c3 = new Circle(-1, 0, .5);
      Console.WriteLine(Sensors2D.Intersecting(arc, c3));
      Assert.False(Sensors2D.Intersecting(arc, c3));
    }
  }
}