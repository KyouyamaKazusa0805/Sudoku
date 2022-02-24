using System.Runtime.InteropServices;
using Sudoku.UI.Data.Interoperability;
using WinRT;

namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileSavePicker"/>.
/// </summary>
/// <seealso cref="FileSavePicker"/>
internal static class FileSavePickerExtensions
{
	/// <summary>
	/// To aware the handle of the window, and apply to the <see cref="FileSavePicker"/> instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AwareHandleOnWin32(this FileSavePicker @this)
	{
		if (Window.Current is null)
		{
			var initializeWithWindowWrapper = @this.As<IInitializeWithWindow>();
			var hwnd = getActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}


		[DllImport("user32", EntryPoint = "GetActiveWindow", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true)]
		static extern IntPtr getActiveWindow();
	}
}
