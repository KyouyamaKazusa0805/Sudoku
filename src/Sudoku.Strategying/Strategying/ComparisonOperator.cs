namespace Sudoku.Strategying;

/// <summary>
/// Represent an operator to be used in comparison.
/// </summary>
public enum ComparisonOperator
{
	/// <summary>
	/// Indicates the comparison rule is to compare equality.
	/// </summary>
	Equality,

	/// <summary>
	/// Indicates the comparison rule is to compare inequality.
	/// </summary>
	Inequality,

	/// <summary>
	/// Indicates the comparison rule is to compare greater-than.
	/// </summary>
	GreaterThan,

	/// <summary>
	/// Indicates the comparison rule is to compare greater-than or equality.
	/// </summary>
	GreaterThanOrEqual,

	/// <summary>
	/// Indicates the comparison rule is to compare less-than.
	/// </summary>
	LessThan,

	/// <summary>
	/// Indicates the comparison rule is to compare less-than or equality.
	/// </summary>
	LessThanOrEqual
}
