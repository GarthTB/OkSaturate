using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Wacton.Unicolour;
using Strategy = System.Func<System.Func<double, double>, System.Func<Wacton.Unicolour.Unicolour, Wacton.Unicolour.Unicolour>>;

namespace OkSaturate.Models;

/// <summary> 饱和度调整器 </summary>
/// <param name="SaturateStrategy"> 饱和度调整策略 </param>
/// <param name="SaturationGain"> 饱和度增益 </param>
/// <param name="UseMask"> 是否使用蒙版 </param>
internal record Saturator(
    Strategy SaturateStrategy, double SaturationGain, bool UseMask)
{
    /// <summary> 运行饱和度调整器（直接修改原图） </summary>
    public void Run(Image image, CancellationToken token)
    {
        if (SaturationGain == 0) return;
        token.ThrowIfCancellationRequested();

        if (!UseMask)
        {
            var saturator = SaturateStrategy(SaturationGain > 0
                ? (sat) => sat + SaturationGain * (1 - sat)
                : (sat) => sat + SaturationGain * sat);
            image.Mutate(context => context.ProcessPixelRowsAsVector4(span =>
            {
                token.ThrowIfCancellationRequested();
                foreach (ref var px in span)
                {
                    var (r, g, b) = (px.X, px.Y, px.Z);
                    Unicolour colour = new(ColourSpace.Rgb, r, g, b);
                    var (mr, mg, mb) = saturator(colour).Rgb.Tuple;
                    (px.X, px.Y, px.Z) = ((float)mr, (float)mg, (float)mb);
                }
            }));
        }
        else image.Mutate(context => context.ProcessPixelRowsAsVector4(span =>
        {
            token.ThrowIfCancellationRequested();
            foreach (ref var px in span)
            {
                var (r, g, b) = (px.X, px.Y, px.Z);
                Unicolour colour = new(ColourSpace.Rgb, r, g, b);
                var gain = GetMask(r, g, b) * SaturationGain;
                var saturator = SaturateStrategy(gain > 0
                    ? (sat) => sat + gain * (1 - sat)
                    : (sat) => sat + gain * sat);
                var (mr, mg, mb) = saturator(colour).Rgb.Tuple;
                (px.X, px.Y, px.Z) = ((float)mr, (float)mg, (float)mb);
            }
        }));
    }

    /// <summary> 计算蒙版（调整强度） </summary>
    private static double GetMask(float r, float g, float b)
    {
        var rDist = Math.Min(r, 1 - r);
        var gDist = Math.Min(g, 1 - g);
        var bDist = Math.Min(b, 1 - b);
        return 2 * Math.Min(rDist, Math.Min(gDist, bDist));
    }
}
