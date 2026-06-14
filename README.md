# Hermes Monitor

<div align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-5B7B5E?style=flat-square" alt=".NET 10">
  <img src="https://img.shields.io/badge/WPF-Windows-7A9A7D?style=flat-square" alt="WPF">
  <img src="https://img.shields.io/badge/license-MIT-5B7B5E?style=flat-square" alt="MIT">
  <br><br>
  <p><strong>A sophisticated desktop monitor for AI Agent ecosystems — MCP servers & Skills at a glance.</strong></p>
  <p>AI Agent 生态监控桌面工具 — 一眼掌握所有 MCP 服务与 Skill 技能的状态。</p>
</div>

---

##  Overview · 概览

**Hermes Monitor** is a native Windows desktop application built with .NET 10 WPF that provides real-time visibility into your AI Agent's tool ecosystem. It automatically discovers and displays:

- **MCP Servers** — All running Model Context Protocol servers detected from system processes
- **Skills** — All installed agent skills found across Claude Code, Codex, Cursor, and other agent directories

| ![Screenshot](docs/screenshot.png) |
|:--:|
| *Vintage-green themed dashboard with warm clay accents* |

## ✨ Features · 功能

| Feature | Description |
|---------|-------------|
| **Auto-discovery** | Scans system processes & config directories — no manual setup |
| **Real-time monitoring** | Auto-refresh every 60s, with manual refresh on demand |
| **Expand details** | Click any card to reveal purpose description & technical details |
| **One-click copy** | Copy raw package names for easy configuration |
| **Agent-wide scanning** | Supports Claude Code, Codex, Cursor, Windsurf, Copilot |
| **Vintage design** | Calm green palette with warm orange accents, paper-like background |
| **Portable EXE** | Self-contained single-file executable, no runtime required |

##  Installation · 安装

### Download

Download the latest release from the [Releases](https://github.com/YOUR_USERNAME/hermes-monitor/releases) page.

Or build from source:

```bash
git clone https://github.com/YOUR_USERNAME/hermes-monitor.git
cd hermes-monitor/HermesMonitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
```

### Requirements

- **Windows 10/11** (x64)
- No additional runtime needed (self-contained build)

##  Usage · 使用

Simply run `HermesMonitor.exe`. The application will:

1. Automatically scan all running `node` and `python` processes for MCP servers
2. Search common agent config directories for MCP configurations
3. Discover installed skills from all agent skill directories
4. Display everything in a clean, organized dashboard

### Controls

| Control | Action |
|---------|--------|
| **↻** | Refresh data |
| **✕** | Close window |
| **Click card** | Expand/collapse details |
| **📋** | Copy raw package name |
| **Tab** | Switch between MCP / Skills |

##  Architecture · 架构

```
HermesMonitor/
├── HermesMonitor.csproj    # .NET 10 WPF project
├── App.xaml / .cs           # Application entry
├── MainWindow.xaml          # UI layout (XAML)
├── MainWindow.xaml.cs       # Logic (C#)
├── dist/                    # Published EXE output
└── docs/                    # Documentation
```

### Built-in Descriptions

The app ships with curated descriptions for common MCP packages and skills. Unknown items are shown with their raw names and can be identified from their command-line paths.

##  Color Palette · 配色

```css
Background:      #F5F2EB  /* Paper white */
Primary Green:   #5B7B5E  /* Sage green */
Light Green:     #7A9A7D  /* Soft sage */
Dark Green:      #3D5B40  /* Deep forest */
Accent Orange:   #C9653B  /* Warm clay */
Text Primary:    #2C2C2A  /* Charcoal */
Text Secondary:  #6B6A66  /* Warm grey */
```

##  License

MIT © 2026

---

<div align="center">
  <sub>Built with .NET 10 WPF · Inspired by modern SaaS dashboard design</sub>
</div>
