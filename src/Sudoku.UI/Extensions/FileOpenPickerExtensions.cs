using System.Runtime.InteropServices;
using Sudoku.UI.Interoperability;
using WinRT;

namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
internal static class FileOpenPickerExtensions
{
	/// <summary>
	/// To aware the handle of the window, and apply to the <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker AwareHandleOnWin32(this FileOpenPicker @this)
	{
		if (Window.Current is null)
		{
			var initializeWithWindowWrapper = @this.As<IInitializeWithWindow>();
			var hwnd = GetActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}

		return @this;
	}

	[DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true)]
	private static extern IntPtr GetActiveWindow();
}
