import os
from os import path
import shutil

# charlie_executable = "charlie"
# charlie_executable = "/usr/bin/mono /home/pacosy/student01/" \
#                      "charlie-v0.2.3/charlie.exe"
# sim_path = path.join(
#     script_dir,
#     "sensor-positioning.dll:sensor_positioning.StaticSpSimulation")

# def exec_charlie(iterations: int, runs: int, config_file: str,
#                  result_dir_name: str):
#     command = [
#         charlie_executable, "--run", sim_path, str(iterations), str(runs),
#         config_file, result_dir_name, "&> /dev/null"]
#     Popen(command)


# script_dir = os.path.dirname(os.path.realpath(__file__))
config_dir = "configurations"

if path.exists(config_dir):
    print("Configs directory already exists, delete it? y/n")
    if input() != 'y':
        exit()
    shutil.rmtree(config_dir)

os.makedirs(config_dir)

# Configuration template for Version 2.1.1
config_template = """
# -- Problem configuration
NumberOfSensors = <nos>
NumberOfObstacles = <noo>
FieldHeight = 6
FieldWidth = 9
SensorRange = 12
SensorFOV = 56.3
ObjectSize = 0.1555
# ObstaclePositions = [[2, 1]]
# ObstacleVelocity = [0.1, 0.1]
# InterestingArea01 = [[0, 0], [2, 0], [2, 1], [0, 1]]
# InterestingArea02 = [[0, 6], [2, 6], [2, 5], [0, 5]]
# InterestingArea03 = [[9, 0], [7, 0], [7, 1], [9, 1]]
# InterestingArea04 = [[9, 6], [9, 5], [7, 5], [7, 6]]
StartPositionDistanceWeight = 0
StartPositionRotationWeight = 0

# -- Optimizer configuration
OptimizationRandomSeed = -1
Optimizer = <opt>
# InitializeEachUpdate
UpdatesPerIteration = 30
# DynamicSearchSpaceRange = [0.1, 0.1]

# -- Rendering configuration
Zoom = 80
DrawGrid
DrawSensorLines
DrawStartPositions

# -- Logging configuration
LogChanges
# LogClearText
# LogEvaluations
LogRoundedPositions
"""


# -- Create static configurations

static_configurations = [
    {"nos": 1, "noo": 1, "opt": "SPSO-2006"},
    {"nos": 2, "noo": 2, "opt": "SPSO-2006"},
    {"nos": 3, "noo": 3, "opt": "SPSO-2006"},
    {"nos": 4, "noo": 4, "opt": "SPSO-2006"},
    {"nos": 5, "noo": 5, "opt": "SPSO-2006"},
    {"nos": 6, "noo": 6, "opt": "SPSO-2006"},
    {"nos": 7, "noo": 7, "opt": "SPSO-2006"},
    {"nos": 8, "noo": 8, "opt": "SPSO-2006"},
    {"nos": 9, "noo": 9, "opt": "SPSO-2006"},
    {"nos": 10, "noo": 10, "opt": "SPSO-2006"},
    {"nos": 11, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 1, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 2, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 3, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 4, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 5, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 6, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 7, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 8, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 9, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 10, "noo": 11, "opt": "SPSO-2006"},
    # Configurations to to examine when agents start to block each other
    {"nos": 1, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 2, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 3, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 4, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 5, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 6, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 7, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 8, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 9, "noo": 11, "opt": "SPSO-2006"},
    {"nos": 10, "noo": 11, "opt": "SPSO-2006"},
]
# iterations = 0  # 1000
# runs = 1  # 100

for config in static_configurations:
    result_dir_name = "static-" + str(config["nos"]) + \
                 "-" + str(config["noo"]) + \
                 "-" + config["opt"]
    config_file = path.join(config_dir, result_dir_name + ".conf")

    config_text = config_template\
        .replace('<nos>', str(config['nos']))\
        .replace('<noo>', str(config['noo']))\
        .replace('<opt>', config['opt'])
    config_file_handler = open(config_file, 'w+')
    config_file_handler.write(config_text)
    config_file_handler.close()

    # exec_charlie(iterations, runs, config_file, result_dir_name)

# -- Create dynamic configurations

print("Do you want to create dynamic configurations? y/n")
if input() != 'y':
    exit()

dynamic_configurations = [
    {"nos": 1, "noo": 1},
    {"nos": 1, "noo": 2},
    {"nos": 1, "noo": 3},
    {"nos": 2, "noo": 1},
    {"nos": 2, "noo": 2},
    {"nos": 2, "noo": 3},
    {"nos": 3, "noo": 1},
    {"nos": 3, "noo": 2},
    {"nos": 3, "noo": 3},
]
# iterations = 1  # 1000
# runs = 1  # 100

for config in dynamic_configurations:
    result_dir_name = "dynamic-" + str(config["nos"]) + \
                      "-" + str(config["noo"])
    config_file = path.join(config_dir, result_dir_name + ".conf")

    config_text = config_template \
        .replace('<nos>', str(config['nos'])) \
        .replace('<noo>', str(config['noo'])) \
        .replace('<opt>', 'SPSO-2006')\
        .replace('# InitializeEachUpdate', 'InitializeEachUpdate')
    config_file_handler = open(config_file, 'w+')
    config_file_handler.write(config_text)
    config_file_handler.close()

    # exec_charlie(iterations, runs, config_file, result_dir_name)
