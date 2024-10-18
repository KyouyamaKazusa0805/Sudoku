namespace SudokuStudio.Runtime.InteropServices;

/// <summary>
/// Used by methods of the <see cref="ITaskbarList3"/> interface to define buttons used in a toolbar
/// embedded in a window's thumbnail representation.
/// </summary>
/// <seealso cref="ITaskbarList3"/>
[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
internal struct THUMBBUTTON
{
	/// <summary>
	/// A combination of <see cref="THUMBBUTTONMASK"/> values that specify which members of this structure contain valid data;
	/// other members are ignored, with the exception of <see cref="iId"/>, which is always required.
	/// </summary>
	public THUMBBUTTONMASK dwMask;

	/// <summary>
	/// The application-defined identifier of the button, unique within the toolbar.
	/// </summary>
	public uint iId;

	/// <summary>
	/// The zero-based index of the button image within the image list set through
	/// <see cref="ITaskbarList3.ThumbBarSetImageList"/>.
	/// </summary>
	public uint iBitmap;

	/// <summary>
	/// The handle of an icon to use as the button image.
	/// </summary>
	public nint hIcon;

	/// <summary>
	/// A wide character array that contains the text of the button's tooltip,
	/// displayed when the mouse pointer hovers over the button.
	/// </summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 259)]
	public string szTip;

	/// <summary>
	/// A combination of <see cref="THUMBBUTTONFLAGS"/> values that control specific states and behaviors of the button.
	/// </summary>
	public THUMBBUTTONFLAGS dwFlags;
}
