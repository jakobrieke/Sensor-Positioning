"""
Load a list of results from folders structured like:

/some/path/2-agents-3-obstacles-SPSO-06
- run-1203947019237432.log
- run-3151231234123512.log
- run-2135123512341234.log
/some/path/1-agent-1-obstacle-JADE-d-3
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
from xml.etree.ElementTree import ParseError
import os.path as path
import os
import sys
import re
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
    """
    Convert a list of changes e.g. 
    """
    iterations += 1  # Include iteration 0 (start state)
    mean_best_changes = [0] * iterations
    mean_total_area_changes = [0] * iterations
    mean_marked_area_changes = [0] * iterations
    for change in changes:
        index = 0
        for i in range(iterations):
            mean_best_changes[i] += change[index][1]
            mean_total_area_changes[i] += change[index][2]
            mean_marked_area_changes[i] += change[index][3]

            if change[index] == change[-1]:
                continue
            if i + 1 == change[index + 1][0]:
                index += 1
    for i in range(iterations):
        mean_best_changes[i] /= len(changes)
        mean_total_area_changes[i] /= len(changes)
        mean_marked_area_changes[i] /= len(changes)

    return {
        "best": mean_best_changes,
        "total_area": mean_total_area_changes,
        "marked_area": mean_marked_area_changes
    }


def retrieve_results(sim_data_directory: str, error_file):
    """
    Combines the results of multiple individual simulations (with the same
    start configuration) to one single average simulation result.
    The simulation log files are parsed (it's expected, that the log files do
    conform with the given XML schema)
    :param sim_data_directory: A directory containing the simulation log files.
    :return: A dictionary with all results combined to a dictionary containing
    the following values:
    - "title": The title of the simulation (created from sim_data_dir).
    - "algorithm": The optimization algorithm used by the simulation.
    - "agents": The number of simulated agents.
    - "obstacles": The number of objects contained in the simulation.
    - "sim_count": The number of individual simulations.
    - "start_areas": The average area at iteration 0, visible to the agents
    - "final_areas": The average area at the last iteration, visible to the
      agents
    - "average_start_areas": The average area seen by all agents after the
      initialization of all simulations in % of the total area.
    - "average_final_areas": The average area seen by all agents at the end of
      all simulations in % of the total area.
    - "confidence_interval"
    - "totalTimeInSec"
    - "averageTimeInSec": The average time need for a simulation to finish in
      seconds.
    - "sensor_positions": A list of the final agent-positions at the end of
      each simulation.
    - "obstacle_positions" A list of obstacle positions of each simulation.
    - "best_changes": A list of the average fitness value changes per time
      step, that occured during all simulations. The first entry of the list is
      the fitness value after the simulation initializations.
    - "total_area_changes":
    - "marked_area_changes":
    """
    sim_data_files = []
    for file in os.listdir(sim_data_directory):
        if file.endswith(".log"):
            sim_data_files.append(path.join(sim_data_directory, file))

    sim_count = len(sim_data_files)  # Number of simulations
    start_areas = []
    final_areas = []
    total_time = 0  # Total time for all simulations in sec
    sensor_positions = []
    obstacle_positions = []
    all_changes = []

    for file in sim_data_files:
        try:
            root = ET.parse(file).getroot()
        except ParseError as e:
            error_file.write("ParseError: " + file + "\n")
            error_file.write("  " + str(e) + "\n")
            continue

        total_time += int(root.find('elapsed-time').text)
        iterations = root.findall('iteration')
        
        sensors = parse_list_of_tuples(iterations[1].find('sensors').text)
        obstacles = parse_list_of_tuples(iterations[1].find('obstacles').text)
        sensor_positions.append(sensors)
        obstacle_positions.append(obstacles)

        # The following produces a list where each element is a tuple
        # containing:
        # - iteration
        # - fitness value
        # - percentage of total visible area
        # - percentage of marked visible area
        changes = parse_list_of_tuples(
            iterations[1].find('changes').text,
            lambda x: [int(x[0]), float(x[1]), float(x[2]), float(x[3])])
        all_changes.append(changes)
        
        start_areas.append(changes[0][2])
        final_areas.append(changes[-1][2])

        # -- Check that visible marked area is not > 100%
        if changes[-1][3] > 1:
            error_file.write("Error MA > 100%: " + str(changes[-1][3]) + "\n")
            error_file.write("  Agents: " + str(sensors) + "\n")
            error_file.write("  Obstacles: " + str(obstacles) + "\n")

    m_changes = mean_changes(all_changes, int(iterations[1].attrib['i']))

    total_time = total_time / 1000
    average_time = total_time / sim_count
    title = path.split(path.abspath(sim_data_directory))[-1]
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
        # Todo: Check if the confidence interval is correctly computed
        "confidence_interval": sum(final_areas) / sim_count - conf_interval[0],
        "totalTimeInSec": total_time,
        "averageTimeInSec": average_time,
        "sensor_positions": sensor_positions,
        "obstacle_positions": obstacle_positions,
        "best_changes": m_changes["best"],
        "total_area_changes": m_changes["total_area"],
        "marked_area_changes": m_changes["marked_area"],
    }


if __name__ == "__main__":

    if len(sys.argv) < 3 or sys.argv[1] == '--help':
        print("Usage: merge_results "
              "<output_file_name> "
              "<directories containing simulation logs> ")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    directories = sys.argv[2:]
    output_path = sys.argv[1] 
    if output_path.endswith(".csv"):
        output_path = output_path[:-4]
    output_file_path = output_path + ".csv"
    error_file_path = output_path + ".errors"

    if path.isfile(output_file_path):
        print("Output file '" + output_file_path + "' exists already")
        answer = input("Override? (y/n): ")
        if answer != "y" and answer != "yes":
            exit()

    output_file = open(output_file_path, "w+")
    output_file.write("Title, "
          "Repetitions, "
          "Algorithm, "
          "Agents, "
          "Obstacles, "
          "Start Area, "
          "Final Area, "
          "Confidence Interval, "
          "Total Time, "
          "Average Time, "
          "Best Changes, "
          "Total Area Changes, "
          "Marked Area Changes"
          "\n")

    error_file = open(error_file_path, "w")
    
    i = 1
    total = len(directories)
    for directory in directories:
        if not os.path.isdir(directory):
            continue

        print(str(i) + "/" + str(total) + ": " + directory)
        
        results = retrieve_results(path.join(CWD, directory), error_file)
        output_file.write(str(results['title']) + ", "
              + str(results["sim_count"]) + ", "
              + str(results["algorithm"]) + ", "
              + str(results["agents"]) + ", "
              + str(results["obstacles"]) + ", "
              + str(results["average_start_areas"]) + ", "
              + str(results["average_final_areas"]) + ", "
              + str(results["confidence_interval"]) + ", "
              + str(results["totalTimeInSec"]) + ", "
              + str(results["averageTimeInSec"]) + ", "
              + ';'.join(str(x) for x in results["best_changes"]) + ", "
              + ';'.join(str(x) for x in results["total_area_changes"]) + ", "
              + ';'.join(str(x) for x in results["marked_area_changes"])
              + "\n")
        i += 1

    output_file.close()
    error_file.close()
