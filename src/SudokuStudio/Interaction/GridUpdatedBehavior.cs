namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a kind that describes which behavior the grid is updated.
/// </summary>
public enum GridUpdatedBehavior
{
	/// <summary>
	/// Indicates the behavior is removing a candidate from the specified grid.
	/// </summary>
	Elimination,

	/// <summary>
	/// Indicates the behavior is removing a list of candidates from the specified grid.
	/// This field will be used for the case that user eliminated candidates by context flyout
	/// after mouse right button is clicked.
	/// </summary>
	EliminationMultiple,

	/// <summary>
	/// Indicates the behavior is assigning a digit into a cell with the specified grid.
	/// </summary>
	Assignment,

	/// <summary>
	/// Indicates the behavior is clearing a cell, resetting the cell as empty one.
	/// </summary>
	Clear,

	/// <summary>
	/// Indicates the behavior is undoing the grid.
	/// </summary>
	Undoing,

	/// <summary>
	/// Indicates the behavior is redoing the grid.
	/// </summary>
	Redoing,

	/// <summary>
	/// Indicates the behavior is changing the grid into another new value, by assigning property <see cref="SudokuPane.Puzzle"/>
	/// with a newer <see cref="Grid"/> value.
	/// </summary>
	Replacing
}
