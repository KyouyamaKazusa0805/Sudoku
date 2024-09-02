namespace Windows.Storage.Pickers;

/// <summary>
/// Provides with extension methods on <see cref="FileSavePicker"/>.
/// </summary>
/// <seealso cref="FileSavePicker"/>
public static class FileSavePickerExtensions
{
	internal static void Initialize<TUIElement>(this FileSavePicker @this, TUIElement control) where TUIElement : UIElement
	{
		var window = Application.Current.AsApp().WindowManager.GetWindowForElement(control);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(@this, hWnd);
	}
}
