# Architecture

## Layers

```text
Application / platform entry
        |
SDL frontend and optional FFmpeg video
        |
Runtime state + script function host
        |
BNR VM, archive reader, text decoder, system store
```

`wa2_core` contains no window, renderer or platform UI. `Runtime` exposes scene state and delegates
audio/video operations through `MediaSink`. `SdlFrontend` is one implementation of that interface.
This boundary lets every target reuse archive, VM and game behavior while replacing only the app
entry or media backend where a platform requires it.

## Platform plan

| Platform | Shared code | Platform work |
|---|---|---|
| Windows | all core and SDL frontend | build/debug scripts, optional installer |
| Linux | all core and SDL frontend | distro dependencies and packaging |
| macOS | all core and SDL frontend | app bundle, signing, sandbox paths |
| Android | all core and most SDL frontend | SDL Activity, touch UI, lifecycle, scoped storage |
| iOS | all core and most SDL frontend | SDL UIKit entry, touch UI, app sandbox, signing |
| Switch Homebrew | all core and SDL frontend | devkitPro/libnx build, SD paths, performance QA |

Do not add platform checks to the VM or archive parser. Platform paths belong in `main.cpp` or a
future platform entry; lifecycle events belong in the frontend. A different renderer should
implement the same scene and `MediaSink` contracts.

## Data ownership

Archive entries hold only archive path, offset, size and compression state. Loading an archive reads
its directory records only. Image/audio data is read and decoded on demand. The SDL frontend owns
decoded textures and mixer chunks; `Runtime` owns script and scene state; `SystemStore` owns the
in-memory `sys.sav` image.

## Compatibility priorities

1. Preserve script and archive semantics with automated tests.
2. Keep platform APIs outside `wa2_core`.
3. Bound allocations from untrusted or damaged PAK metadata.
4. Treat original game assets as external user data and never package them with this source.
5. Add full save slots only after all script stack and scene fields can round-trip safely.
