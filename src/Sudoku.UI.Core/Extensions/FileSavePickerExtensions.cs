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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker WithAwareHandleOnWin32(this FileSavePicker @this)
	{
		if (Window.Current is null)
		{
			var initializeWithWindowWrapper = @this.As<IInitializeWithWindow>();
			nint hwnd = getActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}

		return @this;


		[DllImport("user32", EntryPoint = "GetActiveWindow", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true)]
		static extern nint getActiveWindow();
	}

	/// <summary>
	/// Sets the property <see cref="FileSavePicker.SuggestedFileName"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.SuggestedFileName"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker WithSuggestedFileName(this FileSavePicker @this, string name)
	{
		@this.SuggestedFileName = name;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FileSavePicker.SuggestedStartLocation"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.SuggestedStartLocation"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker WithSuggestedStartLocation(this FileSavePicker @this, PickerLocationId pickerLocationId)
	{
		@this.SuggestedStartLocation = pickerLocationId;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FileSavePicker.DefaultFileExtension"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileSavePicker.DefaultFileExtension"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker WithDefaultFileExtension(this FileSavePicker @this, string defaultFileExtension)
	{
		@this.DefaultFileExtension = defaultFileExtension;
		return @this;
	}

	/// <summary>
	/// Adds the file type choices into the <see cref="FileSavePicker.FileTypeChoices"/> property.
	/// </summary>
	/// <seealso cref="FileSavePicker.FileTypeChoices"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker AddFileTypeChoice(this FileSavePicker @this, string key, params string[] values)
	{
		@this.FileTypeChoices.Add(key, new List<string>(values));
		return @this;
	}
}
