namespace Sudoku.Shuffling.Minlex;

/// <summary>
/// Indicates a data structure that describes the cell and label handled.
/// </summary>
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
