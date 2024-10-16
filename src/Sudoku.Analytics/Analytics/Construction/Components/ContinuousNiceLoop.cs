namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a loop pattern that has no start node (all nodes can be start nodes), forming a closed loop,
/// with eliminations from all weak links.
/// </summary>
/// <param name="lastNode"><inheritdoc/></param>
[TypeImpl(TypeImplFlags.Object_ToString, EmitThisCastToInterface = true)]
public sealed partial class ContinuousNiceLoop(Node lastNode) : NamedChain(lastNode, true)
{
	/// <inheritdoc/>
	protected internal override int WeakStartIdentity => 1;

	/// <inheritdoc/>
	protected internal override ReadOnlySpan<Node> ValidNodes => _nodes;

	/// <inheritdoc/>
	protected override int LoopIdentity => 0;


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

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] ContinuousNiceLoop? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainComparison.Undirected);

	/// <summary>
	/// Determine whether two <see cref="ContinuousNiceLoop"/> instances are same, by using the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The comparison rule on nodes.</param>
	/// <param name="chainComparison">The comparison rule on the whole chain.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="chainComparison"/> is not defined.
	/// </exception>
	public bool Equals([NotNullWhen(true)] ContinuousNiceLoop? other, NodeComparison nodeComparison, ChainComparison chainComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Length != other.Length)
		{
			return false;
		}

		if (other.FindIndex(node => node.Equals(this[0], nodeComparison)) is not (var secondNodeStartIndex and not -1))
		{
			return false;
		}

		// Find two loops with the same node as the start.
		// If found, we should align them as start nodes to iterate; otherwise, they are not same chain.
		// Just check elements one by one.
		switch (chainComparison)
		{
			case ChainComparison.Undirected:
			{
				// Check the second node.
				var previousNode = other[(secondNodeStartIndex - 1 + Length) % Length];
				var nextNode = other[(secondNodeStartIndex + 1) % Length];
				if (this[1].Equals(previousNode, nodeComparison))
				{
					for (var (i, pos) = (0, secondNodeStartIndex); i < Length; i++, pos = (pos - 1 + Length) % Length)
					{
						if (!this[i].Equals(other[pos], nodeComparison))
						{
							return false;
						}
					}
					return true;
				}
				else if (this[1].Equals(nextNode, nodeComparison))
				{
					for (var (i, pos) = (0, secondNodeStartIndex); i < Length; i++, pos = (pos + 1) % Length)
					{
						if (!this[i].Equals(other[pos], nodeComparison))
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					// Both directions won't encounter the same-value node.
					return false;
				}
			}
			case ChainComparison.Directed:
			{
				for (var (i, pos) = (0, secondNodeStartIndex); i < Length; i++, pos = (pos + 1) % Length)
				{
					if (!this[i].Equals(other[pos], nodeComparison))
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
	public override bool Equals([NotNullWhen(true)] Chain? other, NodeComparison nodeComparison, ChainComparison patternComparison)
		=> Equals(other as ContinuousNiceLoop, nodeComparison, patternComparison);

	/// <summary>
	/// Creates a hash code based on the current instance.
	/// </summary>
	/// <param name="nodeComparison">The node comparison.</param>
	/// <param name="patternComparison">The pattern comparison.</param>
	/// <returns>An <see cref="int"/> as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="patternComparison"/> is not defined.
	/// </exception>
	public override int GetHashCode(NodeComparison nodeComparison, ChainComparison patternComparison)
	{
		switch (patternComparison)
		{
			case ChainComparison.Undirected:
			{
				// To guarantee the final hash code is same on different direction, we should sort all nodes,
				// in order to make same nodes are in the same position.
				var nodesSorted = _nodes[..];
				Array.Sort(nodesSorted, (left, right) => left.CompareTo(right, nodeComparison));

				var hashCode = default(HashCode);
				foreach (var node in nodesSorted)
				{
					hashCode.Add(node.GetHashCode(nodeComparison));
				}
				return hashCode.ToHashCode();
			}
			case ChainComparison.Directed:
			{
				var result = default(HashCode);
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
	/// Determine which <see cref="ContinuousNiceLoop"/> instance is greater.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <returns>An <see cref="int"/> result.</returns>
	/// <remarks>
	/// Order rule:
	/// <list type="number">
	/// <item>If <paramref name="other"/> is <see langword="null"/>, <see langword="this"/> is greater, return -1.</item>
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
	public int CompareTo(ContinuousNiceLoop? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <inheritdoc cref="CompareTo(ContinuousNiceLoop?)"/>
	public int CompareTo(ContinuousNiceLoop? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return -1;
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
	public override int CompareTo(Chain? other) => CompareTo(other as ContinuousNiceLoop);

	/// <inheritdoc/>
	public override ConclusionSet GetConclusions(ref readonly Grid grid)
	{
		var result = new ConclusionSet();
		for (var i = 0; i < Length; i += 2)
		{
			foreach (var conclusion in EliminationCalculator.Chain.GetConclusions(in grid, _nodes[i], _nodes[(i + 1) % Length]))
			{
				result.Add(conclusion);
			}
		}
		return result;
	}
}
