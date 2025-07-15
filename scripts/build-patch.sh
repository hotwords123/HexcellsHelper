#!/usr/bin/env bash
set -e

src_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
game_dir=$(cat "$src_dir"/game-directory.txt)
dll_path="$game_dir/Hexcells Infinite_Data/Managed/Assembly-CSharp.dll"
patch_file="$src_dir/patches/mod.patch"

function decompile() {
    if ! command -v ilspycmd &> /dev/null; then
        echo "[!] ILSpyCmd is not installed. Do you want to install it now? (y/N)"
        read -r install_ilspycmd
        if [[ "$install_ilspycmd" =~ ^[Yy]$ ]]; then
            dotnet tool install --global ilspycmd
        else
            echo "[!] ILSpyCmd is required for decompilation. Exiting."
            exit 1
        fi
    fi

    if [ -d "$decompiled_dir" ]; then
        echo "[!] Decompiled directory already exists. Please remove it first."
        exit 1
    fi

    mkdir -p "$decompiled_dir"
    ilspycmd -p -o "$decompiled_dir" --nested-directories "$dll_path" || {
        echo "[!] Decompilation failed. Please check if the DLL exists and is valid."
        exit 1
    }
    echo "[+] Decompiled to $decompiled_dir"

    cd "$decompiled_dir"
    git init -q
    git add .
    git commit -q -m "Base decompiled"

    base_commit=$(git rev-parse HEAD)
    echo "$base_commit" > "$work_dir/base_commit.txt"
    echo "[+] Initialized git repository with base commit: $base_commit"
}

function apply_mod() {
    base_commit=$(cat "$work_dir/base_commit.txt")
    echo "[*] Base commit: $base_commit"

    echo "[*] Copying mod source..."
    cd "$decompiled_dir"
    git reset -q --hard "$base_commit"
    git clean -fdq

    mkdir -p HexcellsHelper
    cp -r "$src_dir"/*.cs HexcellsHelper/
    git add HexcellsHelper

    if [ -f "$patch_file" ]; then
        echo "[*] Applying patch..."
        git apply --allow-empty --reject "$patch_file" || {
            echo "[!] Patch failed to apply"
            exit 1
        }
    else
        echo "[*] No patch to apply"
    fi
}

function generate_patch() {
    echo "[*] Generating new patch..."
    cd "$decompiled_dir"
    git diff > "$patch_file" || true
    git ls-files --others --exclude-standard | while IFS= read -r file; do
        git diff --no-index -- /dev/null "$file" >> "$patch_file" || true
    done
    echo "[+] Patch updated at $patch_file"
}

function build() {
    echo "[*] Building the mod..."
    cd "$decompiled_dir"
    dotnet build -c Release || { echo "[!] Build failed"; exit 1; }

    if [ -f "$work_dir/.no_copy" ]; then
        echo "[*] Skipping DLL copy as per .no_copy file"
        return
    fi

    backup_path="$dll_path.original"
    if [ ! -f "$backup_path" ]; then
        cp "$dll_path" "$backup_path"
        echo "[+] Original DLL backed up to $backup_path"
    else
        echo "[*] Backup already exists at $backup_path"
    fi

    new_dll_path="$decompiled_dir/bin/Release/net35/Assembly-CSharp.dll"
    if [ -f "$new_dll_path" ]; then
        cp "$new_dll_path" "$dll_path"
        echo "[+] New DLL copied to $dll_path"
    else
        echo "[!] Built DLL not found at $new_dll_path"
        exit 1
    fi
}

function restore() {
    echo "[*] Restoring original DLL..."
    backup_path="$dll_path.original"
    if [ -f "$backup_path" ]; then
        cp "$backup_path" "$dll_path"
        echo "[+] Original DLL restored from $backup_path"
    else
        echo "[!] No backup found at $backup_path"
        exit 1
    fi
}

function main() {
    if [ $# -lt 1 ]; then
        echo "Usage: $0 <decompile|apply|generate|build> [work_dir]"
        exit 1
    fi

    local action="$1"
    work_dir="${2:-$(realpath "$src_dir/../HexcellsHelper-AsmPatch")}"
    decompiled_dir="$work_dir/Decompiled"

    case "$action" in
        decompile)
            decompile
            ;;
        apply)
            apply_mod
            ;;
        generate)
            generate_patch
            ;;
        build)
            build
            ;;
        restore)
            restore
            ;;
        *)
            echo "Invalid action: $action"
            exit 1
            ;;
    esac
}

main "$@"
