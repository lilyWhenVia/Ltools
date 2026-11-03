# Ltool

一个轻量的 Windows 搜索入口与本地剪贴板工具。**非商业用途免费，源码公开。**

这是参考启动器交互习惯独立编写的工具，不是 uTools 官方产品，也不是其修改版。不包含 uTools 程序、私有代码或素材。

## 下载与安装

前往 [最新版本](https://github.com/lilyWhenVia/Ltool/releases/latest)，下载 **Ltool-Setup-0.1.1-windows-x64.msi**，双击后使用标准 Windows Installer 界面安装。

- Windows 10 / 11，64 位；需 .NET Framework 4.8。通常 Windows 10 较新版本及 Windows 11 已具备运行环境。
- 默认安装到 `%LOCALAPPDATA%\Programs\Ltool`，无需管理员权限。
- 安装器提供桌面与开始菜单快捷方式，可从 Windows“已安装的应用”卸载。
- 也可下载便携版 ZIP，解压后运行 `Ltool.exe`；旁边的 `.config` 文件必须保留。
- 安装包目前未购买代码签名证书，Windows 可能提示发布者未知；请核对下载来源与发布页的 SHA-256 校验文件。

## 能做什么

- 统一搜索应用、文件、工具和剪贴板历史。
- 识别网址、本地文件路径、邮箱和算式，提供对应操作；支持 `/C:/...` 和 `file:///...` 路径。
- 保存最近 100 条文本、图片和文件剪贴板记录，图片直接显示缩略图。
- 紧凑窗口按结果动态展开，支持轻度半透明与多屏 DPI 适配。
- 单击执行候选；切换子应用保留输入，标签可用 Backspace 删除，右侧 × 清空搜索。

## 快捷键

| 操作 | 快捷键 |
| --- | --- |
| 唤起统一搜索 | Alt+空格 |
| 唤起剪贴板 | Ctrl+Shift+V |
| 切换子应用，保留输入 | Tab / Shift+Tab、Alt+左右 |
| 选择候选 | 上下方向键 |
| 执行候选 | Enter 或单击 |
| 选用剪贴板记录 | Enter 或单击，仅更新系统剪贴板并置顶 |
| 粘贴已选记录 | Ctrl+V，此时才返回原窗口输出 |
| 关闭子应用标签 | 输入为空或光标位于最前端时 Backspace |
| 隐藏面板 | Esc |

文字全选或光标到达输入边界时，也可用左右方向键切换分类；文字内部方向键正常编辑。快捷键冲突时退出其他占用程序后重启。

## 隐私与限制

历史、图片和搜索设置保存在 `%LOCALAPPDATA%\LilyTools`（兼容旧版历史），**未加密**，复制的敏感信息也可能被记录。程序不自动上传历史；主动选择网页搜索会将对应文字交给浏览器与搜索引擎。详情见 [PRIVACY.md](PRIVACY.md)。

文件默认索引桌面、文档和 Downloads，可通过托盘菜单添加目录。当前按名称和路径匹配，不支持拼音、文件全文搜索、OCR、插件市场或云同步。索引上限 15000 项、深度 8 层，跳过 `.git`、`node_modules` 与目录链接。

自动粘贴可能被管理员窗口或部分程序拦截，此时切换到目标窗口后手动 Ctrl+V。半透明是整窗轻度透明，不是毛玻璃。默认不设开机自启。卸载保留用户数据，需要清空时请先使用托盘“清空剪贴板历史”。

## 终端防护告警

如果 Sentinel 等终端防护报告 Suspicious activity，请停止安装并联系公司 IT，参阅 [SECURITY_REVIEW.md](SECURITY_REVIEW.md)。更名和标准 MSI 不代表已解除告警，也不能替代管理员审核。新版仍未签名。

## 非商业用途免费

采用 [PolyForm Noncommercial License 1.0.0](LICENSE)。允许的非商业用途可免费使用、研究、修改及依照许可分享；商业用途不在本许可授权范围内，需另行联系作者。完整许可优先于本段摘要。

本项目采用非商业许可，**不是 OSI 意义上的开源项目**。uTools 及其他商标属于各自权利人。

## 从源码构建

Windows PowerShell 中执行：

```powershell
.\scripts\build.ps1
.\scripts\test.ps1
```

应用使用 Windows 自带的 .NET Framework 编译器，MSI 使用 WiX 3.14.1 构建。先运行 `scripts/get-wix.ps1` 下载并核对构建工具哈希，再执行构建脚本。构建产生标准 MSI、便携版和 `SHA256SUMS.txt`。安装定义位于 `installer/Product.wxs`，不使用项目自定义 EXE 安装器。

问题反馈请使用 GitHub Issues；**不要粘贴真实剪贴板历史、密码、令牌或内部文件路径**。
