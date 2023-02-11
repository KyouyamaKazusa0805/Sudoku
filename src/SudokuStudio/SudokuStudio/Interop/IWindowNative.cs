namespace SudokuStudio.Interop;

/// <summary>
/// <para>
/// Enables interoperability between XAML and a native window. This interface is implemented by <see cref="Window"/>,
/// which desktop apps can use to get the underlying HWND of the window.
/// </para>
/// <para>For more info, and code examples, see
/// <see href="https://learn.microsoft.com/en-us/windows/apps/develop/ui-input/retrieve-hwnd">Retrieve a window handle (HWND).</see>
/// </para>
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
internal interface IWindowNative
{
	/// <summary>
	/// Retrieves the window handle (HWND) of the window represented by the object that implements <see cref="IWindowNative"/>.
	/// </summary>
	nint WindowHandle { get; }
}
