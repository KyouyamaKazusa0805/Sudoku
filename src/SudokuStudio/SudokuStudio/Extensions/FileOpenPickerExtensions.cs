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
			var hwnd = GetActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
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
	public static FileOpenPicker AddFileTypeFilter(this FileOpenPicker @this, string item)
	{
		@this.FileTypeFilter.Add(item);
		return @this;
	}

	/// <summary>
	/// Retrieves the window handle to the active window attached to the calling thread's message queue.
	/// </summary>
	/// <returns>
	/// The return value is the handle to the active window attached to the calling thread's message queue.
	/// Otherwise, the return value is <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// To get the handle to the foreground window, you can use
	/// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getforegroundwindow">GetForegroundWindow</see>.
	/// </para>
	/// <para>
	/// To get the window handle to the active window in the message queue for another thread, use
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getguithreadinfo">GetGUIThreadInfo</see>.
	/// </para>
	/// </remarks>
	[DllImport("user32")]
	[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
	private static extern nint GetActiveWindow();
}
