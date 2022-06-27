namespace Sudoku.UI.Interoperability.NativeTypes;

[StructLayout(LayoutKind.Sequential)]
internal struct AccentPolicy
{
	public AccentState AccentState;
	public uint AccentFlags;
	public uint GradientColor;
	public uint AnimationId;
}
