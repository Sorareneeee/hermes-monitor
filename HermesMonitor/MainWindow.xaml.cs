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
    private readonly List<string> _mcpJsonPaths = new();

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

    public MainWindow() { InitializeComponent(); DiscoverPaths(); LoadData(); var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(60) }; t.Tick += (_, _) => LoadData(); t.Start(); }

    private void Close_Click(object s, RoutedEventArgs e) => Close();
    private void Refresh_Click(object s, RoutedEventArgs e) => LoadData();
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

    private void DiscoverPaths()
    {
        var h = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var a = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var l = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var cs = new HashSet<string>();
        foreach (var d in new[] { Path.Combine(h, ".claude"), Path.Combine(h, ".codex"), Path.Combine(a, "Claude"), Path.Combine(l, "Claude"), Path.Combine(a, "Codex"), Path.Combine(l, "Codex"), Path.Combine(h, ".cursor"), Path.Combine(h, ".windsurf") }) { if (Directory.Exists(d) || File.Exists(d)) cs.Add(d); }
        foreach (var r in new[] { h, a, l }) { try { foreach (var d in Directory.GetDirectories(r)) { var cd = Path.Combine(d, ".claude"); if (Directory.Exists(cd)) cs.Add(cd); } } catch { } }
        foreach (var d in cs) { if (!Directory.Exists(d)) continue; var s = Path.Combine(d, "skills"); if (Directory.Exists(s)) _skillsDirs.Add(s); foreach (var f in Directory.GetFiles(d, "*.json")) { var fn = Path.GetFileName(f).ToLower(); if (fn.Contains("mcp") || fn.Contains("config") || fn.Contains("setting")) _mcpJsonPaths.Add(f); } }
    }

    private JsonElement? ReadJson(string p) { try { if (!File.Exists(p)) return null; var c = File.ReadAllText(p); if (string.IsNullOrWhiteSpace(c)) return null; return JsonSerializer.Deserialize<JsonElement>(c); } catch { return null; } }

    private string JStr(JsonElement e, string k) => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";

    private List<Item> GetMcp()
    {
        var r = new List<Item>(); var seen = new HashSet<string>(); var cl = new Dictionary<int, string>();
        try { using var s = new System.Management.ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process"); foreach (var o in s.Get()) cl[Convert.ToInt32(o["ProcessId"])] = o["CommandLine"]?.ToString() ?? ""; } catch { }
        foreach (var p in Process.GetProcesses()) { try { if (p.ProcessName != "node" && p.ProcessName != "python") continue; if (!cl.TryGetValue(p.Id, out var c) || string.IsNullOrEmpty(c)) continue; var raw = ExtractName(c); if (raw == null || !seen.Add(raw)) continue; r.Add(new Item { Name = NiceName.GetValueOrDefault(raw, raw), Raw = raw, Desc = McpD.GetValueOrDefault(raw, ""), Icon = "⚡", IsMcp = true }); } catch { } }
        return r;
    }

    private List<Item> GetSkills()
    {
        var r = new List<Item>();
        foreach (var d in _skillsDirs) { if (!Directory.Exists(d)) continue; foreach (var s in Directory.GetDirectories(d)) { var n = Path.GetFileName(s); var desc = SkillD.GetValueOrDefault(n, ""); if (string.IsNullOrEmpty(desc)) { var m = ReadJson(Path.Combine(s, "_meta.json")); if (m != null) desc = JStr(m.Value, "description"); } r.Add(new Item { Name = n, Icon = SkillI.GetValueOrDefault(n, "🧩"), Desc = desc }); } }
        return r;
    }

    private static string? ExtractName(string cmd)
    {
        var m = System.Text.RegularExpressions.Regex.Match(cmd, @"npx[^""]*""[^""]*""\s+""([^""]+)"""); if (m.Success) { var p = m.Groups[1].Value; var a = p.IndexOf("@", StringComparison.Ordinal); if (a > 0 && p[a - 1] != '/') p = p[..a]; return p; }
        var cm = System.Text.RegularExpressions.Regex.Match(cmd, @"npx\\.+\\\.\.\\(@?[^\\]+)"); if (cm.Success) return cm.Groups[1].Value; if (cmd.Contains("scrapling")) return "scrapling"; if (cmd.Contains("semble")) return "semble"; return null;
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

        // Copy button (焦橙色)
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

public class Item { public string Name { get; set; } = ""; public string Desc { get; set; } = ""; public string Icon { get; set; } = ""; public string Raw { get; set; } = ""; public bool IsMcp { get; set; } }
