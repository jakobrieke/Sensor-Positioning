import tkinter as tk
import math


teamAInput = """
1.36659594627554, 0.440871326388119, 86.3876789779457
0.692869356396376, 1.06999607830359, 20.9046507807552
9, 0, 153.267289172371
0, 0, 45.3138655888597
0, 0.454986669926998, 134.383765328901
"""

teamBInput = """
5.89130895020967, 1.49328413488962
8.36197797924372, 2.20561549635866
3.74567298951823, 1.9898262377781
7.37532716215371, 4.78581109027649
3.54137660960731, 2.90196111560891
"""

teamAShadows = """
0.960299975740782, 5.99999984842725; 0.434099989033712, 5.99999984842725; 0.434199989031185, 5.99729984849546; 0.960299975740782, 5.99999984842725;
"""


fieldWidth = 9
fieldHeight = 6
playerSensorRange = 12
playerSensorFov = 56.3
playerSize = 0.1555
scale = 50

root = tk.Tk()
canvas = tk.Canvas(root, width=fieldWidth * scale, 
  height=fieldHeight * scale, borderwidth=0, 
  highlightthickness=0, bg="#171717")
canvas.grid()

## Function definitions

def _create_circle(self, x, y, r, **kwargs):
    return self.create_oval(x-r, y-r, x+r, y+r, **kwargs)
tk.Canvas.create_circle = _create_circle


def _create_circle_arc(self, x, y, r, **kwargs):
    if "start" in kwargs and "end" in kwargs:
        kwargs["extent"] = kwargs["end"] - kwargs["start"]
        del kwargs["end"]
    return self.create_arc(x-r, y-r, x+r, y+r, **kwargs)
tk.Canvas.create_circle_arc = _create_circle_arc

def parseVecList(vectors: str):
  result = []
  for v in vectors.split("\n"):
    if v == "":
      continue
    r = [float(x) for x in v.split(",")]
    result.append(r)
  return result

def movePoint(point: tuple, angle: float, distance: float, 
  useDegrees=True):
  if useDegrees:
    angle *= math.pi / 180
  return (math.cos(angle) * distance + point[0],
    math.sin(angle) * distance + point[1])

def drawTeam(team: list, fill="#E43521"):
  for p in posTeamA:
    if len(p) < 2:
      continue

      x = p[0] * scale
      y = p[1] * scale
      x2, y2 = movePoint((x, y), p[2] + hfov / 2, srange)
      x3, y3 = movePoint((x, y), p[2] - hfov / 2, srange)
      canvas.create_polygon([x, y, x2, y2, x3, y3],
        fill="#952418", outline="white")

  for p in posTeamA:
    x = p[0] * scale
    y = p[1] * scale
    if len(p) > 2:
      x1, y1 = movePoint((x, y), p[2], srange)
      canvas.create_line(x, y, x1, y1, fill="black",
        dash=(4, 4))
  canvas.create_circle(x, y, psize, fill="#E43521")


## Main

posTeamA = parseVecList(teamAInput)
posTeamB = parseVecList(teamBInput)

psize = playerSize * scale
srange = playerSensorRange * scale
hfov = playerSensorFov / 2

for p in posTeamA:
  if len(p) < 2:
    continue
  x = p[0] * scale
  y = p[1] * scale
  x2, y2 = movePoint((x, y), p[2] + hfov / 2, srange)
  x3, y3 = movePoint((x, y), p[2] - hfov / 2, srange)
  canvas.create_polygon([x, y, x2, y2, x3, y3],
    fill="#952418", outline="white")

for p in posTeamA:
  x = p[0] * scale
  y = p[1] * scale
  if len(p) > 2:
    x1, y1 = movePoint((x, y), p[2], srange)
    canvas.create_line(x, y, x1, y1, fill="black",
      dash=(4, 4))
  canvas.create_circle(x, y, psize, fill="#E43521")

for p in posTeamB:
  canvas.create_circle(p[0] * scale, p[1] * scale, 
    0.1555 * scale, fill="#36A7FB")

### Draw Shadows
polygon = [
  0.0696999982392299, 5.99909984844999,
  0.070099998229125, 5.99999984842725,
  0, 5.99999984842725,
  0, 5.9978998484803,
  0.0696999982392299, 5.99909984844999
]
canvas.create_polygon(polygon, fill="white")


root.wm_title("")
root.mainloop()
