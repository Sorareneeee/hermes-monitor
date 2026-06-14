using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HermesMonitor;

public partial class MainWindow : Window
{
    private readonly List<string> _skillsDirs = new();
    private readonly List<string> _mcpJsonFiles = new();
    private readonly HashSet<string> _seenAcrossAllScans = new();

    private static readonly Color CardBg = Color.FromRgb(250, 248, 244);
    private static readonly Color CardBorder = Color.FromRgb(230, 224, 212);
    private static readonly Color GreenC = Color.FromRgb(91, 123, 94);
    private static readonly Color GreenDark = Color.FromRgb(61, 91, 64);
    private static readonly Color GreenLight = Color.FromRgb(122, 154, 125);
    private static readonly Color OrangeC = Color.FromRgb(201, 101, 59);
    private static readonly Color TextPri = Color.FromRgb(44, 44, 42);
    private static readonly Color TextSec = Color.FromRgb(107, 106, 102);
    private static readonly Color TextDim = Color.FromRgb(156, 154, 148);

    private static readonly Dictionary<string, string> McpD = new()
    {
        ["@playwright/mcp"] = "浏览器自动化：控制 Chromium/Firefox/WebKit 进行页面导航、点击、截图、表单填写、网络拦截等操作。",
        ["chrome-devtools-mcp"] = "Chrome 开发者工具集成：调试和分析浏览器页面，包括控制台日志、网络请求监控、DOM 检查、性能追踪、Lighthouse 审计。",
        ["@agentdeskai/browser-tools-mcp"] = "浏览器诊断工具：实时获取浏览器控制台日志、网络请求详情、SEO 分析、可访问性审计。",
        ["@upstash/context7-mcp"] = "编程文档即时检索：为任何编程语言、框架或库搜索最新官方文档和代码示例。",
        ["stock-images-mcp"] = "图库搜索下载：聚合 Pexels、Pixabay、Unsplash 三大图库。",
        ["scrapling"] = "高级网页抓取：反爬虫绕过、Cloudflare 验证码自动求解、JS 渲染页面抓取。",
        ["semble"] = "代码语义搜索：用自然语言搜索代码库。",
        ["@github/mcp"] = "GitHub MCP 服务器：管理 Issues、PR、代码搜索、仓库操作等。",
        ["github-mcp-server"] = "GitHub 官方 MCP 服务器：Go 语言实现，支持 OAuth 认证。",
    };

    private static readonly Dictionary<string, string> NiceName = new()
    {
        ["@playwright/mcp"] = "Playwright", ["chrome-devtools-mcp"] = "Chrome DevTools",
        ["@agentdeskai/browser-tools-mcp"] = "Browser Tools", ["@upstash/context7-mcp"] = "Context7 Docs",
        ["stock-images-mcp"] = "Stock Images", ["scrapling"] = "Scrapling", ["semble"] = "Semble",
    };

    private static readonly Dictionary<string, string> SkillD = new()
    {
        ["TrendRadar"] = "AI 舆情监控系统：多平台热点聚合 + RSS + 情感分析 + 多渠道推送。",
        ["claude-mem"] = "持久化记忆系统：跨会话保持记忆，自动写入 MEMORY.md。",
        ["frontend-design"] = "前端 UI 设计生成：创建鲜明设计风格的 Web 界面。",
        ["magazine-web-ppt"] = "网页 PPT 生成器：单 HTML 横向翻页演示文稿。",
        ["multi-search-engine"] = "多搜索引擎聚合：16 个引擎，无需 API Key。",
        ["self-improving-agent"] = "自我改进学习系统：自动记录经验和错误。",
        ["web-fetch-network-troubleshooter"] = "网络抓取故障诊断：Claude Code 官方网络诊断工具。",
    };

    private static readonly Dictionary<string, string> SkillI = new()
    {
        ["TrendRadar"] = "📊", ["claude-mem"] = "🧠", ["frontend-design"] = "🎨",
        ["magazine-web-ppt"] = "📑", ["multi-search-engine"] = "🔍",
        ["self-improving-agent"] = "🔄", ["web-fetch-network-troubleshooter"] = "🌐",
    };

    public MainWindow() { InitializeComponent(); DiscoverAll(); LoadData(); var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(60) }; t.Tick += (_, _) => LoadData(); t.Start(); }

    private void Close_Click(object s, RoutedEventArgs e) => Close();
    private void Refresh_Click(object s, RoutedEventArgs e) { _seenAcrossAllScans.Clear(); LoadData(); }
    private void BtnEnter(object s, MouseEventArgs e) { if (s is TextBlock tb) tb.Foreground = new SolidColorBrush(GreenC); }
    private void BtnLeave(object s, MouseEventArgs e) { if (s is TextBlock tb) tb.Foreground = new SolidColorBrush(TextSec); }
    private void TabMCP_Click(object s, MouseButtonEventArgs e) => SwitchTab(0);
    private void TabSkills_Click(object s, MouseButtonEventArgs e) => SwitchTab(1);

    private void SwitchTab(int i)
    {
        ScrollerMCP.Visibility = i == 0 ? Visibility.Visible : Visibility.Collapsed;
        ScrollerSkills.Visibility = i == 1 ? Visibility.Visible : Visibility.Collapsed;
        MCPTabBg.Background = new SolidColorBrush(i == 0 ? GreenC : Colors.Transparent);
        MCPTabText.Foreground = new SolidColorBrush(i == 0 ? Colors.White : TextSec);
        SkillsTabBg.Background = new SolidColorBrush(i == 1 ? GreenC : Colors.Transparent);
        SkillsTabText.Foreground = new SolidColorBrush(i == 1 ? Colors.White : TextSec);
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }

    // ──────────────────────────────────────────────
    // Phase 1: Discover paths across ALL agent frameworks
    // ──────────────────────────────────────────────
    private void DiscoverAll()
    {
        var h = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var a = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var l = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var cs = new HashSet<string>();

        // === Agent config directories (for scanning JSON files & skills) ===
        string[][] agents = new[] {
            // Claude Code (user-level)
            new[] { Path.Combine(h, ".claude") },
            // Claude Desktop
            new[] { Path.Combine(a, "Claude"), Path.Combine(l, "Claude") },
            // Codex CLI
            new[] { Path.Combine(h, ".codex") },
            // Cursor
            new[] { Path.Combine(h, ".cursor") },
            // Windsurf (Codeium)
            new[] { Path.Combine(h, ".windsurf"), Path.Combine(h, ".codeium", "windsurf") },
            // OpenCode
            new[] { Path.Combine(h, ".opencode"), Path.Combine(Path.GetTempPath(), "opencode") },
            // Gemini CLI
            new[] { Path.Combine(h, ".gemini") },
            // GitHub Copilot CLI
            new[] { Path.Combine(h, ".copilot") },
            // VS Code (user-level MCP)
            new[] { Path.Combine(a, "Code", "User"), Path.Combine(l, "Code", "User") },
        };

        foreach (var dirs in agents)
            foreach (var d in dirs)
                if (Directory.Exists(d) || File.Exists(d))
                    cs.Add(d);

        // Scan all repo directories that have .claude subdirectory (project-level configs)
        foreach (var root in new[] { h, a, l })
        {
            try
            {
                foreach (var d in Directory.GetDirectories(root))
                {
                    var cd = Path.Combine(d, ".claude");
                    if (Directory.Exists(cd)) cs.Add(cd);
                    var cd2 = Path.Combine(d, ".cursor");
                    if (Directory.Exists(cd2)) cs.Add(cd2);
                }
            }
            catch { }
        }

        // Also scan the root-level MCP config files at ~/
        string[] rootMcpFiles = new[] {
            Path.Combine(h, ".claude.json"),
            Path.Combine(h, ".copilot", "mcp-config.json"),
            Path.Combine(h, ".gemini", "settings.json"),
        };
        foreach (var f in rootMcpFiles)
            if (File.Exists(f)) _mcpJsonFiles.Add(f);

        // Collect all JSON files from agent config dirs
        foreach (var d in cs)
        {
            if (!Directory.Exists(d)) continue;

            // Skills directories
            var s = Path.Combine(d, "skills");
            if (Directory.Exists(s)) _skillsDirs.Add(s);

            // Gemini CLI skills
            var gs = Path.Combine(d, "skills");
            if (Directory.Exists(gs) && !_skillsDirs.Contains(gs)) _skillsDirs.Add(gs);

            // Codex skills
            var cxs = Path.Combine(d, "skills");
            if (Directory.Exists(cxs) && !_skillsDirs.Contains(cxs)) _skillsDirs.Add(cxs);

            // Collect all relevant JSON files
            foreach (var f in Directory.GetFiles(d, "*.json"))
            {
                var fn = Path.GetFileName(f).ToLower();
                if (fn.Contains("mcp") || fn.Contains("config") || fn.Contains("setting"))
                    _mcpJsonFiles.Add(f);
            }
        }

        // CRITICAL: Also scan current directory and parent directories for .claude/mcp.json
        try
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir != null)
            {
                var mcpFile = Path.Combine(dir.FullName, ".claude", "mcp.json");
                if (File.Exists(mcpFile) && !_mcpJsonFiles.Contains(mcpFile))
                    _mcpJsonFiles.Add(mcpFile);
                var mcpFile2 = Path.Combine(dir.FullName, "mcp.json");
                if (File.Exists(mcpFile2) && !_mcpJsonFiles.Contains(mcpFile2))
                    _mcpJsonFiles.Add(mcpFile2);
                dir = dir.Parent;
            }
        }
        catch { }

        // Also find .mcp.json / mcp.json in project roots
        try
        {
            foreach (var d in Directory.GetDirectories(h))
            {
                var mcpProj = Path.Combine(d, ".mcp.json");
                if (File.Exists(mcpProj)) _mcpJsonFiles.Add(mcpProj);
                mcpProj = Path.Combine(d, "mcp.json");
                if (File.Exists(mcpProj)) _mcpJsonFiles.Add(mcpProj);
            }
        }
        catch { }

        // Also find skills in standard locations: ~/.claude/skills, ~/.gemini/skills, ~/.opencode/skills
        foreach (var skDir in new[] {
            Path.Combine(h, ".claude", "skills"),
            Path.Combine(h, ".gemini", "skills"),
            Path.Combine(h, ".opencode", "skills"),
            Path.Combine(h, ".cursor", "skills"),
            Path.Combine(a, "Claude", "skills"),
        })
        {
            if (Directory.Exists(skDir) && !_skillsDirs.Contains(skDir))
                _skillsDirs.Add(skDir);
        }
    }

    private JsonElement? ReadJson(string p) { try { if (!File.Exists(p)) return null; var c = File.ReadAllText(p); if (string.IsNullOrWhiteSpace(c)) return null; return JsonSerializer.Deserialize<JsonElement>(c); } catch { return null; } }

    private string JStr(JsonElement e, string k) => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";

    // ──────────────────────────────────────────────
    // Phase 2: Scan MCP servers (running processes + all config files)
    // ──────────────────────────────────────────────
    private List<Item> GetMcp()
    {
        var r = new List<Item>();
        var seen = new HashSet<string>();

        // === Source A: Running processes (node, python, go binaries) ===
        var cl = new Dictionary<int, string>();
        try { using var s = new System.Management.ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process"); foreach (var o in s.Get()) cl[Convert.ToInt32(o["ProcessId"])] = o["CommandLine"]?.ToString() ?? ""; } catch { }

        foreach (var p in Process.GetProcesses())
        {
            try
            {
                // Check node, python, AND go binaries for MCP servers
                if (p.ProcessName != "node" && p.ProcessName != "python" && p.ProcessName != "go" && p.ProcessName != "github-mcp-server")
                    continue;
                if (!cl.TryGetValue(p.Id, out var cmd) || string.IsNullOrEmpty(cmd)) continue;
                var raw = ExtractName(cmd, p.ProcessName);
                if (raw == null || !seen.Add(raw)) continue;
                r.Add(new Item { Name = NiceName.GetValueOrDefault(raw, raw), Raw = raw, Desc = McpD.GetValueOrDefault(raw, ""), Icon = "⚡", IsMcp = true });
            }
            catch { }
        }

        // === Source B: All discovered MCP JSON/config files ===
        foreach (var jsonPath in _mcpJsonFiles)
        {
            try
            {
                var root = ReadJson(jsonPath);
                if (root == null || root.Value.ValueKind != JsonValueKind.Object) continue;

                // Try standard mcpServers key
                System.Text.Json.JsonElement servers = default;
                if (root.Value.TryGetProperty("mcpServers", out var ms1)) servers = ms1;

                // Try Codex-style [mcp_servers.*] in TOML — actually this is JSON from .codex/settings.json
                if (servers.ValueKind != JsonValueKind.Object && root.Value.TryGetProperty("mcp_servers", out var ms2)) servers = ms2;

                // Try Copilot mcp-config format: { "servers": { ... } }
                if (servers.ValueKind != JsonValueKind.Object && root.Value.TryGetProperty("servers", out var ms3)) servers = ms3;

                // Try Gemini settings format: { "mcpServers": { ... } }
                if (servers.ValueKind != JsonValueKind.Object && root.Value.TryGetProperty("url", out var _))
                {
                    // Could be a Gemini/OpenCode remote format — extract what we can
                }

                if (servers.ValueKind != JsonValueKind.Object) continue;

                foreach (var entry in servers.EnumerateObject())
                {
                    if (!seen.Add(entry.Name)) continue;
                    var desc = McpD.GetValueOrDefault(entry.Name, "");

                    // Try to extract command from the server entry for better description
                    if (string.IsNullOrEmpty(desc) && entry.Value.ValueKind == JsonValueKind.Object)
                    {
                        if (entry.Value.TryGetProperty("command", out var cmdEl) && cmdEl.ValueKind == JsonValueKind.String)
                        {
                            var cmdStr = cmdEl.GetString() ?? "";
                            if (cmdStr.Contains("github"))
                                desc = McpD.GetValueOrDefault("@github/mcp", "GitHub integration MCP server");
                            else if (cmdStr.Contains("postgres") || cmdStr.Contains("database"))
                                desc = "数据库 MCP 服务器：直接查询和操作数据库。";
                            else if (cmdStr.Contains("filesystem"))
                                desc = "文件系统 MCP 服务器：读写和搜索本地文件。";
                            else if (cmdStr.Contains("brave"))
                                desc = "Brave 搜索 MCP 服务器：网络搜索和本地搜索。";
                            else if (cmdStr.Contains("playwright"))
                                desc = McpD.GetValueOrDefault("@playwright/mcp", "浏览器自动化 MCP 服务器。");
                        }
                    }

                    r.Add(new Item { Name = NiceName.GetValueOrDefault(entry.Name, entry.Name), Raw = entry.Name, Desc = desc, Icon = "⚡", IsMcp = true });
                }
            }
            catch { }
        }

        return r;
    }

    // ──────────────────────────────────────────────
    // Phase 3: Scan skills from all discovered skill directories
    // ──────────────────────────────────────────────
    private List<Item> GetSkills()
    {
        var r = new List<Item>();
        var seen = new HashSet<string>();

        foreach (var d in _skillsDirs)
        {
            if (!Directory.Exists(d)) continue;
            foreach (var s in Directory.GetDirectories(d))
            {
                var n = Path.GetFileName(s);
                if (!seen.Add(n)) continue;
                var desc = SkillD.GetValueOrDefault(n, "");
                if (string.IsNullOrEmpty(desc))
                {
                    var m = ReadJson(Path.Combine(s, "_meta.json"));
                    if (m != null) desc = JStr(m.Value, "description");
                    if (string.IsNullOrEmpty(desc))
                    {
                        var sk = ReadJson(Path.Combine(s, "SKILL.md"));
                        // SKILL.md is markdown, not JSON — just mark it as found
                        if (sk == null && File.Exists(Path.Combine(s, "SKILL.md")))
                            desc = "已安装的 Agent Skill（SKILL.md）。";
                    }
                }
                r.Add(new Item { Name = n, Icon = SkillI.GetValueOrDefault(n, "🧩"), Desc = desc });
            }
        }
        return r;
    }

    private static string? ExtractName(string cmd, string procName)
    {
        if (procName == "github-mcp-server") return "github-mcp-server";

        // npx with quoted args: npx ... "@playwright/mcp"
        var m = System.Text.RegularExpressions.Regex.Match(cmd, @"npx[^""]*""[^""]*""\s+""([^""]+)""");
        if (m.Success)
        {
            var p = m.Groups[1].Value;
            var a = p.IndexOf("@", StringComparison.Ordinal);
            if (a > 0 && p[a - 1] != '/') p = p[..a];
            return p;
        }

        // npx with unquoted args
        var m2 = System.Text.RegularExpressions.Regex.Match(cmd, @"npx\\.+\\\.\.\\(@?[^\\]+)");
        if (m2.Success) return m2.Groups[1].Value;

        // Direct binary/command patterns
        if (cmd.Contains("scrapling")) return "scrapling";
        if (cmd.Contains("semble")) return "semble";
        if (cmd.Contains("chrome-devtools")) return "chrome-devtools-mcp";
        if (cmd.Contains("stock-images")) return "stock-images-mcp";
        if (cmd.Contains("github-mcp-server")) return "github-mcp-server";
        if (cmd.Contains("context7")) return "@upstash/context7-mcp";

        return null;
    }

    private void LoadData()
    {
        try
        {
            var m = GetMcp(); var sk = GetSkills();
            StatMCP.Text = m.Count.ToString(); StatSkills.Text = sk.Count.ToString();
            MCPTabBadge.Text = m.Count.ToString(); SkillsTabBadge.Text = sk.Count.ToString();
            MCPList.Children.Clear(); if (m.Count == 0) MCPList.Children.Add(EmptyCard("No MCP servers found")); else foreach (var x in m) MCPList.Children.Add(MakeCard(x));
            SkillsList.Children.Clear(); if (sk.Count == 0) SkillsList.Children.Add(EmptyCard("No skills found")); else foreach (var x in sk) SkillsList.Children.Add(MakeCard(x));
            LastUpdateText.Text = $"Updated {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex) { MCPList.Children.Clear(); MCPList.Children.Add(EmptyCard(ex.Message)); }
    }

    private Border MakeCard(Item item)
    {
        var isM = item.IsMcp;
        var card = new Border { Style = FindResource("CardStyle") as Style, Cursor = Cursors.Hand, RenderTransformOrigin = new Point(0.5, 0.5) };
        var outer = new StackPanel();
        var row = new StackPanel { Orientation = Orientation.Horizontal };

        // Icon
        var iconChar = isM ? (string.IsNullOrEmpty(item.Icon) ? "⚡" : item.Icon) : (string.IsNullOrEmpty(item.Icon) ? "🧩" : item.Icon);
        var ic = new Border { CornerRadius = new CornerRadius(8), Width = 36, Height = 36, Background = new SolidColorBrush(isM ? Color.FromArgb(30, 91, 123, 94) : Color.FromArgb(30, 122, 154, 125)), VerticalAlignment = VerticalAlignment.Center };
        ic.Child = new TextBlock { Text = iconChar, FontSize = 16, Foreground = new SolidColorBrush(isM ? GreenC : GreenLight), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        row.Children.Add(ic);

        // Body
        var body = new StackPanel { Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        var titleRow = new StackPanel { Orientation = Orientation.Horizontal };

        var copyName = item.Raw ?? item.Name;
        titleRow.Children.Add(new TextBlock { Text = item.Name, FontSize = 14, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(TextPri), VerticalAlignment = VerticalAlignment.Center });

        // Copy button
        var cp = new Border { CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Color.FromArgb(25, 201, 101, 59)), Width = 22, Height = 22, Margin = new Thickness(8, 0, 0, 0), Cursor = Cursors.Hand, VerticalAlignment = VerticalAlignment.Center };
        var cpT = new TextBlock { Text = "📋", FontSize = 10, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        cp.Child = cpT;
        cp.MouseLeftButtonDown += (_, a) => { try { Clipboard.SetText(copyName); cp.Background = new SolidColorBrush(GreenC); cpT.Text = "✓"; a.Handled = true; } catch { } };
        titleRow.Children.Add(cp);

        // Tag
        var tg = new Border { CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 2, 8, 2), Margin = new Thickness(8, 0, 0, 0), Background = new SolidColorBrush(isM ? Color.FromArgb(25, 91, 123, 94) : Color.FromArgb(25, 122, 154, 125)), VerticalAlignment = VerticalAlignment.Center };
        tg.Child = new TextBlock { Text = isM ? "active" : "installed", FontSize = 10, FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(isM ? GreenDark : GreenLight), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        titleRow.Children.Add(tg);
        body.Children.Add(titleRow);

        body.Children.Add(new TextBlock { Text = string.IsNullOrEmpty(item.Desc) ? "Running" : item.Desc, FontSize = 11, Foreground = new SolidColorBrush(TextSec), Margin = new Thickness(0, 3, 0, 0), TextTrimming = TextTrimming.CharacterEllipsis });
        row.Children.Add(body);

        var arrow = new TextBlock { Text = "▾", FontSize = 12, Foreground = new SolidColorBrush(TextDim), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
        row.Children.Add(arrow);
        outer.Children.Add(row);

        // Detail
        var detail = new Border { Height = 0, Opacity = 0, Margin = new Thickness(48, 8, 0, 2) };
        var dc = new StackPanel();
        if (!string.IsNullOrEmpty(item.Desc))
        {
            dc.Children.Add(new TextBlock { Text = "● 用途", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(GreenC), Margin = new Thickness(0, 0, 0, 4) });
            dc.Children.Add(new TextBlock { Text = item.Desc, FontSize = 12, Foreground = new SolidColorBrush(TextPri), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) });
        }
        if (!string.IsNullOrEmpty(item.Raw))
        {
            dc.Children.Add(new TextBlock { Text = "● 包名", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(GreenLight), Margin = new Thickness(0, 0, 0, 4) });
            dc.Children.Add(new TextBlock { Text = item.Raw, FontSize = 10, FontFamily = new FontFamily("Consolas"), Foreground = new SolidColorBrush(TextSec), TextWrapping = TextWrapping.Wrap, Background = new SolidColorBrush(Color.FromArgb(12, 0, 0, 0)), Padding = new Thickness(12, 10, 12, 10) });
        }
        detail.Child = dc;
        outer.Children.Add(detail);
        card.Child = outer;

        card.MouseEnter += (_, _) => { card.Background = new SolidColorBrush(Color.FromRgb(245, 243, 238)); card.BorderBrush = new SolidColorBrush(GreenLight); };
        card.MouseLeave += (_, _) => { card.Background = new SolidColorBrush(CardBg); card.BorderBrush = new SolidColorBrush(CardBorder); };
        card.MouseLeftButtonDown += (_, _) =>
        {
            var o = detail.Height != 0; detail.Height = o ? 0 : double.NaN; detail.Opacity = o ? 0 : 1; arrow.Text = o ? "▾" : "▴";
        };
        return card;
    }

    private Border EmptyCard(string text)
    {
        var c = new Border { Style = FindResource("CardStyle") as Style, Padding = new Thickness(30) };
        c.Child = new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(TextDim), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        return c;
    }
}

public class Item { public string Name { get; set; } = ""; public string Desc { get; set; } = ""; public string Icon { get; set; } = ""; public string Raw { get; set; } = ""; public bool IsMcp { get; set; }
}
