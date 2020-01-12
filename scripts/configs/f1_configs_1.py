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
  {"ag": 1, "obs": 1,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-1-obstacle-JADE"},
  {"ag": 1, "obs": 1,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-1-obstacle-SPSO-2006"},
  {"ag": 5, "obs": 5,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "5-agents-5-obstacles-JADE"},
  {"ag": 5, "obs": 5,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "5-agents-5-obstacles-SPSO-2006"},
  {"ag": 6, "obs": 6,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "6-agents-6-obstacles-JADE"},
  {"ag": 6, "obs": 6,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "6-agents-6-obstacles-SPSO-2006"},
]
