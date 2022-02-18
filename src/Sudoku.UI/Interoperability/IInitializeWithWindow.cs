using System.Runtime.InteropServices;

namespace Sudoku.UI.Interoperability;

[ComImport]
[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithWindow
{
	void Initialize([In] IntPtr hwnd);
}
