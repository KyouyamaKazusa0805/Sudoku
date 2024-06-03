namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.EqualityOperators)]
public sealed partial class Chain : IChainPattern, IElementAtMethod<Chain, Node>, IEquatable<Chain>, ISliceMethod<Chain, Node>
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
	public Node First => Span[0];

	/// <inheritdoc/>
	public Node Last => Span[^1];

	/// <inheritdoc/>
	Node[] IChainPattern.BackingNodes => _nodes;

	/// <summary>
	/// Create a <see cref="ReadOnlySpan{T}"/> instance that holds valid <see cref="Node"/> instances to be used in a chain.
	/// </summary>
	private ReadOnlySpan<Node> Span => _nodes.AsReadOnlySpan()[_weakStart ? 1..^1 : ..];


	/// <inheritdoc/>
	public Node this[int index] => Span[_weakStart ? index - 1 : index];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Chain? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected);

	/// <inheritdoc cref="IChainPattern.Equals(IChainPattern?, NodeComparison, ChainPatternComparison)"/>
	public bool Equals([NotNullWhen(true)] Chain? other, NodeComparison nodeComparison, ChainPatternComparison chainComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Length != other.Length)
		{
			return false;
		}

		var span1 = Span;
		var span2 = other.Span;
		switch (chainComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				if (span1[0].Equals(span2[0], nodeComparison))
				{
					for (var i = 0; i < Length; i++)
					{
						if (!span1[i].Equals(span2[i], nodeComparison))
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					for (var (i, j) = (0, Length - 1); i < Length; i++, j--)
					{
						if (!span1[i].Equals(span2[j], nodeComparison))
						{
							return false;
						}
					}
					return true;
				}
			}
			case ChainPatternComparison.Directed:
			{
				for (var i = 0; i < Length; i++)
				{
					if (!span1[i].Equals(span2[i], nodeComparison))
					{
						return false;
					}
				}
				return true;
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(chainComparison));
			}
		}
	}

	/// <inheritdoc/>
	public override int GetHashCode() => GetHashCode(NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected);

	/// <inheritdoc/>
	public int GetHashCode(NodeComparison nodeComparison, ChainPatternComparison patternComparison)
	{
		var result = new HashCode();
		var span = Span;
		switch (patternComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				for (var i = 0; i < Length; i++)
				{
					result.Add(span[i].GetHashCode(nodeComparison));
				}
				for (var i = Length - 1; i >= 0; i--)
				{
					result.Add(span[i].GetHashCode(nodeComparison));
				}
				break;
			}
			case ChainPatternComparison.Directed:
			{
				foreach (var element in span)
				{
					result.Add(element.GetHashCode(nodeComparison));
				}
				break;
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(patternComparison));
			}
		}
		return result.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var span = Span;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (0, 0); i < span.Length; linkIndex++, i++)
		{
			var inference = IChainPattern.Inferences[linkIndex & 1];
			sb.Append(span[i].ToString(format, formatProvider));
			if (i != span.Length - 1)
			{
				sb.Append(inference.ConnectingNotation());
			}
		}
		return sb.ToString();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Node> Slice(int start, int length) => Span[start..(start + length)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConclusionSet GetConclusions(ref readonly Grid grid) => [.. IChainPattern.GetConclusions(in grid, First, Last)];

	/// <inheritdoc/>
	bool IEquatable<IChainPattern>.Equals(IChainPattern? other) => other is Chain comparer && Equals(comparer);

	/// <inheritdoc/>
	bool IChainPattern.Equals(IChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
		=> other is Chain comparer && Equals(comparer, nodeComparison, patternComparison);

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
	IEnumerator IEnumerable.GetEnumerator() => Span.ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Node> IEnumerable<Node>.GetEnumerator() => ((IEnumerable<Node>)Span.ToArray()).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<Node> ISliceMethod<Chain, Node>.Slice(int start, int count) => Slice(start, count).ToArray();
}
