namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a property that will be checked in a constraint.
/// </summary>
[Flags]
public enum ConstraintCheckingProperty
{
	/// <summary>
	/// Indicates no elements will be used.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the constraint checks for grid.
	/// </summary>
	Grid = 1,

	/// <summary>
	/// Indicates the constraint checks for analyzer result.
	/// </summary>
	AnalyzerResult = 1 << 1,

	/// <summary>
	/// Indicates the constraint checks for collector result.
	/// </summary>
	CollectorResult = 1 << 2
}
