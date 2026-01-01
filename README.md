# 🎨 Ok Saturate | 饱和度工具 🖌

`Ok Saturate` 是一款 Windows x64 单平台的 GUI 应用程序，
用于在多种色彩空间（包括Oklch、JzCzHz、HCT等）中调整图像的
饱和度（色度、纯度、彩度），并提供像素值蒙版来避免高饱和度区域溢出，
以求相较于传统 HSV/HSL 更均衡、更安全、更符合感知的调整效果。

[![框架 .NET 10.0](https://img.shields.io/badge/框架-.NET%20%2010.0-blueviolet)](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)
[![语言 C# 14.0](https://img.shields.io/badge/语言-C%23%2014.0-navy.svg)](https://github.com/dotnet/csharplang)
[![许可 MIT](https://img.shields.io/badge/许可-MIT-brown)](https://mit-license.org)
[![平台 Windows x64](https://img.shields.io/badge/平台-Windows%20x64-orange.svg)](https://github.com/GarthTB/OkSaturate)
[![版本 1.2.0](https://img.shields.io/badge/版本-1.2.0-brightgreen)](https://github.com/GarthTB/OkSaturate/releases/latest)

## ✨ 特点

- 🎨 **10+色彩空间**：丰富先进色彩空间，随心切换
- 🛡 **蒙版保护**：像素值蒙版，避免溢出
- ⚡ **实时预览**：
    - 调整立即响应，所见即所得
    - 调整前/后来回切换，一目了然
- 🏭 **批量处理**：同一参数，多图共用

## 📥 安装与使用

### 系统要求

- 操作系统：Windows 10 或更高版本
- 架构：x64
- 运行依赖：[.NET 10.0 运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)

### 使用步骤

1. 下载 [最新版本压缩包](https://github.com/GarthTB/OkSaturate/releases/latest)
2. 解压后运行 `OkSaturate.exe`
3. 添加图像，调整参数、预览效果，执行处理

## 🛠 技术栈

- **框架**：.NET 10.0 WPF
- **语言**：C# 14.0
- **依赖**：
    - [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
    - [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)
    - [Wacton.Unicolour](https://github.com/waacton/Unicolour)

## 📜 开源信息

- **作者**：GarthTB | 天卜 <g-art-h@outlook.com>
- **许可证**：[MIT 许可证](https://mit-license.org/)
- **项目地址**：https://github.com/GarthTB/OkSaturate

## 💡 补充说明

- 底层运算精度：32位
- 不保证严格的色彩科学性，请勿用于科学研究

## 📝 更新日志

### v1.2.0 (20260101)

- 优化蒙版算法，提升饱和度更均衡
- 优化饱和度提升参数，更稳健

### v1.1.0 (20250813)

- 优化：低饱和度区域提升更柔和

### v1.0.0 (20250808)

- 首个发布！
