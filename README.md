# 🎨 OkSaturate 饱和度调整工具 🖌

[![用前必读 README.md](https://img.shields.io/badge/用前必读-README.md-red)](https://github.com/GarthTB/OkSaturate/blob/master/README.md)
[![开发框架 .NET 10.0](https://img.shields.io/badge/开发框架-.NET%2010.0-blueviolet)](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)
[![最新版本 1.1.0](https://img.shields.io/badge/最新版本-1.1.0-brightgreen)](https://github.com/GarthTB/OkSaturate/releases/latest)
[![开源协议 MIT](https://img.shields.io/badge/开源协议-MIT-brown)](https://mit-license.org/)

## 📖 项目简介

**OkSaturate** 是一款 Windows 平台上的 GUI 应用程序，
用于在多种色彩空间（包括HCT、JzCzhz、OkLCh等）中调整图像的饱和度（色度、纯度、彩度），
并提供像素值蒙版来避免高饱和度区域溢出，
以求相较于传统 HSL/HSV 更自然、更安全、更符合感知的调整效果。

## ✨ 功能特点

- 🎨 **10+色彩空间**：多种先进色彩空间，随心切换
- 🛡 **蒙版保护**：像素值蒙版，避免溢出
- ⚡ **实时预览**：
    - 调整立即响应，所见即所得
    - 调整前/后来回切换，一目了然
- 🏭 **批量处理**：同一参数，多图共用

## 📥 安装与使用

### 系统要求

- 操作系统：Windows 10 或更高版本
- 运行依赖：[.NET 10.0 运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)

### 使用步骤

1. 下载 [最新版本压缩包](https://github.com/GarthTB/OkSaturate/releases/latest)
2. 解压后运行 `OkSaturate.exe`
3. 添加图像，调整参数、预览效果，执行处理

## 🛠 技术架构

- **语言**：C#
- **框架**：.NET 10.0 WPF
- **依赖**：
    - [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
    - [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)
    - [Wacton.Unicolour](https://github.com/waacton/Unicolour)

## 💡 补充说明

- 底层运算精度：16位
- 不保证严格的色彩科学性，请勿用于科学研究
- 已验证支持的输入图像：sRGB，8位/16位，RGB/RGBA，常见格式

## 📜 开源信息

- **作者**：GarthTB | 天卜 <g-art-h@outlook.com>
- **许可证**：[MIT 许可证](https://mit-license.org/)
    - 可以自由使用、修改和分发软件
    - 可以用于商业项目
    - 必须保留原始版权声明 `Copyright (c) 2025 GarthTB | 天卜`
- **项目地址**：https://github.com/GarthTB/OkSaturate

## 📝 更新日志

### v1.1.0 (20250813)

- 优化：低饱和度区域提升更柔和

### v1.0.0 (20250808)

- 首个发布！
