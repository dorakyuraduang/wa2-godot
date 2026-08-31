[简体中文](README.md) | **English**

## Native C++ Port Branch

The `cpp-native-port` branch keeps the Godot/C# implementation as a behavioral reference while
developing a standalone C++17 + SDL2 runtime under [`native-cpp/`](native-cpp/README.md). Windows
is the current primary debugging platform; Linux, Android, macOS, iOS, and Nintendo Switch
Homebrew are later targets. Original game resources remain local and are never committed.

## Game Installation (Android)

1. Download and install the APK.
2. Copy the entire PC game directory to the root of your phone's storage.
3. Rename the copied directory to `Wa2Res`.

## Android Controls

1. Press and hold anywhere on the screen to fast-forward. This is disabled by default and becomes available after changing the display mode to Fullscreen in Settings.
2. Press the Back button during gameplay to open the return-to-main-menu confirmation dialog.

## Important Notes

1. You must obtain the PC game files yourself.
2. Do not place the data inside additional nested directories. The game data must be directly inside `Wa2Res` to avoid path errors.
3. To update the app, install the latest APK over the existing version. You do not need to copy the data again.
4. Uninstalling the app does not automatically remove the `Wa2Res` data directory. Delete it manually from the storage root if you want to remove it completely.
5. Grant file access permission on first launch. The application cannot read local resources without this permission.
6. If the game freezes after running for some time, try changing the display mode to Fullscreen in Settings.

Example directory layout:

![Wa2Res directory layout](https://github.com/user-attachments/assets/303a7d6e-99e1-4683-bed9-671d271e08be)

## Video Playback Plugin

The project uses the FFmpeg GDExtension in `addons/wmv_video` to play the original `.pak` movie files. The plugin accepts only ASF content containing WMV1, WMV2, WMV3, or VC-1 video. File names may use either the `.wmv` or `.pak` extension.

- Windows: x86_64
- Android: `arm64-v8a`, minimum API level 24
- FFmpeg: 7.1.5, minimal LGPL-2.1-or-later shared build

The Windows and Android native libraries are included in the plugin's `bin` directory. The Android export preset automatically packages the GDExtension and its shared FFmpeg dependencies into the APK.

## Licensing and Asset Boundaries

This repository is not covered by a single project-wide MIT license. See [LICENSE](LICENSE) for the exact scope:

- Contributor-authored resource readers, shaders, Godot scenes, the native C++ runtime, animation configuration, font mapping, and project configuration explicitly listed in [PORTING_CODE.md](PORTING_CODE.md) are licensed under Apache License 2.0. Anyone may fork, modify, port, and redistribute them without opening a pull request, requesting separate approval, or contributing changes back to this repository.
- Contributor-authored plugin source, build tools, and documentation under `addons/wmv_video` are licensed under the MIT License in that directory.
- FFmpeg, godot-cpp, libwinpthread, and YamlDotNet remain subject to their respective licenses.
- `assets/sub.yaml` contains mixed third-party subtitle data credited to Moegirlpedia and CK-GAL. It is not covered by Apache-2.0. See [SUBTITLE_NOTICE.md](SUBTITLE_NOTICE.md) for details.
- This repository grants no permission to copy, modify, redistribute, or sell files that are not explicitly covered by `PORTING_CODE.md`, a component license, or a file-level SPDX identifier.
- Source availability alone does not imply an open-source or redistribution license.

This repository does not license the original WHITE ALBUM2 software, data, story, movies, audio, images, fonts, trademarks, or content extracted, transcribed, translated, converted, or decompiled from them. Content without sufficient redistribution permission must not be committed or published. Users must provide legally obtained original game resources locally. See [ASSET_POLICY.md](ASSET_POLICY.md) for repository rules and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for third-party component versions and licenses.

## Porting to Other Platforms

Anyone may directly fork, modify, port, and publish the Apache-2.0 files listed in [PORTING_CODE.md](PORTING_CODE.md). No pull request, separate approval, or contribution of modifications back to this repository is required. Redistributions must include the Apache-2.0 license and [NOTICE](NOTICE), retain applicable copyright and attribution notices, and identify changes in modified files.

Recommended porting workflow:

1. Open the project with the Godot 4.5 .NET edition and install the Godot export templates, compiler, and platform SDK required by the target platform.
2. Do not place original game data in the repository or application package. A port should load resources prepared locally by the user from a legally obtained copy of the game.
3. Build `addons/wmv_video` for the target operating system and CPU architecture. The plugin requires matching godot-cpp libraries, FFmpeg 7.1.5 development libraries, and FFmpeg runtime libraries.
4. Confirm that `addons/wmv_video/wmv_video.gdextension` contains an entry for the target platform, and enable only architectures with available native libraries in the Godot export preset.

Unified build entry points:

```powershell
# Windows x86_64
addons\wmv_video\tools\build.ps1 --platform windows --target both

# Android ARM64; replace the NDK path with the local installation path
addons\wmv_video\tools\build.ps1 --platform android --arch arm64 --android-ndk D:\Android\ndk\25.2.9519653
```

```bash
# Linux x86_64
bash addons/wmv_video/tools/build.sh --platform linux --arch x86_64

# macOS universal
bash addons/wmv_video/tools/build.sh --platform macos --arch universal

# iOS device and simulator XCFramework
bash addons/wmv_video/tools/build.sh --platform ios
```

For dependency locations, FFmpeg SDK layout, optional arguments, and LGPL considerations, see the [Chinese build guide](addons/wmv_video/BUILDING.zh-CN.md) and the [complete build guide](addons/wmv_video/BUILDING.md). Native binaries have currently been produced only for Windows x86_64 and Android ARM64. Android APK packaging has been verified, but no physical-device test result has been recorded. Linux, macOS, iOS, and Android x86_64 remain build configurations that must be compiled and tested on their respective systems.

Port maintainers may change the project name, interface, and implementation or create an independent repository, but Apache-2.0 covers only the original porting code in the authorized file list. `addons/wmv_video` remains under its directory-level MIT License. FFmpeg, godot-cpp, YamlDotNet, and other third-party components remain subject to their own licenses. `assets/sub.yaml` requires separate handling because it includes third-party sources with noncommercial or unidentified licensing. No rights to original game assets are transferred with this repository's code.
