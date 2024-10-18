namespace SudokuStudio.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
internal struct THUMBBUTTON
{
	public THUMBBUTTONMASK dwMask;
	public uint iId;
	public uint iBitmap;
	public nint hIcon;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 259)]
	public string szTip;

	public THUMBBUTTONFLAGS dwFlags;
}
