namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents prefer way that specifies a kind that a user will perfer using a single technique to fill a grid.
/// </summary>
public enum SingleTechniquePrefer
{
	/// <summary>
	/// Indicates a user likes an unknown behavior to finish a grid.
	/// </summary>
	Unknown,

	/// <summary>
	/// Indicates a user likes using hidden singles to finish a grid.
	/// </summary>
	HiddenSingle,

	/// <summary>
	/// Indicates a user likes using naked singles to finish a grid.
	/// </summary>
	NakedSingle
}
