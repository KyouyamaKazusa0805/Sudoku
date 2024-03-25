namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a single technique.
/// </summary>
public enum SingleTechnique
{
	/// <summary>
	/// Represents none of technique used.
	/// </summary>
	None,

	/// <summary>
	/// Represents full house.
	/// </summary>
	FullHouse,

	/// <summary>
	/// Represents last digit.
	/// </summary>
	LastDigit,

	/// <summary>
	/// Represents hidden single.
	/// </summary>
	HiddenSingle,

	/// <summary>
	/// Represents naked single.
	/// </summary>
	NakedSingle
}
