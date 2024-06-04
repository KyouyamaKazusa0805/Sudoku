namespace Sudoku.Concepts;

/// <summary>
/// Represents a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class Loop(Node lastNode) : ChainPattern(lastNode, true)
{
	/// <inheritdoc/>
	public override bool IsGrouped => Array.Exists(_nodes, static node => node.IsGroupedNode);

	/// <inheritdoc/>
	public override int Length => _nodes.Length;

	/// <inheritdoc/>
	public override int Complexity => _nodes.Length;

	/// <inheritdoc/>
	public override Node First => _nodes[0];

	/// <inheritdoc/>
	public override Node Last => _nodes[^1];


	/// <inheritdoc/>
	public override Node this[int index] => _nodes[index];


	/// <inheritdoc/>
	public override void Reverse()
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
	public override bool Equals(ChainPattern? other) => Equals(other as Loop);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
		=> Equals(other as Loop, nodeComparison, patternComparison);

	/// <summary>
	/// Creates a hash code based on the current instance.
	/// </summary>
	/// <param name="nodeComparison">The node comparison.</param>
	/// <param name="patternComparison">The pattern comparison.</param>
	/// <returns>An <see cref="int"/> as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public override int GetHashCode(NodeComparison nodeComparison, ChainPatternComparison patternComparison)
	{
		switch (patternComparison)
		{
			case ChainPatternComparison.Undirected:
			{
				// To guarantee the final hash code is same on different direction, we should sort all nodes,
				// in order to make same nodes are in the same position.
				var nodesSorted = _nodes[..];
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
				foreach (var element in _nodes)
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
	/// Determine which <see cref="Loop"/> instance is greater.
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
	/// Determines the loop nodes used one by one. If a node is greater, the chain will be greater;
	/// otherwise, they are same, 0 will be returned.
	/// <b>
	/// This operation will adjust the checking node index on the other loop <paramref name="other"/>.
	/// Two loops with same nodes will be considered as equal no matter what order they will be.
	/// For example, <c><![CDATA[A == B -- C == D -- A]]></c> is equal to <c><![CDATA[C == D -- A == B -- C]]></c>.
	/// </b>
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Loop? other) => CompareTo(other, NodeComparison.IncludeIsOn);

	/// <inheritdoc cref="CompareTo(Loop?)"/>
	public int CompareTo(Loop? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return 1;
		}

		if (Length.CompareTo(other.Length) is var lengthResult and not 0)
		{
			return lengthResult;
		}

		// Find two loops with the same node as the start.
		// If found, we should align them as start nodes to iterate; otherwise, they are not same chain.
		// Just check elements one by one.
		switch (other.FindIndex(node => node.Equals(this[0], nodeComparison)))
		{
			case -1:
			{
				for (var i = 0; i < Length; i++)
				{
					if (this[i].CompareTo(other[i], nodeComparison) is var nodeResult and not 0)
					{
						return nodeResult;
					}
				}
				goto default;
			}
			case var secondNodeStartIndex:
			{
				for (var (i, pos) = (0, secondNodeStartIndex); i < Length; i++, pos = (pos + 1) % Length)
				{
					if (this[i].CompareTo(other[pos], nodeComparison) is var nodeResult and not 0)
					{
						return nodeResult;
					}
				}
				goto default;
			}
			default:
			{
				return 0;
			}
		}
	}

	/// <inheritdoc/>
	public override int CompareTo(ChainPattern? other) => CompareTo(other as Loop);

	/// <inheritdoc/>
	public override string ToString(string? format, IFormatProvider? formatProvider)
	{
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (1, 0); i < _nodes.Length; linkIndex++, i++)
		{
			var inference = Inferences[linkIndex & 1];
			sb.Append(_nodes[i].ToString(format, formatProvider));
			sb.Append(inference.ConnectingNotation());
		}
		sb.Append(_nodes[0].ToString(formatProvider));
		return sb.ToString();
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<Node> Slice(int start, int length) => _nodes.AsReadOnlySpan()[start..(start + length)];

	/// <inheritdoc/>
	public override ConclusionSet GetConclusions(ref readonly Grid grid)
	{
		var result = new ConclusionSet();
		for (var i = 0; i < Length; i += 2)
		{
			var node1 = _nodes[i];
			var node2 = _nodes[i == Length ? 0 : i + 1];
			foreach (var conclusion in GetConclusions(in grid, node1, node2))
			{
				result.Add(conclusion);
			}
		}
		return result;
	}
}
