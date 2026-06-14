using System.Windows.Media;

namespace HermesMonitor.Helpers;

public record AppColors
{
    public Color Bg { get; init; } = Color.FromRgb(245, 242, 235);
    public Color BgDark { get; init; } = Color.FromRgb(236, 232, 221);
    public Color CardBg { get; init; } = Color.FromRgb(250, 248, 244);
    public Color CardBorder { get; init; } = Color.FromRgb(230, 224, 212);
    public Color Green { get; init; } = Color.FromRgb(91, 123, 94);
    public Color GreenLight { get; init; } = Color.FromRgb(122, 154, 125);
    public Color GreenDark { get; init; } = Color.FromRgb(61, 91, 64);
    public Color GreenBg { get; init; } = Color.FromRgb(228, 237, 224);
    public Color GreenBgLight { get; init; } = Color.FromRgb(239, 245, 236);
    public Color Orange { get; init; } = Color.FromRgb(201, 101, 59);
    public Color OrangeLight { get; init; } = Color.FromRgb(224, 122, 78);
    public Color OrangeBg { get; init; } = Color.FromRgb(247, 237, 224);
    public Color TextPri { get; init; } = Color.FromRgb(44, 44, 42);
    public Color TextSec { get; init; } = Color.FromRgb(107, 106, 102);
    public Color TextDim { get; init; } = Color.FromRgb(156, 154, 148);

    public static AppColors Light => new();
    public static AppColors Dark => new()
    {
        Bg = Color.FromRgb(26, 26, 28), BgDark = Color.FromRgb(34, 34, 37),
        CardBg = Color.FromRgb(34, 34, 37), CardBorder = Color.FromRgb(50, 50, 55),
        Green = Color.FromRgb(100, 140, 110), GreenLight = Color.FromRgb(130, 170, 140),
        GreenDark = Color.FromRgb(70, 110, 80), GreenBg = Color.FromRgb(35, 50, 40),
        GreenBgLight = Color.FromRgb(40, 58, 48), Orange = Color.FromRgb(210, 120, 80),
        OrangeLight = Color.FromRgb(230, 140, 95), OrangeBg = Color.FromRgb(55, 40, 30),
        TextPri = Color.FromRgb(220, 220, 218), TextSec = Color.FromRgb(160, 159, 155),
        TextDim = Color.FromRgb(110, 109, 105),
    };
}
