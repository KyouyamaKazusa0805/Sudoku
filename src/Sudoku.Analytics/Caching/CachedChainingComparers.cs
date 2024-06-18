namespace Sudoku.Caching;

/// <summary>
/// Represents a list of cached chaining comparers <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/>.
/// </summary>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="IComparer{T}"/>
internal static class CachedChainingComparers
{
	/// <summary>
	/// Indicates the backing field of chain pattern comparer instance.
	/// </summary>
	private static IComparer<ChainOrLoop>? _chainPatternComparer;

	/// <summary>
	/// Indicates the backing field of node map comparer instance.
	/// </summary>
	private static IEqualityComparer<Node>? _nodeComparer;

	/// <summary>
	/// Indicates the backing field of multiple forcing chains comparer instance.
	/// </summary>
	private static IEqualityComparer<MultipleForcingChains>? _multipleForcingChainsPatternComparer;


	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="ChainOrLoop"/> on equality comparison
	/// in order to filter duplicate chains.
	/// </summary>
	public static IComparer<ChainOrLoop> ChainPatternComparer
		=> _chainPatternComparer ??= Comparer<ChainOrLoop>.Create(static (left, right) => left.CompareTo(right));

	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="MultipleForcingChains"/> on equality comparison
	/// in order to filter duplicate multiple forcing chains.
	/// </summary>
	public static IEqualityComparer<MultipleForcingChains> MultipleForcingChainsPatternComparer
		=> _multipleForcingChainsPatternComparer ??= EqualityComparer<MultipleForcingChains>.Create(
			static (left, right) => (left, right) switch
			{
				(null, null) => true,
				(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected),
				_ => false
			},
			static obj => obj.GetHashCode(NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected)
		);

	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="Node"/> on equality comparison
	/// in order to filter duplicate nodes on its containing map, guaranteeing same nodes won't be traversed multiple times.
	/// </summary>
	public static IEqualityComparer<Node> NodeMapComparer
		=> _nodeComparer ??= EqualityComparer<Node>.Create(
			static (left, right) => (left, right) switch
			{
				(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn),
				(null, null) => true,
				_ => false
			},
			static obj => obj.GetHashCode(NodeComparison.IgnoreIsOn)
		);
}
