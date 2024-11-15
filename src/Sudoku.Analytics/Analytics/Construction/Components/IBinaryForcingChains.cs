namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a binary forcing chains.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TBranch">The type of each branch.</typeparam>
/// <typeparam name="TNode">The type of each node.</typeparam>
public interface IBinaryForcingChains<TSelf, TBranch, TNode> : IForcingChains<TNode>
	where TSelf : IBinaryForcingChains<TSelf, TBranch, TNode>
	where TBranch : IChainOrForcingChains, IEnumerable<TNode>
	where TNode : IParentLinkedNode<TNode>
{
	/// <summary>
	/// Indicates the backing branches.
	/// </summary>
	protected abstract ReadOnlySpan<TBranch> Branches { get; }
}
