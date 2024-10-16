namespace Sudoku.Analytics.Caching;

/// <summary>
/// Represents a list of cached chaining comparers <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/>.
/// </summary>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="IComparer{T}"/>
internal static class ChainingComparers
{
	/// <summary>
	/// Creates an instance of type <see cref="Comparer{T}"/> of <see cref="Chain"/> on comparison.
	/// </summary>
	[field: MaybeNull]
	public static IComparer<Chain> ChainComparer => field ??= Comparer<Chain>.Create(static (l, r) => l.CompareTo(r));

	/// <summary>
	/// Creates an instance of type <see cref="Comparer{T}"/> of <see cref="MultipleForcingChains"/> on comparison.
	/// </summary>
	[field: MaybeNull]
	public static IComparer<MultipleForcingChains> MultipleForcingChainsComparer
		=> field ??= Comparer<MultipleForcingChains>.Create(static (l, r) => l.CompareTo(r));

	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="Node"/> on equality comparison
	/// in order to filter duplicate nodes on its containing map, guaranteeing same nodes won't be traversed multiple times.
	/// </summary>
	[field: MaybeNull]
	public static IEqualityComparer<Node> NodeMapComparer
		=> field ??= EqualityComparer<Node>.Create(
			static (left, right) => (left, right) switch
			{
				(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn),
				(null, null) => true,
				_ => false
			},
			static obj => obj.GetHashCode(NodeComparison.IgnoreIsOn)
		);
}
