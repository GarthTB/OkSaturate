namespace OkSaturate.Views;

using System.Windows;
using ViewModels;

public sealed partial class MainWindow
{
    public MainWindow() => InitializeComponent();

    /// <summary> 向ListBox中拖放待处理的图像 </summary>
    private void ListBox_Drop(object sender, DragEventArgs e) {
        if (DataContext is not MainViewModel vm
         || e.Data.GetData(DataFormats.FileDrop) is not string[] paths)
            return;
        foreach (var path in paths.Where(path => !vm.ImagePaths.Contains(path)))
            vm.ImagePaths.Add(path);
        vm.ProcessCommand.NotifyCanExecuteChanged();
    }
}
