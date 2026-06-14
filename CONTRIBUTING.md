# Contributing to Hermes Monitor

Thank you for considering contributing to Hermes Monitor! We welcome contributions of all kinds: bug reports, feature suggestions, documentation improvements, and code changes.

## Getting Started

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/your-username/hermes-monitor.git`
3. **Create a branch**: `git checkout -b feature/my-change`
4. **Open in IDE**: Open `HermesMonitor/HermesMonitor.csproj` in your preferred .NET IDE

## Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows 10 or later (WPF dependency)

### Build & Run

```bash
# Debug build
dotnet build HermesMonitor/HermesMonitor.csproj

# Release (self-contained single-file)
dotnet publish HermesMonitor/HermesMonitor.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../dist
```

### Code Style

- Use **file-scoped namespaces** (`namespace X;`)
- Use **implicit usings** (already enabled in `.csproj`)
- Follow **C# 13** conventions
- Keep methods focused and under 50 lines where possible
- Add XML doc comments for public APIs

## Pull Request Checklist

- [ ] Build succeeds with no warnings
- [ ] Tested on actual hardware (not just build)
- [ ] UI changes include screenshots
- [ ] New features include documentation updates

## Questions?

Open a [GitHub Discussion](https://github.com/Sorareneeee/hermes-monitor/discussions) for questions and ideas.
