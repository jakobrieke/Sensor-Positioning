#!/bin/bash

for i in $(ls ~/.charlie/SensorPositioning.StaticSpSimulation)
do
    dir="$HOME/.charlie/SensorPositioning.StaticSpSimulation/$i"
	v=$(find $dir -type f -name '*.log' |wc -l |xargs)
	# v=$(ls -1q ~/.charlie/SensorPositioning.StaticSpSimulation/$i/*.log |wc -l |xargs)
	echo "$i - $v"
done