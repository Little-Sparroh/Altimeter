# Altimeter

A BepInEx mod for MycoPunk that adds a simple client-side altitude HUD, showing how high you are above the ground.

## Features

- **Altitude HUD**: Displays your current height above ground in meters
- **Ground / ceiling states**: Shows "On Ground" when near the floor and "Too High" when nothing is hit within range
- **Configurable position**: Move the HUD with X/Y anchor settings
- **Toggleable**: Enable or disable the display from the config file
- **Hot reload**: Config changes are picked up while the game is running

## Getting Started

### Dependencies

* MycoPunk (base game)
* [BepInEx](https://github.com/BepInEx/BepInEx) - Version 5.4.2403 or compatible
* .NET Framework 4.8
* [HarmonyLib](https://github.com/pardeike/Harmony) (included via NuGet)

### Building/Compiling

1. Clone this repository
2. Open the solution file in Visual Studio, Rider, or your preferred C# IDE
3. Build the project in Release mode to generate the .dll file

Alternatively, use dotnet CLI:
```bash
dotnet build --configuration Release
```

### Installing

**Via Thunderstore (Recommended)**:
1. Download and install via Thunderstore Mod Manager
2. The mod will be automatically installed to the correct directory

**Manual Installation**:
1. Place the built `Altimeter.dll` in your `<MycoPunk Directory>/BepInEx/plugins/` folder

### Executing program

The mod loads automatically through BepInEx when the game starts. Check the BepInEx console for loading confirmation messages.

## Configuration

Access mod settings through the BepInEx configuration file at `<MycoPunk Directory>/BepInEx/config/sparroh.altimeter.cfg`. Options include:

| Setting | Default | Description |
| --- | --- | --- |
| `EnableAltimeterHUD` | `true` | Enables the Altimeter HUD display |
| `AltimeterAnchorX` | `0.15` | X anchor position (0–1) |
| `AltimeterAnchorY` | `0.8375` | Y anchor position (0–1) |

## Help

* **Mod not loading?** Verify BepInEx is installed correctly and check console logs for errors
* **HUD not visible?** Confirm `EnableAltimeterHUD` is true and that you are in-game with a local player
* **Wrong position?** Adjust `AltimeterAnchorX` / `AltimeterAnchorY` in the config file

## Authors

- Sparroh

## License

This project is licensed under the MIT License - see the LICENSE file for details
