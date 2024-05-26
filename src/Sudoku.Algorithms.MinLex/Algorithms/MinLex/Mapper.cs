namespace Sudoku.Algorithms.MinLex;

/// <summary>
/// Indicates a data structure that describes the cell and label handled.
/// </summary>
/// <remarks>
/// <para>This type is translated from project <c>Gsf.MinLex</c> in solution folder "Interim Projects".</para>
/// </remarks>
public unsafe struct Mapper
{
	/// <summary>
	/// Indicates the cell <see cref="byte"/> values.
	/// </summary>
	public fixed byte Cell[81];

	/// <summary>
	/// Indicates the label <see cref="byte"/> values.
	/// </summary>
	public fixed byte Label[10];
}
