namespace Sudoku.Concepts;

/// <summary>
/// Provides with a comparison rule on <see cref="IChainPattern"/> instances (i.e. <see cref="Chain"/> or <see cref="Loop"/>).
/// </summary>
/// <seealso cref="IChainPattern"/>
/// <seealso cref="Chain"/>
/// <seealso cref="Loop"/>
public enum ChainPatternComparison
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
