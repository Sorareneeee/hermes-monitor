<div align="center">

# Hermes Monitor

**See every MCP server and agent Skill running on your machine — no config required.**

<br />

[![Star this repo](https://img.shields.io/github/stars/Sorareneeee/hermes-monitor?style=for-the-badge&logo=github&label=%E2%AD%90%20Star%20this%20repo&color=yellow)](https://github.com/Sorareneeee/hermes-monitor/stargazers)

<br />

[![Release](https://img.shields.io/github/v/release/Sorareneeee/hermes-monitor?style=for-the-badge&color=5B7B5E)](https://github.com/Sorareneeee/hermes-monitor/releases)
&nbsp;
[![Downloads](https://img.shields.io/github/downloads/Sorareneeee/hermes-monitor/total?style=for-the-badge&color=7A9A7D)](https://github.com/Sorareneeee/hermes-monitor/releases)
&nbsp;
[![License](https://img.shields.io/badge/License-MIT-5B7B5E?style=for-the-badge)](LICENSE)
&nbsp;
[![.NET](https://img.shields.io/badge/.NET-10.0-7A9A7D?style=for-the-badge)](https://dotnet.microsoft.com)

---

You have MCP servers running. You have agent Skills installed. You just can't see them all in one place. Hermes Monitor is a native Windows desktop app that auto-discovers every MCP server and Skill on your machine — supporting Claude Code, Codex, Cursor, Windsurf, and any agent using standard config paths. One executable, zero setup, instant visibility.

[Download](#quick-start) · [How It Works](#how-it-works) · [Features](#features) · [Build](#building) · [Contributing](#contributing)

</div>

<br>

## The problem

Your AI agent ecosystem grows faster than you can track. New MCP servers appear with every `npx` command. Skills accumulate across projects. There's no single view that shows what's actually running.

Hermes Monitor solves this. It scans your running processes and agent config directories, then presents everything in a clean, expandable dashboard.

<br>

## Before vs After

| Without Hermes Monitor | With Hermes Monitor |
|---|---|
| Open three terminals to check what's running | Launch one app |
| Dig through config files to find Skill paths | Click to reveal full details |
| Manually grep process lists for MCP servers | Auto-detected and categorized |
| No way to quickly copy package names | One-click clipboard |

<br>

## Quick start

```bash
# Download the latest release from GitHub, or build from source:
git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
./dist/HermesMonitor.exe
```

No runtime required. No dependencies. The executable is self-contained.

<br>

## How it works

On launch, Hermes Monitor runs three scans in sequence:

```
┌──────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  PROCESSES   │ ──▶ │  CONFIG FILES    │ ──▶ │  SKILL DIRS     │
│              │     │                  │     │                 │
│  node        │     │  mcp.json        │     │  ~/.claude/     │
│  python      │     │  config.json     │     │  ~/.codex/      │
│  (WMI query) │     │  settings.json   │     │  ~/.cursor/     │
└──────────────┘     └──────────────────┘     └─────────────────┘
                           │                          │
                           ▼                          ▼
                    ┌────────────────────────────────────────┐
                    │  Unified Dashboard                     │
                    │  MCP tab · Skills tab · Live status    │
                    └────────────────────────────────────────┘
```

### What gets scanned

- **All running `node` and `python` processes** — these are how MCP servers are typically launched
- **All standard agent config dirs** — Claude Code, Codex, Cursor, Windsurf, Copilot
- **All `skills/` directories** — any agent skill installed on your system

<br>

## Features

| What | Why it matters |
|------|----------------|
| **Auto-discovery** | No manual config. Launch the app and everything appears. |
| **Live dashboard** | Auto-refreshes every 60 seconds. Manual refresh on demand. |
| **Expandable cards** | Click any MCP or Skill to see its purpose, runtime, and raw package name. |
| **One-click copy** | Copy raw identifiers to your clipboard for configuration. |
| **Cross-agent** | Works with Claude Code, Codex, Cursor, Windsurf, Copilot — any agent using standard paths. |
| **Self-contained** | Single EXE (~80MB compressed). No .NET runtime needed. |

<br>

## Design

Paper-white background. Sage green accents. Clay orange for interactive elements. Card-based layout with smooth hover transitions and click animations. Built for extended monitoring sessions without visual fatigue.

```
 Background:   #F5F2EB  ·  Paper white
 Primary:      #5B7B5E  ·  Sage green
 Accent:       #C9653B  ·  Clay orange
 Text:         #2C2C2A  ·  Charcoal
```

<br>

## Building

```bash
# Prerequisites: .NET 10 SDK
# https://dotnet.microsoft.com/download/dotnet/10.0

git clone https://github.com/Sorareneeee/hermes-monitor.git
cd hermes-monitor/HermesMonitor

# Debug
dotnet build

# Release (self-contained single-file)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
```

### Stack

| Layer | What |
|---|---|
| **Framework** | .NET 10 WPF |
| **Language** | C# 13 |
| **Process scanning** | WMI via System.Management |
| **Packaging** | Self-contained single-file |

<br>

## Roadmap

- Cross-platform support (Linux · macOS)
- MCP server start/stop panel
- Usage history with charts
- Dark mode

<br>

## Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feature/my-change`
3. Commit: `git commit -m 'Add my change'`
4. Push: `git push origin feature/my-change`
5. Open a PR

Questions or ideas? Open an issue first.

<br>

## License

MIT — see [LICENSE](LICENSE).

<br>

---

<div align="true">

**Does this save you time?**

[![Star this repo](https://img.shields.io/github/stars/Sorareneeee/hermes-monitor?style=for-the-badge&logo=github&label=%E2%AD%90%20Star%20this%20repo&color=yellow)](https://github.com/Sorareneeee/hermes-monitor/stargazers)

Built with .NET 10 WPF for the AI agent ecosystem.

</div>
