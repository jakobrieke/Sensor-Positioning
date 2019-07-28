#!/bin/bash

if [ "$#" -lt 3 ] || [ "$1" == "-h" ] #|| [ "$1" == "--help" ]
then
  echo "Usage"
  echo "> ./run.sh <iterations> <runs> <optimizers>"
  echo
  echo "Examples"
  echo "> ./run.sh 100 3 PSO"
  echo "> ./run.sh 100 3 ADE PSO SPSO-2006"
  exit
fi

MONO="/usr/bin/mono"
CHARLIEEXE="/home/pacosy/student01/charlie-v0.2.3/charlie.exe"
CHARLIE="$MONO $CHARLIEEXE"

# Simulation to run
SIMULATION="/home/pacosy/student01/sim-build-1.7.2/sensor-positioning.dll:sensor_positioning.StaticSpSimulation"

CONFIGTEMPLATE="
# -- Problem configuration
NumberOfSensors = <i>
NumberOfObstacles = <j>
FieldHeight = 6
FieldWidth = 9
PlayerSensorRange = 12
PlayerSensorFOV = 56.3
PlayerSize = 0.1555
# ObstaclePositions = [[2, 1]]
ObstacleVelocity = [0.1, 0.1]
StartPositionDistanceWeight = 0
StartPositionRotationWeight = 0

# -- Optimizer configuration
# The function used to optimize the problem,
# possible values are:
# PSO, SPSO-2006, SPSO-2007, SPSO-2011, ADE
Optimizer = <Opt>
InitializeEachUpdate
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
LogEvaluations
LogRoundedPositions"

ITERATIONS=$1
RUNS=$2
SensorCount=(1 2 3)
ObstacleCount=(1 2 3)
OPTIMIZERS=${@:3}
OUTPUTDIR="sim-logs"

if [ -d $OUTPUTDIR ]
then
  echo "Overwrite existing output directory y/n?"
  read shouldOverwrite

  if [  "$shouldOverwrite" == "y" ]
  then
    rm -fr $OUTPUTDIR
  else
    exit
  fi
fi

mkdir $OUTPUTDIR

for optimizer in ${OPTIMIZERS[*]}
do
  for i in ${SensorCount[*]}
  do
    for j in ${ObstacleCount[*]}
    do
      RESULT="$CONFIGTEMPLATE"
      RESULT="${RESULT/<i>/$i}"
      RESULT="${RESULT/<j>/$j}"
      RESULT="${RESULT/<Opt>/$optimizer}"

      FILEBASE="$OUTPUTDIR/$optimizer-s$i-o$j"
      CONFIGFILE="$FILEBASE.config"
      RESULTDIR="$optimizer-i$ITERATIONS-s$i-o$j"
      echo "$RESULT" > "$CONFIGFILE"
      echo "$FILEBASE"

      LOGFILE="$FILEBASE.log"
      $CHARLIE --run $SIMULATION $ITERATIONS $RUNS $CONFIGFILE $RESULTDIR &>$LOGFILE &
    done
  done
done
