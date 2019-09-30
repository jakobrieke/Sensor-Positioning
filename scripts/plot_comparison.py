#!/usr/bin/env python3

import sys
import os.path as path
import os
import matplotlib.pyplot as plt
import statsmodels.stats.api as sms
from merge_results import retrieve_results, get_alg_and_group_size


def get_algorithm(directory: str):
    return get_alg_and_group_size(directory)[0]


def get_group_size(directory: str):
    return get_alg_and_group_size(directory)[1:]


def get_sensor_group_size(directory: str):
    return get_alg_and_group_size(directory)[1]


def plot_results(results: dict):

    # -- Sort results
    sorted_keys = sorted(results, key=get_sensor_group_size)
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
        print("Usage: plot_comparison <directories-containing-simulation-logs>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:len(sys.argv)]

    simResults = {}

    for directory in simDataDirectories:
        if not os.path.isdir(directory):
            continue
        results = retrieve_results(path.join(CWD, directory))
        simResults[results['title']] = results['final_areas']

    plot_results(simResults)
