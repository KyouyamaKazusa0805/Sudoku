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
			var hwnd = GetActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}

		return @this;
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
