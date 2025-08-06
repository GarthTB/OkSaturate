using Wacton.Unicolour;
using Strategy = System.Func<System.Func<double, double>, System.Func<Wacton.Unicolour.Unicolour, Wacton.Unicolour.Unicolour>>;

namespace OkSaturate.Services;

/// <summary> 饱和度调整策略 </summary>
internal static class SaturateStrategies
{
    /// <summary> 所有可用色彩空间的名称 </summary>
    public static string[] ColourSpaceNames => [.. _strategies.Keys];

    /// <returns> 某色彩空间的饱和度调整策略 </returns>
    public static Strategy GetStrategy(string colourSpaceName)
        => _strategies[colourSpaceName];

    /// <summary> 饱和度最大的颜色 </summary>
    private static readonly Unicolour _maxChroma = new(ColourSpace.Rgb, 1, 0, 1);

    /// <summary> 各色彩空间的饱和度调整策略 </summary>
    private static readonly Dictionary<string, Strategy> _strategies = new()
    {
        ["Munsell HVC"] = (func) => (colour) =>
        {
            var (h, v, c) = colour.Munsell.Tuple;
            var factor = _maxChroma.Munsell.C;
            return new(ColourSpace.Munsell, h, v, func(c / factor) * factor);
        },
        ["HSB / HSV"] = (func) => (colour) =>
        {
            var (h, s, b) = colour.Hsb.Tuple;
            return new(ColourSpace.Hsb, h, func(s), b);
        },
        ["HSL"] = (func) => (colour) =>
        {
            var (h, s, l) = colour.Hsl.Tuple;
            return new(ColourSpace.Hsl, h, func(s), l);
        },
        ["HSI"] = (func) => (colour) =>
        {
            var (h, s, i) = colour.Hsi.Tuple;
            return new(ColourSpace.Hsi, h, func(s), i);
        },
        ["TSL"] = (func) => (colour) =>
        {
            var (t, s, l) = colour.Tsl.Tuple;
            return new(ColourSpace.Tsl, t, func(s), l);
        },
        ["LCH (CIELAB)"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Lchab.Tuple;
            var factor = _maxChroma.Lchab.C;
            return new(ColourSpace.Lchab, l, func(c / factor) * factor, h);
        },
        ["LCH (CIELUV)"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Lchuv.Tuple;
            var factor = _maxChroma.Lchuv.C;
            return new(ColourSpace.Lchuv, l, func(c / factor) * factor, h);
        },
        ["HSL (CIELUV)"] = (func) => (colour) =>
        {
            var (h, s, l) = colour.Hsluv.Tuple;
            return new(ColourSpace.Hsluv, h, func(s), l);
        },
        ["HPL (CIELUV)"] = (func) => (colour) =>
        {
            var (h, p, l) = colour.Hpluv.Tuple;
            var factor = _maxChroma.Hpluv.S;
            return new(ColourSpace.Hpluv, h, func(p / factor) * factor, l);
        },
        ["JCH (Jzazbz)"] = (func) => (colour) =>
        {
            var (j, c, h) = colour.Jzczhz.Tuple;
            var factor = _maxChroma.Jzczhz.C;
            return new(ColourSpace.Jzczhz, j, func(c / factor) * factor, h);
        },
        ["Oklch"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Oklch.Tuple;
            var factor = _maxChroma.Oklch.C;
            return new(ColourSpace.Oklch, l, func(c / factor) * factor, h);
        },
        ["Okhsv"] = (func) => (colour) =>
        {
            var (h, s, v) = colour.Okhsv.Tuple;
            return new(ColourSpace.Okhsv, h, func(s), v);
        },
        ["Okhsl"] = (func) => (colour) =>
        {
            var (h, s, l) = colour.Okhsl.Tuple;
            return new(ColourSpace.Okhsl, h, func(s), l);
        },
        ["Oklrch"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Oklrch.Tuple;
            return new(ColourSpace.Oklrch, l, func(c), h);
        },
        ["HCT"] = (func) => (colour) =>
        {
            var (h, c, t) = colour.Hct.Tuple;
            var factor = _maxChroma.Hct.C;
            return new(ColourSpace.Hct, h, func(c / factor) * factor, t);
        }
    };
}
