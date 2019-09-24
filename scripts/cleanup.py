#!/usr/bin/env python3

# Removes faulty .log files from
# ~/.charlie/SensorPositioning.StaticSpSimulation/**
# A file is faulty, if the simulation was interrupted

import os
from os import path
from os.path import expanduser

home = expanduser("~")
sim_results_dir = path.join(
    home, '.charlie/SensorPositioning.StaticSpSimulation')


def get_faulty_files(print_files = False):
    _faulty_files = []

    for dirName, subdirList, fileList in os.walk(sim_results_dir):
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
    faulty_files = get_faulty_files(True)

    # -- Ask user if he wants to remove files
    if len(faulty_files) > 0 and input("Remove files (y/n): ") == "y":
        for faulty_file in faulty_files:
            os.remove(faulty_file)
