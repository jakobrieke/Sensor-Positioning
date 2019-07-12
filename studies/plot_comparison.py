#!/usr/bin/env python3

"""
Load a list of results from folders structured like:

/some/path/spso2006-1vs1
- run-01.log
- run-02.log
- run-03.log
/some/path/pso-1vs1
- run-01.log
- run-02.log

where each .log file is a structured XML file like:

- simulation
  - title
  - meta
  - datetime
  - system
  - model
  - iteration (i="1, 2, 3, ...")
    - sensors
    - obstacles
    - global-best
  - elapsed-time
  - average-time

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

import sys
import xml.etree.ElementTree as ET
import os.path as path
import os
import matplotlib.pyplot as plt
import statsmodels.stats.api as sms


def parse_positions(positions_str: str):
    result = []

    positions_str = positions_str[1:-1]
    positions = positions_str.split('],')

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
    # Improvement here is defined as difference between ?
    average_improvement = 0
    # Total time for all simulations in sec
    total_time = 0
    # Average time for all simulations in sec
    # totalAverageTime = 0

    sensor_positions = []
    obstacle_positions = []

    for i in range(0, len(sim_data_files)):
        root = ET.parse(sim_data_files[i]).getroot()
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

    average_start_areas = sum(start_areas) / sim_count
    average_final_areas = sum(final_areas) / sim_count
    average_improvement /= sim_count
    total_time = total_time / 1000
    average_time = total_time / sim_count

    return {
        "sim_count": sim_count,
        "start_areas": start_areas,
        "final_areas": final_areas,
        "average_start_areas": average_start_areas,
        "average_final_areas": average_final_areas,
        "average_improvement": average_improvement,
        "totalTimeInSec": total_time,
        "averageTimeInSec": average_time,
        "sensor_positions": sensor_positions,
        "obstacle_positions": obstacle_positions
    }


def get_title(directory: str):
    """
    Creates a string of form Algorithm-GroupSize
    :param directory:
    :return:
    """
    result = directory.split('/')[-1].split('vs')[0]
    result = result.replace('adaptive-diff-evolution', 'ADE')
    result = result.replace('ade', 'ADE')
    result = result.replace('spso2006', 'SPSO06')
    return result.replace('pso', 'PSO')


def get_algorithm(elem: str):
    return elem[:elem.rfind('-')]


def get_group_size(elem: str):
    return int(elem.split('-')[-1])


def plot_results(results: dict):

    # -- Sort results
    sorted_keys = sorted(results, key=get_group_size)
    sorted_results = {}
    for key in sorted_keys:
        sorted_results[key] = results[key]
    results = sorted_results

    # -- Calculate confidence intervals
    tconfint_means = []
    for k in results.keys():
        m = sms.DescrStatsW(results[k]).tconfint_mean()
        tconfint_means.append(m[1] - m[0])

    # -- Calculate average values

    for k in results.keys():
        mean = sum(results[k]) / len(results[k])
        results[k] = mean

    # -- Plot bar chart

    plt.ylabel('Nicht einsehbare Fläche')
    plt.xlabel('Algorithmus - Gruppengröße')
    plt.title("Optimale Positionierung von Sensoren zwischen Hindernissen")

    graph = plt.bar(range(len(results)), list(results.values()),
                    align='center', yerr=tconfint_means)
    plt.xticks(range(len(results)), list(results.keys()))

    # text = "Hello here is some text"
    # plt.text(.1, .1, text)

    # -- Colorize every bar of the same algorithm with
    colors = ['C0', 'C1', 'C2', 'C3']
    algorithms = []

    for key in results.keys():
        algo = get_algorithm(key)
        if not algorithms.__contains__(algo):
            algorithms.append(algo)

    i = 0
    for key in results.keys():
        index = algorithms.index(get_algorithm(key))
        graph[i].set_color(colors[index])
        i += 1

    file_title = 'comparison'
    for algo in algorithms:
        file_title += "-" + algo

    # plt.savefig(file_title, bbox_inches='tight')
    # print("Created file: " + file_title)
    plt.show()


if __name__ == "__main__":

    if len(sys.argv) == 1 or len(sys.argv) == 2 and sys.argv[1] == '--help':
        print("Usage: plot-results <directories-containing-simulation-logs>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:len(sys.argv)]

    simResults = {}

    for directory in simDataDirectories:
        result = retrieve_results(path.join(CWD, directory))
        title = get_title(directory)
        simResults[title] = result['final_areas']

    plot_results(simResults)
