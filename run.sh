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

MONO="/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono"
CHARLIEEXE="/Users/littlebrother/Projects/4-Uni/bachelor/charlie/bin/Debug/net471/charlie.exe"
CHARLIE="$MONO $CHARLIEEXE"

# Simulation to run
SIMULATION="/Users/littlebrother/Projects/4-Uni/bachelor/sensor-positioning/bin/Debug/SensorPositioning.dll:sensor_positioning.StaticSpSimulation"

CONFIGTEMPLATE="Zoom = 85
DrawSensorLines
NumberOfSensors = <i>
NumberOfObstacles = <i>
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
  echo "Overwrite existing configurations y/n?" 
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
  for i in {1..11}
  do
    RESULT="$CONFIGTEMPLATE"
    RESULT="${RESULT/<i>/$i}"
    RESULT="${RESULT/<i>/$i}"
    RESULT="${RESULT/<Opt>/$optimizer}"
    
    CONFIGFILE="$OUTPUTDIR/$optimizer-$i"
    RESULTDIR="$optimizer-$i"
    echo "$RESULT" > "$CONFIGFILE"
    echo "$CONFIGFILE"

    LOGFILE="$OUTPUTDIR/$optimizer-$i.log"
    # $CHARLIE --run $SIMULATION $ITERATIONS $RUNS $CONFIGFILE $RESULTDIR &>$LOGFILE #&
  done
done
