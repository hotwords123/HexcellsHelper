# HexcellsHelper

## Installation

1. **Set up the .NET development environment**
   - Ensure that the [.NET SDK](https://dotnet.microsoft.com/download) is installed. You can verify the installation by running the following command:
     ```bash
     dotnet --version
     ```

2. **Install BepInEx**
   - Go to the [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases) page and download the version suitable for your game. We use BepInEx 5 for this project.
   - Extract the downloaded files into the root directory of your game. This should create a `BepInEx` folder alongside the game executable.
   - Additionally, ensure that the `doorstop_config.ini` and `winhttp.dll` files are also extracted into the root directory of your game. These files are required for BepInEx to function properly.

3. **Configure the project**
   - Edit the project file (`HexcellsHelper.csproj`) based on the actual game installation path to ensure the paths are configured correctly.

4. **Build the plugin**
   - Run the following command in the root directory of the project:
     ```bash
     dotnet restore
     dotnet build --no-restore -c Release
     ```
   - After building, the generated DLL file will be automatically copied to the `plugins` directory of BepInEx.

5. **Launch the game**
   - Start the game to load the plugin.

## Usage

The HexcellsHelper plugin provides the following functionalities in game:

- **F2**: Copies the current level state to the clipboard in the [SixCells editor format](https://github.com/oprypin/sixcells?tab=readme-ov-file#level-file-structure).
- **Z**: Undoes the last action (left-click or right-click).
- **T**: Toggles the display mode (whether the numbers on the cells represent the total number of blue cells or the remaining blue cells).
- **Tab**: Temporarily switches the display mode while held down (reverts when released).
