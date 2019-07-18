from matplotlib import cm
from matplotlib.pyplot import subplots
from numpy import linspace
from numpy import sin, cos, abs, exp, sqrt, pi, meshgrid
import matplotlib.pyplot as plt


# Stablinsky Tang
def stablTang(x, y):
  return sin(x + y) + (x - y)**2 - 1.5 * x + 2.5 * y + 1

# Hoelder-Table
def hoelderTableFct(x, y):
  return -abs(sin(x) * cos(y) * exp(abs(1 - sqrt(x**2 + y**2) / pi)))

phi_m = linspace(-3, 3, 100)
phi_p = linspace(-3, 3, 100)
X,Y = meshgrid(phi_p, phi_m)
# Z = flux_qubit_potential(X, Y).T
Z = stablTang(X, Y).T

fig, ax = subplots()

p = ax.pcolor(X, Y, Z, cmap=cm.RdBu, vmin=Z.min(), vmax=Z.max())
cb = fig.colorbar(p)


plt.ylabel('y')
plt.xlabel('x')
plt.title("Stablinsky-Tang Funktion")
plt.show()
