# Generate data:
# 
# ./scripts/run 3.1.0 configurations/single-sensor/no-obstacles-SPSO-06.conf 5000 no-obstacles-SPSO-06
# ./scripts/run 3.1.0 configurations/single-sensor/no-obstacles-JADE.conf 5000 no-obstacles-JADE
# Aggregate data:
# 
# grep -nr "<global-best>" run* > global_best.py
# grep -nr "<sensors>" run* > sensors.py
# grep -nr "<changes>" run* > changes.py

import global_best as gb
import sensors
# import changes

# -- Check if results seem ok

results = [t[1] for t in gb.global_bests if True]

if sum(results) / len(results) > 12.1:
  print("Error in data: Average final area is too big!")
  exit()

# -- 

results = [d['final'] for d in sensors.sensors]

# 28°, 151°, 208°, 331°
possible_angles = [28, 151, 208, 331]
occurences = [0, 0, 0, 0]
allowed_deviation = 3

for sensors in results:
  i = 0
  for degree in possible_angles:
    left = degree - allowed_deviation
    right = degree + allowed_deviation
    
    # -- Cleanup values to be in range [0, 360)
    value = sensors[0][2]
    
    while value < 0:
      value += 360
    value = value % 360
    
    if left <= value <= right:
      occurences[i] += 1
      break
    if i == 3:
      print("Error in data: " + str(value) + "° exceeds allowed boundries (" + str(sensors[0]) + ")")
      break
    i += 1

i = 0
for angle in possible_angles:
  result = 10 * occurences[i] / len(results)
  print(str(angle) + "°: " + str(result) + "%")
  i += 1

latex_line = ""
for occurence in occurences:
  result = round(100 * occurence / len(results), 2)
  latex_line += "& " + str(result) + "\\pct "

print(latex_line)

# Cleaning data "changes"
# Input:
# run-2019-09-16T11-12-02Z324.log:99:<changes>[..., [252, 3.37507799486048E-14], [255, 1.06581410364015E-14], [258, 3.5527136788005E-15], [259, 2.48689957516035E-14], ...]</changes>
# Output:
# [..., [252, 3.37507799486048E-14], [255, 1.06581410364015E-14], [258, 3.5527136788005E-15], [259, 2.48689957516035E-14], ...]
