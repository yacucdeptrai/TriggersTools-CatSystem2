# TriggersTools.CatSystem2 ![AppIcon](https://i.imgur.com/RxP6ZZL.png)

[![NuGet Version](https://img.shields.io/nuget/vpre/TriggersTools.CatSystem2.svg?style=flat)](https://www.nuget.org/packages/TriggersTools.CatSystem2/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/TriggersTools.CatSystem2.svg?style=flat)](https://www.nuget.org/packages/TriggersTools.CatSystem2/)
[![Creation Date](https://img.shields.io/badge/created-february%202019-A642FF.svg?style=flat)](https://github.com/trigger-death/TriggersTools.CatSystem2/commit/b5a6288423842d3289f9770d0294d01b105348be)
[![Discord](https://img.shields.io/discord/436949335947870238.svg?style=flat&logo=discord&label=chat&colorB=7389DC&link=https://discord.gg/vB7jUbY)](https://discord.gg/vB7jUbY)

A library for extracting resources from, decompiling scripts from, and working with the CatSystem2 visual novel game engine.

See the **[CatSystem2 Documentation Wiki](https://github.com/trigger-death/TriggersTools.CatSystem2/wiki)** for ongoing documentation of CatSystem2 file specifications, undocumented features, debugging, decompiling scripts, and more.

## Features

* Patches for translating CatSystem2 tools into English.
* Decrypting and extracting from KIF int archive files.
* Extracting images from HG-2 and HG-3 format. (Improved over asmodean's code)
* Decompiling cst, fes, and anm scripts into a modifiable and compileable state.


### This is a Work in Progress.

![CatSystem2 English Debug Mode](https://i.imgur.com/r8g2vqJ.png)

## Build From Terminal

```powershell
dotnet restore .\TriggersTools.CatSystem2.sln
dotnet build .\TriggersTools.CatSystem2.sln -c Release
```

### Prerequisites

- .NET SDK installed (`dotnet --version`)
- Windows PowerShell (examples below use PowerShell syntax)
- Script inputs available (`*.anm`, `*.cst`, `*.fes`, or decompiled `*.txt`)

### Build Only Tool Projects

```powershell
dotnet build .\tools\CatCompiler\CatCompiler.csproj -c Release
dotnet build .\tools\CatDecompiler\CatDecompiler.csproj -c Release
```

## CLI Tools

### `CatCompiler`

Compile script text (`.txt`) back into CatSystem2 script binaries:

```powershell
dotnet run --project .\tools\CatCompiler -- --help
```

Usage:

```text
catcompiler <anm|cst|fes> <input> [output]
catcompiler --type <anm|cst|fes> --input <input> [--output <output>]
```

Examples:

```powershell
dotnet run --project .\tools\CatCompiler -- anm .\input\*.txt .\out
dotnet run --project .\tools\CatCompiler -- --type cst --input .\input\scene_*.txt --output .\out
dotnet run --project .\tools\CatCompiler -- --type fes --input .\scripts\*.txt
```

Notes:
- `output` is optional; default is input file directory.
- Wildcards are supported for input patterns.

### `CatDecompiler`

Decompile CatSystem2 script binaries into editable text:

```powershell
dotnet run --project .\tools\CatDecompiler -- --help
```

Usage:

```text
cs2_decompile [infiles...] [options]
```

Common options:
- `-i <infiles...>` input files (supports wildcards)
- `-o, --output <outdir>` output directory
- `-ext, --extension <.ext>` output extension (`.txt` by default)
- `-utf8, --utf8` output UTF-8 (`Shift JIS` by default)
- `-x, --error-level <none|low|high>` stop behavior on errors/warnings
- `--extract-int <archive.int>` extract all files from an archive
- `--exe <game.exe>` executable used to derive V_CODE2 for archive decryption
- `--extract-output <outdir>` output directory for extracted archive files

Examples:

```powershell
dotnet run --project .\tools\CatDecompiler -- .\input\*.cst -o .\out -utf8
dotnet run --project .\tools\CatDecompiler -- -i .\input\*.anm .\input\*.fes --output .\out --extension .txt
dotnet run --project .\tools\CatDecompiler -- -i .\input\scene\*.cst --output .\decompiled --error-level low
dotnet run --project .\tools\CatDecompiler -- --extract-int .\game\scene.int --exe .\game\Game.exe --extract-output .\scene-extract
```

## End-to-End Terminal Workflow

```powershell
# 1) Build tools
dotnet restore .\TriggersTools.CatSystem2.sln
dotnet build .\tools\CatDecompiler\CatDecompiler.csproj -c Release
dotnet build .\tools\CatCompiler\CatCompiler.csproj -c Release

# 2) Extract scripts from .int archive first (required for many games)
dotnet run --project .\tools\CatDecompiler -- --extract-int .\game\scene.int --exe .\game\Game.exe --extract-output .\work\scene

# 3) Decompile extracted scripts
dotnet run --project .\tools\CatDecompiler -- -i .\work\scene\*.cst --output .\work\txt --utf8

# 4) Edit generated text files in .\work\txt

# 5) Recompile edited text back to binary scripts
dotnet run --project .\tools\CatCompiler -- --type cst --input .\work\txt\*.txt --output .\rebuilt
```

### Tested Example: `アマカノ～Second Season～`

```powershell
dotnet run --project .\tools\CatDecompiler -- --extract-int ".\アマカノ～Second Season～\scene.int" --exe ".\アマカノ～Second Season～\amakano2s.exe" --extract-output ".\tmp-amakano-scene"
dotnet run --project .\tools\CatDecompiler -- -i ".\tmp-amakano-scene\01_s01_01.cst" ".\tmp-amakano-scene\01_s01_02.cst" --output ".\tmp-amakano-decompiled" --utf8
```

## Run Built Executables Directly

After a release build, executables are typically under:

- `.\tools\CatCompiler\bin\Release\net462\CatCompiler.exe`
- `.\tools\CatDecompiler\bin\Release\net462\CatDecompiler.exe`

Example:

```powershell
.\tools\CatDecompiler\bin\Release\net462\CatDecompiler.exe -i .\input\*.anm -o .\out -utf8
```

## Other Projects/Tools of Note & Special Thanks

* **[Japanese Command Prompt](https://github.com/trigger-death/jpcmd)**
* **[Locale Emulator](https://github.com/xupefei/Locale-Emulator)**
* **[marcussacana's CatSceneEditor](https://github.com/marcussacana/CatSceneEditor)**
* **[FuckGalEngine/CatSystem2](https://github.com/Inori/FuckGalEngine/tree/master/CatSystem2)**
* **[chinesize/CatSystem2](https://github.com/regomne/chinesize/tree/master/CatSystem2)**
* **[Doddler's Kamikaze Tools](http://www.doddlercon.com/main/?p=120)**
* **[asmodean's exkifint & hgx2bmp](http://asmodean.reverse.net/pages/exkifint.html)**
* **[ClrPlus.Windows.PeBinary.ResourceLib](https://github.com/perpetual-motion/clrplus/tree/master/Windows.PeBinary/ResourceLib)**
