#!/bin/bash

# Crop all images inside ~/.charlie/SensorPositioning.StaticSpSimulation
# to a size of 800 x 550 px

images=$(ls ~/.charlie/SensorPositioning.StaticSpSimulation/*.png)
for image in $images
do
    echo $image
    mogrify -crop 800x550+0+125 $image
done
