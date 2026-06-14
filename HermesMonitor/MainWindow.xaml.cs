using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HermesMonitor.Helpers;
using HermesMonitor.Models;
using HermesMonitor.Services;

namespace HermesMonitor;

public partial class MainWindow : Window
{
    private readonly PathDiscoveryService _discovery = new();
    private readonly McpScanner _mcpScanner = new();
    private readonly SkillScanner _skillScanner = new();
    private readonly ThemeService _themeService = new();

    private List<Item> _allMcp = new();
    private List<Item> _allSkills = new();
    private string _searchFilter = "";
    private int _activeTab;

    public MainWindow()
    {
        InitializeComponent();
        ApplyTheme();
        DiscoverAndLoad();
        var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
        t.Tick += (_, _) => { _discovery.Discover(); LoadData(); };
        t.Start();
    }

    private void Close_Click(object s, RoutedEventArgs e) => Close();
    private void Window_MouseLeftButtonDown(object s, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    private void BtnEnter(object s, MouseEventArgs e) { if (s is TextBlock tb) tb.Foreground = new SolidColorBrush(_themeService.IsDarkMode ? AppColors.Dark.Green : AppColors.Light.Green); }
    private void BtnLeave(object s, MouseEventArgs e) { if (s is TextBlock tb) tb.Foreground = new SolidColorBrush(_themeService.IsDarkMode ? AppColors.Dark.TextSec : AppColors.Light.TextSec); }
    private void TabMCP_Click(object s, MouseButtonEventArgs e) => SwitchTab(0);
    private void TabSkills_Click(object s, MouseButtonEventArgs e) => SwitchTab(1);
    private void Search_TextChanged(object s, TextChangedEventArgs e) { _searchFilter = SearchBox.Text?.Trim() ?? ""; RenderCurrentList(); }

    private void Refresh_Click(object s, RoutedEventArgs e) { _discovery.Discover(); LoadData(); }

    private void ThemeToggle_Click(object s, MouseButtonEventArgs e)
    {
        _themeService.Toggle();
        ApplyTheme();
        RenderCurrentList();
    }

    private void Export_Click(object s, RoutedEventArgs e)
    {
        try
        {
            var export = new { exportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), mcpServers = _allMcp.Select(m => new { name = m.Name, raw = m.Raw, description = m.Desc }), skills = _allSkills.Select(s => new { name = s.Name, description = s.Desc }), summary = new { totalMcp = _allMcp.Count, totalSkills = _allSkills.Count } };
            var save = new Microsoft.Win32.SaveFileDialog { FileName = $"hermes-snapshot-{DateTime.Now:yyyyMMdd-HHmmss}.json", Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*" };
            if (save.ShowDialog() == true) File.WriteAllText(save.FileName, JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex) { MessageBox.Show($"Export failed: {ex.Message}", "Hermes Monitor", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void ApplyTheme()
    {
        var c = _themeService.IsDarkMode ? AppColors.Dark : AppColors.Light;
        Background = new SolidColorBrush(c.Bg);
        foreach (var key in new[] { "Bg", "BgDark", "Green", "GreenLight", "GreenDark", "GreenBg", "GreenBgLight", "Orange", "OrangeLight", "OrangeBg", "TextPri", "TextSec", "TextDim", "CardBg", "CardBorder" })
            Resources[key] = c.GetType().GetProperty(key)?.GetValue(c) ?? Resources[key];
        foreach (var key in new[] { "BgBrush", "BgDarkBrush", "GreenBrush", "GreenLightBrush", "GreenDarkBrush", "GreenBgBrush", "GreenBgLightBrush", "OrangeBrush", "OrangeLightBrush", "OrangeBgBrush", "TextPriBrush", "TextSecBrush", "TextDimBrush", "CardBgBrush", "CardBorderBrush" })
            Resources[key] = new SolidColorBrush((Color)Resources[key[..^4]]);
        ThemeIcon.Text = _themeService.IsDarkMode ? "☀️" : "🌙";
        LiveDot.Fill = new SolidColorBrush(c.Green);
    }

    private void DiscoverAndLoad() { _discovery.Discover(); LoadData(); }

    private void LoadData()
    {
        try
        {
            _allMcp = _mcpScanner.Scan(_discovery.McpJsonFiles);
            _allSkills = _skillScanner.Scan(_discovery.SkillDirs);
            StatMCP.Text = _allMcp.Count.ToString();
            StatSkills.Text = _allSkills.Count.ToString();
            MCPTabBadge.Text = _allMcp.Count.ToString();
            SkillsTabBadge.Text = _allSkills.Count.ToString();
            RenderCurrentList();
            LastUpdateText.Text = $"Updated {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex) { MCPList.Children.Clear(); MCPList.Children.Add(CreateEmptyCard(ex.Message)); }
    }

    private void SwitchTab(int i)
    {
        _activeTab = i;
        var c = _themeService.IsDarkMode ? AppColors.Dark : AppColors.Light;
        ScrollerMCP.Visibility = i == 0 ? Visibility.Visible : Visibility.Collapsed;
        ScrollerSkills.Visibility = i == 1 ? Visibility.Visible : Visibility.Collapsed;
        MCPTabBg.Background = new SolidColorBrush(i == 0 ? c.Green : Colors.Transparent);
        MCPTabText.Foreground = new SolidColorBrush(i == 0 ? Colors.White : c.TextSec);
        SkillsTabBg.Background = new SolidColorBrush(i == 1 ? c.Green : Colors.Transparent);
        SkillsTabText.Foreground = new SolidColorBrush(i == 1 ? Colors.White : c.TextSec);
        RenderCurrentList();
    }

    private void RenderCurrentList()
    {
        var c = _themeService.IsDarkMode ? AppColors.Dark : AppColors.Light;
        var items = FilterItems(_activeTab == 0 ? _allMcp : _allSkills);
        var container = _activeTab == 0 ? MCPList : SkillsList;
        container.Children.Clear();
        if (items.Count == 0) { container.Children.Add(CreateEmptyCard(string.IsNullOrEmpty(_searchFilter) ? (_activeTab == 0 ? "No MCP servers found" : "No skills found") : "No results match your search")); return; }
        foreach (var item in items) container.Children.Add(CreateCard(item, c));
    }

    private List<Item> FilterItems(List<Item> items)
    {
        if (string.IsNullOrEmpty(_searchFilter)) return items;
        var f = _searchFilter.ToLower();
        return items.Where(i => i.Name.ToLower().Contains(f) || i.Desc.ToLower().Contains(f) || i.Raw.ToLower().Contains(f)).ToList();
    }

    private Border CreateCard(Item item, AppColors c)
    {
        var isM = item.IsMcp;
        var card = new Border { Style = FindResource("CardStyle") as Style, Cursor = Cursors.Hand, RenderTransformOrigin = new Point(0.5, 0.5) };
        var outer = new StackPanel();
        var row = new StackPanel { Orientation = Orientation.Horizontal };
        var ic = new Border { CornerRadius = new CornerRadius(8), Width = 36, Height = 36, Background = new SolidColorBrush(Color.FromArgb(30, isM ? c.Green.R : c.GreenLight.R, isM ? c.Green.G : c.GreenLight.G, isM ? c.Green.B : c.GreenLight.B)), VerticalAlignment = VerticalAlignment.Center };
        ic.Child = new TextBlock { Text = string.IsNullOrEmpty(item.Icon) ? (isM ? "⚡" : "🧩") : item.Icon, FontSize = 16, Foreground = new SolidColorBrush(isM ? c.Green : c.GreenLight), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        row.Children.Add(ic);
        var body = new StackPanel { Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        var titleRow = new StackPanel { Orientation = Orientation.Horizontal };
        titleRow.Children.Add(new TextBlock { Text = item.Name, FontSize = 14, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(c.TextPri), VerticalAlignment = VerticalAlignment.Center });
        var cp = new Border { CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Color.FromArgb(25, c.Orange.R, c.Orange.G, c.Orange.B)), Width = 22, Height = 22, Margin = new Thickness(8, 0, 0, 0), Cursor = Cursors.Hand, VerticalAlignment = VerticalAlignment.Center };
        var cpT = new TextBlock { Text = "📋", FontSize = 10, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; cp.Child = cpT;
        cp.MouseLeftButtonDown += (_, a) => { try { Clipboard.SetText(item.CopyText); cp.Background = new SolidColorBrush(c.Green); cpT.Text = "✓"; a.Handled = true; } catch { } };
        titleRow.Children.Add(cp);
        var tg = new Border { CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 2, 8, 2), Margin = new Thickness(8, 0, 0, 0), Background = new SolidColorBrush(Color.FromArgb(25, isM ? c.Green.R : c.GreenLight.R, isM ? c.Green.G : c.GreenLight.G, isM ? c.Green.B : c.GreenLight.B)), VerticalAlignment = VerticalAlignment.Center };
        tg.Child = new TextBlock { Text = item.TagLabel, FontSize = 10, FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(isM ? c.GreenDark : c.GreenLight), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        titleRow.Children.Add(tg);
        body.Children.Add(titleRow);
        body.Children.Add(new TextBlock { Text = string.IsNullOrEmpty(item.Desc) ? "Running" : item.Desc, FontSize = 11, Foreground = new SolidColorBrush(c.TextSec), Margin = new Thickness(0, 3, 0, 0), TextTrimming = TextTrimming.CharacterEllipsis });
        row.Children.Add(body);
        var arrow = new TextBlock { Text = "▾", FontSize = 12, Foreground = new SolidColorBrush(c.TextDim), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
        row.Children.Add(arrow);
        outer.Children.Add(row);
        var detail = new Border { Height = 0, Opacity = 0, Margin = new Thickness(48, 8, 0, 2) };
        var dc = new StackPanel();
        if (!string.IsNullOrEmpty(item.Desc)) { dc.Children.Add(new TextBlock { Text = "● Purpose / 用途", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(c.Green), Margin = new Thickness(0, 0, 0, 4) }); dc.Children.Add(new TextBlock { Text = item.Desc, FontSize = 12, Foreground = new SolidColorBrush(c.TextPri), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) }); }
        if (!string.IsNullOrEmpty(item.Raw)) { dc.Children.Add(new TextBlock { Text = "● Package / 包名", FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(c.GreenLight), Margin = new Thickness(0, 0, 0, 4) }); dc.Children.Add(new TextBlock { Text = item.Raw, FontSize = 10, FontFamily = new FontFamily("Consolas"), Foreground = new SolidColorBrush(c.TextSec), TextWrapping = TextWrapping.Wrap, Background = new SolidColorBrush(Color.FromArgb(12, c.TextPri.R, c.TextPri.G, c.TextPri.B)), Padding = new Thickness(12, 10, 12, 10) }); }
        detail.Child = dc; outer.Children.Add(detail); card.Child = outer;
        card.MouseEnter += (_, _) => { card.Background = new SolidColorBrush(Color.FromRgb(245, 243, 238)); card.BorderBrush = new SolidColorBrush(c.GreenLight); };
        card.MouseLeave += (_, _) => { card.Background = new SolidColorBrush(c.CardBg); card.BorderBrush = new SolidColorBrush(c.CardBorder); };
        card.MouseLeftButtonDown += (_, _) => { var o = detail.Height != 0; detail.Height = o ? 0 : double.NaN; detail.Opacity = o ? 0 : 1; arrow.Text = o ? "▾" : "▴"; };
        return card;
    }

    private Border CreateEmptyCard(string text)
    {
        var c = _themeService.IsDarkMode ? AppColors.Dark : AppColors.Light;
        return new Border { Style = FindResource("CardStyle") as Style, Padding = new Thickness(30), Child = new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(c.TextDim), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
    }
}
