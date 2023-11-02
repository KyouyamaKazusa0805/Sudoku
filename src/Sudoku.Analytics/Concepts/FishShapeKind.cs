namespace Sudoku.Analytics.Steps;

/// <summary>
/// Indicates a shape modifier that is used for a complex fish pattern.
/// </summary>
[Flags]
internal enum FishShapeKind
{
	/// <summary>
	/// Indicates the basic fish.
	/// </summary>
	Basic = 1,

	/// <summary>
	/// Indicates the franken fish.
	/// </summary>
	Franken = 1 << 1,

	/// <summary>
	/// Indicates the mutant fish.
	/// </summary>
	Mutant = 1 << 2
}
