import shutil
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

def find_dlls(project_root: Path) -> list[str]:
    tree = ET.parse(project_root / "HexcellsHelper.csproj")
    root = tree.getroot()

    dll_names: list[str] = []
    for elem in root.iterfind("./ItemGroup/Reference"):
        include = elem.get("Include")
        if include:
            dll_names.append(include)

    return dll_names


def copy_dlls(src_dir: Path, dest_dir: Path, dll_names: list[str]) -> None:
    for dll_name in dll_names:
        filename = dll_name.split(",")[0] + ".dll"
        src_path = src_dir / filename
        dest_path = dest_dir / filename
        try:
            shutil.copy(src_path, dest_path)
            print(f"[+] Copied {filename}")
        except FileNotFoundError:
            print(f"[!] {filename} not found in {src_dir}")


def main():
    project_root = Path(__file__).resolve().parent.parent
    game_dir = (project_root / "game-directory.txt").read_text().strip()
    game_dir = Path(game_dir)

    if sys.platform == "darwin":
        src_dir = game_dir / "Contents" / "Resources" / "Data" / "Managed"
    else:
        src_dir = game_dir / "Hexcells Infinite_Data" / "Managed"
    print(f"[*] Source directory: {src_dir}")

    dest_dir = project_root / "lib"
    dest_dir.mkdir(parents=True, exist_ok=True)
    print(f"[*] Destination directory: {dest_dir}")

    dll_names = find_dlls(project_root)
    print(f"[*] DLLs to copy: {dll_names}")

    copy_dlls(src_dir, dest_dir, dll_names)


if __name__ == "__main__":
    main()
