namespace OkSaturate.ViewModels;

using System.Windows;

/// <summary> 用于简化Try-Catch块的工具类 </summary>
internal static class Try
{
    /// <summary> 执行action，若有异常则弹窗提示 </summary>
    public static void Do(string actionName, Action action) {
        try {
            action();
        } catch (Exception ex) when (ex is not OperationCanceledException) {
            _ = MessageBox.Show(
                $"{actionName} 出错：\n{FormatException(ex)}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary> 执行并等待task，静默处理打断，若有异常则弹窗提示 </summary>
    public static async Task DoAsync(string actionName, Func<Task> task) {
        try {
            await task();
        } catch (Exception ex) when (ex is not OperationCanceledException) {
            _ = MessageBox.Show(
                $"{actionName} 出错：\n{FormatException(ex)}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary> 格式化异常信息 </summary>
    private static string FormatException(Exception ex) =>
        string.IsNullOrWhiteSpace(ex.Message)
            ? "无详细错误信息"
            : string.IsNullOrWhiteSpace(ex.StackTrace)
                ? ex.Message
                : $"{ex.Message}\n\n栈跟踪：\n\n{ex.StackTrace}";
}
