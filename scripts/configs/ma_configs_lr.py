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
  {"ag": 1, "obs": 1, "alg": "JADE",       "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-1-obstacle-ma-lr-JADE"},
  {"ag": 1, "obs": 1, "alg": "SPSO-2006",  "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "1-agent-1-obstacle-ma-lr-SPSO-2006"},
  {"ag": 2, "obs": 2, "alg": "JADE",       "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "2-agents-2-obstacles-ma-lr-JADE"},
  {"ag": 2, "obs": 2, "alg": "SPSO-2006",  "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "2-agents-2-obstacles-ma-lr-SPSO-2006"},
  {"ag": 3, "obs": 3, "alg": "JADE",       "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "3-agents-3-obstacles-ma-lr-JADE"},
  {"ag": 3, "obs": 3, "alg": "SPSO-2006",  "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "3-agents-3-obstacles-ma-lr-SPSO-2006"},
  {"ag": 4, "obs": 4, "alg": "JADE",       "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "4-agents-4-obstacles-ma-lr-JADE"},
  {"ag": 4, "obs": 4, "alg": "SPSO-2006",  "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "4-agents-4-obstacles-ma-lr-SPSO-2006"},
  {"ag": 5, "obs": 5, "alg": "JADE",       "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "5-agents-5-obstacles-ma-lr-JADE"},
  {"ag": 5, "obs": 5, "alg": "SPSO-2006",  "ma": [[9, 0], [7, 0], [7, 1], [9, 1]], "dist": -1, "rot": -1, "reps": 1000, "its": 600, "id": "5-agents-5-obstacles-ma-lr-SPSO-2006"},
]
