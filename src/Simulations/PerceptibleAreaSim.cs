using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using charlie;
using Geometry;
using Rectangle = Geometry.Rectangle;

namespace sensor_positioning.Simulations
{
  public class PerceptibleAreaSim : AbstractSimulation
  {
    private double _zoom = 80;
    private Arc _s1;
    private Circle _o1;
    private Circle _o2;
    private Circle _o3;
    private List<Circle> _obstacles;
    private Rectangle _field;
    private int _samples;
    private double _totalPerceptible;
    private double _totalNotPerceptible;
    private int _measurementsCount;
    
    public override string GetTitle()
    {
      return "Perceptible Area";
    }

    public override string GetDescr()
    {
      return "Test which method is faster:\n" +
             "- Sensors2D.Perceptible(..) or\n" +
             "- Sensors2D.NotPerceptible(..)";
    }

    public override string GetConfig()
    {
      return "SensorArcPrecision = 10\n" +
             "SamplesPerUpdate = 500";
    }

    public override void Init(Dictionary<string, string> config)
    {
      Sensors2D.SensorArcPrecision = GetInt(
        config, "SensorArcPrecision", 10);
      _samples = GetInt(config, "SamplesPerUpdate", 10);
      
      _s1 = new Arc(4, 6, 4, 360, -135);
      _o1 = new Circle(5, 5, 0.2);
      _o2 = new Circle(3.5, 5, 0.2);
      _o3 = new Circle(4, 5, 0.2);
      _obstacles = new List<Circle> {_o1, _o2, _o3};
      _field = new Rectangle(0, 0, 5, 7);
      _totalPerceptible = 0;
      _totalNotPerceptible = 0;
      _measurementsCount = 0;
    }

    public override void Update(long deltaTime)
    {
      double area01 = 0, area02 = 0;
      var timer = new Stopwatch();

      timer.Start();
      for (var i = 0; i < _samples; i++)
      {
        var area = Polygon.Difference(
          _field.ToPolygon(), 
          Sensors2D.Perceptible(_s1, _obstacles));
        area01 = Polygon.Area(area);
      }
      timer.Stop();
      _totalPerceptible += timer.ElapsedMilliseconds;
      
      timer.Restart();
      for (var i = 0; i < _samples; i++)
      {
        var area = Sensors2D.Imperceptible(_s1, _obstacles, _field);
        area02 = Polygon.Area(area);
      }
      timer.Stop();
      _totalNotPerceptible += timer.ElapsedMilliseconds;
      
      var diff = area01 - area02;
      if (Math.Abs(diff) > 0.0000000001) throw new Exception(
        $"Areas should be equal but size difference is {diff}");
      
      _measurementsCount++;
    }

    // Can't draw polygons with holes inside
    private void DrawPolygons(Context cr, List<Polygon> polygons)
    {
      polygons.ForEach(p => DrawPolygon(cr, p));
    }
    
    // Can't draw polygons with holes inside
    private void DrawPolygon(Context cr, Polygon polygon)
    {
      cr.NewPath();
      cr.MoveTo(polygon[0].X * _zoom, polygon[0].Y * _zoom);
      for (var i = 1; i < polygon.Count; i++)
      {
        cr.LineTo(polygon[i].X * _zoom, polygon[i].Y * _zoom);
      }
      cr.ClosePath();
      cr.Fill();
    }
    
    public override void Render(Context cr, int width, int height)
    {
      var perceptible = Sensors2D.Perceptible(_s1, _obstacles);
      var area = Polygon.Difference(_field.ToPolygon(), perceptible);

      cr.SetSourceRGBA(1, 1, 1, 0.3);
//      DrawPolygon(cr, s1.ToPolygon(10));
      _obstacles.ForEach(o => DrawPolygon(cr, o.ToPolygon(20)));
//      cr.SetSourceRGBA(0.3, 0.3, 1, 0.3);
//      DrawPolygon(cr, perceptible);
      cr.SetSourceRGBA(1, 1, 1, 0.3);
      DrawPolygons(cr, area);
    }

    public override string Log()
    {
      return "Average perceptible: " +
             $"{_totalPerceptible / _measurementsCount}\n" +
             "Average not perceptible: " +
             $"{_totalNotPerceptible / _measurementsCount}";
    }
  }
}