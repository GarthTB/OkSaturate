using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OkSaturate.Models;
using OkSaturate.Services;
using OkSaturate.Utils;
using SixLabors.ImageSharp;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using SaturateStrategy = System.Func<System.Func<double, double>, System.Func<Wacton.Unicolour.Unicolour, Wacton.Unicolour.Unicolour>>;
using SaveStrategy = System.Func<SixLabors.ImageSharp.Image, string, System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace OkSaturate.ViewModels;

/// <summary> MainWindow.xaml 的视图模型 </summary>
internal partial class MainViewModel : ObservableObject
{
    #region 添加和移除待处理图像

    /// <summary> 待处理图像的路径 </summary>
    public ObservableCollection<string> ImagePaths { get; } = [];

    /// <summary> 选中的待处理图像路径的索引 </summary>
    [ObservableProperty]
    private int _selectedImagePathIndex = -1;

    partial void OnSelectedImagePathIndexChanged(int value)
    {
        _ = Try.DoAsync("加载预览", async () =>
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            var token = _cts.Token;

            _srcPreview = HasSelectedPath
                ? await Image.Load(ImagePaths[value])
                    .ToThumbnail()
                    .ToBitmapSourceAsync(token)
                : null;

            await UpdateDstPreviewAsync();
        });
        RemovePathCommand.NotifyCanExecuteChanged();
    }

    /// <returns> 是否选中了待处理图像路径 </returns>
    private bool HasSelectedPath
        => SelectedImagePathIndex >= 0
        && SelectedImagePathIndex < ImagePaths.Count;

    /// <summary> 添加待处理图像路径 </summary>
    [RelayCommand]
    private void AddPath() => Try.Do("添加图像", () =>
    {
        OpenFileDialog dialog = new()
        {
            Title = "选取待处理的图像",
            Filter = "图像文件 (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp)|"
                   + "*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp|"
                   + "所有文件 (*.*)|*.*",
            Multiselect = true
        };
        if (dialog.ShowDialog() == true)
            foreach (string path in dialog.FileNames)
                if (!ImagePaths.Contains(path))
                    ImagePaths.Add(path);
        ProcessCommand.NotifyCanExecuteChanged();
    });

    /// <summary> 移除选中的待处理图像路径 </summary>
    [RelayCommand(CanExecute = nameof(HasSelectedPath))]
    private void RemovePath() => Try.Do("移除图像", () =>
    {
        ImagePaths.RemoveAt(SelectedImagePathIndex);
        SelectedImagePathIndex = -1; // 避免连续移除
        ProcessCommand.NotifyCanExecuteChanged();
    });

    #endregion

    #region 增益值配置

    /// <summary> 增益值 </summary>
    [ObservableProperty]
    private double _saturationGain = 0;

    partial void OnSaturationGainChanged(double value)
    {
        if (!double.TryParse(GainText, out var gain) || value != gain)
            GainText = value.ToString("0.##"); // Slider步长0.04
        _ = UpdateDstPreviewAsync();
    }

    /// <summary> 增益值文本 </summary>
    [ObservableProperty]
    private string _gainText = "0";

    partial void OnGainTextChanged(string value)
        => SaturationGain = double.TryParse(value, out var gain)
        ? Math.Clamp(gain, -1, 1)
        : 0; // 输入无效时设为0（文本也会被更新）

    #endregion

    #region 选项型配置

    /// <summary> 是否使用蒙版 </summary>
    [ObservableProperty]
    private bool _useMask = true;

    partial void OnUseMaskChanged(bool value) => _ = UpdateDstPreviewAsync();

    /// <summary> 所有可用色彩空间的名称 </summary>
    public string[] ColourSpaceNames { get; } = SaturateStrategies.ColourSpaceNames;

    /// <summary> 当前选择的色彩空间的索引 </summary>
    [ObservableProperty]
    private byte _selectedColourSpaceIndex = 0;

    partial void OnSelectedColourSpaceIndexChanged(byte value) => _ = UpdateDstPreviewAsync();

    /// <returns> 当前选择的饱和度调整策略实现 </returns>
    private SaturateStrategy SaturateStrategy
        => SaturateStrategies.Get(ColourSpaceNames[SelectedColourSpaceIndex]);

    /// <summary> 所有可用保存策略的名称 </summary>
    public string[] SaveFormatNames { get; } = SaveStrategies.SaveFormatNames;

    /// <summary> 当前选择的保存策略的索引 </summary>
    [ObservableProperty]
    private byte _selectedSaveFormatIndex = 0;

    /// <returns> 当前选择的保存策略实现 </returns>
    private SaveStrategy SaveStrategy
        => SaveStrategies.Get(SaveFormatNames[SelectedSaveFormatIndex]);

    #endregion

    #region 刷新预览和执行处理

    /// <summary> 预览和处理的CancellationTokenSource </summary>
    private CancellationTokenSource? _cts = null;

    /// <returns> 当前配置的饱和度调整器 </returns>
    private Saturator Saturator => new(SaturateStrategy, SaturationGain, UseMask);

    /// <summary> true预览修改后的，false预览修改前的 </summary>
    [ObservableProperty]
    private bool _previewToggle = true;

    partial void OnPreviewToggleChanged(bool value)
    {
        if (!value)
            PreviewImage = _srcPreview;
        else if (_dstPreview is null)
            _ = UpdateDstPreviewAsync();
        else PreviewImage = _dstPreview;
    }

    /// <summary> 选中图像的修改前后预览 </summary>
    private BitmapSource? _srcPreview = null, _dstPreview = null;

    /// <summary> 界面上的预览图像 </summary>
    [ObservableProperty]
    private BitmapSource? _previewImage = null;

    /// <summary> 打断并刷新修改后预览 </summary>
    private async Task UpdateDstPreviewAsync()
        => await Try.DoAsync("刷新修改后预览", async () =>
        {

        });

    /// <summary> 是否可以运行 </summary>
    private bool CanProcess => ImagePaths.Count > 0;

    /// <summary> 打断预览并执行处理 </summary>
    [RelayCommand(CanExecute = nameof(CanProcess))]
    private async Task ProcessAsync() => await Try.DoAsync("执行处理", async () =>
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new();
        var token = _cts.Token;

        var saturator = Saturator;
        var saveAsync = SaveStrategy;
        List<string> issues = ["以下文件处理出错："];

        foreach (var path in ImagePaths)
            try
            {
                using var image = Image.Load(path);
                saturator.Process(image, token);
                await saveAsync(image, path, token);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            { issues.Add($"{path} 出错：\n{ex.Message}"); }

        if (issues.Count == 1)
            MsgBox.Info("成功", "所有图像处理完成！");
        else throw new AggregateException(string.Join("\n\n", issues));
    });

    #endregion
}
