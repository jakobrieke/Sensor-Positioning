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

# -- Setup charlie command and path to simulation

MONO="charlie"
SIMULATION="/Users/littlebrother/Projects/4-Uni/bachelor/sensor-positioning/bin/Debug/netstandard2.0/publish//sensor-positioning.dll:sensor_positioning.StaticSpSimulation"

MONOSERVER="/usr/bin/mono"
CHARLIEEXESERVER="/home/pacosy/student01/charlie-v0.2.3/charlie.exe"
SIMULATIONSERVER="/home/pacosy/student01/sensor-positioning/SensorPositioning.dll:sensor_positioning.StaticSpSimulation"

CHARLIE="$MONO $CHARLIEEXE"

# -- Setup and run configurations

CONFIGTEMPLATE="Zoom = 85
DrawSensorLines
NumberOfSensors = <i>
NumberOfObstacles = <j>
FieldHeight = 9
FieldWidth = 6
PlayerSensorRange = 12
PlayerSensorFOV = 56.3
PlayerSize = 0.1555
Optimizer = <Opt>"

ITERATIONS=$1
RUNS=$2
OPTIMIZERS=${@:3}
OUTPUTDIR="simulation-results"

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
  for SENSORCOUNTER in {1..1}
  do
    for OBSTACLECOUNTER in {1..1}
    do
      RESULT="$CONFIGTEMPLATE"
      RESULT="${RESULT/<i>/$SENSORCOUNTER}"
      RESULT="${RESULT/<j>/$OBSTACLESCOUNTER}"
      RESULT="${RESULT/<Opt>/$optimizer}"

      FILEBASE="$OUTPUTDIR/$optimizer-s$SENSORCOUNTER-o$OBSTACLECOUNTER"
      CONFIGFILE="$FILEBASE.config"
      RESULTDIR="$optimizer-s$SENSORCOUNTER-o$OBSTACLECOUNTER"
      echo "$RESULT" > "$CONFIGFILE"
      echo "$FILEBASE"

      LOGFILE="$FILEBASE.log"
      $CHARLIE --run $SIMULATION $ITERATIONS $RUNS $CONFIGFILE $RESULTDIR &>$LOGFILE &
    done
  done
done
