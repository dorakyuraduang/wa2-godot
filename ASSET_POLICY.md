# Asset and Repository Policy

This repository contains Apache-2.0 porting code listed in `PORTING_CODE.md`, a separately MIT-licensed WMV plugin, and other files with different or unspecified license status. It does not grant any rights to WHITE ALBUM2 or to material extracted, copied, translated, transcribed, converted, decompiled, or otherwise derived from the original game.

Do not commit or upload:

- original `.pak` archives or files extracted from them;
- movie, voice, BGM, sound-effect, image, font, or script data from the game;
- built APK/AAB packages containing any of those assets;
- signing keystores, private keys, passwords, tokens, or local environment files;
- Godot import caches, Android build directories, or native build intermediates.

The following local files are specifically excluded because this repository does not contain sufficient redistribution permission for them:

- `assets/fonts/AlibabaPuHuiTi-3-65-Medium.ttf`;
- files under `assets/grp`, `assets/movie`, `assets/se`, `assets/IC`, `assets/fonts/cn`, and `assets/fonts/jp`.

`assets/ani/**/*.tres` and `assets/font.map` are intentionally tracked,
contributor-authored project configuration covered by the Apache-2.0 scope in
`PORTING_CODE.md`. This grant covers their original Godot configuration and
mapping data, not any external textures or other resources they reference.

`assets/sub.yaml` is intentionally tracked mixed-source subtitle data and is not
covered by Apache-2.0. See `SUBTITLE_NOTICE.md`. Its presence does not represent
a repository-wide grant of rights to the subtitle text.

The project uses Godot's bundled default font for its normal UI, so the excluded Alibaba font is not required by a clean clone. Animation `.tres` files may be committed when they are contributor-authored configuration and do not embed original images, audio, dialogue, or other game data.

The `.gitignore` file enforces the common paths and extensions. Before every push, review `git status` and the staged diff. A file being technically uploadable does not mean its copyright owner has authorized redistribution.

Files intended for Git include contributor-authored files whose provenance and redistribution permission have been verified, the Apache-2.0 paths listed in `PORTING_CODE.md`, plugin descriptors and source, redistributable plugin runtime binaries, build instructions, and required license notices. A project file must not be treated as contributor-authored merely because it has been converted to a Godot format.

This policy is a repository hygiene measure, not legal advice. Contributors remain responsible for confirming that they own or have permission to distribute every submitted file.
