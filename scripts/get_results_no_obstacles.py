# Usage:
#
# Generate data with
# > run_scenario <build-dir> <output-dir>
# > run_scenario <build-dir> <output-dir> <config_file>
# - See configs/test_configs.py for an example of a config_file
#
# Aggregate data
# > grep -nr "<global-best>" run* > global_best.py
# > grep -nr "<sensors>" run* > sensors.py
# > grep -nr "<changes>" run* > changes.py
#
# Cleanup data
# - remove every line i % 2 == 0
# - in sensors.py create a list of floats called "sensors"
# - in global_best.py create a list of float lists called "global_bests"
#
# Start a python shell inside the directory of sensors.py and global_best.py,
# copy and execute the following script inside the shell
#

import global_best as gb
import sensors

# -- Check if results seem ok

if sum(gb.global_bests) / len(gb.global_bests) > 12.1:
    print("Error in data: Average final area is too big!")
    exit()

results = sensors.sensors

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
            print("Error in data: " + str(
                value) + "° exceeds allowed boundries (" + str(
                sensors[0]) + ")")
            break
        i += 1

# -- Print statistic

i = 0
for angle in possible_angles:
    result = 100 * occurences[i] / len(results)
    print(str(angle) + "°: " + str(result) + "%")
    i += 1

# -- Print Latex formatted output

latex_line = ""
for occurence in occurences:
    result = round(100 * occurence / len(results), 2)
    latex_line += "& " + str(result) + "\\pct "

print(latex_line)
