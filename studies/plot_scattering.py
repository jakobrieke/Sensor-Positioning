# Plot's the positions of all sensors and obstacles from a list of
#
import sys
import os.path as path
import numpy as np
import matplotlib.pyplot as plt
from plot_comparison import retrieve_results, get_title

if __name__ == "__main__":
    if len(sys.argv) == 1 or len(sys.argv) == 2 and sys.argv[1] == '--help':
        print("Usage: plot-scattering <directory-containing-simulation-logs>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:len(sys.argv)]

    fig = plt.figure(figsize=(12, 6), dpi=80)

    # for i in range(len(simDataDirectories)):
    #     pass

    xSensors = []
    ySensors = []
    xObstacles = []
    yObstacles = []

    directory = simDataDirectories[0]
    simRunResult = retrieve_results(path.join(CWD, directory))
    title = get_title(directory)

    for sim_run in simRunResult['sensor_positions']:
        for obstacle in sim_run:
            xSensors.append(obstacle['x'])
            ySensors.append(obstacle['y'])

    for sim_run in simRunResult['obstacle_positions']:
        for obstacle in sim_run:
            xObstacles.append(obstacle['x'])
            yObstacles.append(obstacle['y'])

    # -- Plot

    radius = np.pi * 10

    # plt.subplot(len(simDataDirectories), 2, i * 2 + 1)
    # plt.subplot(1, 2, 1)
    plt.scatter(xObstacles, yObstacles, c=(0, 0, 1, 0.5), s=radius,
                label="Hindernisse")
    plt.scatter(xSensors, ySensors, c=(1, 0, 0, 0.5), s=radius, label="Sensoren")
    plt.xlabel('Feldweite')
    plt.ylabel('Feldhöhe')
    plt.title('Verteilung von Sensoren und Hindernissen - ' + title)
    plt.legend(bbox_to_anchor=(1.05, 1), loc=2, borderaxespad=0.)

    # plt.subplot(1, 2, 2)
    # axes = plt.gca()
    # axes.set_xlim([-.5, 9.5])
    # axes.set_ylim([-.5, 6.5])
    # plt.scatter(xObstacles, yObstacles, c=(0, 0, 1, 0.5), s=radius)
    # plt.scatter(xSensors, ySensors, c=(1, 0, 0, 0.5), s=radius)
    # plt.xlabel('Feldweite')
    # plt.ylabel('Feldhöhe')
    # plt.title(title + ' - Achsenbeschnitten')

    plt.savefig('scattering-' + title, bbox_inches='tight')
    print("Created file: " + 'scattering-' + title)
    # plt.show()
