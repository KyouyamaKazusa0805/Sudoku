namespace Sudoku.Concepts;

/// <summary>
/// Represents a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class Loop : IChainPattern, IElementAtMethod<Loop, Node>, ISliceMethod<Loop, Node>
{
	/// <summary>
	/// Indicates the nodes.
	/// </summary>
	private readonly Node[] _nodes;


	/// <summary>
	/// Initializes a <see cref="Loop"/> instance via a <see cref="Node"/> instance belonging to a loop.
	/// </summary>
	/// <param name="lastNode">The last node.</param>
	public Loop(Node lastNode)
	{
		var nodes = new List<Node> { lastNode };
		for (var node = lastNode.Parent!; node != lastNode; node = node.Parent!)
		{
			nodes.Add(node);
		}
		nodes.Reverse();
		_nodes = [.. nodes];
	}


	/// <inheritdoc/>
	public int Length => _nodes.Length;

	/// <inheritdoc/>
	public int Complexity => _nodes.Length;

	/// <inheritdoc/>
	public Node First => _nodes[0];

	/// <inheritdoc/>
	public Node Last => _nodes[^1];

	/// <inheritdoc/>
	Node[] IChainPattern.BackingNodes => _nodes;


	/// <inheritdoc/>
	public Node this[int index] => _nodes[index];


	/// <inheritdoc/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (0, 0); i < _nodes.Length; linkIndex++, i++)
		{
			var inference = IChainPattern.Inferences[linkIndex & 1];
			sb.Append(_nodes[i].ToString(formatProvider));
			sb.Append(inference.ConnectingNotation());
		}
		sb.Append(_nodes[0].ToString(formatProvider));
		return sb.ToString();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Node> Slice(int start, int length) => _nodes.AsReadOnlySpan()[start..(start + length)];

	/// <inheritdoc/>
	public ConclusionSet GetConclusions(ref readonly Grid grid)
	{
		var result = new ConclusionSet();
		for (var i = 0; i <= Length; i++)
		{
			var node1 = _nodes[i];
			var node2 = _nodes[i == Length ? 0 : i + 1];
			foreach (var conclusion in IChainPattern.GetConclusions(in grid, node1, node2))
			{
				result.Add(conclusion);
			}
		}
		return result;
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	Node IElementAtMethod<Loop, Node>.ElementAt(int index) => this[index];

	/// <inheritdoc/>
	Node IElementAtMethod<Loop, Node>.ElementAt(Index index) => this[index];

	/// <inheritdoc/>
	Node? IElementAtMethod<Loop, Node>.ElementAtOrDefault(int index) => index < 0 || index >= Length ? default : this[index];

	/// <inheritdoc/>
	Node? IElementAtMethod<Loop, Node>.ElementAtOrDefault(Index index)
		=> index.GetOffset(Length) is var i && i >= 0 && i < Length ? this[i] : default;

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)_nodes).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<Node> ISliceMethod<Loop, Node>.Slice(int start, int count) => Slice(start, count).ToArray();
}
