<div align="center">

# Hermes Monitor

**See every MCP server and agent Skill running on your machine — no config required.**

**实时查看你机器上所有的 MCP 服务和 Agent 技能，无需任何配置。**

<br />

[![Star this repo](https://img.shields.io/github/stars/Sorareneeee/hermes-monitor?style=for-the-badge&logo=github&label=%E2%AD%90%20Star%20this%20repo&color=yellow)](https://github.com/Sorareneeee/hermes-monitor/stargazers)
&nbsp;
[![Follow](https://img.shields.io/badge/Follow_%40Sorareneeee-000000?style=for-the-badge&logo=x&logoColor=white)](https://x.com/Sorareneeee)

<br />

[![Release](https://img.shields.io/github/v/release/Sorareneeee/hermes-monitor?style=for-the-badge&color=5B7B5E)](https://github.com/Sorareneeee/hermes-monitor/releases)
&nbsp;
[![Downloads](https://img.shields.io/github/downloads/Sorareneeee/hermes-monitor/total?style=for-the-badge&color=7A9A7D)](https://github.com/Sorareneeee/hermes-monitor/releases)
&nbsp;
[![License](https://img.shields.io/badge/License-MIT-5B7B5E?style=for-the-badge)](LICENSE)
&nbsp;
[![.NET](https://img.shields.io/badge/.NET-10.0-7A9A7D?style=for-the-badge)](https://dotnet.microsoft.com)
&nbsp;
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-C9653B?style=for-the-badge)](https://github.com/Sorareneeee/hermes-monitor/pulls)

---

You have MCP servers running. You have agent Skills installed. You just can't see them all in one place. Hermes Monitor is a native Windows desktop app that auto-discovers every MCP server and Skill on your machine — one executable, zero setup, instant visibility.

你的 MCP 服务在跑，你的 Agent 技能已安装，但你无法在一个地方看到它们全部。Hermes Monitor 是一款 Windows 原生桌面应用，自动发现机器上所有的 MCP 服务和 Agent 技能。一个可执行文件，零配置，即刻可见。

[Download](#quick-start) · [How It Works](#how-it-works) · [Features](#features) · [Build](#building) · [中文说明](#中文说明)

</div>

<br>

## The Problem

Your AI agent ecosystem grows faster than you can track. New MCP servers appear with every `npx` command. Skills accumulate across projects. There is no single view that shows what's actually running.

你的 AI Agent 生态增长速度快到你无法追踪。每次执行 `npx` 都可能产生新的 MCP 服务，技能文件在各个项目中堆积。没有一个统一的视图告诉你当前到底在运行什么。

Hermes Monitor solves this. It scans your running processes and agent config directories, then presents everything in a clean, expandable dashboard.

Hermes Monitor 解决了这个问题。它扫描你正在运行的进程和 Agent 配置目录，然后将所有信息呈现在一个干净、可展开的仪表盘中。

<br>

## Before vs After

| 之前 Without | 之后 With |
|---|---|
| 打开三个终端查看运行状态 | 启动一个应用 |
| 翻遍配置文件找 Skill 路径 | 点击即可查看全部详情 |
| 手动 grep 进程列表找 MCP 服务 | 自动检测并分类 |
| 没办法快速复制包名 | 一键复制到剪贴板 |

<br>

## Quick Start

```bash
# Download the latest release, or build from source:
# 下载最新版本，或从源码构建：
git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
./dist/HermesMonitor.exe
```

No runtime required. No dependencies. The executable is self-contained. Run it from anywhere — it will find your MCP configs automatically.

无需运行环境，无依赖，单文件可执行。放在任何位置运行，它会自动找到你的所有 MCP 配置。

<br>

## How It Works

On launch, Hermes Monitor runs three scans in sequence:

启动时，Hermes Monitor 依次执行三轮扫描：

```
┌──────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  PROCESSES   │ ──▶ │  CONFIG FILES    │ ──▶ │  SKILL DIRS     │
│  进程扫描     │     │  配置文件扫描     │     │  技能目录扫描    │
│              │     │                  │     │                 │
│  node        │     │  mcp.json        │     │  ~/.claude/     │
│  python      │     │  config.json     │     │  ~/.codex/      │
│  (WMI query) │     │  settings.json   │     │  ~/.cursor/     │
└──────────────┘     └──────────────────┘     └─────────────────┘
                           │                          │
                           ▼                          ▼
                    ┌────────────────────────────────────────┐
                    │  Unified Dashboard                     │
                    │  统一仪表盘                             │
                    │  MCP tab · Skills tab · Live status    │
                    │  MCP 标签 · 技能标签 · 实时状态        │
                    └────────────────────────────────────────┘
```

### Three-Stage Discovery / 三阶段发现

**Stage 1: Process Scan / 进程扫描**

Scans all running `node` and `python` processes via WMI to detect active MCP server instances. Also detects `github-mcp-server` and other binary-based MCP servers.

通过 WMI 扫描所有正在运行的 `node` 和 `python` 进程，检测活跃的 MCP 服务实例。同时支持 `github-mcp-server` 等二进制 MCP 服务。

**Stage 2: Config Scan / 配置扫描**

Crawls agent config directories and parses `~/.claude.json` projects to find every `mcp.json`, `config.json`, and `settings.json` across your system.

遍历 Agent 配置目录，解析 `~/.claude.json` 中的 projects 区块，找到系统中所有的 `mcp.json`、`config.json` 和 `settings.json`。

**Stage 3: Skill Scan / 技能扫描**

Enumerates installed agent skills from all discovered `skills/` directories across Claude Code, Codex, Cursor, and other agents.

从所有发现的 `skills/` 目录中枚举已安装的 Agent 技能，覆盖 Claude Code、Codex、Cursor 等。

<br>

### Supported Agents / 支持的 Agent

| Agent | Config Location / 配置位置 |
|---|---|
| **Claude Code** | `~/.claude/` · `%APPDATA%/Claude/` · `%LOCALAPPDATA%/Claude/` |
| **Codex** | `~/.codex/` · `%APPDATA%/Codex/` · `%LOCALAPPDATA%/Codex/` |
| **Cursor** | `~/.cursor/` · `%APPDATA%/Cursor/` |
| **Windsurf** | `~/.windsurf/` · `~/.codeium/windsurf/` |
| **OpenCode** | `~/.opencode/` |
| **Gemini CLI** | `~/.gemini/` |
| **GitHub Copilot** | `~/.copilot/` |
| **VS Code (MCP)** | `%APPDATA%/Code/User/` · `%LOCALAPPDATA%/Code/User/` |

Key insight: the EXE also walks **up from its own location** and **scans all fixed drive roots** to find `.claude/mcp.json` in any project directory. Put it anywhere and it just works.

关键设计：EXE 还会从**自身所在位置向上遍历**，并**扫描所有硬盘根目录**，找到任意项目目录下的 `.claude/mcp.json`。放在任何位置都能用。

<br>

## Features

| 功能 | 说明 |
|------|------|
| **Auto-discovery / 自动发现** | Launch and everything appears. No manual config. / 启动即用，无需手动配置 |
| **Live dashboard / 实时仪表盘** | Auto-refreshes every 60s. Manual refresh on demand. / 每 60 秒自动刷新，支持手动刷新 |
| **Expandable cards / 可展开卡片** | Click any item to see purpose, runtime, and package details / 点击任意项目查看用途、运行时和包详情 |
| **One-click copy / 一键复制** | Copy raw identifiers to your clipboard / 复制原始标识符到剪贴板 |
| **Cross-agent / 多 Agent 支持** | Works with 8+ agent frameworks / 兼容 8 种以上 Agent 框架 |
| **Self-contained / 单文件运行** | Single EXE (~80MB). No .NET runtime needed / 单文件可执行，无需 .NET 运行时 |

<br>

## Design

Paper-white background. Sage green accents. Clay orange for interactive elements. Card-based layout with smooth hover transitions and click animations. Built for extended monitoring sessions without visual fatigue.

纸质米白背景，复古绿主色调，焦橙色点缀交互元素。卡片式布局，平滑的悬停过渡和点击动画。专为长时间监控设计，减少视觉疲劳。

```
 Background:   #F5F2EB  ·  Paper white / 纸质米白
 Primary:      #5B7B5E  ·  Sage green / 复古绿
 Accent:       #C9653B  ·  Clay orange / 焦橙色
 Text:         #2C2C2A  ·  Charcoal / 炭黑
```

<br>

## Building

```bash
# Prerequisites: .NET 10 SDK
# 前置条件：.NET 10 SDK
# https://dotnet.microsoft.com/download/dotnet/10.0

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor

# Debug build / 调试构建
dotnet build

# Release build (self-contained single-file) / 发布构建（自包含单文件）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
```

### Tech Stack / 技术栈

| Layer | Technology |
|---|---|
| **Framework** | .NET 10 WPF |
| **Language** | C# 13 |
| **Process Scanning** | WMI via System.Management |
| **Config Parsing** | System.Text.Json |
| **Packaging** | Self-contained single-file deploy |

<br>

## 中文说明

Hermes Monitor 是一款面向 AI Agent 生态系统的 Windows 原生桌面监控工具。它能自动发现并展示你电脑上所有正在运行的 MCP（Model Context Protocol）服务和已安装的 Agent 技能。

### 核心优势

- **全面兼容** — 支持 Claude Code、Codex、Cursor、Windsurf、OpenCode、Gemini CLI、Copilot、VS Code 等 8 种以上 Agent 框架
- **三源扫描** — 进程扫描 + 配置文件扫描 + 技能目录扫描，三重保障不错过任何服务
- **全盘搜索** — 自动遍历硬盘根目录、`~/.claude.json` projects 配置、EXE 所在目录，无论你把项目放在哪里都能找到
- **中文描述** — 内置常用 MCP 和 Skill 的中文用途说明，展开卡片即可查看
- **零配置** — 下载即用，无需安装 .NET 运行时，无需设置环境变量

### 操作方式

| 操作 | 功能 |
|------|------|
| **↻** | 强制刷新全部数据 |
| **✕** | 关闭应用 |
| **单击卡片** | 展开 / 收起详情信息 |
| **📋** | 复制原始包名到剪贴板 |
| **标签栏** | 在 MCP / Skills 视图间切换 |

<br>

## Roadmap

- [ ] Cross-platform support (Linux · macOS) / 跨平台支持
- [ ] MCP server start/stop control / MCP 服务启停管理
- [ ] Usage history with charts / 使用统计与图表
- [ ] Dark mode / 深色模式
- [ ] Plugin system for custom data sources / 自定义数据源插件系统

<br>

## Contributing

1. Fork the repo / Fork 本仓库
2. Create a branch: `git checkout -b feature/my-change` / 创建功能分支
3. Commit: `git commit -m 'Add my change'` / 提交变更
4. Push: `git push origin feature/my-change` / 推送到远端
5. Open a PR / 创建 Pull Request

Questions or ideas? Open an issue first.

有问题或想法？请先创建 Issue。

<br>

## License

MIT — see [LICENSE](LICENSE).

<br>

---

<div align="center">

**Does this save you time? / 对你有帮助吗？**

[![Star this repo](https://img.shields.io/github/stars/Sorareneeee/hermes-monitor?style=for-the-badge&logo=github&label=%E2%AD%90%20Star%20this%20repo&color=yellow)](https://github.com/Sorareneeee/hermes-monitor/stargazers)

Built with .NET 10 WPF for the AI agent ecosystem.

基于 .NET 10 WPF 构建，专为 AI Agent 生态设计。

</div>
