#undef LIBRARY_IMPORT_STYLE_PINVOKE
#define DLL_IMPORT_STYLE_PIVOKE

namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
public static
#if LIBRARY_IMPORT_STYLE_PINVOKE && !DLL_IMPORT_STYLE_PIVOKE
partial
#endif
class FileOpenPickerExtensions
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
			var hwnd = GetActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}

		return @this;
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

#if LIBRARY_IMPORT_STYLE_PINVOKE
	[LibraryImport("user32")]
	private static partial nint GetActiveWindow();
#elif DLL_IMPORT_STYLE_PIVOKE
	[DllImport("user32", CharSet = CharSet.Auto)]
	private static extern nint GetActiveWindow();
#else
#error Cannot import method 'GetActiveWindow' as P/Invoke due to not being found.
#endif
}
