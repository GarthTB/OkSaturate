using System.Windows;

namespace OkSaturate.Utils;

/// <summary> 用于显示弹窗的工具类 </summary>
internal static class MsgBox
{
    /// <summary> 显示消息弹窗 </summary>
    public static void Info(string caption, string text)
        => _ = MessageBox.Show(
            text, caption, MessageBoxButton.OK, MessageBoxImage.Information);

    /// <summary> 显示错误弹窗 </summary>
    public static void Error(string text)
        => _ = MessageBox.Show(
            text, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
}
