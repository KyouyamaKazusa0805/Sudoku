namespace Sudoku.Concepts;

/// <summary>
/// Represents a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.EqualityOperators)]
public sealed partial class Loop : IChainPattern, IElementAtMethod<Loop, Node>, IEquatable<Loop>, ISliceMethod<Loop, Node>
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
	public bool IsGrouped => Array.Exists(_nodes, static node => node.IsGroupedNode);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Loop? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected);

	/// <summary>
	/// Determine whether two <see cref="Loop"/> instances are same, by using the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The comparison rule on nodes.</param>
	/// <param name="chainComparison">The comparison rule on the whole chain.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="chainComparison"/> is not defined.
	/// </exception>
	public bool Equals([NotNullWhen(true)] Loop? other, NodeComparison nodeComparison, ChainPatternComparison chainComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Length != other.Length)
		{
			return false;
		}

		switch (chainComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				if (_nodes[0].Equals(other._nodes[0], nodeComparison))
				{
					for (var i = 0; i < Length; i++)
					{
						if (!_nodes[i].Equals(other._nodes[i], nodeComparison))
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
						if (!_nodes[i].Equals(other._nodes[j], nodeComparison))
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
					if (!_nodes[i].Equals(other._nodes[i], nodeComparison))
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

	/// <summary>
	/// Creates a hash code based on the current instance.
	/// </summary>
	/// <param name="nodeComparison">The node comparison.</param>
	/// <param name="patternComparison">The pattern comparison.</param>
	/// <returns>An <see cref="int"/> as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public int GetHashCode(NodeComparison nodeComparison, ChainPatternComparison patternComparison)
	{
		var result = new HashCode();
		switch (patternComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				for (var i = 0; i < Length; i++)
				{
					result.Add(_nodes[i].GetHashCode(nodeComparison));
				}
				for (var i = Length - 1; i >= 0; i--)
				{
					result.Add(_nodes[i].GetHashCode(nodeComparison));
				}
				break;
			}
			case ChainPatternComparison.Directed:
			{
				foreach (var element in _nodes)
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
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (0, 0); i < _nodes.Length; linkIndex++, i++)
		{
			var inference = IChainPattern.Inferences[linkIndex & 1];
			sb.Append(_nodes[i].ToString(format, formatProvider));
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
	bool IEquatable<IChainPattern>.Equals(IChainPattern? other) => other is Loop comparer && Equals(comparer);

	/// <inheritdoc/>
	bool IChainPattern.Equals(IChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
		=> other is Loop comparer && Equals(comparer, nodeComparison, patternComparison);

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
