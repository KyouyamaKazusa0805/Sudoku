namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a multiple forcing chains.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TBranch">The type of branch.</typeparam>
/// <typeparam name="TNode">The type of each node.</typeparam>
public interface IMultipleForcingChains<TSelf, TBranch, TNode> :
	IDictionary<Candidate, TBranch>,
	IForcingChains<TNode>,
	IReadOnlyDictionary<Candidate, TBranch>
	where TSelf : IMultipleForcingChains<TSelf, TBranch, TNode>
	where TBranch : IChainOrForcingChains, IEnumerable<TNode>
	where TNode : IParentLinkedNode<TNode>
{
	/// <summary>
	/// Indicates whether the pattern is aimed to a cell, producing multiple branches.
	/// </summary>
	/// <remarks>
	/// This value is conflict with <see cref="IsHouseMultiple"/>. If this property is <see langword="true"/>,
	/// the property <see cref="IsHouseMultiple"/> will always return <see langword="false"/> and vice versa.
	/// </remarks>
	/// <seealso cref="IsHouseMultiple"/>
	public abstract bool IsCellMultiple { get; }

	/// <summary>
	/// Indicates whether the pattern is aimed to a house, producing multiple branches.
	/// </summary>
	/// <remarks>
	/// This value is conflict with <see cref="IsCellMultiple"/>. If this property is <see langword="true"/>,
	/// the property <see cref="IsCellMultiple"/> will always return <see langword="false"/> and vice versa.
	/// </remarks>
	/// <seealso cref="IsCellMultiple"/>
	public abstract bool IsHouseMultiple { get; }

	/// <summary>
	/// Indicates whether the pattern is advanced. In other words, the start candidates are not inside a cell or a house.
	/// </summary>
	public abstract bool IsAdvancedMultiple { get; }

	/// <summary>
	/// Returns a <see cref="CandidateMap"/> indicating all candidates used in this pattern, as the start.
	/// </summary>
	public abstract CandidateMap Candidates { get; }


	/// <summary>
	/// Determines whether the collection contains at least one element satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public abstract bool Exists(Func<TBranch, bool> predicate);

	/// <summary>
	/// Determines whether all elements in this collection satisfy the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public abstract bool TrueForAll(Func<TBranch, bool> predicate);
}
