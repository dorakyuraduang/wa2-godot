# WA2 C++ Runtime

这是以 `wa2-godot` 仓库 `main` 分支的 Godot/C# 实现为行为参考迁移出的原生 C++17 运行时。当前优先目标是 Windows
调试；核心和 SDL2 前端不依赖 Godot、C# 或 .NET，后续可复用到 Linux、Android、
macOS、iOS 和 Nintendo Switch Homebrew。

项目不包含游戏 PAK、字体、电影、语音、图片、脚本或其他受版权保护的素材。运行者
必须自行从合法持有的游戏副本准备资源。

## 当前可用功能

- PACK/LAC 目录解析，按需读取文件，不会把数 GB PAK 整包载入内存；
- 原版 LZSS 解压，兼容两种压缩长度头；
- CP932 到 UTF-8 文本转换；
- BNR 脚本 VM、跳转、表达式、局部变量、函数调用和脚本调用栈；
- 背景、角色、精灵、消息、选项、日历和基础天气状态；
- 背景交叉淡化、灰度遮罩转场、角色淡入淡出和黑白场渐变；
- 精灵普通/加色/通道遮罩混合及透明度缓动；
- SDL2 图片、字体、键盘、鼠标、手柄和音频前端；
- BGM、语音和音效按需从 PAK 播放；
- 与原实现布局一致的 `sys.sav` 系统标志、CG 标志和已读位；
- 可选 FFmpeg 视频解码构架。

Windows 已用本地真实 PAK 验证标题菜单和 `2001` 脚本启动。核心自动测试覆盖 PACK、
LZSS、BNR VM 和 `sys.sav` 往返。

## Windows 快速开始

准备 64 位 MinGW-w64，并安装 SDL2、SDL2_image、SDL2_mixer、SDL2_ttf 的开发包。
脚本会优先检查 `$env:MINGW_PREFIX`，随后检查 `D:\mingw64` 和
`C:\msys64\mingw64`。

```powershell
cd D:\godot_cs\wa2
.\build-windows.ps1 -Configuration Debug -RunTests
```

如果工具链不在上述位置：

```powershell
.\build-windows.ps1 -Configuration Debug -MingwRoot C:\your\mingw64 -RunTests
```

运行：

```powershell
.\build-windows\Debug\wa2_cpp.exe `
  --res .\assets `
  --save .\sav `
  --font C:\Windows\Fonts\msyh.ttc
```

直接启动章节：

```powershell
.\build-windows\Debug\wa2_cpp.exe --res .\assets `
  --save .\sav --font C:\Windows\Fonts\msyh.ttc --script 2001
```

Debug 程序静态链接 SDL2 和 MinGW 运行库，因此不需要复制 SDL DLL；程序自身保留 GDB
符号。构建脚本只会清除预编译 SDL 静态库中会让旧 MinGW 生成无效 PE 的第三方 DWARF
节。

## 控制

| 操作 | 键盘 | 手柄 |
|---|---|---|
| 确认/推进 | Enter 或 Space | A |
| 选择 | 上/下 | D-pad 上/下 |
| 按住快进 | Ctrl 或 Tab | L |
| 自动模式 | X | X |
| 退出 | Esc | B |

## 资源目录

`--res` 指向含原始 PAK 的目录。至少需要所选语言脚本包及脚本引用的资源包，例如：

```text
Wa2Res/
  ck-gal.pak        # 中文脚本，--lang cn
  script.pak        # 日文脚本，--lang jp
  bak.pak
  grp.pak
  char.pak
  BGM.PAK
  VOICE.PAK
  SE.PAK
  IC/
    bak.pak
    grp.pak
    char.pak
    BGM.PAK
    VOICE.PAK
    SE.PAK
```

缺少非必要包会输出 warning；缺少当前语言脚本包会终止启动。电影 PAK 保持在相同目录。

## 其他桌面系统

Linux/macOS 使用 CMake 和 pkg-config：

```sh
cmake -S . -B build -DCMAKE_BUILD_TYPE=Debug -DWA2_WITH_FFMPEG=OFF
cmake --build build -j
ctest --test-dir build --output-on-failure
```

需要开发包：SDL2、SDL2_image、SDL2_mixer、SDL2_ttf；开启电影时还需要 FFmpeg 的
`libavcodec/libavformat/libavutil/libswresample/libswscale`。这些平台目前尚未做实机验证。

## 已知限制

- 完整游戏槽位存档/读档尚未移植；当前只兼容 `sys.sav`。
- 复杂转场、部分精灵动画和专用菜单仍是简化实现。
- FFmpeg 路径当前只解码视频画面，电影音轨尚未接入混音器。
- Android、iOS、Switch 的平台工程、生命周期和发布打包尚未完成。
- 本项目是运行时移植，不提供或授权任何游戏内容。

结构和后续平台约束见 [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)，Windows 调试细节见
[docs/WINDOWS_DEBUG.md](docs/WINDOWS_DEBUG.md)。
