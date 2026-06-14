<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/Sorareneeee/hermes-monitor/main/assets/logo-dark.svg">
    <img alt="Hermes Monitor" src="https://raw.githubusercontent.com/Sorareneeee/hermes-monitor/main/assets/logo-light.svg" width="280">
  </picture>
</p>

<p align="center">
  <em>A native desktop instrument panel for AI Agent infrastructure — real-time visibility into MCP servers and Skills across your entire agent ecosystem.</em>
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

**Hermes Monitor** is a Windows-native desktop application that provides real-time observability into your AI agent's tooling infrastructure. It automatically discovers and monitors all active MCP (Model Context Protocol) servers and installed agent Skills across your development environment — supporting Claude Code, Codex, Cursor, Windsurf, and other major agent frameworks.

Built with .NET 10 WPF, the application delivers a responsive, low-latency monitoring experience with zero external dependencies. Download the single-file executable and gain instant visibility into your agent ecosystem — no configuration required.

<br>

## Features

| Capability | Detail |
|---|---|
| **Automatic Discovery** | Scans running processes and agent configuration directories to enumerate every MCP server and Skill on your system — no manual setup |
| **Cross-Framework Support** | Compatible with Claude Code, Codex, Cursor, Windsurf, Copilot, and any agent that stores configuration in standard locations |
| **Live Monitoring** | Auto-refreshes every 60 seconds with manual refresh on demand. Hover states, click feedback, and smooth transitions throughout |
| **Detail Expansion** | Click any card to reveal curated purpose descriptions (Chinese), raw package names, runtime paths, and technical metadata |
| **Clipboard Integration** | One-click copy of raw package names for rapid configuration and toolchain integration |
| **Self-Contained Binary** | Single-file executable (~80MB compressed). No .NET runtime, no dependencies — just download and run |

## Quick Start

```bash
# Download the latest release
# https://github.com/Sorareneeee/hermes-monitor/releases

# Or build from source
git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist

# Run
./dist/HermesMonitor.exe
```

## How It Works

### Discovery Mechanism

On launch, Hermes Monitor performs a comprehensive scan across your system:

1. **Process Scanning** — Queries the Windows process table via WMI, identifying running `node` and `python` processes to detect active MCP server instances
2. **Configuration Scanning** — Crawls standard agent configuration directories for `mcp.json`, `config.json`, and `settings.json` files
3. **Skill Enumeration** — Reads installed agent skills from all discovered `skills/` directories

### Supported Agent Paths

| Agent | Configuration Location |
|---|---|
| **Claude Code** | `~/.claude/` · `%APPDATA%/Claude/` · `%LOCALAPPDATA%/Claude/` |
| **Codex** | `~/.codex/` · `%APPDATA%/Codex/` · `%LOCALAPPDATA%/Codex/` |
| **Cursor** | `~/.cursor/` · `%APPDATA%/Cursor/` |
| **Windsurf** | `~/.windsurf/` |
| **GitHub Copilot** | `~/.copilot/` |

## Interface

### Controls

| Interaction | Behavior |
|---|---|
| **↻** | Force refresh all data |
| **✕** | Close application |
| **Card click** | Expand / collapse detail section |
| **📋** | Copy raw identifier to system clipboard |
| **Tab bar** | Toggle between MCP and Skills views |

### Design System

The interface employs a considered visual language — a warm paper-white background (`#F5F2EB`) reduces visual fatigue during extended monitoring sessions, while sage green (`#5B7B5E`) anchors the primary actions and clay orange (`#C9653B`) draws attention to interactive elements. Card-based information architecture provides clear content hierarchy.

## Architecture

```
hermes-monitor/
├── HermesMonitor/
│   ├── App.xaml              # Application entry point
│   ├── MainWindow.xaml       # XAML layout definition
│   ├── MainWindow.xaml.cs    # Core application logic
│   └── HermesMonitor.csproj  # .NET 10 project configuration
├── dist/                     # Published executable output
└── README.md
```

### Stack

| Component | Technology |
|---|---|
| **Framework** | .NET 10 Windows Presentation Foundation (WPF) |
| **Language** | C# 13 |
| **Process Introspection** | System.Management (WMI) |
| **Packaging** | Single-file self-contained deployment |

## Building

```bash
# Prerequisites: .NET 10 SDK
# https://dotnet.microsoft.com/download/dotnet/10.0

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor

# Debug build
dotnet build

# Release build (self-contained)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
```

## Roadmap

- [ ] Linux and macOS support (via .NET MAUI or Avalonia)
- [ ] MCP server start/stop management
- [ ] Historical usage statistics and charts
- [ ] Dark mode toggle
- [ ] Plugin system for custom data sources

## Contributing

Contributions are welcome and appreciated. Please open an issue for discussion before submitting significant changes.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  <sub>Built with .NET 10 WPF · Designed for the AI agent ecosystem</sub>
  <br>
  <a href="https://github.com/Sorareneeee/hermes-monitor/issues">Report Bug</a> ·
  <a href="https://github.com/Sorareneeee/hermes-monitor/issues">Request Feature</a> ·
  <a href="https://github.com/Sorareneeee/hermes-monitor/discussions">Discussions</a>
</p>
