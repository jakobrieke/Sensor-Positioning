# Legend:
# 
# ag := agents
# obs := obstacles
# alg := algorithm
# ma := marked-areas
# dist := allowed-distance
# rot := allowed-rotation
# reps := repetitions
# its := iterations
# id := used for output directory and configuration file name

configs = [
  {"ag": 1, "obs": 1, "alg": "JADE",       "ma": [[0, 0], [2, 0], [2, 1], [0, 1]], "dist": -1, "rot": -1, "reps": 100, "its": 50, "id": "1-agent-1-obstacle-ma-ll-JADE"},
  {"ag": 1, "obs": 1, "alg": "SPSO-2006",  "ma": [], "dist": -1, "rot": -1, "reps": 3, "its": 100, "id": "1-agent-1-obstacle-ma-ll-SPSO-2006"},
  {"ag": 2, "obs": 2, "alg": "JADE",       "ma": [], "dist": 3, "rot": -1, "reps": 5, "its": 20, "id": "2-agents-2-obstacles-ma-ll-JADE"},
]
