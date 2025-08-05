using Wacton.Unicolour;
using IndefSaturator = System.Func<System.Func<double, double>, System.Func<Wacton.Unicolour.Unicolour, Wacton.Unicolour.Unicolour>>;

namespace OkSaturate.Services;

/// <summary> 饱和度调整器工厂（未明确调整算法） </summary>
internal static class IndefSaturatorFactory
{
    /// <summary> 所有可用色彩空间的名称 </summary>
    public static string[] ColourSpaceNames => [.. _saturators.Keys];

    /// <returns> 指定色彩空间的饱和度调整器 </returns>
    public static IndefSaturator GetSaturator(string colourSpaceName)
        => _saturators[colourSpaceName];

    /// <summary> 所有可用色彩空间的饱和度调整器 </summary>
    private static readonly Dictionary<string, IndefSaturator> _saturators = new()
    {
        ["Munsell HVC"] = (func) => (colour) =>
        {
            var (h, v, c) = colour.Munsell.Tuple;
            return new(ColourSpace.Munsell, h, v, func(c));
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
            return new(ColourSpace.Lchab, l, func(c), h);
        },
        ["LCH (CIELUV)"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Lchuv.Tuple;
            return new(ColourSpace.Lchuv, l, func(c), h);
        },
        ["HSL (CIELUV)"] = (func) => (colour) =>
        {
            var (h, s, l) = colour.Hsluv.Tuple;
            return new(ColourSpace.Hsluv, h, func(s), l);
        },
        ["HPL (CIELUV)"] = (func) => (colour) =>
        {
            var (h, p, l) = colour.Hpluv.Tuple;
            return new(ColourSpace.Hpluv, h, func(p), l);
        },
        ["JCH (Jzazbz)"] = (func) => (colour) =>
        {
            var (j, c, h) = colour.Jzazbz.Tuple;
            return new(ColourSpace.Jzazbz, j, func(c), h);
        },
        ["Oklch"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Oklch.Tuple;
            return new(ColourSpace.Oklch, l, func(c), h);
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
            return new(ColourSpace.Hct, h, func(c), t);
        }
    };
}
