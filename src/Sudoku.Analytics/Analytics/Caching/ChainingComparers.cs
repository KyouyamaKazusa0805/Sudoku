namespace Sudoku.Analytics.Caching;

/// <summary>
/// Represents a list of cached chaining comparers <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/>.
/// </summary>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="IComparer{T}"/>
internal static class ChainingComparers
{
	/// <summary>
	/// Indicates the backing field of chain pattern comparer instance.
	/// </summary>
	private static IComparer<Chain>? _chainComparer;

	/// <summary>
	/// Indicates the backing field of multiple forcing chains comparer instance.
	/// </summary>
	private static IComparer<MultipleForcingChains>? _multipleForcingChainsComparer;

	/// <summary>
	/// Indicates the backing field of node map comparer instance.
	/// </summary>
	private static IEqualityComparer<Node>? _nodeComparer;


	/// <summary>
	/// Creates an instance of type <see cref="Comparer{T}"/> of <see cref="Chain"/> on comparison.
	/// </summary>
	public static IComparer<Chain> ChainComparer
		=> _chainComparer ??= Comparer<Chain>.Create(static (l, r) => l.CompareTo(r));

	/// <summary>
	/// Creates an instance of type <see cref="Comparer{T}"/> of <see cref="MultipleForcingChains"/> on comparison.
	/// </summary>
	public static IComparer<MultipleForcingChains> MultipleForcingChainsComparer
		=> _multipleForcingChainsComparer ??= Comparer<MultipleForcingChains>.Create(static (l, r) => l.CompareTo(r));

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
