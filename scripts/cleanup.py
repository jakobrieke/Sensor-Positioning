#!/usr/bin/env python3

# Removes faulty .log files from
# ~/.charlie/SensorPositioning.StaticSpSimulation/**
# A file is faulty, if the simulation was interrupted

import os
from os import path
from os.path import expanduser
import sys

home = expanduser("~")
sim_results_dir = path.join(
    home, '.charlie/SensorPositioning.StaticSpSimulation')


def get_faulty_files(directory: str, print_files=False):
    _faulty_files = []

    for dirName, subdirList, fileList in os.walk(directory):
        folder_name = path.split(dirName)[-1]
        if folder_name.startswith(".") or folder_name.startswith("__"):
            continue

        for fname in fileList:
            if not fname.endswith(".log"):
                continue
            file = path.join(dirName, fname)
            with open(file, 'rb') as fh:
                fh.seek(-1024, 2)
                last = fh.readlines()[-1].decode()
                if last != "</simulation>":
                    if print_files:
                        print("Faulty file: " + file)
                    _faulty_files.append(file)

    return _faulty_files


if __name__ == "__main__":
    if len(sys.argv) == 1 or len(sys.argv) == 2 and sys.argv[1] == '--help':
        print("Usage: cleanup.py <directories containing simulation logs>")
        exit()

    CWD = path.dirname(path.realpath(__file__))
    simDataDirectories = sys.argv[1:]
    faulty_files = []

    # -- Search for faulty files
    for directory in simDataDirectories:
        if not os.path.isdir(directory):
            continue
        path.join(CWD, directory)

        faulty_files.extend(get_faulty_files(directory, True))

    if len(faulty_files) > 0:
        # -- Ask user if he wants to remove files
        answer = input("Remove (" + str(len(faulty_files)) + ") files (y/n): ")
        if answer != "y":
            exit()

        for faulty_file in faulty_files:
            os.remove(faulty_file)
    else:
        print("No faulty files found")
