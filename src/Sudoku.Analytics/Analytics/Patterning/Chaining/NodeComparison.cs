namespace Sudoku.Analytics.Patterning.Chaining;

/// <summary>
/// Represents a comparison rule on <see cref="Node"/> instances.
/// </summary>
/// <seealso cref="Node"/>
public enum NodeComparison
{
	/// <summary>
	/// Indicates the comparison rule checks on <see cref="Node.IsOn"/> property.
	/// </summary>
	/// <seealso cref="Node.IsOn"/>
	IncludeIsOn,

	/// <summary>
	/// Indicates the comparison rule ignores for <see cref="Node.IsOn"/> property, and only checks on <see cref="Node.Map"/>.
	/// </summary>
	/// <seealso cref="Node.IsOn"/>
	/// <seealso cref="Node.Map"/>
	IgnoreIsOn
}
