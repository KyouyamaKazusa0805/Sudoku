namespace Windows.Storage.Pickers;

/// <summary>
/// Provides with extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
public static class FileOpenPickerExtensions
{
	internal static void Initialize<TUIElement>(this FileOpenPicker @this, TUIElement control) where TUIElement : UIElement
	{
		var window = ((App)Application.Current).WindowManager.GetWindowForElement(control);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(@this, hWnd);
	}
}
