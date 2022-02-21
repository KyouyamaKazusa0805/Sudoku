using System.Runtime.InteropServices;

namespace Sudoku.UI.Interoperability;

/// <summary>
/// Exposes a method through which a client can provide an owner window
/// to a Windows Runtime (WinRT) object used in a desktop application.
/// </summary>
/// <remarks>
/// <para>
/// <b>When to initialize:</b><br/>
/// Implement this interface if your object needs to be provided with an owner window,
/// generally to display UI. Most third-party applications will not need to implement this interface.
/// </para>
/// <para>
/// <b>When to use:</b><br/>
/// Use this interface if you will provide an owner window to a WinRT object in a desktop application.
/// For more information about this scenario, see
/// <see href="https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-supported-api?tabs=csharp#classes-that-use-iinitializewithwindow">
/// Classes that use IInitializeWithWindow
/// </see>.
/// </para>
/// <para>
/// This interface is implemented by the following objects. Note that this is necessarily an incomplete list;
/// refer to an individual object's documentation to determine whether that object implements this interface.
/// </para>
/// </remarks>
[ComImport]
[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithWindow
{
	/// <summary>
	/// Specifies an owner window to be used by a Windows Runtime (WinRT) object that is used in a desktop app.
	/// </summary>
	/// <param name="hwnd">The handle of the window to be used as the owner window.</param>
	/// <remarks>
	/// The method doesn't return anything, but the method returns an HResult value
	/// in the Win32 API. For more information, please visit
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-iinitializewithwindow-initialize">
	/// this link
	/// </see>.
	/// </remarks>
	void Initialize([In] IntPtr hwnd);
}
