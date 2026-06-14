namespace HermesMonitor.Models;

public class Item
{
    public string Name { get; set; } = "";
    public string Desc { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Raw { get; set; } = "";
    public bool IsMcp { get; set; }

    public string CopyText => Raw ?? Name;
    public string TagLabel => IsMcp ? "active" : "installed";
}
