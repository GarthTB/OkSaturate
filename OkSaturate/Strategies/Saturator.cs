namespace OkSaturate.Strategies;

using Wacton.Unicolour;

/// <summary> 饱和度调整方法 </summary>
internal static class Saturator
{
    /// <summary> 可用的色彩空间名称 </summary>
    public static string[] ColorSpaces => [
        "HSB / HSV",
        "HSL",
        "HSI",
        "LCHab",
        "LCHuv",
        "HSLuv",
        "HPLuv",
        "TSL",
        "JzCzHz",
        "Oklch",
        "Okhsv",
        "Okhsl",
        "Oklrch",
        "HCT"
    ];

    /// <summary> 根据色彩空间名称获取饱和度调整方法 </summary>
    /// <param name="colorSpace"> 色彩空间名称 </param>
    /// <returns> 饱和度调整方法，输入rgb值[0,1]和增益，得到新的rgb值[0,1] </returns>
    public static Func<(double, double, double), double, (double, double, double)> Get(
        string colorSpace) =>
        colorSpace switch {
            "HSB / HSV" => static (rgb, gain) => {
                var (h, s, b) = new Unicolour(ColourSpace.Rgb, rgb).Hsb.Tuple;
                return new Unicolour(ColourSpace.Hsb, h, Gain(s, gain), b).Rgb.Tuple;
            },
            "HSL" => static (rgb, gain) => {
                var (h, s, l) = new Unicolour(ColourSpace.Rgb, rgb).Hsl.Tuple;
                return new Unicolour(ColourSpace.Hsl, h, Gain(s, gain), l).Rgb.Tuple;
            },
            "HSI" => static (rgb, gain) => {
                var (h, s, i) = new Unicolour(ColourSpace.Rgb, rgb).Hsi.Tuple;
                return new Unicolour(ColourSpace.Hsi, h, Gain(s, gain), i).Rgb.Tuple;
            },
            "LCHab" => static (rgb, gain) => {
                var (l, c, h) = new Unicolour(ColourSpace.Rgb, rgb).Lchab.Tuple;
                c = Gain(c / 133.80761432012983, gain) * 133.80761432012983;
                return new Unicolour(ColourSpace.Lchab, l, c, h).Rgb.Tuple;
            },
            "LCHuv" => static (rgb, gain) => {
                var (l, c, h) = new Unicolour(ColourSpace.Rgb, rgb).Lchuv.Tuple;
                c = Gain(c / 179.04142708939614, gain) * 179.04142708939614;
                return new Unicolour(ColourSpace.Lchuv, l, c, h).Rgb.Tuple;
            },
            "HSLuv" => static (rgb, gain) => {
                var (h, s, l) = new Unicolour(ColourSpace.Rgb, rgb).Hsluv.Tuple;
                s = Gain(s / 100.05922027251566, gain) * 100.05922027251566;
                return new Unicolour(ColourSpace.Hsluv, h, s, l).Rgb.Tuple;
            },
            "HPLuv" => static (rgb, gain) => {
                var (h, s, l) = new Unicolour(ColourSpace.Rgb, rgb).Hpluv.Tuple;
                s = Gain(s / 1784.328864093446, gain) * 1784.328864093446;
                return new Unicolour(ColourSpace.Hpluv, h, s, l).Rgb.Tuple;
            },
            "TSL" => static (rgb, gain) => {
                var (t, s, l) = new Unicolour(ColourSpace.Rgb, rgb).Tsl.Tuple;
                return new Unicolour(ColourSpace.Tsl, t, Gain(s, gain), l).Rgb.Tuple;
            },
            "JzCzHz" => static (rgb, gain) => {
                var (j, c, h) = new Unicolour(ColourSpace.Rgb, rgb).Jzczhz.Tuple;
                c = Gain(c / 0.19027906590136512, gain) * 0.19027906590136512;
                return new Unicolour(ColourSpace.Jzczhz, j, c, h).Rgb.Tuple;
            },
            "Oklch" => static (rgb, gain) => {
                var (l, c, h) = new Unicolour(ColourSpace.Rgb, rgb).Oklch.Tuple;
                c = Gain(c / 0.32249096477516476, gain) * 0.32249096477516476;
                return new Unicolour(ColourSpace.Oklch, l, c, h).Rgb.Tuple;
            },
            "Okhsv" => static (rgb, gain) => {
                var (h, s, v) = new Unicolour(ColourSpace.Rgb, rgb).Okhsv.Tuple;
                s = Gain(s / 1.0119788693130793, gain) * 1.0119788693130793;
                return new Unicolour(ColourSpace.Okhsv, h, s, v).Rgb.Tuple;
            },
            "Okhsl" => static (rgb, gain) => {
                var (h, s, l) = new Unicolour(ColourSpace.Rgb, rgb).Okhsl.Tuple;
                s = Gain(s / 1.012342864781526, gain) * 1.012342864781526;
                return new Unicolour(ColourSpace.Okhsl, h, s, l).Rgb.Tuple;
            },
            "Oklrch" => static (rgb, gain) => {
                var (l, c, h) = new Unicolour(ColourSpace.Rgb, rgb).Oklrch.Tuple;
                c = Gain(c / 0.32249096477516476, gain) * 0.32249096477516476;
                return new Unicolour(ColourSpace.Oklrch, l, c, h).Rgb.Tuple;
            },
            "HCT" => static (rgb, gain) => {
                var (h, c, t) = new Unicolour(ColourSpace.Rgb, rgb).Hct.Tuple;
                c = Gain(c / 113.35620829574427, gain) * 113.35620829574427;
                return new Unicolour(ColourSpace.Hct, h, c, t).Rgb.Tuple;
            },
            _ => throw new ArgumentException($"色彩空间'{colorSpace}'无效", nameof(colorSpace))
        };

    /// <summary> 应用饱和度增益 </summary>
    /// <param name="sat"> 饱和度原始值[0,1] </param>
    /// <param name="gain"> 增益值[-1,1] </param>
    /// <returns> 增益后的饱和度值[0,1] </returns>
    private static double Gain(double sat, double gain) =>
        gain > 0
            ? Math.Pow(sat, 1 - gain)
            : sat * (1 + gain);
}
