namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a binary forcing chains.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TBranch">The type of each branch.</typeparam>
internal interface IBinaryForcingChains<TSelf, TBranch> : IForcingChains
	where TSelf : IBinaryForcingChains<TSelf, TBranch>
	where TBranch : IChainOrForcingChains, IEnumerable<Node>;
