# Windows Debugging

## Build

```powershell
.\build-windows.ps1 -Configuration Debug -MingwRoot D:\mingw64 -RunTests
```

Outputs:

```text
build-windows/Debug/wa2_cpp.exe
build-windows/Debug/wa2_core_tests.exe
```

The executable is a console application so runtime warnings and failures remain visible. It keeps
source debug symbols and statically links SDL2/MinGW runtime libraries.

Open `native-cpp` itself as the VS Code workspace with the Microsoft C/C++ extension, then press `F5` and select
`WA2: Title Menu` or `WA2: Script 2001`. The included task rebuilds and runs tests before launch;
the resource-directory prompt defaults to the current local Godot project's `assets` directory.

## GDB

```powershell
D:\mingw64\bin\gdb.exe .\build-windows\Debug\wa2_cpp.exe
```

Example session:

```gdb
break wa2::Runtime::call_function
run --res D:/path/to/Wa2Res --save D:/tmp/wa2-save --font C:/Windows/Fonts/msyh.ttc --script 2001
bt
print function
```

Useful breakpoints:

- `wa2::ArchiveIndex::read` for resource lookup/decompression;
- `wa2::ScriptVm::run` for bytecode execution;
- `wa2::Runtime::call_function` for script opcode behavior;
- `wa2::SdlFrontend::render` for scene rendering;
- `wa2::SystemStore::flush` for persistent system state.

## Sanitizers

The current MinGW 8 toolchain does not provide a reliable AddressSanitizer setup for this SDL build.
Run ASan/UBSan through the Linux CMake build when that platform environment is available. Keep the
Windows Debug build for GDB and native graphics/audio testing.

## Resource failures

Warnings about optional missing PAK files are printed to stderr. A missing selected script package,
invalid archive table, failed LZSS decode, or unwritable save directory is fatal and returns a
nonzero exit code. Use forward slashes in GDB arguments to avoid escape confusion.
