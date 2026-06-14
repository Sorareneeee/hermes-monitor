<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/Sorareneeee/hermes-monitor/main/assets/logo-dark.svg">
    <img alt="Hermes Monitor" src="https://raw.githubusercontent.com/Sorareneeee/hermes-monitor/main/assets/logo-light.svg" width="420">
  </picture>
</p>

<p align="center">
  <em>A native desktop instrument panel for AI Agent infrastructure — real-time visibility into MCP servers and Skills across your entire ecosystem.</em>
  <br>
  <em>AI Agent 基础设施桌面仪表盘 — 实时监控 MCP 服务与 Skill 技能的全栈状态。</em>
</p>

<p align="center">
  <a href="https://github.com/Sorareneeee/hermes-monitor/releases"><img src="https://img.shields.io/github/v/release/Sorareneeee/hermes-monitor?style=flat-square&label=Release&color=5B7B5E" alt="Release"></a>
  <a href="https://github.com/Sorareneeee/hermes-monitor/releases"><img src="https://img.shields.io/github/downloads/Sorareneeee/hermes-monitor/total?style=flat-square&label=Downloads&color=5B7B5E" alt="Downloads"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-5B7B5E?style=flat-square" alt="License"></a>
  <a href="https://dotnet.microsoft.com"><img src="https://img.shields.io/badge/.NET-10.0-7A9A7D?style=flat-square" alt=".NET 10"></a>
  <a href="https://github.com/Sorareneeee/hermes-monitor/issues"><img src="https://img.shields.io/github/issues/Sorareneeee/hermes-monitor?style=flat-square&label=Issues&color=C9653B" alt="Issues"></a>
</p>

<br>

---

##  /  Overview

**Hermes Monitor** is a native Windows desktop application that provides real-time observability into your AI agent's tooling infrastructure.

**Hermes Monitor** 是一款 Windows 原生桌面应用，为你的 AI Agent 工具链基础设施提供实时可观测能力。它能自动发现并监控开发环境中所有活跃的 MCP（Model Context Protocol）服务和已安装的 Agent 技能（Skills），全面支持 Claude Code、Codex、Cursor、Windsurf 等主流 Agent 框架。

Built with .NET 10 WPF, the application delivers a responsive, low-latency monitoring experience with zero external dependencies. Download the single-file executable and gain instant visibility into your ecosystem — no configuration required.

基于 .NET 10 WPF 构建，响应迅速、延迟极低，无任何外部依赖。单文件可执行程序，下载即用，无需任何配置。

<br>

---

## Features  / 特性

| Capability | Detail | 说明 |
|---|---|---|
| **Automatic Discovery** | Scans running processes and agent config directories to enumerate every MCP server and Skill | 自动扫描运行进程和 Agent 配置目录，枚举所有 MCP 服务与技能 |
| **Cross-Framework** | Supports Claude Code, Codex, Cursor, Windsurf, Copilot, and more | 兼容 Claude Code、Codex、Cursor、Windsurf、Copilot 等主流框架 |
| **Live Monitoring** | Auto-refresh every 60s with manual refresh; hover states, click animations | 每 60 秒自动刷新，支持手动刷新；悬停动效与点击反馈 |
| **Detail Expansion** | Click any card to reveal Chinese descriptions, package names, runtime paths | 点击卡片展开中文用途说明、原始包名、运行时路径等技术详情 |
| **Clipboard Copy** | One-click copy of raw package names for rapid configuration | 一键复制原始包名，便于快速配置与集成 |
| **Self-Contained** | Single-file executable (~80MB). No runtime required | 单文件可执行程序，无需安装 .NET 运行时 |

<br>

---

## Quick Start  /  快速开始

```bash
# Download / 下载最新版本
# https://github.com/Sorareneeee/hermes-monitor/releases

# Or build from source / 或从源码构建
git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist

# Run / 运行
./dist/HermesMonitor.exe
```

<br>

---

## How It Works  /  工作原理

Hermes Monitor performs a comprehensive scan across your system on launch:

启动时，Hermes Monitor 会对系统进行全面扫描：

### Step 1: Process Scanning / 进程扫描

Queries the Windows process table via WMI, identifying running `node` and `python` processes to detect active MCP server instances.

通过 WMI 查询 Windows 进程表，识别运行中的 `node` 和 `python` 进程，检测活跃的 MCP 服务实例。

### Step 2: Configuration Scanning / 配置扫描

Crawls standard agent configuration directories for `mcp.json`, `config.json`, and `settings.json` files.

遍历标准 Agent 配置目录，查找 `mcp.json`、`config.json`、`settings.json` 等配置文件。

### Step 3: Skill Enumeration / 技能枚举

Reads installed agent skills from all discovered `skills/` directories.

读取所有发现的 `skills/` 目录中的已安装 Agent 技能。

### Supported Agents / 支持的 Agent

| Agent | Configuration Locations / 配置位置 |
|---|---|
| **Claude Code** | `~/.claude/` · `%APPDATA%/Claude/` · `%LOCALAPPDATA%/Claude/` |
| **Codex** | `~/.codex/` · `%APPDATA%/Codex/` · `%LOCALAPPDATA%/Codex/` |
| **Cursor** | `~/.cursor/` · `%APPDATA%/Cursor/` |
| **Windsurf** | `~/.windsurf/` |
| **GitHub Copilot** | `~/.copilot/` |

<br>

---

## Interface  /  界面

### Controls / 操作

| Interaction | Behavior | 功能 |
|---|---|---|
| **↻** | Force refresh all data | 强制刷新全部数据 |
| **✕** | Close application | 关闭应用 |
| **Card click** | Expand / collapse detail section | 展开 / 收起详情 |
| **📋** | Copy raw identifier to clipboard | 复制原始标识符到剪贴板 |
| **Tab bar** | Toggle between MCP and Skills views | 切换 MCP / Skills 视图 |

### Design System / 设计系统

The interface employs a considered visual language to reduce fatigue during extended monitoring sessions:

界面采用经过深思熟虑的视觉语言，降低长时间监控时的视觉疲劳：

```
 Background 背景    #F5F2EB   Paper white / 纸质米白
 Primary 主色        #5B7B5E   Sage green / 复古绿
 Secondary 辅助色    #7A9A7D   Soft sage / 浅灰绿
 Accent 点缀色       #C9653B   Clay orange / 焦橙色
 Text Primary 正文   #2C2C2A   Charcoal / 炭黑
 Text Secondary 辅助 #6B6A66   Warm grey / 暖灰
```

Card-based information architecture provides clear content hierarchy at a glance.

卡片式信息架构，内容层级一目了然。

<br>

---

## Architecture  /  架构

```
hermes-monitor/
├── HermesMonitor/
│   ├── App.xaml              # Application entry / 应用入口
│   ├── App.xaml.cs
│   ├── MainWindow.xaml       # XAML layout / 布局定义
│   ├── MainWindow.xaml.cs    # Core logic / 核心逻辑
│   └── HermesMonitor.csproj  # .NET 10 project config / 项目配置
├── dist/                     # Published executable / 构建输出
├── assets/                   # Logos and resources
└── README.md
```

### Stack / 技术栈

| Component | Technology |
|---|---|
| **Framework** | .NET 10 Windows Presentation Foundation (WPF) |
| **Language** | C# 13 |
| **Process Introspection** | System.Management (WMI) |
| **Packaging** | Single-file self-contained deployment |

<br>

---

## Building  /  编译

```bash
# Prerequisites: .NET 10 SDK / 前置条件：.NET 10 SDK
# https://dotnet.microsoft.com/download/dotnet/10.0

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor

# Debug build / 调试构建
dotnet build

# Release build (self-contained) / 发布构建（自包含）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
```

<br>

---

## Roadmap  /  路线图

- [ ] **Cross-platform support / 跨平台支持** — Linux & macOS via .NET MAUI or Avalonia
- [ ] **MCP server management / 服务管理** — Start/stop control panel
- [ ] **Usage statistics / 使用统计** — Historical charts and metrics
- [ ] **Theme system / 主题系统** — Dark mode toggle
- [ ] **Plugin system / 插件系统** — Custom data sources

<br>

---

## Contributing  /  参与贡献

Contributions are welcome and appreciated. Please open an issue for discussion before submitting significant changes.

欢迎贡献代码。在提交重大变更之前，请先创建 Issue 进行讨论。

1. Fork the repository / Fork 本仓库
2. Create a feature branch / 创建功能分支 (`git checkout -b feature/amazing-feature`)
3. Commit your changes / 提交变更 (`git commit -m 'Add amazing feature'`)
4. Push to the branch / 推送到远端 (`git push origin feature/amazing-feature`)
5. Open a Pull Request / 创建 Pull Request

<br>

---

## License  /  许可

This project is licensed under the MIT License.

本项目基于 MIT 许可证发布。

---

<p align="center">
  <sub>Built with .NET 10 WPF · Designed for the AI agent ecosystem</sub>
  <br>
  <sub>基于 .NET 10 WPF 构建 · 专为 AI Agent 生态设计</sub>
  <br><br>
  <a href="https://github.com/Sorareneeee/hermes-monitor/issues">Report Bug / 报告问题</a> ·
  <a href="https://github.com/Sorareneeee/hermes-monitor/issues">Request Feature / 请求功能</a> ·
  <a href="https://github.com/Sorareneeee/hermes-monitor/discussions">Discussions / 讨论</a>
</p>
