namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with a list of nodes.
/// </summary>
[CollectionBuilder(typeof(NodeSet), nameof(Create))]
[TypeImpl(TypeImplFlags.Object_ToString)]
public sealed partial class NodeSet : IComponent, IFormattable, IReadOnlyList<Node>, IReadOnlyCollection<Node>
{
	/// <summary>
	/// Indicates the internal list of nodes.
	/// </summary>
	private readonly List<Node> _nodes = new(6);


	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance via the specified node.
	/// </summary>
	/// <param name="node">The node.</param>
	private NodeSet(Node node) => _nodes = [node];

	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance via the specified list of nodes.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	private NodeSet(List<Node> nodes) => _nodes = nodes;


	/// <summary>
	/// Indicates the number of elements in the collection.
	/// </summary>
	public int Length => _nodes.Count;

	/// <inheritdoc/>
	int IReadOnlyCollection<Node>.Count => Length;

	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.ChainNodeSet;

	/// <inheritdoc/>
	DataStructureType IDataStructure.Type => DataStructureType.SequenceList;

	/// <inheritdoc/>
	DataStructureBase IDataStructure.Base => DataStructureBase.ArrayBased;


	/// <inheritdoc/>
	public Node this[int index] => _nodes[index];


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var s = string.Join(", ", from element in _nodes select element.ToString(formatProvider));
		return $"[{s}]";
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public AnonymousSpanEnumerator<Node> GetEnumerator() => new(_nodes.AsSpan());

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => _nodes.GetEnumerator();


	/// <summary>
	/// Creates a <see cref="NodeSet"/> instance via a list of nodes.
	/// </summary>
	/// <param name="nodes">A list of nodes.</param>
	/// <returns>A <see cref="NodeSet"/> instance returned.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static NodeSet Create(ReadOnlySpan<Node> nodes) => new([.. nodes]);


	/// <summary>
	/// Explicit cast from a <see cref="NodeSet"/> to a <see cref="Node"/> instance.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	public static explicit operator Node?(NodeSet? nodes) => nodes?._nodes is [var node] ? node : null;

	/// <summary>
	/// Implicit cast from a <see cref="Node"/> to a <see cref="NodeSet"/> instance.
	/// </summary>
	/// <param name="node">The node.</param>
	public static implicit operator NodeSet?(Node? node) => node is null ? null : new(node);
}
