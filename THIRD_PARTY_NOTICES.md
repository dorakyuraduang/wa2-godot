# Third-Party Notices

This repository is not licensed as a single work. Only files explicitly identified by a component license or file-level notice receive that license. The following components retain their own licenses.

## WA2 Godot porting code

- License: Apache License 2.0
- Scope: the exact paths listed in `PORTING_CODE.md`
- Full license: `LICENSES/Apache-2.0.txt`

These files are contributor-authored resource readers, runtime logic, shaders, Godot scenes, animation configuration, font mapping, and project integration files. They are intended to work with data supplied separately by a user. The Apache-2.0 grant covers only that original implementation and does not license referenced game data or other proprietary material.

## Subtitle data

- Location: `assets/sub.yaml`
- License: mixed third-party content; not covered by Apache-2.0
- Detailed notice: `SUBTITLE_NOTICE.md`

Embedded comments attribute portions to 萌娘百科 (Moegirlpedia) and CK-GAL汉化组. Moegirlpedia states that its text is generally available under CC BY-NC-SA 3.0 CN unless otherwise noted, but the exact source pages and any underlying third-party lyrics or dialogue are not fully documented in this repository. No redistribution license has been identified for the CK-GAL portions. Redistributors must verify, replace, or remove this data as appropriate.

## WMV Video GDExtension

- Version: 0.6.0
- License: MIT
- Location: `addons/wmv_video`
- Copyright: WMV Video plugin contributors

The plugin source and binary are separate from original WHITE ALBUM2 game data.

## FFmpeg

- Version: 7.1.5 (`n7.1.5`, commit `3a0867c2bfda4a4d4309ca1a8cbdc6175e67f587`)
- Upstream source: https://github.com/FFmpeg/FFmpeg/tree/n7.1.5
- License used by these binaries: GNU LGPL version 2.1 or later
- Full license: `addons/wmv_video/licenses/FFmpeg-LGPL-2.1-or-later.txt`

The bundled Windows and Android FFmpeg libraries are minimal shared builds. They were configured with GPL and nonfree components disabled and only the ASF demuxer, WMV/VC-1 video decoders, WMA audio decoders, `avcodec`, `avformat`, `avutil`, `swresample`, and `swscale` enabled. Users may replace the shared libraries with a compatible modified build, as permitted by the LGPL.

The optional iOS build tooling packages FFmpeg static libraries as XCFramework dependencies. An application distributor using that configuration must provide a practical way to relink the application with a modified FFmpeg, such as application object files and link instructions, or obtain a different FFmpeg license. Publishing only this plugin source does not by itself satisfy that static-linking obligation.

The exact build flags and reproducible commands are documented in `addons/wmv_video/BUILDING.md`.

## godot-cpp

- Commit: `27d9dd23c83871e0619fca5dc2cddfbfd69e926a` from the Godot 4.5 branch
- Upstream source: https://github.com/godotengine/godot-cpp
- License: MIT
- Full license: `addons/wmv_video/licenses/godot-cpp-MIT.txt`

## YamlDotNet

- Version: 16.3.0
- Upstream source: https://github.com/aaubry/YamlDotNet
- License: MIT
- Full license: `LICENSES/YamlDotNet-MIT.txt`

## Godot Engine and other export-template components

Godot Engine is licensed under the MIT License. Android export templates also contain third-party components whose notices are distributed by the Godot project. See the Godot Engine license and copyright documentation for the template version used to create a release.

## Excluded proprietary material

WHITE ALBUM2 game files, media, fonts, trademarks, and other original assets are not licensed by this repository. See `ASSET_POLICY.md`.

Files extracted, copied, translated, transcribed, converted, decompiled, or otherwise derived from proprietary material are likewise not covered by the WMV plugin's MIT License. No license is granted for other unmarked repository files; see the root `LICENSE` for the controlling scope notice.
