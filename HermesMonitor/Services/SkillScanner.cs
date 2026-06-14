using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HermesMonitor.Models;

namespace HermesMonitor.Services;

public class SkillScanner
{
    private static readonly Dictionary<string, string> KnownDesc = new()
    {
        ["TrendRadar"] = "AI-powered trend monitoring: multi-platform aggregation + RSS + sentiment analysis + multi-channel push.",
        ["claude-mem"] = "Persistent memory: cross-session memory with automatic MEMORY.md writes.",
        ["frontend-design"] = "Frontend UI design generator: create distinctive web interfaces.",
        ["magazine-web-ppt"] = "Web PPT generator: single HTML horizontal-flip presentation deck.",
        ["multi-search-engine"] = "Multi-search engine: 16 engines, no API keys needed.",
        ["self-improving-agent"] = "Self-improving learning: automatic error and experience logging.",
        ["web-fetch-network-troubleshooter"] = "Web fetch diagnostics: Claude Code's official network troubleshooting tool.",
    };

    private static readonly Dictionary<string, string> Icons = new()
    {
        ["TrendRadar"] = "📊", ["claude-mem"] = "🧠", ["frontend-design"] = "🎨",
        ["magazine-web-ppt"] = "📑", ["multi-search-engine"] = "🔍",
        ["self-improving-agent"] = "🔄", ["web-fetch-network-troubleshooter"] = "🌐",
    };

    public List<Item> Scan(List<string> skillDirs)
    {
        var r = new List<Item>(); var seen = new HashSet<string>();
        foreach (var d in skillDirs)
        {
            if (!Directory.Exists(d)) continue;
            foreach (var s in Directory.GetDirectories(d))
            {
                var n = Path.GetFileName(s);
                if (!seen.Add(n)) continue;
                var desc = KnownDesc.GetValueOrDefault(n, "");
                if (string.IsNullOrEmpty(desc)) { var m = ReadJson(Path.Combine(s, "_meta.json")); if (m != null) desc = JStr(m.Value, "description"); }
                if (string.IsNullOrEmpty(desc) && File.Exists(Path.Combine(s, "SKILL.md"))) desc = "Installed agent skill (SKILL.md).";
                r.Add(new Item { Name = n, Icon = Icons.GetValueOrDefault(n, "🧩"), Desc = desc });
            }
        }
        return r;
    }

    private static JsonElement? ReadJson(string p) { try { if (!File.Exists(p)) return null; var c = File.ReadAllText(p); return string.IsNullOrWhiteSpace(c) ? null : JsonSerializer.Deserialize<JsonElement>(c); } catch { return null; } }
    private static string JStr(JsonElement e, string k) => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";
}
