using System;
using System.Collections.Generic;
using Cairo;
using charlie;
using Geometry;
using Optimization;

namespace sensor_positioning
{
  public class MovingObstacle : Obstacle
  {
    public double Velocity;
    
    public MovingObstacle(Vector2 position, double size) : base(position, size)
    {
    }

    public MovingObstacle(Vector2 position, 
      double size, double rotation) : base(position, size, rotation)
    {
    }
  }
  
  public class DynamicSpSimulation : AbstractSimulation
  {
    public override string GetTitle()
    {
      return "Dynamic Sensor Positioning";
    }

    public override string GetDescr()
    {
      return "";
    }

    public override string GetConfig()
    {
      return "SizeTeamA = 1\n" + 
             "SizeTeamB = 1\n" +
             "ObstacleVelocity = 0.1\n" +
             "FieldHeight = 9\n" +
             "FieldWidth = 6\n";
    }

    private List<MovingObstacle> _obstacles;
    private Environment _env;
    
    public bool CheckCollision(Obstacle o)
    {
      foreach (var o2 in _obstacles)
      {
        if (o == o2) continue;

        var d = Vector2.Distance(o2.Position, o.Position);
        if (d < o2.Size + o.Size) return true;
      }

      return false;
    }
    
    public void PlaceRandom(Obstacle o)
    {
      o.Position = new Vector2(
        Pso.UniformRand(_env.Bounds.Min.X,_env.Bounds.Max.X), 
        Pso.UniformRand(_env.Bounds.Min.Y,_env.Bounds.Max.Y));
    }
    
    public void PlaceWithoutCollision(Obstacle o)
    {
      var collided = true;
      while (collided)
      {
        PlaceRandom(o);
        collided = CheckCollision(o);
      }
    }
    
    public override void Init(Dictionary<string, string> model)
    {
      var fieldWidth = GetDouble(model, "FieldHeight", 9);
      var fieldHeight = GetDouble(model, "FieldWidth", 6);
      var sizeTeamB = GetInt(model, "SizeTeamB", 1);
      var velocity = GetDouble(model, "ObstacleVelocity", 0.1);
      
      _env = new Environment(0, 0, fieldWidth, fieldHeight);
      _obstacles = new List<MovingObstacle>(sizeTeamB);

      for (var i = 0; i < sizeTeamB; i++)
      {
        var obstacle = new MovingObstacle(Vector2.Zero, 0.1555)
        {
          Velocity = velocity,
          Rotation = Pso.UniformRand(0, 360)
        };
        PlaceWithoutCollision(obstacle);
        _obstacles.Add(obstacle);
      }
    }

    public static double Reflect(Vector2 position, Vector2 reflectionPoint, 
      Segment reflectionAxis)
    {
      var angle = Vector2.Gradient(position, reflectionPoint);
      var reflAngle = Vector2.Gradient(
        reflectionAxis.Start, reflectionAxis.End);
      
      return angle + reflAngle;
    }
    
    public override void Update(long deltaTime)
    {
      foreach (var obstacle in _obstacles)
      {
        var position = obstacle.Position.Move(
          obstacle.Rotation, obstacle.Velocity);
        var x = position.X;
        var y = position.Y;
        var angle = Vector2.Gradient(obstacle.Position, position);
        
        // Collision with X bounds
        if (x > _env.Bounds.Max.X)
        {
          x = _env.Bounds.Max.X;
          
          var lowerRight = new Vector2(_env.Bounds.Max.X, _env.Bounds.Min.Y);
          obstacle.Rotation = Reflect(obstacle.Position, position,
            new Segment(_env.Bounds.Max, lowerRight));
        }
        else if (x < _env.Bounds.Min.X)
        {
          x = _env.Bounds.Min.X;
          
          var upperLeft = new Vector2(_env.Bounds.Min.X, _env.Bounds.Max.Y);
          obstacle.Rotation = Reflect(obstacle.Position, position,
            new Segment(_env.Bounds.Max, upperLeft));
        }
        
        // Collision with Y bounds
        if (y > _env.Bounds.Max.Y)
        {
          y = _env.Bounds.Max.Y;
          obstacle.Rotation = - angle;
        }
        else if (y < _env.Bounds.Min.Y)
        {
          y = _env.Bounds.Min.Y;
          obstacle.Rotation = - angle;
        }
        
        obstacle.Position = new Vector2(x, y);

//        foreach (var o2 in _obstacles)
//        {
//          if (obstacle == o2) continue;
//
//          var d = Vector2.Distance(o2.Position, obstacle.Position);
//          if (d < o2.Size + obstacle.Size)
//          {
//            
//          }
//        }
      }
    }

    private static void DrawCoordinateSystem(Context ctx, 
      double width, double height)
    {
      ctx.SetSourceRGB(.8, .8, .8);
      ctx.Rectangle(0, 0, width, height);
      ctx.ClosePath();
      ctx.Fill();

      ctx.SetSourceRGBA(0, 0, 0, 0.2);
      ctx.LineWidth = .01;
      for (var i = 0.0; i < width; i += width / 10)
      {
        ctx.NewPath();
        ctx.MoveTo(i, 0);
        ctx.LineTo(i, height);
        ctx.ClosePath();
        ctx.Stroke();
      }

      for (var i = 0.0; i < height; i += height / 10)
      {
        ctx.NewPath();
        ctx.MoveTo(0, i);
        ctx.LineTo(width, i);
        ctx.ClosePath();
        ctx.Stroke();
      }
    }
    
    private static void DrawObstacles(Context ctx, 
      IEnumerable<Obstacle> obstacles)
    {
      foreach (var obstacle in obstacles)
      {
        ctx.SetSourceRGB(0.1, 0.1, 1);
        ctx.Arc(
          obstacle.Position.X,
          obstacle.Position.Y,
          obstacle.Size, 0, 2 * Math.PI);
        ctx.ClosePath();
      }

      ctx.Fill();
    }
    
    public override void Render(Context cr, int width, int height)
    {
      var fieldWidth = _env.Bounds.Max.X * 80;
      var fieldHeight = _env.Bounds.Max.Y * 80;
      cr.Translate((width - fieldWidth) / 2, (width - fieldHeight) / 2);
      cr.Scale(80, 80);
      
      DrawCoordinateSystem(cr, _env.Bounds.Max.X, _env.Bounds.Max.Y);
      DrawObstacles(cr, _obstacles);
    }
  }
}