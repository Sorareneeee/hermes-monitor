using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.Json;
using System.Text.RegularExpressions;
using HermesMonitor.Models;

namespace HermesMonitor.Services;

public class McpScanner
{
    private static readonly Dictionary<string, string> KnownDesc = new()
    {
        ["@playwright/mcp"] = "Browser automation: control Chromium/Firefox/WebKit for navigation, clicks, screenshots, forms, network interception.",
        ["chrome-devtools-mcp"] = "Chrome DevTools integration: debug console logs, network requests, DOM inspection, performance traces, Lighthouse audits.",
        ["@agentdeskai/browser-tools-mcp"] = "Browser diagnostics: real-time console logs, network details, SEO analysis, accessibility audits.",
        ["@upstash/context7-mcp"] = "Instant programming docs: search latest official docs and code examples for any library or framework.",
        ["stock-images-mcp"] = "Stock image search: aggregate Pexels, Pixabay, Unsplash — search and download.",
        ["scrapling"] = "Advanced web scraping: anti-bot bypass, Cloudflare Turnstile auto-solve, JS rendered pages.",
        ["semble"] = "Semantic code search: search codebases with natural language queries.",
        ["@github/mcp"] = "GitHub MCP server: manage Issues, PRs, code search, repository operations.",
        ["github-mcp-server"] = "GitHub official MCP server: Go implementation with OAuth support.",
    };

    private static readonly Dictionary<string, string> NiceName = new()
    {
        ["@playwright/mcp"] = "Playwright",
        ["chrome-devtools-mcp"] = "Chrome DevTools",
        ["@agentdeskai/browser-tools-mcp"] = "Browser Tools",
        ["@upstash/context7-mcp"] = "Context7 Docs",
        ["stock-images-mcp"] = "Stock Images",
        ["scrapling"] = "Scrapling",
        ["semble"] = "Semble",
    };

    public List<Item> Scan(List<string> mcpJsonFiles)
    {
        var results = new List<Item>();
        var seen = new HashSet<string>();
        ScanProcesses(results, seen);
        foreach (var path in mcpJsonFiles) ScanConfigFile(path, results, seen);
        return results;
    }

    private void ScanProcesses(List<Item> results, HashSet<string> seen)
    {
        Dictionary<int, string> cl = new();
        try { using var s = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process"); foreach (var o in s.Get()) cl[Convert.ToInt32(o["ProcessId"])] = o["CommandLine"]?.ToString() ?? ""; } catch { }
        foreach (var p in Process.GetProcesses())
        {
            try
            {
                if (p.ProcessName is not ("node" or "python" or "go" or "github-mcp-server")) continue;
                if (!cl.TryGetValue(p.Id, out var cmd) || string.IsNullOrEmpty(cmd)) continue;
                var raw = ExtractName(cmd, p.ProcessName);
                if (raw == null || !seen.Add(raw)) continue;
                results.Add(new Item { Name = NiceName.GetValueOrDefault(raw, raw), Raw = raw, Desc = KnownDesc.GetValueOrDefault(raw, ""), Icon = "⚡", IsMcp = true });
            }
            catch { }
        }
    }

    private void ScanConfigFile(string path, List<Item> results, HashSet<string> seen)
    {
        try
        {
            var root = ReadJson(path);
            if (root == null) return;
            var servers = default(JsonElement);
            if (root.Value.TryGetProperty("mcpServers", out var ms1)) servers = ms1;
            if (servers.ValueKind != JsonValueKind.Object && root.Value.TryGetProperty("mcp_servers", out var ms2)) servers = ms2;
            if (servers.ValueKind != JsonValueKind.Object && root.Value.TryGetProperty("servers", out var ms3)) servers = ms3;
            if (servers.ValueKind != JsonValueKind.Object) return;
            foreach (var entry in servers.EnumerateObject())
            {
                if (!seen.Add(entry.Name)) continue;
                var desc = KnownDesc.GetValueOrDefault(entry.Name, "");
                if (string.IsNullOrEmpty(desc) && entry.Value.ValueKind == JsonValueKind.Object && entry.Value.TryGetProperty("command", out var ce) && ce.ValueKind == JsonValueKind.String)
                {
                    var cs = ce.GetString() ?? "";
                    if (cs.Contains("github")) desc = KnownDesc.GetValueOrDefault("@github/mcp", "GitHub MCP server.");
                    else if (cs.Contains("postgres") || cs.Contains("database")) desc = "Database MCP: query and operate databases.";
                    else if (cs.Contains("filesystem")) desc = "Filesystem MCP: read, write, search local files.";
                    else if (cs.Contains("brave")) desc = "Brave Search MCP: web and local search.";
                    else if (cs.Contains("playwright")) desc = KnownDesc.GetValueOrDefault("@playwright/mcp", "Browser automation MCP server.");
                }
                results.Add(new Item { Name = NiceName.GetValueOrDefault(entry.Name, entry.Name), Raw = entry.Name, Desc = desc, Icon = "⚡", IsMcp = true });
            }
        }
        catch { }
    }

    private static JsonElement? ReadJson(string p) { try { if (!File.Exists(p)) return null; var c = File.ReadAllText(p); return string.IsNullOrWhiteSpace(c) ? null : JsonSerializer.Deserialize<JsonElement>(c); } catch { return null; } }

    private static string? ExtractName(string cmd, string pn)
    {
        if (pn == "github-mcp-server") return "github-mcp-server";
        var m = Regex.Match(cmd, @"npx[^""]*""[^""]*""\s+""([^""]+)""");
        if (m.Success) { var p = m.Groups[1].Value; var a = p.IndexOf("@", StringComparison.Ordinal); if (a > 0 && p[a - 1] != '/') p = p[..a]; return p; }
        var m2 = Regex.Match(cmd, @"npx\\.+\\\.\.\\(@?[^\\]+)");
        if (m2.Success) return m2.Groups[1].Value;
        if (cmd.Contains("scrapling")) return "scrapling";
        if (cmd.Contains("semble")) return "semble";
        if (cmd.Contains("chrome-devtools")) return "chrome-devtools-mcp";
        if (cmd.Contains("stock-images")) return "stock-images-mcp";
        if (cmd.Contains("github-mcp-server")) return "github-mcp-server";
        if (cmd.Contains("context7")) return "@upstash/context7-mcp";
        return null;
    }
}
