#!/bin/bash

charlie="/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono /Users/littlebrother/Projects/4-Uni/bachelor/charlie/bin/Charlie/charlie.exe"

# Simulation to run
SIM="/Users/littlebrother/Projects/4-Uni/bachelor/sensor-positioning/bin/Debug/SensorPositioning.dll:sensor_positioning.StaticSpSimulation"
CONFIGPATH="configs/staticsp-config"
# Iterations per run
ITS=600
# Number of runs
RUNS=100


# Test with PSO from LibOptimization Library
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-1.txt" pso-1vs1
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-2.txt" pso-2vs2
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-3.txt" pso-3vs3
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-4.txt" pso-4vs4
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-7.txt" pso-7vs7
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-pso-11.txt" pso-11vs11

# Test with Adaptive Differential Evolution
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade-1.txt" ade-1vs1
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade-2.txt" ade-2vs2
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade-3.txt" ade-3vs3
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade-4.txt" ade-4vs4
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade-7.txt" ade-7vs7
$charlie --run $SIM $ITS $RUNS "$CONFIGPATH-ade11.txt" ade-11vs11

# Test with SPSO 2006
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-1.txt" spso2006-1vs1
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-2.txt" spso2006-2vs2
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-3.txt" spso2006-3vs3
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-4.txt" spso2006-4vs4
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-7.txt" spso2006-7vs7
# $charlie --run $SIM $ITS $RUNS "$CONFIGPATH-spso2006-11.txt" spso2006-11vs11
