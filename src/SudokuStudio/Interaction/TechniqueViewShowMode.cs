namespace SudokuStudio.Interaction;

/// <summary>
/// The technique view show mode.
/// </summary>
public enum TechniqueViewShowMode
{
	/// <summary>
	/// Indicates no technique will be displayed.
	/// </summary>
	None,

	/// <summary>
	/// Indicates only assignments will be displayed.
	/// </summary>
	OnlyAssignments,

	/// <summary>
	/// Indicates only eliminations will be displayed.
	/// </summary>
	OnlyEliminations,

	/// <summary>
	/// Indicates all techniques will be displayed.
	/// </summary>
	Both
}
