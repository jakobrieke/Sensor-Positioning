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
  {"ag": 1, "obs": 2,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-2-obstacles-JADE"},
  {"ag": 1, "obs": 2,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-2-obstacles-SPSO-2006"},
  {"ag": 2, "obs": 2,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "2-agents-2-obstacles-JADE"},
  {"ag": 2, "obs": 2,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "2-agents-2-obstacles-SPSO-2006"},
  {"ag": 9, "obs": 9,  "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "9-agents-9-obstacles-JADE"},
  {"ag": 9, "obs": 9,  "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "9-agents-9-obstacles-SPSO-2006"},
  {"ag": 10, "obs": 10, "alg": "JADE",      "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "10-agents-10-obstacles-JADE"},
  {"ag": 10, "obs": 10, "alg": "SPSO-2006", "ma": [], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "10-agents-10-obstacles-SPSO-2006"},
]
