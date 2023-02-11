namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileSavePicker"/>.
/// </summary>
/// <seealso cref="FileSavePicker"/>
public static class FileSavePickerExtensions
{
	/// <summary>
	/// To aware the handle of the window, and apply to the <see cref="FileSavePicker"/> instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	public static FileSavePicker WithAwareHandleOnWin32(this FileSavePicker @this)
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
	/// Sets the property <see cref="FileSavePicker.SuggestedFileName"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.SuggestedFileName"/>
	public static FileSavePicker WithSuggestedFileName(this FileSavePicker @this, string name)
	{
		@this.SuggestedFileName = name;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FileSavePicker.SuggestedStartLocation"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.SuggestedStartLocation"/>
	public static FileSavePicker WithSuggestedStartLocation(this FileSavePicker @this, PickerLocationId pickerLocationId)
	{
		@this.SuggestedStartLocation = pickerLocationId;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FileSavePicker.DefaultFileExtension"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.DefaultFileExtension"/>
	public static FileSavePicker WithDefaultFileExtension(this FileSavePicker @this, string defaultFileExtension)
	{
		@this.DefaultFileExtension = defaultFileExtension;
		return @this;
	}

	/// <summary>
	/// Adds the file type choices into the <see cref="FileSavePicker.FileTypeChoices"/> property.
	/// </summary>
	/// <seealso cref="FileSavePicker.FileTypeChoices"/>
	public static FileSavePicker AddFileTypeChoice(this FileSavePicker @this, string key, params string[] values)
	{
		@this.FileTypeChoices.Add(key, new List<string>(values));
		return @this;
	}
}
