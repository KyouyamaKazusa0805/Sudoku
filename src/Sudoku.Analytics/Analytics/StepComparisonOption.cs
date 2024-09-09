namespace Sudoku.Analytics;

/// <summary>
/// Represents an option that compares two <see cref="Step"/> instances using a specified kind of value to be compared.
/// </summary>
/// <seealso cref="Step"/>
public enum StepComparisonOption
{
	/// <summary>
	/// Indicates the comparison rule is to compare internal values of the step.
	/// </summary>
	Default = 0,

	/// <summary>
	/// Indicates the comparison rule is to compare the name of the step.
	/// </summary>
	Name = 1
}
