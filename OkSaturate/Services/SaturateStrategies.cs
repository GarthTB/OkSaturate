using Wacton.Unicolour;
using Strategy = System.Func<System.Func<double, double>, System.Func<Wacton.Unicolour.Unicolour, Wacton.Unicolour.Unicolour>>;

namespace OkSaturate.Services;

/// <summary> 饱和度调整策略 </summary>
internal static class SaturateStrategies
{
    /// <summary> 所有可用色彩空间的名称 </summary>
    public static string[] ColourSpaceNames => [.. _strategies.Keys];

    /// <returns> 某个调整策略的实现 </returns>
    public static Strategy GetStrategy(string colourSpaceName)
        => _strategies[colourSpaceName];

    /// <summary> 各调整策略的实现 </summary>
    private static readonly Dictionary<string, Strategy> _strategies = new()
    {
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
            // 实测8位RGB最大值为133.80761432012983
            return new(ColourSpace.Lchab, l, func(c / 134.2) * 134.2, h);
        },
        ["LCH (CIELUV)"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Lchuv.Tuple;
            // 实测8位RGB最大值为179.04142708939614
            return new(ColourSpace.Lchuv, l, func(c / 179.6) * 179.6, h);
        },
        ["HSL (CIELUV)"] = (func) => (colour) =>
        {
            var (h, s, l) = colour.Hsluv.Tuple;
            return new(ColourSpace.Hsluv, h, func(s), l);
        },
        ["JCH (Jzazbz)"] = (func) => (colour) =>
        {
            var (j, c, h) = colour.Jzczhz.Tuple;
            // 实测8位RGB最大值为0.19027906590136512
            return new(ColourSpace.Jzczhz, j, func(c / 0.1908) * 0.1908, h);
        },
        ["Oklch"] = (func) => (colour) =>
        {
            var (l, c, h) = colour.Oklch.Tuple;
            // 实测8位RGB最大值为0.32249096477516476
            return new(ColourSpace.Oklch, l, func(c / 0.3235) * 0.3235, h);
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
            // 实测8位RGB最大值为113.35620829574427
            return new(ColourSpace.Hct, h, func(c / 113.7) * 113.7, t);
        }
    };
}
