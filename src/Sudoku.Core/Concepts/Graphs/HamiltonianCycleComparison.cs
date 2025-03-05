namespace Sudoku.Concepts.Graphs;

/// <summary>
/// Represents a comparison rule to a <see cref="HamiltonianCycle"/> instance.
/// </summary>
/// <seealso cref="HamiltonianCycle"/>
public enum HamiltonianCycleComparison
{
	/// <summary>
	/// Indicates the comparison rule will consider direction.
	/// </summary>
	Default,

	/// <summary>
	/// Indicates the comparison rule won't consider direction.
	/// </summary>
	IgnoreDirection
}
