#!/usr/bin/env python3

# This is a small script to fix errors in the old 
# charlie html encoding
# Alternative methods might be using 'rpl' or 'sed'
# > rpl "old-string" "new-string" files
# > sed -i.bak 's/old-string/new-string/g' files

import sys
import os.path as path
import re
import xml.etree.ElementTree as ET

cwd = path.dirname(path.realpath(__file__))
inputFiles = sys.argv[1:len(sys.argv)]

totalChanges = 0

for i in range(0, len(inputFiles)):
  filePath = path.join(cwd, inputFiles[i])

  file = open(filePath, 'r')
  content = file.read()
  occurences = re.findall(r'i=\d+', content)
  file.close()
  
  if len(occurences) == 0:
    continue

  for occurence in occurences:
    split = occurence.split('=')
    value = int(split[1])
    content = content.replace(occurence, 'i="' + str(value) + '"')
    totalChanges += 1

  try:
    root = ET.fromstring(content)
    file = open(filePath, 'w')
    file.write(content)
    file.close()
  except Exception as e:
    print("Invalid XML: " + str(e))

print("Total changes: " + str(totalChanges))

