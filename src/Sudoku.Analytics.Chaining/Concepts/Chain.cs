namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class Chain : IChainPattern, IElementAtMethod<Chain, Node>, ISliceMethod<Chain, Node>
{
	/// <summary>
	/// Indicates whether the chain starts with weak link.
	/// </summary>
	private readonly bool _weakStart;

	/// <summary>
	/// Indicates the nodes.
	/// </summary>
	private readonly Node[] _nodes;


	/// <summary>
	/// Initializes an <see cref="Chain"/> instance via the specified node belonging to a chain at the last position.
	/// </summary>
	/// <param name="lastNode">The last node.</param>
	/// <param name="weakStart">Indicates whether the chain starts with weak link.</param>
	public Chain(Node lastNode, bool weakStart)
	{
		_weakStart = weakStart;
		var nodes = new List<Node> { lastNode };
		for (var node = lastNode.Parent!; !node.Equals(lastNode, NodeComparison.IgnoreIsOn); node = node.Parent!)
		{
			nodes.Add(new Node(node, null));
		}
		nodes.Add(~lastNode);
		nodes.Reverse();
		_nodes = [.. nodes];
	}


	/// <inheritdoc/>
	public int Length => _weakStart ? _nodes.Length - 2 : _nodes.Length;

	/// <inheritdoc/>
	public int Complexity => _nodes.Length;

	/// <inheritdoc/>
	public Node First => Snapshot[0];

	/// <inheritdoc/>
	public Node Last => Snapshot[^1];

	/// <inheritdoc/>
	Node[] IChainPattern.BackingNodes => _nodes;

	/// <summary>
	/// Create a <see cref="ReadOnlySpan{T}"/> instance that holds valid <see cref="Node"/> instances to be used in a chain.
	/// </summary>
	private ReadOnlySpan<Node> Snapshot => _nodes.AsReadOnlySpan()[_weakStart ? 1..^1 : ..];


	/// <inheritdoc/>
	public Node this[int index] => Snapshot[_weakStart ? index - 1 : index];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Node> Slice(int start, int length) => Snapshot[start..(start + length)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConclusionSet GetConclusions(ref readonly Grid grid) => [.. IChainPattern.GetConclusions(in grid, First, Last)];

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var snapshot = Snapshot;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (0, 0); i < snapshot.Length; linkIndex++, i++)
		{
			var inference = IChainPattern.Inferences[linkIndex & 1];
			sb.Append(snapshot[i].ToString(format, formatProvider));
			if (i != snapshot.Length - 1)
			{
				sb.Append(inference.ConnectingNotation());
			}
		}
		return sb.ToString();
	}

	/// <inheritdoc/>
	Node IElementAtMethod<Chain, Node>.ElementAt(int index) => this[index];

	/// <inheritdoc/>
	Node IElementAtMethod<Chain, Node>.ElementAt(Index index) => this[index];

	/// <inheritdoc/>
	Node? IElementAtMethod<Chain, Node>.ElementAtOrDefault(int index) => index < 0 || index >= Length ? default : this[index];

	/// <inheritdoc/>
	Node? IElementAtMethod<Chain, Node>.ElementAtOrDefault(Index index)
		=> index.GetOffset(Length) is var i && i >= 0 && i < Length ? this[i] : default;

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => Snapshot.ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)Snapshot.ToArray()).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<Node> ISliceMethod<Chain, Node>.Slice(int start, int count) => Slice(start, count).ToArray();
}
