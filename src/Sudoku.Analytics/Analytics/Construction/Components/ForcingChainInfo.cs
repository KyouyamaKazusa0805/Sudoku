namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a pair of data on a certain <see cref="Node"/>, describing all possible <see cref="Node"/> instances
/// that can implicitly connect to it, grouped by its <see cref="Node.IsOn"/> property value.
/// </summary>
/// <param name="OnNodes">Indicates all possible nodes that can implicitly connect to node, supposed to "on".</param>
/// <param name="OffNodes">Indicates all possible nodes that can implicitly connect to node, supposed to "off".</param>
/// <seealso cref="Node"/>
/// <seealso cref="Node.IsOn"/>
[TypeImpl(TypeImplFlags.Object_GetHashCode)]
public readonly partial record struct ForcingChainInfo(HashSet<Node> OnNodes, HashSet<Node> OffNodes) :
	IEnumerable<Node>,
	IReadOnlyCollection<Node>
{
	/// <inheritdoc/>
	public int Count => OnNodes.Count + OffNodes.Count;

	/// <summary>
	/// Indicates the start node.
	/// </summary>
	public Node StartNode => OnNodes.First().Root;

	[HashCodeMember]
	private int StartNodeHashCode => StartNode.GetHashCode();


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

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new([.. OnNodes, .. OffNodes]);

	/// <summary>
	/// Try to enumerate for nodes that supposed with "on" state.
	/// </summary>
	/// <returns>An enumerator that can iterate on nodes supposed with "on" state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodesEnumerator EnumerateOnNodes() => new(OnNodes.GetEnumerator());

	/// <summary>
	/// Try to enumerate for nodes that supposed with "off" state.
	/// </summary>
	/// <returns>An enumerator that can iterate on nodes supposed with "off" state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodesEnumerator EnumerateOffNodes() => new(OffNodes.GetEnumerator());

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => this.AsEnumerable().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
	{
		foreach (var node in OnNodes)
		{
			yield return node;
		}
		foreach (var node in OffNodes)
		{
			yield return node;
		}
	}

	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append($"{nameof(StartNode)} = {StartNode}");
		return true;
	}
}
