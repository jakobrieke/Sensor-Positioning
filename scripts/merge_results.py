"""
Load a list of results from folders structured like:

/some/path/2-agents-3-obstacles-SPSO-06
- run-1203947019237432.log
- run-3151231234123512.log
- run-2135123512341234.log
/some/path/pso-1vs1
- run-1235134613241233.log
- run-8979013849238475.log

where each .log file is a structured XML file like:

simulation
simulation/software
simulation/system
simulation/datetime
simulation/title
simulation/meta
simulation/configuration
simulation/iteration[@i='0']
simulation/iteration/sensors
simulation/iteration/obstacles
simulation/iteration/changes
simulation/iteration/objective-evaluations
simulation/iteration/global-best
simulation/iteration[@i='600']
simulation/iteration/sensors
simulation/iteration/obstacles
simulation/iteration/changes
simulation/iteration/objective-evaluations
simulation/iteration/global-best
simulation/iterations
simulation/elapsed-time
simulation/average-time

Parse all .log files from each folder to a dictionary like:

SPSO2006-1
- sim_count
- start_areas
- final_areas
- average_start_areas
- average_final_areas
- average_improvement
- totalTimeInSec
- averageTimeInSec
- sensor_positions
- obstacle_positions

and plot all dictionaries.
"""

import xml.etree.ElementTree as ET
import os.path as path
import os
import sys


def get_alg_and_group_size(sim_title: str):
    """
    Parses a string like "1-agent-3-obstacles-JADE" or
    "4-agents-4-obstacles-SPSO-2006" to a list
    like ["JADE", 1, 1] or ["SPSO-2006", 4, 2]
    :param sim_title: A string like "4-agents-4-obstacles-SPSO-2006".
    :return: A list of three elements [algorithm name, number of sensors,
    number of obstacles]
    """
    buffer = sim_title.split("-")
    return ['-'.join(buffer[4:]), buffer[0], buffer[2]]


def parse_positions(positions_str: str):
    result = []

    positions_str = positions_str[1:-1]
    positions = positions_str.split('],')

    if positions[0] == '':
        return []

    for position_str in positions:
        sensor = {}
        position_str = position_str.replace('[', '')
        position_str = position_str.replace(']', '')

        values = position_str.split(',')
        sensor['x'] = float(values[0])
        sensor['y'] = float(values[1])

        if len(values) == 3:
            sensor['rotation'] = float(values[2])

        result.append(sensor)

    return result


def retrieve_results(sim_data_directory: str):
    """
    Parses and combines all results from within a directory.
    :param sim_data_directory: The directory containing the log files.
    :return: A dictionary with all results combined.
    """
    sim_data_files = []
    for file in os.listdir(sim_data_directory):
        if file.endswith(".log"):
            sim_data_files.append(path.join(sim_data_directory, file))

    # Number of simulations
    sim_count = len(sim_data_files)
    start_areas = []
    final_areas = []
    # The average improvement of the sensor positions in all simulations
    # Improvement here is defined as difference between
    # start area and final area
    average_improvement = 0
    # Total time for all simulations in sec
    total_time = 0
    # Average time for all simulations in sec
    # totalAverageTime = 0

    sensor_positions = []
    obstacle_positions = []

    for file in sim_data_files:
        # try:
        root = ET.parse(file).getroot()
        total_time += int(root.find('elapsed-time').text)
        iterations = root.findall('iteration')
        start_area = float(iterations[0].find('global-best').text)
        final_area = float(iterations[1].find('global-best').text)
        start_areas.append(start_area)
        final_areas.append(final_area)
        average_improvement += start_area - final_area

        sensor_positions.append(parse_positions(
            iterations[1].find('sensors').text))
        obstacle_positions.append(parse_positions(
            iterations[1].find('obstacles').text))
        # except ValueError as error:
        #     print("Error parsing file: " + file)
        #     print('"' + error + '"')

    average_improvement /= sim_count
    total_time = total_time / 1000
    average_time = total_time / sim_count
    title = path.split(sim_data_directory)[-1]
    alg_group_size = get_alg_and_group_size(title)

    return {
        "title": title,
        "algorithm": alg_group_size[0],
        "sensors": str(alg_group_size[1]),
        "obstacles": str(alg_group_size[2]),
        "sim_count": sim_count,
        "start_areas": start_areas,
        "final_areas": final_areas,
        "average_start_areas": sum(start_areas) / sim_count,
        "average_final_areas": sum(final_areas) / sim_count,
        "average_improvement": average_improvement,
        "totalTimeInSec": total_time,
        "averageTimeInSec": average_time,
        "sensor_positions": sensor_positions,
        "obstacle_positions": obstacle_positions
    }


if __name__ == "__main__":

    if len(sys.argv) == 1 or len(sys.argv) == 2 and sys.argv[1] == '--help':
        print("Usage: merge_results <directories-containing-simulation-logs>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:]

    print("Title, "
          "Repetitions, "
          "Algorithm, "
          "Sensors, "
          "Obstacles, "
          "Start Area, "
          "Final Area, "
          "Improvement, "
          "Total Time, "
          "Average Time")

    for directory in simDataDirectories:
        if not os.path.isdir(directory):
            continue
        results = retrieve_results(path.join(CWD, directory))

        print(str(results['title']) + ", "
              + str(results["sim_count"]) + ", "
              + str(results["algorithm"]) + ", "
              + str(results["sensors"]) + ", "
              + str(results["obstacles"]) + ", "
              + str(results["average_start_areas"]) + ", "
              + str(results["average_final_areas"]) + ", "
              + str(results["average_improvement"]) + ", "
              + str(results["totalTimeInSec"]) + ", "
              + str(results["averageTimeInSec"]))
