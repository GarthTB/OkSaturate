using OkSaturate.ViewModels;
using System.Windows;

namespace OkSaturate.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    /// <summary> 向ListBox中拖拽待处理的图像 </summary>
    private void ListBox_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is MainViewModel vm
            && e.Data.GetData(DataFormats.FileDrop) is string[] paths)
        {
            foreach (var path in paths)
                if (!vm.ImagePaths.Contains(path))
                    vm.ImagePaths.Add(path);
            vm.ProcessCommand.NotifyCanExecuteChanged();
        }
    }
}
