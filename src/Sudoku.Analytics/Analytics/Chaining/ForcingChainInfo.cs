namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a pair of data on a centain <see cref="Node"/>, describing all possible <see cref="Node"/> instances
/// that can implicitly connect to it, grouped by its <see cref="Node.IsOn"/> property value.
/// </summary>
/// <param name="StartNode">Indicates the start node.</param>
/// <param name="OnNodes">Indicates all possible nodes that can implicitly connect to node, supposed to "on".</param>
/// <param name="OffNodes">Indicates all possible nodes that can implicitly connect to node, supposed to "off".</param>
/// <seealso cref="Node"/>
/// <seealso cref="Node.IsOn"/>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public readonly partial record struct ForcingChainInfo([property: HashCodeMember] Node StartNode, HashSet<Node> OnNodes, HashSet<Node> OffNodes)
{
	[StringMember]
	private string StartNodeString => StartNode.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ForcingChainInfo other) => StartNode == other.StartNode;

	/// <summary>
	/// Determines whether two <see cref="ForcingChainInfo"/> instances contain a same <see cref="StartNode"/> property,
	/// with the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ForcingChainInfo other, NodeComparison nodeComparison) => StartNode.Equals(other.StartNode, nodeComparison);
}
