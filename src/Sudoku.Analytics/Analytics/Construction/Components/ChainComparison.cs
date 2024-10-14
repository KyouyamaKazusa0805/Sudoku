namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with a comparison rule on <see cref="Chain"/> instances.
/// </summary>
/// <seealso cref="Chain"/>
public enum ChainComparison
{
	/// <summary>
	/// Indicates the comparison will ignore direction on chains.
	/// For example, <c>A == B -- C == D</c> is equal to <c>D == C -- B == A</c>.
	/// </summary>
	Undirected,

	/// <summary>
	/// Indicates the comparison will also check on direction.
	/// For example, <c>A == B -- C == D</c> is not equal to <c>D == C -- B == A</c>.
	/// </summary>
	Directed
}
