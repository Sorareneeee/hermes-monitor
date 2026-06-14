namespace HermesMonitor.Services;

using System.Text.Json;

public class ThemeService
{
    private const string ConfigFile = "HermesMonitor.theme.json";
    public bool IsDarkMode { get; private set; }
    public ThemeService() { IsDarkMode = Load(); }
    public void Toggle() { IsDarkMode = !IsDarkMode; Save(); }
    private bool Load() { try { var p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile); if (!File.Exists(p)) return false; var d = JsonDocument.Parse(File.ReadAllText(p)); return d.RootElement.TryGetProperty("darkMode", out var v) && v.GetBoolean(); } catch { return false; } }
    private void Save() { try { File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile), JsonSerializer.Serialize(new { darkMode = IsDarkMode })); } catch { } }
}
