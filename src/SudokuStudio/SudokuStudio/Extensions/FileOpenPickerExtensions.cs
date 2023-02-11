namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
public static class FileOpenPickerExtensions
{
	/// <summary>
	/// To aware the handle of the window, and apply to the <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	public static FileOpenPicker WithAwareHandleOnWin32(this FileOpenPicker @this)
	{
		if (Window.Current is null)
		{
			var initializeWithWindowWrapper = @this.As<IInitializeWithWindow>();
			var hWnd = ((App)Application.Current).RunningWindow.As<IWindowNative>().WindowHandle;
			initializeWithWindowWrapper.Initialize(hWnd);
		}

		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FileOpenPicker.SuggestedStartLocation"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileOpenPicker.SuggestedStartLocation"/>
	public static FileOpenPicker WithSuggestedStartLocation(this FileOpenPicker @this, PickerLocationId pickerLocationId)
	{
		@this.SuggestedStartLocation = pickerLocationId;
		return @this;
	}

	/// <summary>
	/// Adds the file type filter into the <see cref="FileOpenPicker.FileTypeFilter"/> property.
	/// </summary>
	/// <seealso cref="FileOpenPicker.FileTypeFilter"/>
	public static FileOpenPicker AddFileTypeFilter(this FileOpenPicker @this, string item)
	{
		@this.FileTypeFilter.Add(item);
		return @this;
	}
}
