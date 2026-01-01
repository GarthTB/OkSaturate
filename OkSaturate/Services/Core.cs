namespace OkSaturate.Services;

using System.IO;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

/// <summary> 核心图像处理 </summary>
internal static class Core
{
    /// <summary> 计算当前像素的饱和度调整比例 </summary>
    private static double GetRatio(float r, float g, float b) =>
        2 * MathF.Min(MathF.Min(MathF.Min(MathF.Min(MathF.Min(r, g), b), 1 - r), 1 - g), 1 - b);

    /// <param name="image"> 待处理的图像 </param>
    extension(Image image)
    {
        /// <summary> 缩放为不超过65536像素的小图 </summary>
        public void ToThumb() {
            var (w, h) = image.Size;
            var scale = Math.Sqrt(65536.0 / (w * h));
            if (scale < 1)
                image.Mutate(context => context.Resize(
                    (int)Math.Round(w * scale),
                    (int)Math.Round(h * scale),
                    KnownResamplers.MitchellNetravali));
        }

        /// <summary> 异步转换为BitmapSource以显示在界面上 </summary>
        /// <param name="token"> 取消令牌 </param>
        /// <returns> BitmapSource图像：源为PNG 8位RGBA </returns>
        public async Task<BitmapSource> ToBitmapSourceAsync(CancellationToken token) {
            PngEncoder encoder = new() {
                BitDepth = PngBitDepth.Bit8,
                ColorType = PngColorType.RgbWithAlpha,
                CompressionLevel = PngCompressionLevel.NoCompression
            };
            using MemoryStream ms = new();
            await image.SaveAsPngAsync(ms, encoder, token);
            ms.Position = 0;

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        /// <summary> 调整饱和度 </summary>
        /// <param name="saturate"> 饱和度调整方法 </param>
        /// <param name="useMask"> 是否使用蒙版 </param>
        /// <param name="token"> 取消令牌 </param>
        public void Saturate(
            Func<(double, double, double), (double, double, double)> saturate,
            bool useMask,
            CancellationToken? token) {
            if (useMask)
                image.Mutate(context => context.ProcessPixelRowsAsVector4(span => {
                    foreach (ref var px in span) {
                        token?.ThrowIfCancellationRequested();
                        var (r, g, b) = saturate((px.X, px.Y, px.Z));
                        var ratio = GetRatio(px.X, px.Y, px.Z);
                        px.X += (float)(ratio * (r - px.X));
                        px.Y += (float)(ratio * (g - px.Y));
                        px.Z += (float)(ratio * (b - px.Z));
                    }
                }));
            else
                image.Mutate(context => context.ProcessPixelRowsAsVector4(span => {
                    foreach (ref var px in span) {
                        token?.ThrowIfCancellationRequested();
                        var (r, g, b) = saturate((px.X, px.Y, px.Z));
                        (px.X, px.Y, px.Z) = ((float)r, (float)g, (float)b);
                    }
                }));
        }
    }
}
