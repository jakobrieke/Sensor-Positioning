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
import re
import csv
import statsmodels.stats.api as sms


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


def parse_list_of_tuples(list_string: str, parse_fct=None):
    """
    Parses a list of tuples like "[[1.12, 4.1], [5, 1.4, 3]]". It's possible to
    pass a custom parse function which takes a list of strings of arbitrary
    length and returns a new object.
    Without a custom parse function a list of lists of floats is returned.
    """
    values = re.sub("[\\[\\] ]", "", list_string.replace("],", ";")).split(";")
    result = []
    if values[0] == '':
        return result
    for value in values:
        str_tuple = value.split(',')

        if parse_fct is not None:
            result.append(parse_fct(str_tuple))
            continue

        float_tuple = []
        for i in range(len(str_tuple)):
            float_tuple.append(float(str_tuple[i]))
        result.append(float_tuple)
    return result


def mean_changes(changes: list, iterations: int):
    iterations += 1  # Include iteration 0 (start state)
    result = [0] * iterations
    for repetition in changes:
        index = 0
        value = repetition[index][1]
        for i in range(iterations):
            result[i] += value
            if repetition[index] == repetition[-1]:
                continue
            if i + 1 == repetition[index + 1][0]:
                index += 1
                value -= repetition[index][1]
    for i in range(iterations):
        result[i] /= len(changes)
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
    # Improvement here is defined as mean-difference between
    # start area and final area
    average_improvement = 0
    # Total time for all simulations in sec
    total_time = 0

    sensor_positions = []
    obstacle_positions = []
    changes = []

    for file in sim_data_files:
        root = ET.parse(file).getroot()
        total_time += int(root.find('elapsed-time').text)
        iterations = root.findall('iteration')
        start_area = float(iterations[0].find('global-best').text)
        final_area = float(iterations[1].find('global-best').text)
        start_areas.append(start_area)
        final_areas.append(final_area)
        average_improvement += start_area - final_area

        sensor_positions.append(parse_list_of_tuples(
            iterations[1].find('sensors').text))
        obstacle_positions.append(parse_list_of_tuples(
            iterations[1].find('obstacles').text))
        changes.append(parse_list_of_tuples(
            iterations[1].find('changes').text,
            lambda x: [int(x[0]), float(x[1])]))

    for i in range(len(changes)):
        changes[i].insert(0, [0, start_areas[i]])

    # Todo: Get iteration count dynamically
    m_changes = mean_changes(changes, 600)

    average_improvement /= sim_count
    total_time = total_time / 1000
    average_time = total_time / sim_count
    title = path.split(sim_data_directory)[-1]
    alg_group_size = get_alg_and_group_size(title)
    conf_interval = sms.DescrStatsW(final_areas).tconfint_mean()

    return {
        "title": title,
        "algorithm": alg_group_size[0],
        "agents": str(alg_group_size[1]),
        "obstacles": str(alg_group_size[2]),
        "sim_count": sim_count,
        "start_areas": start_areas,
        "final_areas": final_areas,
        "average_start_areas": sum(start_areas) / sim_count,
        "average_final_areas": sum(final_areas) / sim_count,
        "confidence_interval": sum(final_areas) / sim_count - conf_interval[0],
        "average_improvement": average_improvement,
        "totalTimeInSec": total_time,
        "averageTimeInSec": average_time,
        "sensor_positions": sensor_positions,
        "obstacle_positions": obstacle_positions,
        "mean_changes": m_changes
    }


if __name__ == "__main__":

    if len(sys.argv) == 1 or len(sys.argv) == 2 and sys.argv[1] == '--help':
        print("Usage: merge_results <directories containing simulation logs>"
              "<output name>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:]

    print("Title, "
          "Repetitions, "
          "Algorithm, "
          "Agents, "
          "Obstacles, "
          "Start Area, "
          "Final Area, "
          "Confidence Interval, "
          "Improvement, "
          "Total Time, "
          "Average Time, "
          "Changes")

    for directory in simDataDirectories:
        if not os.path.isdir(directory):
            continue
        results = retrieve_results(path.join(CWD, directory))

        print(str(results['title']) + ", "
              + str(results["sim_count"]) + ", "
              + str(results["algorithm"]) + ", "
              + str(results["agents"]) + ", "
              + str(results["obstacles"]) + ", "
              + str(results["average_start_areas"]) + ", "
              + str(results["average_final_areas"]) + ", "
              + str(results["confidence_interval"]) + ", "
              + str(results["average_improvement"]) + ", "
              + str(results["totalTimeInSec"]) + ", "
              + str(results["averageTimeInSec"]) + ", "
              + ';'.join(str(x) for x in results["mean_changes"]))
