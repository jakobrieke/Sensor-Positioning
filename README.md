# Sensor Positioning

## Installation
- Install .NET Core (https://dotnet.microsoft.com/download)
- Install Charlie (github.com/TheRainbowPalace/charlie)
- Clone the repository:
```
> git clone https://github.com/TheRainbowPalace/Sensor-Positioning.git
```

## Usage
- Open the project folder and build the project:
```
> cd Sensor-Positioning
> dotnet publish
```
- Run:
```
> realpath bin/Debug/netcoreapp2.2/publish/sensor-positioning.dll
```
- Copy the output path and append ":SensorPositioning.StaticSpSimulation"
- The complete string should look something like
  /Users/SomeUser/Projects/Sensor-Positioning/bin/Debug/netcoreapp2.2/publish/sensor-positioning.dll:SensorPositioning.StaticSpSimulation
- Paste the complete string into the Charlie load field and press "Load"%
- The simulation should load
- Press "Start" to start the simulation