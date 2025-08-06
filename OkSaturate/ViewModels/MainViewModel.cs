using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OkSaturate.Services;
using OkSaturate.Utils;
using System.Collections.ObjectModel;

namespace OkSaturate.ViewModels;

/// <summary> MainWindow.xaml 的视图模型 </summary>
internal partial class MainViewModel : ObservableObject
{
    #region 添加和移除待处理图像

    /// <summary> 待处理图像的路径 </summary>
    public ObservableCollection<string> ImagePaths { get; } = [];

    /// <summary> 选中的待处理图像路径的索引 </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemovePathCommand))]
    private int _selectedImagePathIndex = -1;

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
    });

    /// <summary> 移除选中的待处理图像路径 </summary>
    [RelayCommand(CanExecute = nameof(HasSelectedPath))]
    private void RemovePath() => Try.Do("移除图像", () =>
    {
        ImagePaths.RemoveAt(SelectedImagePathIndex);
        SelectedImagePathIndex = -1; // 避免连续移除
    });

    #endregion

    #region 增益值配置

    /// <summary> 增益值 </summary>
    [ObservableProperty]
    private double _saturationGain = 0;

    /// <summary> 增益值文本 </summary>
    [ObservableProperty]
    private string _gainText = "0";

    #endregion

    #region 选项型配置

    /// <summary> 是否使用蒙版 </summary>
    [ObservableProperty]
    private bool _useMask = true;

    /// <summary> 所有可用色彩空间的名称 </summary>
    public string[] ColourSpaceNames { get; } = SaturateStrategies.ColourSpaceNames;

    /// <summary> 当前选择的色彩空间的索引 </summary>
    [ObservableProperty]
    private byte _selectedColourSpaceIndex = 0;

    /// <summary> 所有可用保存策略的名称 </summary>
    public string[] SaveFormatNames { get; } = SaveStrategies.SaveFormatNames;

    /// <summary> 当前选择的保存策略的索引 </summary>
    [ObservableProperty]
    private byte _selectedSaveFormatIndex = 0;

    #endregion
}
