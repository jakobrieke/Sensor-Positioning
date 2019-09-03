using System;
using System.Collections.Generic;
using System.Linq;
using LinearAlgebra;
using Optimization;

namespace SensorPositioning
{
  public class ToManyAttempts : Exception
  {
    public ToManyAttempts(string message) : base(message) {}
  }
  
  public class SensorPositionObj : Objective
  {
    /// <summary>
    /// Gets the number of evaluations that were done on this function.
    /// </summary>
    public uint Evaluations { get; private set; }
    
    /// <summary>
    /// Gets the list of obstacles.
    /// Note that this list can be modified between evaluations of this
    /// function.
    /// </summary>
    public readonly List<Circle> Obstacles = new List<Circle>();
    
    /// <summary>
    /// Gets a list of polygons marking interesting areas on the field.
    /// These areas are valued higher if they are under perception by an agent.
    /// </summary>
    public readonly List<Polygon> InterestingAreas = new List<Polygon>();
    
    /// <summary>
    /// Gets a list of polygons which are marked as known on the field.
    /// This means these areas are assumed to be known to the agents and
    /// therefor always added to the perceived area of the sensor.
    /// </summary>
    public readonly List<Polygon> KnownAreas = new List<Polygon>();
    
    /// <summary>
    /// Gets the area that is examined by the sensors.
    /// </summary>
    public readonly Rectangle Field;
    
    /// <summary>
    /// Gets the width of the examined area.
    /// </summary>
    public readonly double FieldWidth;
    
    /// <summary>
    /// Gets the height of the examined area.
    /// </summary>
    public readonly double FieldHeight;
    
    /// <summary>
    /// Gets the range of an agents sensor which is used to create new
    /// instances of agents when this instance is evaluated. 
    /// </summary>
    public readonly double SensorRange;
    
    /// <summary>
    /// Gets the field of view of an agents sensor which is used to create new
    /// instances of agents when this instance is evaluated. 
    /// </summary>
    public readonly double SensorFov;
    
    /// <summary>
    /// Gets the size of an agents body.
    /// </summary>
    public readonly double ObjectSize;

    /// <summary>
    /// Gets the start position which is used as a reference point to value
    /// each agents position and rotation by it's distance to that reference
    /// point.
    /// It is ignored if set to null.
    /// Note that the length of StartPosition should equal the length of agents
    /// that are placed on evaluation of this instance.
    /// </summary>
    public List<Agent> StartPosition;
    
    /// <summary>
    /// Gets or sets the weight by which the distance to the start position is
    /// taken into account.
    /// </summary>
    public double StartPositionDistanceWeight = 0.5;
    
    /// <summary>
    /// Gets or sets the weight by which the distance to the start position is
    /// taken into account.
    /// </summary>
    public double StartPositionRotationWeight = 0.5;

    /// <summary>
    /// Gets or sets the function which is applied agents in a placement
    /// collide/intersect with each other or an obstacle. 
    /// Possible values are:
    /// 0 -> Positive infinity is added,
    /// 1 -> The distance towards the center of the intersecting obstacle is
    /// added.
    /// </summary>
    public uint CollisionPenaltyFct = 0;

    /// <summary>
    /// Gets or sets the function which is applied if the center of an agents
    /// body is outside the examined field.
    /// Whereby the borders belong to the field.
    /// Possible values are:
    /// 0 -> Positive infinity is added, 
    /// 1 -> The distance towards the center is added
    /// </summary>
    public uint OutsideFieldPenaltyFct = 0;

    /// <summary>
    /// Gets or sets a random instance used to generate obstacle positions.
    /// </summary>
    public Random Random;

    public SensorPositionObj(
      double fieldWidth, double fieldHeight, 
      double sensorRange, double sensorFov,
      double objectSize)
    {
      FieldWidth = fieldWidth;
      FieldHeight = fieldHeight;
      Field = new Rectangle(0, 0, FieldWidth, FieldHeight);
      SensorRange = sensorRange;
      SensorFov = sensorFov;
      ObjectSize = objectSize;
      Random = new MathNet.Numerics.Random.MersenneTwister();
    }

    /// <summary>
    /// Generates a number of randomly not intersecting obstacles on the field
    /// and replaces the previous obstacles.
    /// </summary>
    /// <param name="numberOfObstacles"></param>
    public void SetObstaclesRandom(double numberOfObstacles)
    {
      Obstacles.Clear();

      for (var i = 0; i < numberOfObstacles; i++)
      {
        Obstacles.Add(WithoutIntersection(Obstacles));
      }
    }

    /// <summary>
    /// Replaces the current obstacles with a new list of obstacles.
    /// Their radius is defined by ObjectSize.
    /// </summary>
    /// <param name="positions"></param>
    public void SetObstacles(double[][] positions)
    {
      Obstacles.Clear();

      foreach (var pos in positions)
      {
        Obstacles.Add(new Circle(new Vector2(pos[0], pos[1]), ObjectSize));
      }
    }

    /// <summary>
    /// Places an obstacle randomly without intersection inside the environment.
    /// </summary>
    /// <remarks>
    /// After 10,000 attempts the 
    /// </remarks>
    /// <param name="obstacles"></param>
    private Circle WithoutIntersection(List<Circle> obstacles)
    {
      for (var i = 0; i < 10000; i++)
      {
        var obstacle = AtRandomPosition();
        if (!CheckCollision(obstacle, obstacles)) return obstacle;
      }
      throw new ToManyAttempts(
        "After 10,000 attempts no suitable position for a new " +
        "obstacle could be found.");
    }

    /// <summary>
    /// Generates a new obstacle at a random position.
    /// </summary>
    /// <returns></returns>
    private Circle AtRandomPosition()
    {
      return new Circle(
        RandomExtension.Uniform(Random, 0, FieldWidth),
        RandomExtension.Uniform(Random, 0, FieldHeight),
        ObjectSize);
    }

    /// <summary>
    /// Check if a given object collides with any object inside the
    /// problem environment. If the object already exists inside the environment
    /// it won't collide with itself.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="obstacles"></param>
    /// <returns></returns>
    public bool CheckCollision(Circle o, List<Circle> obstacles)
    {
      foreach (var o2 in obstacles)
      {
        if (o == o2) continue;

        var d = Vector2.Distance(o2.Position, o.Position);
        if (d < o2.Radius + o.Radius) return true;
      }

      return false;
    }

    /// <summary>
    /// Generate a list of agents from a vector with length n * 3, n >= 0. 
    /// </summary>
    /// <param name="vector"></param>
    public List<Agent> ToAgents(double[] vector)
    {
      var agents = new List<Agent>();
      for (var i = 0; i < vector.Length; i += 3)
      {
        var sensor = new Agent(
          vector[i], vector[i + 1], vector[i + 2], 
          SensorRange, SensorFov, ObjectSize);
        agents.Add(sensor);
      }

      return agents;
    }

    /// <summary>
    /// Calculates how many per cent of the fields area a given value is. 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public double PerCent(double value, bool round = true)
    {
      if (round) return Math.Round(value / Field.Area() * 100, 2);
      return value / Field.Area() * 100;
    }

    /// <summary>
    /// Creates search space intervals for a certain number of sensors, based
    /// on the instances field.
    /// </summary>
    /// <param name="numberOfSensors"></param>
    /// <returns></returns>
    public double[][] Intervals(uint numberOfSensors)
    {
      var intervals = new double[numberOfSensors * 3][];
      for (var i = 0; i < intervals.Length; i += 3)
      {
        intervals[i] = new[] {Field.Min.X, Field.Max.X};
        intervals[i + 1] = new[] {Field.Min.Y, Field.Max.Y};
        intervals[i + 2] = new[] {0, 360.0};
      }

      return intervals;
    }

    /// <summary>
    /// Creates a search space based on the instances field.  
    /// </summary>
    /// <param name="numberOfSensors"></param>
    /// <returns></returns>
    public SearchSpace SearchSpace(uint numberOfSensors)
    {
      return new SearchSpace(Intervals(numberOfSensors));
    }

    private List<Circle> Others(List<Agent> agents, Agent agent)
    {
      var others = new List<Circle>();
      others.AddRange(Obstacles);
        
      foreach (var s in agents)
      {
        if (s != agent) others.Add(s.ToCircle());
      }

      return others;
    }
    
    /// <summary>
    /// Gets the imperceptible area for a list of agents for this instance.
    /// </summary>
    /// <param name="agents"></param>
    /// <returns></returns>
    public List<Polygon> Imperceptible(List<Agent> agents)
    {
      var area = agents.AsParallel().Select(sensor => Sensors2D.Imperceptible(
        sensor.AreaOfActivity(), Others(agents, sensor), Field)
      ).ToList();
      
      var result = area[0];
      for (var i = 1; i < area.Count; i++)
      {
        result = Polygon.Intersection(result, area[i]);
      }

      return result;
    }

    private double CalcPenalty(List<Agent> agents)
    {
      return agents.Sum(agent =>
      {
        // -- Test if the agent is outside of the field

        if (!Field.Contains(agent.Position, true))
        {
          if (OutsideFieldPenaltyFct == 0) return 0;
          if (OutsideFieldPenaltyFct == 1) return double.PositiveInfinity;
          
          var x = Field.Min.X + FieldWidth / 2;
          var y = Field.Min.Y + FieldHeight / 2;
          return Vector2.Distance(new Vector2(x, y), agent.Position);
        }

        // -- Test if the agent collides with any obstacle
        
        foreach (var obstacle in Others(agents, agent))
        {
          if (OutsideFieldPenaltyFct == 0) return 0;
          if (CollisionPenaltyFct == 1) return double.PositiveInfinity;

          var d = Vector2.Distance(obstacle.Position, agent.Position);
          if (d < obstacle.Radius + agent.Size) return Field.Area() + d;
        }

        return 0;
      });
    }
    
    /// <summary>
    /// Evaluates this instance at a certain "position" that is for a given
    /// configuration (positions and rotations) of agents.
    /// </summary>
    /// <param name="vector">
    /// A vector of size n * 3, n >= 0 structured like
    /// (x1, y1, rot1, x2, y2, rot2, ..., xn, yn, rotn). 
    /// </param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override double Eval(Vector vector)
    {
      var agents = ToAgents(vector.ToArray());
      var penalty = CalcPenalty(agents);

      var imperceptible = Imperceptible(agents);
      if (KnownAreas.Count > 0)
      {
        imperceptible = Polygon.Difference(
          imperceptible, KnownAreas);
      }
      var notPerceptibleArea = Polygon.Area(imperceptible);

      if (InterestingAreas.Count > 0)
      {
        var importantHidden = Polygon.Intersection(
          InterestingAreas, imperceptible);
        penalty -= Polygon.Area(InterestingAreas) - Polygon.Area(importantHidden);
      }

      if (StartPosition != null)
      {
        if (StartPosition.Count != agents.Count) throw new Exception(
          "StartPosition and vector should have the same length " +
          $"{StartPosition.Count} != {agents.Count}");

        for (var i = 0; i < StartPosition.Count; i++)
        {
          var d = Vector2.Distance(StartPosition[i].Position,
            agents[i].Position);
          var rotDiff = Math.Abs(
            StartPosition[i].Rotation - agents[i].Rotation);
          penalty += rotDiff * StartPositionRotationWeight + 
                     d * StartPositionDistanceWeight;
        }
      }

      Evaluations++;
      return penalty + notPerceptibleArea;
    }
  }
}
