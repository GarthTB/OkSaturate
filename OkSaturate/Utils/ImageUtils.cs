using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Windows.Media.Imaging;

namespace OkSaturate.Utils;

/// <summary> 处理图像的工具类 </summary>
internal static class ImageUtils
{
    /// <summary> 将图像缩放为最大1m像素的缩略图 </summary>
    public static void ToThumbnail(Image image)
    {
        var (w, h) = image.Size;
        var scale = Math.Sqrt(1 << 20 / (w * h));
        w = (int)Math.Round(w * scale);
        h = (int)Math.Round(h * scale);

        if (scale < 1)
            image.Mutate(context => context.Resize(new ResizeOptions()
            { Mode = ResizeMode.Max, Size = new(w, h) }));
    }

    /// <returns> 8位带Alpha通道的 BitmapImage </returns>
    public static async Task<BitmapSource> ToBitmapSourceAsync(
        this Image image, CancellationToken token)
    {
        PngEncoder encoder = new()
        {
            BitDepth = PngBitDepth.Bit8,
            ColorType = PngColorType.RgbWithAlpha,
            CompressionLevel = PngCompressionLevel.BestSpeed
        };
        using MemoryStream stream = new();
        await image.SaveAsPngAsync(stream, encoder, token);
        stream.Position = 0;

        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        bitmap.Freeze();

        return bitmap;
    }
}
