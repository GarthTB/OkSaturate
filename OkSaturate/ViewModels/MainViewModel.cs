// ReSharper disable UnusedParameterInPartialMethod

namespace OkSaturate.ViewModels;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Strategies;

/// <summary> 主窗口的ViewModel </summary>
internal sealed partial class MainViewModel: ObservableObject
{
    #region 待处理图像

    /// <summary> 待处理图像的路径 </summary>
    public ObservableCollection<string> ImagePaths { get; } = [];

    /// <summary> 选中的待处理图像路径的索引 </summary>
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RemovePathCommand))]
    private int _selectedPathIndex = -1;

    partial void OnSelectedPathIndexChanged(int value) => _ = UpdatePreviewAsync(true);

    /// <returns> 是否选中了图像路径 </returns>
    private bool PathSelected => SelectedPathIndex >= 0 && SelectedPathIndex < ImagePaths.Count;

    /// <summary> 添加待处理图像路径 </summary>
    [RelayCommand]
    private void AddPath() =>
        Try.Do(
            "添加图像",
            () => {
                OpenFileDialog dialog = new() {
                    Title = "选取待处理的图像",
                    Filter = "图像文件 (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp)|"
                           + "*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp|"
                           + "所有文件 (*.*)|*.*",
                    Multiselect = true
                };
                if (dialog.ShowDialog() == true)
                    foreach (var path in dialog.FileNames.Where(path => !ImagePaths.Contains(path)))
                        ImagePaths.Add(path);
                ProcessCommand.NotifyCanExecuteChanged();
            });

    /// <summary> 移除选中的图像路径 </summary>
    [RelayCommand(CanExecute = nameof(PathSelected))]
    private void RemovePath() =>
        Try.Do(
            "移除图像",
            () => {
                ImagePaths.RemoveAt(SelectedPathIndex);
                SelectedPathIndex = -1; // 避免连续移除
                ProcessCommand.NotifyCanExecuteChanged();
            });

    #endregion

    #region 增益值

    /// <summary> 增益值 </summary>
    [ObservableProperty]
    private double _gainValue;

    partial void OnGainValueChanged(double value) {
        if (!double.TryParse(GainText, out var gain) || Math.Abs(value - gain) > 0.025)
            GainText = value.ToString("0.##"); // Slider步长0.05
        _ = UpdatePreviewAsync(false);
    }

    /// <summary> 增益值文本 </summary>
    [ObservableProperty]
    private string _gainText = "0";

    partial void OnGainTextChanged(string value) =>
        GainValue = double.TryParse(value, out var gain)
            ? Math.Clamp(gain, -1, 1)
            : 0; // 输入无效时设为0（文本也会被更新）

    #endregion

    #region 其他选项

    /// <summary> 是否使用蒙版 </summary>
    [ObservableProperty]
    private bool _useMask = true;

    partial void OnUseMaskChanged(bool value) => _ = UpdatePreviewAsync(false);

    /// <summary> 所有可用色彩空间的名称 </summary>
    public string[] ColorSpaces { get; } = Saturator.ColorSpaces;

    /// <summary> 当前选择的色彩空间的索引 </summary>
    [ObservableProperty]
    private byte _selectedColorSpaceIndex;

    partial void OnSelectedColorSpaceIndexChanged(byte value) => _ = UpdatePreviewAsync(false);

    /// <summary> 所有可用保存策略的名称 </summary>
    public string[] SaveFormats { get; } = Saver.Formats;

    /// <summary> 当前选择的保存策略的索引 </summary>
    [ObservableProperty]
    private byte _selectedSaveFormatIndex;

    #endregion

    #region 刷新预览

    /// <summary> true预览修改后的，false预览修改前的 </summary>
    [ObservableProperty]
    private bool _previewDst = true;

    partial void OnPreviewDstChanged(bool value) => _ = UpdatePreviewAsync(null);

    /// <summary> 选中图像（已载入并缩小） </summary>
    private Image? _srcImage;

    /// <summary> 选中图像的修改前后预览 </summary>
    private BitmapSource? _srcPreview, _dstPreview;

    /// <summary> 界面上的预览图像 </summary>
    [ObservableProperty]
    private BitmapSource? _previewImage;

    /// <summary> 异步预览的令牌源 </summary>
    private CancellationTokenSource _cts = new();

    /// <summary> 打断并刷新预览 </summary>
    /// <param name="srcChanged"> 换文件true，仅调参false，仅切换null </param>
    private Task UpdatePreviewAsync(bool? srcChanged) =>
        Try.DoAsync(
            "刷新预览",
            async () => {
                if (srcChanged is {} changed) {
                    if (!changed) // 仅调参
                        _dstPreview = null;
                    else { // 换文件
                        _srcPreview = _dstPreview = null;
                        if (!PathSelected) { // 取消选中图像
                            _srcImage = null;
                            return;
                        }
                    }
                    await GenPreview();
                }
                PreviewImage = PreviewDst
                    ? _dstPreview
                    : _srcPreview;
            });

    /// <summary> 从_srcImage生成预览图像 </summary>
    private async Task GenPreview() {
        await _cts.CancelAsync();
        _cts.Dispose();
        var token = (_cts = new()).Token;
        await Task.Delay(128, _cts.Token).ConfigureAwait(true); // 防抖

        if (_srcImage is null) {
            token.ThrowIfCancellationRequested();
            _srcImage = await Image.LoadAsync(ImagePaths[SelectedPathIndex], token);
            token.ThrowIfCancellationRequested();
            _srcImage.ToThumb();
        }

        token.ThrowIfCancellationRequested();
        _srcPreview ??= await _srcImage.ToBitmapSourceAsync(token);

        if (GainValue == 0)
            _dstPreview ??= _srcPreview;
        else {
            token.ThrowIfCancellationRequested();
            using var dst = _srcImage.CloneAs<Rgba32>();
            token.ThrowIfCancellationRequested();
            var saturate = Saturator.Get(ColorSpaces[SelectedColorSpaceIndex], GainValue);
            dst.Saturate(saturate, UseMask, token);
            token.ThrowIfCancellationRequested();
            _dstPreview ??= await dst.ToBitmapSourceAsync(token);
        }
    }

    #endregion

    #region 执行处理

    /// <summary> 是否可以运行 </summary>
    private bool CanProcess => ImagePaths.Count > 0;

    /// <summary> 打断预览并执行处理 </summary>
    [RelayCommand(CanExecute = nameof(CanProcess))]
    private void Process() =>
        Try.Do(
            "执行处理",
            () => {
                _ = _cts.CancelAsync();
                var saturate = Saturator.Get(ColorSpaces[SelectedColorSpaceIndex], GainValue);
                var save = Saver.Get(SaveFormats[SelectedSaveFormatIndex]);

                List<string> issues = ["以下文件处理出错："];
                foreach (var path in ImagePaths)
                    try {
                        using var image = Image.Load(path);
                        image.Saturate(saturate, UseMask, null);
                        save(image, path);
                    } catch (Exception ex) {
                        issues.Add($"{path}出错：\n{ex.Message}");
                    }
                if (issues.Count > 1)
                    throw new AggregateException(string.Join('\n', issues));

                _ = MessageBox.Show(
                    "所有图像处理完成！",
                    "成功",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            });

    #endregion
}
