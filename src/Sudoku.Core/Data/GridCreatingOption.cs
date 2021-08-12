namespace Sudoku.Data;

/// <summary>
/// Indicates the grid creating option.
/// </summary>
[Closed]
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
