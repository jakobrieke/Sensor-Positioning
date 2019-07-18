import math

#public List<List<Vector2>> Shadow(Environment env)
#{
#    var light = AreaOfActivity();
#    var obstacles = new List<Circle>();
#     
#    env.Objects.ForEach(o =>
#    {
#        if (o != this) obstacles.Add(
#            new Circle(o.Position.X, o.Position.Y, o.Size));
#    });
#
#    return Shadows2D.Shadows(light, obstacles, env.Region);
#}


# public static List<List<Vector2>> Shadows(
#         Arc sensor, List<Circle> obstacles, Rectangle bounds)
# {
#     if (!bounds.IsInside(sensor.Position))
#          throw new ArgumentException("Sensor is outside bounds.");
# 
#     var polygons = new List<List<Vector2>>();
#     foreach (var obstacle in obstacles)
#     {
#         if (!bounds.IsInside(obstacle.Position))
#             throw new ArgumentException("Obstacle is outside bounds.");
#         
#         polygons.Add(HiddenArea(sensor, obstacle, bounds));
#     }
#     polygons.Add(UnseenArea(sensor, bounds));
# 
#     return Polygon.Union(polygons);
# }


def distance(x1: float, y1: float, x2: float, y2: float):
    return math.sqrt((x1 - x2)**2 + (y1 - y2)**2)


# Sensor: (x, y, radius, angle, width, ...)
# Obstacle: (x, y, radius)
# Bounds: (x1, y1, x2, y2)
def hiddenArea(sensor: tuple, obstacle: tuple, bounds: tuple):
    d = distance(sensor[0], sensor[1], ostacle[0], ostacle[1])
    if d <= obstacle.Radius:
        raise AttributeError("Sensor is inside obstacle.")
    
    tangents = outerTangents(sensor.Position, obstacle)
    
    i1 = Rectangle.Intersection(bounds, 
        new Segment(sensor.Position, tangents[0]))
    i2 = Rectangle.Intersection(bounds,
        new Segment(sensor.Position, tangents[1]))
    
    if i1.X == i2.X or i1.Y == i2.Y:
        return [tangents[0], i1, i2, tangents[1], tangents[0]]
    
    # Add border edge if necessary
    
    minX = bounds.Min.X
    minY = bounds.Min.Y
    maxX = bounds.Max.X
    maxY = bounds.Max.Y

    x = i1.X == minX || i2.X == minX ? minX : maxX
    y = i1.Y == minY || i2.Y == minY ? minY : maxY

    return [
        tangents[0], i1, new Vector2(x, y), i2, 
        tangents[1], tangents[0]]


# Calculate the intersection of a rectangle and a segment
def intersection(p1: tuple, p2: tuple, p3: tuple, p4: tuple):
    values = [
        (p1[0] - p3[0]) / (p4[0] - p3[0]),
        (p1[1] - p3[1]) / (p4[1] - p3[1]),
        (p2[0] - p3[0]) / (p4[0] - p3[0]),
        (p2[1] - p3[1]) / (p4[1] - p3[1])
    ]
    λ = values[0]
    for value in values:
        if λ > value and value >= 0:
            λ = value
        if λ < 0:
            λ = value
    if λ < 0:
        raise AttributeError("Segment is outside of borders")
    x = p3[0] + λ * (p4[0] - p3[0])
    y = p3[1] + λ * (p4[1] - p3[1])
    return (x, y)


def testIntersection():
    intersection((0, 0), (1, 1), (0, 0), (0.5, 0.5)) == (0, 0)
    intersection((0, 0), (1, 1), (0, 0), (1, 1)) == (0, 0)
    intersection((0, 0), (1, 1), (0.1, 0.1), (1, 1)) == (1, 1)
    intersection((0, 0), (1, 1), (0.1, 0.1), (0.1, -0.1)) -> ZeroDivisionError
    intersection((0, 0), (1, 1), (0.1, -0.1), (0.1, 0.1))


def testOuterTangents():
    x1 = outerTangent(0, 0, 3, 4, 4, 3)
    outerTangents(1, 0, 0.1555, 6.49, 2.3, 0.1555)
    [(0.9399141733736638, 0.14342225572981993),
     (6.429914173373664, 2.4434222557298195),
     (1.0600858266263362, -0.14342225572981995),
     (6.550085826626336, 2.1565777442701797)]


# Calculate the outer tangents of two circles.
def outerTangents(x1: float, y1: float, r1: float, x2: 
    float, y2: float, r2: float):
    tangent1 = outerTangent(x1, y1, r1, x2, y2, r2)
    tangent2 = outerTangent(x2, y2, r2, x1, y1, r1)
    return [tangent1[0], tangent1[1], 
        tangent2[1], tangent2[0]]


def outerTangent(x1: float, y1: float, r1: float, x2: 
    float, y2: float, r2: float):
    γ = math.atan2(y2 - y1, x2 - x1)
    β = math.asin((r2 - r1) / math.sqrt(math.pow(x2 - x1, 2) + 
        math.pow(y2 - y1, 2)))
    α = -γ - β
    x3 = x1 + r1 * math.cos(math.pi / 2 - α)
    y3 = y1 + r1 * math.sin(math.pi / 2 - α)
    x4 = x2 + r2 * math.cos(math.pi / 2 - α)
    y4 = y2 + r2 * math.sin(math.pi / 2 - α)
    return [(x3, y3), (x4, y4)]
