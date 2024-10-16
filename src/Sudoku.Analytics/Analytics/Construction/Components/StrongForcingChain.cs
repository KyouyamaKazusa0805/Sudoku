namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a chain segment (forcing chain) starting with strong link.
/// </summary>
[TypeImpl(TypeImplFlags.Object_ToString, EmitThisCastToInterface = true)]
public sealed partial class StrongForcingChain(Node lastNode) : UnnamedChain(lastNode)
{
	/// <inheritdoc/>
	protected internal override int WeakStartIdentity => 0;


	/// <summary>
	/// Determine which <see cref="StrongForcingChain"/> instance is greater.
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
	/// Determine whether one of two has "self constraint" (i.e. false -> true confliction).
	/// <list type="number">
	/// <item>If so, it will be treated as "less than" the other one.</item>
	/// <item>
	/// Otherwise, determine the chain nodes used one by one. If a node is greater, the chain will be greater;
	/// otherwise, they are same, 0 will be returned.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(StrongForcingChain? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <inheritdoc/>
	public override int CompareTo(Chain? other) => CompareTo(other as StrongForcingChain);

	/// <summary>
	/// Compares the value with the other one, to get which one is greater.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <returns>An <see cref="int"/> value indicating which instance is better.</returns>
	public int CompareTo(StrongForcingChain? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return -1;
		}

		if (Length.CompareTo(other.Length) is var lengthResult and not 0)
		{
			return lengthResult;
		}

		var thisHasSelfConstraint = First == ~Last;
		var otherHasSelfConstraint = other.First == ~other.Last;
		if (thisHasSelfConstraint ^ otherHasSelfConstraint)
		{
			// If a chain has a self constraint (false -> true contradiction), it should be treated as "less than" the other.
			return thisHasSelfConstraint ? -1 : 1;
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

	/// <inheritdoc cref="Chain.Equals(Chain?, NodeComparison, ChainComparison)"/>
	public bool Equals([NotNullWhen(true)] StrongForcingChain? other, NodeComparison nodeComparison, ChainComparison patternComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Length != other.Length)
		{
			return false;
		}

		var span1 = ValidNodes;
		var span2 = other.ValidNodes;
		switch (patternComparison)
		{
			case ChainComparison.Undirected:
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
			case ChainComparison.Directed:
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
				throw new ArgumentOutOfRangeException(nameof(patternComparison));
			}
		}
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Chain? other, NodeComparison nodeComparison, ChainComparison patternComparison)
		=> Equals(other as StrongForcingChain, nodeComparison, patternComparison);

	/// <inheritdoc/>
	public override int GetHashCode(NodeComparison nodeComparison, ChainComparison patternComparison)
	{
		var span = ValidNodes;
		switch (patternComparison)
		{
			case ChainComparison.Undirected:
			{
				// To guarantee the final hash code is same on different direction, we should sort all nodes,
				// in order to make same nodes are in the same position.
				var nodesSorted = span.ToArray();
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
}
