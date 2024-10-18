namespace SudokuStudio.Runtime.InteropServices;

/// <summary>
/// Used by the <see cref="THUMBBUTTON"/> structure to specify which members of that structure contain valid data.
/// </summary>
internal enum THUMBBUTTONMASK
{
	/// <summary>
	/// The <see cref="THUMBBUTTON.iBitmap"/> member contains valid information.
	/// </summary>
	THB_BITMAP = 0x1,

	/// <summary>
	/// The <see cref="THUMBBUTTON.hIcon"/> member contains valid information.
	/// </summary>
	THB_ICON = 0x2,

	/// <summary>
	/// The <see cref="THUMBBUTTON.szTip"/> member contains valid information.
	/// </summary>
	THB_TOOLTIP = 0x4,

	/// <summary>
	/// The <see cref="THUMBBUTTON.dwFlags"/> member contains valid information.
	/// </summary>
	THB_FLAGS = 0x8
}
