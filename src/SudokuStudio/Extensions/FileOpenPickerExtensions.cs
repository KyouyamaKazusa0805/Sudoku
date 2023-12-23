namespace Windows.Storage.Pickers;

/// <summary>
/// Provides with extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
public static class FileOpenPickerExtensions
{
	internal static void Initialize<T>(this FileOpenPicker @this, T control) where T : UIElement
	{
		var window = ((App)Application.Current).WindowManager.GetWindowForElement(control);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(@this, hWnd);
	}
}
