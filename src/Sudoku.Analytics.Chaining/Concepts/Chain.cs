namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class Chain :
	IChainPattern,
	IComparable<Chain>,
	IElementAtMethod<Chain, Node>,
	IEquatable<Chain>,
	ISliceMethod<Chain, Node>
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
		for (var node = lastNode.Parent; node is not null; node = node.Parent)
		{
			nodes.Add(new Node(node, null));
		}

		// To cover the nodes.
		_nodes = [.. nodes];

		// Reverse the whole chain if the first node is greater than the last node in logic.
		if (nodes[1].CompareTo(nodes[^2], NodeComparison.IgnoreIsOn) >= 0)
		{
			Reverse();
		}
	}


	/// <inheritdoc/>
	public bool IsGrouped => Span.Any(static (ref readonly Node node) => node.IsGroupedNode);

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
	public Node this[int index] => Span[index];


	/// <summary>
	/// Reverse the whole chain.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reverse()
	{
		var newNodes = new Node[_nodes.Length];
		for (var (i, pos) = (0, _nodes.Length - 1); i < _nodes.Length; i++, pos--)
		{
			// Reverse and negate its "IsOn" property to keep the chain starting with same "IsOn" property value.
			newNodes[i] = ~_nodes[pos];
		}
		Array.Copy(newNodes, _nodes, _nodes.Length);
	}

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
		var span = Span;
		switch (patternComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				// To guarantee the final hash code is same on different direction, we should sort all nodes,
				// in order to make same nodes are in the same position.
				var nodesSorted = span.ToArray();
				Array.Sort(nodesSorted, (left, right) => left.CompareTo(right, nodeComparison));

				var hashCode = new HashCode();
				foreach (var node in nodesSorted)
				{
					hashCode.Add(node.GetHashCode(nodeComparison));
				}
				return hashCode.ToHashCode();
			}
			case ChainPatternComparison.Directed:
			{
				var result = new HashCode();
				foreach (var element in span)
				{
					result.Add(element.GetHashCode(nodeComparison));
				}
				return result.ToHashCode();
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(patternComparison));
			}
		}
	}

	/// <summary>
	/// Determine which <see cref="Chain"/> instance is greater.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <returns>An <see cref="int"/> result.</returns>
	/// <remarks>
	/// Order rule:
	/// <list type="number">
	/// <item>If <paramref name="other"/> is <see langword="null"/>, <see langword="this"/> is greater, return 1.</item>
	/// <item>
	/// If <paramref name="other"/> is not <see langword="null"/>, checks on length:
	/// <list type="number">
	/// <item>
	/// If length is not same, return 1 when <see langword="this"/> is longer
	/// or -1 when <paramref name="other"/> is longer.
	/// </item>
	/// <item>
	/// Determines the chain nodes used one by one. If a node is greater, the chain will be greater;
	/// otherwise, they are same, 0 will be returned.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Chain? other) => CompareTo(other, NodeComparison.IncludeIsOn);

	/// <inheritdoc cref="CompareTo(Chain?)"/>
	public int CompareTo(Chain? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return 1;
		}

		if (Length.CompareTo(other.Length) is var lengthResult and not 0)
		{
			return lengthResult;
		}

		for (var i = 0; i < Length; i++)
		{
			var (left, right) = (this[i], other[i]);
			if (left.CompareTo(right, nodeComparison) is var nodeResult and not 0)
			{
				return nodeResult;
			}
		}
		return 0;
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

#if false
	/// <inheritdoc/>
	bool IEquatable<IChainPattern>.Equals(IChainPattern? other) => other is Chain comparer && Equals(comparer);
#endif

	/// <inheritdoc/>
	bool IChainPattern.Equals(IChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
		=> other is Chain comparer && Equals(comparer, nodeComparison, patternComparison);

#if false
	/// <inheritdoc/>
	int IComparable<IChainPattern>.CompareTo(IChainPattern? other) => CompareTo(other as Chain);
#endif

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
