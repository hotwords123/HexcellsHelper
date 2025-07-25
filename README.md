# HexcellsHelper

A QoL helper for the puzzle game [Hexcells Infinite](https://store.steampowered.com/app/304410/Hexcells_Infinite/).

## Prerequisites

- Ensure that the [.NET SDK](https://dotnet.microsoft.com/download) is installed. You can verify the installation by running the following command:
  ```bash
  dotnet --version
  ```
- [BepInEx](https://github.com/BepInEx/BepInEx) for dynamic injection (if using Method 1).
- [ILSpyCmd](https://github.com/icsharpcode/ILSpy/tree/master/ICSharpCode.ILSpyCmd) for decompilation (if using Method 2).

## Installation

### Configuration

- Create `game-directory.txt` in the project root and input your game's installation path. For example:
  ```
  C:\Program Files (x86)\Steam\steamapps\common\Hexcells Infinite
  ```

### Method 1: Dynamic Injection via BepInEx

This method uses [BepInEx](https://github.com/BepInEx/BepInEx) for dynamic injection of the plugin into the game. It is the recommended way to install the HexcellsHelper plugin, as it allows for easier updates and management of the plugin, and does not require modifying the game's original files.

1. **Install BepInEx**
   - Go to the [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases) page and download the version suitable for your game. We use BepInEx 5 for this project.
   - Extract the downloaded files into the root directory of your game. This should create a `BepInEx` folder alongside the game executable.
   - Additionally, ensure that the `doorstop_config.ini` and `winhttp.dll` files are also extracted into the root directory of your game. These files are required for BepInEx to function properly.

2. **Copy Reference Assemblies**
   - Use the provided script `python scripts/copy-dlls.py` to copy the necessary game DLL files as reference assemblies.
   - Alternatively, you can manually copy the required DLL files from the game directory to the `lib` folder in the project root.

3. **Build the plugin**
   - Run the following command in the root directory of the project:
   ```bash
   dotnet publish -c Release
   ```
   - After publishing, the generated DLL file will be automatically copied to the `plugins` directory of BepInEx.

4. **Launch the game**
   - Start the game to load the plugin.

### Method 2: Static Injection via Assembly-CSharp Patch

Alternatively, you can apply the mod by patching the game's `Assembly-CSharp.dll` directly. This method uses decompilation, patch application, and recompilation to produce a modified `Assembly-CSharp.dll`.

#### Workflow

Use the provided script `scripts/build-patch.sh` to manage the workflow.

```bash
bash scripts/build-patch.sh <action> [work_dir]
```

Supported actions:

* `decompile`
  Decompile the original `Assembly-CSharp.dll` to a working directory and initialize a git repo. [ILSpyCmd](https://github.com/icsharpcode/ILSpy/tree/master/ICSharpCode.ILSpyCmd) is used for decompilation. If it is not installed, the script will prompt you to install it.

* `apply`
  Copy mod source files and apply the existing patch on top of the decompiled files (if any).

* `generate`
  Generate an updated patch file based on differences from the original decompiled files.

* `build`
  Build the patched project into a new `Assembly-CSharp.dll`. Backs up the original DLL if not backed up yet and copies the new DLL to the game directory.

* `restore`
  Restore the original `Assembly-CSharp.dll` backup.

To install the mod, run the following commands:

```bash
bash scripts/build-patch.sh decompile
bash scripts/build-patch.sh apply
bash scripts/build-patch.sh build
```

#### Notes

* The working directory defaults to `../HexcellsHelper-AsmPatch` relative to the project root.
* The generated patch file is stored as `patches/mod.patch`.
* Backup of the original DLL is stored as `Assembly-CSharp.dll.original`.
* To skip copying the rebuilt DLL to the game directory, create a `.no_copy` file in the working directory.

## Usage

The HexcellsHelper plugin provides the following functionalities in game:

- **F2**: Copies the current level state to the clipboard in the [SixCells editor format](https://github.com/oprypin/sixcells?tab=readme-ov-file#level-file-structure). Shift held: copy initial state.
- **Z**: Undoes the last action (left-click or right-click).
- **T**: Toggles the display mode (whether the numbers on the cells represent the total number of blue cells or the remaining blue cells).
- **Tab**: Temporarily switches the display mode while held down (reverts when released).
- **Space**: Invokes the trivial solver to resolve simple cases automatically.
- **H**: Toggles hypothesis mode on and off. In hypothesis mode, you can mark cells as hypothetical blue or black by left-clicking or right-clicking them (depending on the *"Swap Mouse Buttons"* setting). To remove a hypothesis mark, right-click the cell again.
- **C**: Clears all hypothesis marks from the current level.
- **G**: Toggles column guides for all column numbers that are not marked as complete.
- **Shift + G**: Toggles flower guides for all blue hex flowers that are not marked as complete.

### Additional Features

- **Puzzle History**: In the *Puzzle Generator* screen, you can click *"Puzzles Completed"* to view your puzzle completion history. Only puzzles completed after installing the plugin will be displayed.
