namespace Sudoku.Collections;

/// <summary>
/// Indicates the grid creating option.
/// </summary>
public enum GridCreatingOption : byte
{
	/// <summary>
	/// Indicates the option is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates each value should minus one before creation.
	/// </summary>
	MinusOne
}
