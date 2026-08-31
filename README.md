【游戏安装方法】  
1.安装程序： 下载并安装 APK 文件。   
2.复制文件： 将 PC 版游戏文件夹整体复制到手机 根目录。  
3.重命名： 将该文件夹重命名为 Wa2Res。  
【针对安卓平台的操作优化】  
1.游戏中长按屏幕任意位置可以快进,默认关闭需要在设置页面里把画面显示更改为全屏模式后才能开启  
2.游戏中按下返回键,弹出返回主菜单的确认弹窗  
⚠️ 重要注意事项：  
1.pc版的游戏文件请自行获取  
2.严禁套多层文件夹！ 请确保 Wa2Res 文件夹内直接是游戏数据，避免路径错误。  
3.关于更新： 后续更新只需覆盖安装最新版 APK 即可，无需重复复制数据包。  
4.关于卸载： 卸载游戏 APP 不会自动删除 Wa2Res 数据文件夹，如需彻底清除请手动前往根目录删除。  
5.请在首次启动时授权游戏文件读写权限,若未授权程序无法读取到本地的资源文件  
6.如果游戏内出现游玩一定时间卡死,可以尝试在设置页面里把画面显示更改为全屏模式  
文件放置参考示例  
![e74b63e84385f853f6112d605b38093c](https://github.com/user-attachments/assets/303a7d6e-99e1-4683-bed9-671d271e08be)  

## 视频播放插件

项目使用 `addons/wmv_video` 中的 FFmpeg GDExtension 播放原始 `.pak` 影片。该插件只接受内容为 ASF、视频编码为 WMV1/WMV2/WMV3/VC-1 的文件；文件扩展名可以是 `.wmv` 或 `.pak`。

- Windows：x86_64
- Android：`arm64-v8a`，最低 API 24
- FFmpeg：7.1.5，最小 LGPL 2.1-or-later shared 构建

Windows 与 Android 原生运行库已经放在插件的 `bin` 目录中。Android 导出预设会自动把 GDExtension 及其 FFmpeg shared 依赖放进 APK。

## 许可与素材边界

本仓库不适用统一的项目级 MIT 许可证，具体范围见 [LICENSE](LICENSE)：

- [PORTING_CODE.md](PORTING_CODE.md) 明确列出的原创资源读取脚本、shader、Godot 场景、动画配置、字体映射和项目配置按 Apache License 2.0 开放，任何人都可直接 fork、修改、移植和再发布，无需提交 PR、单独申请批准或把修改交回本仓库；
- `addons/wmv_video` 中由贡献者原创的插件源码、构建工具和文档按该目录内的 MIT License 开放；
- FFmpeg、godot-cpp、libwinpthread 和 YamlDotNet 保留各自许可证；
- `assets/sub.yaml` 是带有萌娘百科和 CK-GAL汉化组来源标记的混合第三方字幕，不属于 Apache-2.0，具体限制见 [SUBTITLE_NOTICE.md](SUBTITLE_NOTICE.md)；
- 未被 `PORTING_CODE.md`、组件许可证或文件级 SPDX 标记明确覆盖的文件，本仓库暂不授予复制、修改、再发布或销售许可；
- 源码能够查看不等于已经取得开源或再发布授权。

《WHITE ALBUM2》的原始软件、数据、剧情、影片、音频、图片、字体、商标，以及从中提取、转写、翻译、转换或反编译得到的内容，均不由本仓库授权。没有充分传播授权的内容不得提交或发布。运行游戏所需的合法原作资源须由使用者在本地自行提供；具体规则见 [ASSET_POLICY.md](ASSET_POLICY.md)，第三方组件版本和许可见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。

## 移植到其他平台

任何人都可以基于 [PORTING_CODE.md](PORTING_CODE.md) 列出的 Apache-2.0 代码直接 fork、修改、移植并发布自己的版本，无需提交 PR、征得单独批准或把修改交回本仓库。再发布时必须附带 Apache-2.0 许可证和 [NOTICE](NOTICE)，保留适用的版权与归属声明，并在修改过的文件中说明变更。

建议按以下顺序进行移植：

1. 使用 Godot 4.5 .NET 版本打开项目，并安装目标平台对应的 Godot 导出模板、编译器和平台 SDK。
2. 不要把原作数据放入仓库或安装包。移植版本应在运行时读取用户从合法游戏副本自行准备的资源。
3. 为目标系统和 CPU 架构构建 `addons/wmv_video`。插件需要匹配架构的 godot-cpp、FFmpeg 7.1.5 开发库和运行库。
4. 检查 `addons/wmv_video/wmv_video.gdextension` 中是否存在目标平台条目，并在 Godot 导出预设中只启用已经生成原生库的架构。

统一构建入口：

```powershell
# Windows x86_64
addons\wmv_video\tools\build.ps1 --platform windows --target both

# Android ARM64；NDK 路径按本机安装位置修改
addons\wmv_video\tools\build.ps1 --platform android --arch arm64 --android-ndk D:\Android\ndk\25.2.9519653
```

```bash
# Linux x86_64
bash addons/wmv_video/tools/build.sh --platform linux --arch x86_64

# macOS universal
bash addons/wmv_video/tools/build.sh --platform macos --arch universal

# iOS 设备与模拟器 XCFramework
bash addons/wmv_video/tools/build.sh --platform ios
```

依赖目录、FFmpeg SDK 布局、可选参数和 LGPL 注意事项见 [中文构建说明](addons/wmv_video/BUILDING.zh-CN.md) 与 [完整构建说明](addons/wmv_video/BUILDING.md)。当前只有 Windows x86_64 和 Android ARM64 生成过二进制；Android 已验证 APK 打包但尚未记录真机测试，Linux、macOS、iOS 和 Android x86_64 仍属于构建配置，移植者需要在对应系统上自行编译和测试。

移植者可以更改项目名称、界面和实现，也可以建立独立仓库，但 Apache-2.0 只覆盖授权清单中的原创移植代码。`addons/wmv_video` 继续使用其目录内的 MIT License；FFmpeg、godot-cpp、YamlDotNet 等第三方组件继续遵守各自许可证；`assets/sub.yaml` 需要单独处理其第三方来源和非商业/未知授权部分；原作资源和权利不随本仓库代码一起授权。
