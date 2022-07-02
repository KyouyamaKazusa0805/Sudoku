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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker WithAwareHandleOnWin32(this FileOpenPicker @this)
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
	/// Sets the property <see cref="FileOpenPicker.SuggestedStartLocation"/> with the specified value.
	/// </summary>
	/// <seealso cref="FileOpenPicker.SuggestedStartLocation"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker WithSuggestedStartLocation(this FileOpenPicker @this, PickerLocationId pickerLocationId)
	{
		@this.SuggestedStartLocation = pickerLocationId;
		return @this;
	}

	/// <summary>
	/// Adds the file type filter into the <see cref="FileOpenPicker.FileTypeFilter"/> property.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker AddFileTypeFilter(this FileOpenPicker @this, string item)
	{
		@this.FileTypeFilter.Add(item);
		return @this;
	}
}
