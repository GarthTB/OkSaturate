namespace OkSaturate.Strategies;

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Webp;

/// <summary> 图像文件保存方法 </summary>
internal static class Saver
{
    /// <summary> 可用的保存格式名称 </summary>
    public static string[] Formats => [
        "BMP 8位RGB",
        "BMP 8位RGBA",
        "JPG 80质量",
        "JPG 90质量",
        "JPG 99质量",
        "JPG 100质量",
        "PNG 8位RGB",
        "PNG 8位RGBA",
        "PNG 16位RGB",
        "PNG 16位RGBA",
        "TIF 8位RGB",
        "TIF 8位RGBA",
        "WebP 无损最小"

        // "TIF 16位RGB",
        // "TIF 16位RGBA",
    ];

    /// <summary> 根据保存格式名称获取图像文件保存方法 </summary>
    /// <param name="format"> 保存格式名称 </param>
    /// <returns> 图像文件保存方法：输入图像、原始路径，自动存至唯一路径 </returns>
    public static Action<Image, string> Get(string format) =>
        format switch {
            "BMP 8位RGB" =>
                static (image, inPath) => image.SaveAsBmp(
                    GetOutPath(inPath, "bmp"),
                    new() { BitsPerPixel = BmpBitsPerPixel.Pixel24, SupportTransparency = false }),
            "BMP 8位RGBA" =>
                static (image, inPath) => image.SaveAsBmp(
                    GetOutPath(inPath, "bmp"),
                    new() { BitsPerPixel = BmpBitsPerPixel.Pixel32, SupportTransparency = true }),
            "JPG 80质量" =>
                static (image, inPath) => image.SaveAsJpeg(
                    GetOutPath(inPath, "jpg"),
                    new() { Quality = 80, ColorType = JpegEncodingColor.YCbCrRatio411 }),
            "JPG 90质量" =>
                static (image, inPath) => image.SaveAsJpeg(
                    GetOutPath(inPath, "jpg"),
                    new() { Quality = 90, ColorType = JpegEncodingColor.YCbCrRatio411 }),
            "JPG 99质量" =>
                static (image, inPath) => image.SaveAsJpeg(
                    GetOutPath(inPath, "jpg"),
                    new() { Quality = 99, ColorType = JpegEncodingColor.YCbCrRatio411 }),
            "JPG 100质量" =>
                static (image, inPath) => image.SaveAsJpeg(
                    GetOutPath(inPath, "jpg"),
                    new() { Quality = 100, ColorType = JpegEncodingColor.YCbCrRatio411 }),
            "PNG 8位RGB" =>
                static (image, inPath) => image.SaveAsPng(
                    GetOutPath(inPath, "png"),
                    new() {
                        BitDepth = PngBitDepth.Bit8,
                        ColorType = PngColorType.Rgb,
                        CompressionLevel = PngCompressionLevel.BestCompression
                    }),
            "PNG 8位RGBA" =>
                static (image, inPath) => image.SaveAsPng(
                    GetOutPath(inPath, "png"),
                    new() {
                        BitDepth = PngBitDepth.Bit8,
                        ColorType = PngColorType.RgbWithAlpha,
                        CompressionLevel = PngCompressionLevel.BestCompression
                    }),
            "PNG 16位RGB" =>
                static (image, inPath) => image.SaveAsPng(
                    GetOutPath(inPath, "png"),
                    new() {
                        BitDepth = PngBitDepth.Bit16,
                        ColorType = PngColorType.Rgb,
                        CompressionLevel = PngCompressionLevel.BestCompression
                    }),
            "PNG 16位RGBA" =>
                static (image, inPath) => image.SaveAsPng(
                    GetOutPath(inPath, "png"),
                    new() {
                        BitDepth = PngBitDepth.Bit16,
                        ColorType = PngColorType.RgbWithAlpha,
                        CompressionLevel = PngCompressionLevel.BestCompression
                    }),
            "TIF 8位RGB" => static (image, inPath) => image.SaveAsTiff(
                GetOutPath(inPath, "tif"),
                new() {
                    BitsPerPixel = TiffBitsPerPixel.Bit24,
                    Compression = TiffCompression.Deflate,
                    CompressionLevel = DeflateCompressionLevel.BestCompression
                }),
            "TIF 8位RGBA" => static (image, inPath) => image.SaveAsTiff(
                GetOutPath(inPath, "tif"),
                new() {
                    BitsPerPixel = TiffBitsPerPixel.Bit32,
                    Compression = TiffCompression.Deflate,
                    CompressionLevel = DeflateCompressionLevel.BestCompression
                }),
            "WebP 无损最小" => static (image, inPath) => image.SaveAsWebp(
                GetOutPath(inPath, "webp"),
                new() { Quality = 100, FileFormat = WebpFileFormatType.Lossless }),
            _ => throw new ArgumentException($"保存格式'{format}'无效", nameof(format))

            // "TIF 16位RGB" => static (image, inPath) => image.SaveAsTiff(
            //     GetOutPath(inPath, "tif"),
            //     new() {
            //         BitsPerPixel = TiffBitsPerPixel.Bit48, 未支持
            //         Compression = TiffCompression.Deflate,
            //         CompressionLevel = DeflateCompressionLevel.BestCompression
            //     }),
            // "TIF 16位RGBA" => static (image, inPath) => image.SaveAsTiff(
            //     GetOutPath(inPath, "tif"),
            //     new() {
            //         BitsPerPixel = TiffBitsPerPixel.Bit64, 未支持
            //         Compression = TiffCompression.Deflate,
            //         CompressionLevel = DeflateCompressionLevel.BestCompression
            //     }),
        };

    /// <summary> 获取输出路径 </summary>
    /// <param name="inPath"> 原始图像文件路径 </param>
    /// <param name="ext"> 输出文件的扩展名 </param>
    /// <returns> 输出图像文件路径 </returns>
    private static string GetOutPath(string inPath, string ext) {
        var dir = Path.GetDirectoryName(inPath);
        if (!Path.Exists(dir))
            throw new ArgumentException("无法获取输入路径的目录", nameof(inPath));
        var name = Path.GetFileNameWithoutExtension(inPath);
        var outPath = Path.Combine(dir, $"{name}_OkSat.{ext}");
        for (var i = 2; File.Exists(outPath); i++)
            outPath = Path.Combine(dir, $"{name}_OkSat_{i}.{ext}");
        return outPath;
    }
}
