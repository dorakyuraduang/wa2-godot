# Apache-2.0 Porting Code Scope

The following paths contain contributor-authored WA2 Godot and native C++ porting code and are
licensed under the Apache License, Version 2.0 in
`LICENSES/Apache-2.0.txt`:

```text
/script/**/*.cs
/scene/**/*.tscn
/shader/**/*.gdshader
/assets/ani/**/*.tres
/assets/font.map
/main.tscn
/project.godot
/wa2.csproj
/default_bus_layout.tres
/export_presets.cfg
/native-cpp/**
```

Anyone may fork, use, modify, port, and redistribute these covered files in a
different repository or product. No pull request, separate approval, or
contribution of modifications back to this repository is required. A
redistributor must still comply with Apache-2.0, including providing the
license, marking modified files, and retaining applicable attribution and
NOTICE information.

The scripts implement resource loading, runtime behavior, and Godot integration.
The shaders and animation resources are contributor-authored implementations
created by reference to visible game behavior and effects, without copying
original source code. The scenes contain contributor-authored Godot node
structures and configuration. `assets/font.map` is contributor-authored project
mapping data. `native-cpp` contains the contributor-authored standalone C++17
runtime, SDL2 frontend, build configuration, tests, and documentation; its
third-party dependencies retain their own licenses.

This license covers only the original expression in the listed files. It does
not cover any external file or content referenced or loaded by them. In
particular, it does not grant rights to WHITE ALBUM2 software, scripts,
dialogue, images, audio, video, fonts, data archives, trademarks, or other
proprietary material.

The following are not covered by this Apache-2.0 scope unless a file is later
added here or receives an explicit file-level license:

- `assets/sub.yaml`;
- all other `assets/**` paths not explicitly listed above; and
- `addons/wmv_video/**`.

`addons/wmv_video` has its own MIT and third-party license terms. See the root
`LICENSE`, `NOTICE`, `ASSET_POLICY.md`, `SUBTITLE_NOTICE.md`, and
`THIRD_PARTY_NOTICES.md` for the complete repository boundaries.
