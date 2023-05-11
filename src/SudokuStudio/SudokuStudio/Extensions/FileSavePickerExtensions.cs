namespace Windows.Storage.Pickers;

/// <summary>
/// Provides with extension methods on <see cref="FileSavePicker"/>.
/// </summary>
/// <seealso cref="FileSavePicker"/>
public static class FileSavePickerExtensions
{
	internal static void Initialize<T>(this FileSavePicker @this, T control) where T : UIElement
	{
		var window = ((App)Application.Current).WindowManager.GetWindowForElement(control);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(@this, hWnd);
	}
}
