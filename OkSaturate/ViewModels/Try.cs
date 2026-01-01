namespace OkSaturate.ViewModels;

using System.Windows;

/// <summary> 用于简化Try-Catch块的工具类 </summary>
internal static class Try
{
    /// <summary> 尝试执行动作，静默处理打断，弹窗显示其他错误信息 </summary>
    /// <param name="actionName"> 操作名称 </param>
    /// <param name="action"> 委托动作 </param>
    public static void Do(string actionName, Action action) {
        try {
            action();
        } catch (Exception ex) {
            if (ex is not OperationCanceledException)
                ShowErr(actionName, ex);
        }
    }

    /// <summary> 尝试等待任务执行，静默处理打断，弹窗显示其他错误信息 </summary>
    /// <param name="actionName"> 操作名称 </param>
    /// <param name="task"> 委托任务 </param>
    public static async Task DoAsync(string actionName, Func<Task> task) {
        try {
            await task();
        } catch (Exception ex) {
            if (ex is not OperationCanceledException)
                ShowErr(actionName, ex);
        }
    }

    /// <summary> 弹窗显示错误信息 </summary>
    /// <param name="actionName"> 操作名称 </param>
    /// <param name="ex"> 异常对象 </param>
    private static void ShowErr(string actionName, Exception ex) {
        var info = string.IsNullOrWhiteSpace(ex.Message)
            ? "无详细错误信息"
            : string.IsNullOrWhiteSpace(ex.StackTrace)
                ? ex.Message
                : $"{ex.Message}\n\n栈跟踪：\n\n{ex.StackTrace}";
        _ = MessageBox.Show(
            $"{actionName}出错：\n{info}",
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
