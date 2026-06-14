using System.Text.Json;
using HermesMonitor.Models;

namespace HermesMonitor.Services;

public class PathDiscoveryService
{
    public List<string> McpJsonFiles { get; } = new();
    public List<string> SkillDirs { get; } = new();

    public void Discover()
    {
        McpJsonFiles.Clear(); SkillDirs.Clear();
        var h = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var a = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var l = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var cs = new HashSet<string>();
        foreach (var dirs in new[] {
            new[] { Path.Combine(h, ".claude") },
            new[] { Path.Combine(a, "Claude"), Path.Combine(l, "Claude") },
            new[] { Path.Combine(h, ".codex") },
            new[] { Path.Combine(h, ".cursor") },
            new[] { Path.Combine(h, ".windsurf"), Path.Combine(h, ".codeium", "windsurf") },
            new[] { Path.Combine(h, ".opencode"), Path.Combine(Path.GetTempPath(), "opencode") },
            new[] { Path.Combine(h, ".gemini") },
            new[] { Path.Combine(h, ".copilot") },
            new[] { Path.Combine(a, "Code", "User"), Path.Combine(l, "Code", "User") },
        }) foreach (var d in dirs) if (Directory.Exists(d) || File.Exists(d)) cs.Add(d);
        foreach (var root in new[] { h, a, l }) { try { foreach (var d in Directory.GetDirectories(root)) { foreach (var c in new[] { ".claude", ".cursor" }) { var cd = Path.Combine(d, c); if (Directory.Exists(cd)) cs.Add(cd); } } } catch { } }
        foreach (var f in new[] { Path.Combine(h, ".claude.json"), Path.Combine(h, ".copilot", "mcp-config.json"), Path.Combine(h, ".gemini", "settings.json") }) { if (File.Exists(f)) McpJsonFiles.Add(f); }
        var cj = Path.Combine(h, ".claude.json");
        if (File.Exists(cj)) { try { var root = ReadJson(cj); if (root?.TryGetProperty("projects", out var pj) == true && pj.ValueKind == JsonValueKind.Object) foreach (var p in pj.EnumerateObject()) { if (p.Value.ValueKind != JsonValueKind.Object || !p.Value.TryGetProperty("mcpServers", out _)) continue; var pp = p.Name.Replace('/', Path.DirectorySeparatorChar); foreach (var c in new[] { Path.Combine(pp, ".claude", "mcp.json"), Path.Combine(pp, ".mcp.json"), Path.Combine(pp, "mcp.json") }) { if (File.Exists(c) && !McpJsonFiles.Contains(c)) McpJsonFiles.Add(c); } } } catch { } }
        foreach (var d in cs) { if (!Directory.Exists(d)) continue; var sd = Path.Combine(d, "skills"); if (Directory.Exists(sd)) SkillDirs.Add(sd); foreach (var f in Directory.GetFiles(d, "*.json")) { var fn = Path.GetFileName(f).ToLower(); if (fn.Contains("mcp") || fn.Contains("config") || fn.Contains("setting")) McpJsonFiles.Add(f); } }
        try { var exeDir = AppDomain.CurrentDomain.BaseDirectory; if (!string.IsNullOrEmpty(exeDir)) { var di = new DirectoryInfo(exeDir); while (di != null) { TryAdd(Path.Combine(di.FullName, ".claude", "mcp.json")); TryAdd(Path.Combine(di.FullName, ".mcp.json")); TryAdd(Path.Combine(di.FullName, "mcp.json")); di = di.Parent; } } var cwd = new DirectoryInfo(Environment.CurrentDirectory); if (cwd.FullName != AppDomain.CurrentDomain.BaseDirectory) { while (cwd != null) { TryAdd(Path.Combine(cwd.FullName, ".claude", "mcp.json")); TryAdd(Path.Combine(cwd.FullName, ".mcp.json")); TryAdd(Path.Combine(cwd.FullName, "mcp.json")); cwd = cwd.Parent; } } } catch { }
        try { foreach (var dr in DriveInfo.GetDrives()) { if (dr.DriveType != DriveType.Fixed || !dr.IsReady) continue; try { foreach (var sub in Directory.GetDirectories(dr.RootDirectory.FullName)) { TryAdd(Path.Combine(sub, ".claude", "mcp.json")); TryAdd(Path.Combine(sub, ".mcp.json")); TryAdd(Path.Combine(sub, "mcp.json")); } } catch { } } } catch { }
        foreach (var sk in new[] { Path.Combine(h, ".claude", "skills"), Path.Combine(h, ".gemini", "skills"), Path.Combine(h, ".opencode", "skills"), Path.Combine(h, ".cursor", "skills"), Path.Combine(a, "Claude", "skills"), }) { if (Directory.Exists(sk) && !SkillDirs.Contains(sk)) SkillDirs.Add(sk); }
    }
    private void TryAdd(string p) { if (File.Exists(p) && !McpJsonFiles.Contains(p)) McpJsonFiles.Add(p); }
    private static JsonElement? ReadJson(string p) { try { if (!File.Exists(p)) return null; var c = File.ReadAllText(p); return string.IsNullOrWhiteSpace(c) ? null : JsonSerializer.Deserialize<JsonElement>(c); } catch { return null; } }
}
