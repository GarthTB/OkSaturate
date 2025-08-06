using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Webp;
using System.IO;
using Strategy = System.Func<SixLabors.ImageSharp.Image, string, System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace OkSaturate.Services;

/// <summary> 保存图像策略 </summary>
internal static class SaveStrategies
{
    /// <summary> 所有可用保存格式的名称 </summary>
    public static string[] SaveFormatNames => [.. _strategies.Keys];

    /// <returns> 某个保存策略的实现 </returns>
    public static Strategy GetStrategy(string saveFormatName)
        => _strategies[saveFormatName];

    /// <summary> 各保存策略的实现 </summary>
    private static readonly Dictionary<string, Strategy> _strategies = new()
    {
        ["BMP 8位RGB"] = async (image, inputPath, token)
        => await image.SaveAsBmpAsync(
            GenOutputPath(inputPath, ".bmp"),
            new() { BitsPerPixel = BmpBitsPerPixel.Pixel24 },
            token),
        ["BMP 8位RGBA"] = async (image, inputPath, token)
        => await image.SaveAsBmpAsync(
            GenOutputPath(inputPath, ".bmp"),
            new() { BitsPerPixel = BmpBitsPerPixel.Pixel32, SupportTransparency = true },
            token),
        ["JPG 75质量"] = async (image, inputPath, token)
        => await image.SaveAsJpegAsync(
            GenOutputPath(inputPath, ".jpg"),
            new() { Quality = 75, ColorType = JpegEncodingColor.YCbCrRatio411 },
            token),
        ["JPG 90质量"] = async (image, inputPath, token)
        => await image.SaveAsJpegAsync(
            GenOutputPath(inputPath, ".jpg"),
            new() { Quality = 90, ColorType = JpegEncodingColor.YCbCrRatio411 },
            token),
        ["JPG 100质量"] = async (image, inputPath, token)
        => await image.SaveAsJpegAsync(
            GenOutputPath(inputPath, ".jpg"),
            new() { Quality = 100, ColorType = JpegEncodingColor.YCbCrRatio411 },
            token),
        ["PNG 8位RGB"] = async (image, inputPath, token)
        => await image.SaveAsPngAsync(
            GenOutputPath(inputPath, ".png"),
            new() { BitDepth = PngBitDepth.Bit8, ColorType = PngColorType.Rgb, CompressionLevel = PngCompressionLevel.BestCompression },
            token),
        ["PNG 8位RGBA"] = async (image, inputPath, token)
        => await image.SaveAsPngAsync(
            GenOutputPath(inputPath, ".png"),
            new() { BitDepth = PngBitDepth.Bit8, ColorType = PngColorType.RgbWithAlpha, CompressionLevel = PngCompressionLevel.BestCompression },
            token),
        ["PNG 16位RGB"] = async (image, inputPath, token)
        => await image.SaveAsPngAsync(
            GenOutputPath(inputPath, ".png"),
            new() { BitDepth = PngBitDepth.Bit16, ColorType = PngColorType.Rgb, CompressionLevel = PngCompressionLevel.BestCompression },
            token),
        ["PNG 16位RGBA"] = async (image, inputPath, token)
        => await image.SaveAsPngAsync(
            GenOutputPath(inputPath, ".png"),
            new() { BitDepth = PngBitDepth.Bit16, ColorType = PngColorType.RgbWithAlpha, CompressionLevel = PngCompressionLevel.BestCompression },
            token),
        ["TIF 8位RGB"] = async (image, inputPath, token)
        => await image.SaveAsTiffAsync(
            GenOutputPath(inputPath, ".tif"),
            new() { BitsPerPixel = TiffBitsPerPixel.Bit24, Compression = TiffCompression.Deflate, CompressionLevel = DeflateCompressionLevel.BestCompression },
            token),
        ["TIF 8位RGBA"] = async (image, inputPath, token)
        => await image.SaveAsTiffAsync(
            GenOutputPath(inputPath, ".tif"),
            new() { BitsPerPixel = TiffBitsPerPixel.Bit32, Compression = TiffCompression.Deflate, CompressionLevel = DeflateCompressionLevel.BestCompression },
            token),
        ["WebP 无损"] = async (image, inputPath, token)
        => await image.SaveAsWebpAsync(
            GenOutputPath(inputPath, ".webp"),
            new() { Quality = 100, FileFormat = WebpFileFormatType.Lossless },
            token)
    };

    /// <summary> 生成输出路径 </summary>
    private static string GenOutputPath(string inputPath, string ext)
    {
        var dir = Path.GetDirectoryName(inputPath);
        if (!Path.Exists(dir))
            throw new ArgumentException("无法获取输入路径的目录");
        var name = Path.GetFileNameWithoutExtension(inputPath);
        var outputPath = Path.Combine(dir, $"{name}_Saturated{ext}");
        for (var i = 2; File.Exists(outputPath); i++)
            outputPath = Path.Combine(dir, $"{name}_Saturated_{i}{ext}");
        return outputPath;
    }
}
