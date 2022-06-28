namespace Sudoku.UI.Interoperability;

[StructLayout(LayoutKind.Sequential)]
internal struct DispatcherQueueOptions
{
	internal int dwSize;
	internal int threadType;
	internal int apartmentType;
}
