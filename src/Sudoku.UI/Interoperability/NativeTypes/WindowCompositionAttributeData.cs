namespace Sudoku.UI.Interoperability.NativeTypes;

[StructLayout(LayoutKind.Sequential)]
internal struct WindowCompositionAttributeData
{
	public WindowCompositionAttribute Attribute;
	public nint Data;
	public int SizeOfData;
}
