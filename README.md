# Hermes Monitor

**AI Agent Ecosystem Monitor — Native Windows Desktop Application**

---

## 中文介绍

Hermes Monitor 是一款基于 .NET 10 的 Windows 原生桌面应用，专为 AI Agent 生态系统的实时监控而设计。它能够自动发现并可视化展示你系统中所有正在运行的 MCP（Model Context Protocol）服务与已安装的 Agent 技能（Skills）。无论你使用的是 Claude Code、Codex、Cursor 还是 Windsurf，Hermes Monitor 都能自动扫描并呈现完整的工具链状态。

### 核心特性

| 特性 | 说明 |
|------|------|
| **自动发现** | 自动扫描系统进程和配置目录，无需手动配置即可发现所有 MCP 服务和 Agent 技能 |
| **多 Agent 支持** | 兼容 Claude Code、Codex、Cursor、Windsurf、Copilot 等主流 AI Agent 框架 |
| **实时监控** | 每 60 秒自动刷新，也可手动点击刷新按钮立即更新 |
| **详情展开** | 点击任意卡片可展开查看用途说明、包名、运行时路径等技术细节 |
| **一键复制** | 点击 📋 按钮即可复制原始包名，方便配置和集成 |
| **自包含可执行文件** | 单文件发布，无需安装 .NET 运行时，双击即用 |
| **现代复古设计** | 纸质米白背景搭配复古绿主色调，焦橙色点缀，营造「温暖科技」的高级感 |

### 支持扫描的 Agent 目录

- **Claude Code** — `~/.claude/`、`%APPDATA%/Claude/`、`%LOCALAPPDATA%/Claude/`
- **Codex** — `~/.codex/`、`%APPDATA%/Codex/`、`%LOCALAPPDATA%/Codex/`
- **Cursor** — `~/.cursor/`、`%APPDATA%/Cursor/`
- **Windsurf** — `~/.windsurf/`
- **Copilot** — `~/.copilot/`

### 安装与使用

```bash
# 从 GitHub Releases 下载最新版本
# 或从源码构建：

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
```

直接运行 `HermesMonitor.exe` 即可。程序会自动扫描系统，无需任何配置。

### 操作说明

| 操作 | 功能 |
|------|------|
| **↻** | 手动刷新数据 |
| **✕** | 关闭窗口 |
| **单击卡片** | 展开/收起详情 |
| **📋** | 复制原始包名到剪贴板 |
| **标签切换** | ⚡ MCP / 🧩 Skills 标签页切换 |

### 技术栈

- **框架**: .NET 10 WPF (Windows Presentation Foundation)
- **语言**: C# 13
- **UI**: XAML + 纯代码绘制
- **进程扫描**: WMI (System.Management)
- **发布**: 自包含单文件 (Self-contained single-file)

### 配色方案

Hermes Monitor 采用精心设计的「现代复古」配色——冷静的灰绿色为主调，温暖的焦橙色作为操作点缀，纸质米白背景降低视觉疲劳，营造专业而舒适的监控体验。

---

## English Description

Hermes Monitor is a native Windows desktop application built with .NET 10 WPF, designed for real-time monitoring of AI Agent ecosystems. It automatically discovers and visualizes all running MCP (Model Context Protocol) servers and installed Agent skills across your system. Whether you use Claude Code, Codex, Cursor, or Windsurf, Hermes Monitor scans and presents your complete toolchain status at a glance.

### Key Features

| Feature | Description |
|---------|-------------|
| **Auto Discovery** | Automatically scans system processes and config directories — no manual setup required |
| **Multi-Agent Support** | Compatible with Claude Code, Codex, Cursor, Windsurf, Copilot and other AI agent frameworks |
| **Real-time Monitoring** | Auto-refreshes every 60 seconds; manual refresh available on demand |
| **Expandable Details** | Click any card to reveal purpose descriptions, package names, runtime paths and technical details |
| **One-Click Copy** | Copy raw package names with a single click for easy configuration and integration |
| **Self-Contained Binary** | Single-file executable — no .NET runtime installation required, just double-click to run |
| **Modern Vintage Design** | Paper-white background with sage green palette and warm clay orange accents |

### Scanned Agent Directories

- **Claude Code** — `~/.claude/`, `%APPDATA%/Claude/`, `%LOCALAPPDATA%/Claude/`
- **Codex** — `~/.codex/`, `%APPDATA%/Codex/`, `%LOCALAPPDATA%/Codex/`
- **Cursor** — `~/.cursor/`, `%APPDATA%/Cursor/`
- **Windsurf** — `~/.windsurf/`
- **Copilot** — `~/.copilot/`

### Installation & Usage

```bash
# Download the latest release from GitHub Releases
# Or build from source:

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
```

Simply run `HermesMonitor.exe`. The application will automatically scan your system — no configuration needed.

### Controls

| Action | Function |
|--------|----------|
| **↻** | Refresh data manually |
| **✕** | Close window |
| **Click card** | Expand/collapse details |
| **📋** | Copy raw package name to clipboard |
| **Tab switch** | Toggle between ⚡ MCP / 🧩 Skills tabs |

### Tech Stack

- **Framework**: .NET 10 WPF (Windows Presentation Foundation)
- **Language**: C# 13
- **UI**: XAML + Code-behind
- **Process Scanning**: WMI (System.Management)
- **Deployment**: Self-contained single-file executable

### Color Palette

```
Background:      #F5F2EB  ·  Paper white / 纸质米白
Primary Green:   #5B7B5E  ·  Sage green / 复古绿
Light Green:     #7A9A7D  ·  Soft sage / 浅灰绿
Dark Green:      #3D5B40  ·  Deep forest / 深墨绿
Accent Orange:   #C9653B  ·  Warm clay / 焦橙
Text Primary:    #2C2C2A  ·  Charcoal / 炭黑
Text Secondary:  #6B6A66  ·  Warm grey / 暖灰
```

### Screenshots

> *Screenshots coming soon. For now, build and run the application to experience the interface.*

---

## Building from Source

```bash
# Clone the repository
git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor

# Publish self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist

# The output HermesMonitor.exe will be in the dist/ directory
```

## Release Notes

### v1.0.0 (2026-06-14)

- Initial public release
- Real-time MCP server auto-discovery via process scanning
- Agent skill directory scanning across multiple frameworks
- Expandable detail cards with curated descriptions
- One-click package name copy
- Vintage green/paper-white design system
- Self-contained .NET 10 single-file executable (~135MB)

---

## License

MIT © 2026

---

<div align="center">
  <sub>Built with .NET 10 WPF · Designed for the AI Agent ecosystem</sub>
  <br>
  <sub>基于 .NET 10 WPF 构建 · 专为 AI Agent 生态打造</sub>
</div>
