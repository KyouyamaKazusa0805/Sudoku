namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain or a loop.
/// </summary>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class Chain(Node lastNode) : ChainPattern(lastNode, false)
{
	/// <summary>
	/// Indicates whether the chain starts with weak link.
	/// </summary>
	private readonly bool _weakStart = lastNode.IsOn;


	/// <inheritdoc/>
	public override bool IsGrouped => Span.Any(static (ref readonly Node node) => node.IsGroupedNode);

	/// <inheritdoc/>
	public override bool SatisfyXRule
	{
		get
		{
			var digitsMask = (Mask)0;
			foreach (var node in Span)
			{
				digitsMask |= node.Map.Digits;
			}
			return IsPow2(digitsMask);
		}
	}

	/// <summary>
	/// Indicates whether the chain is formed a W-Wing.
	/// </summary>
	/// <remarks>
	/// A valid pattern of W-Wing is <c><![CDATA[(x=y)-y=y-(y=x)]]></c>, symmetric.
	/// </remarks>
	public bool IsWoodsWing
		=> SplitMask is (true, var m1, var m2, var m3, var m4, var m5, var m6)
		&& m2 == m3 && m2 == m4 && m2 == m5 && m1 == m6 && m1 != m2;

	/// <summary>
	/// Indicates whether the chain is formed a M-Wing.
	/// </summary>
	/// <remarks>
	/// A valid pattern of M-Wing is <c><![CDATA[(x=y)-y=(y-x)=x]]></c>, asymmetric.
	/// </remarks>
	public bool IsMedusaWing
		=> SplitMask is (true, var m1, var m2, var m3, var m4, var m5, var m6) && (
			m1 == m5 && m1 == m6 && m2 == m3 && m2 == m4 && m1 != m2
			|| m1 == m2 && m1 == m6 && m3 == m4 && m3 == m5 && m2 != m3
		);

	/// <summary>
	/// Indicates whether the chain is formed a S-Wing.
	/// </summary>
	/// <remarks>
	/// A valid pattern of S-Wing is <c><![CDATA[x=x-(x=y)-y=y]]></c>, symmetric.
	/// </remarks>
	public bool IsSplitWing
		=> SplitMask is (true, var m1, var m2, var m3, var m4, var m5, var m6)
		&& m1 == m2 && m1 == m3 && m4 == m5 && m4 == m6 && m1 != m4;

	/// <summary>
	/// Indicates whether the chain is formed a L-Wing.
	/// </summary>
	/// <remarks>
	/// A valid pattern of L-Wing is <c><![CDATA[x=(x-y)=(y-z)=z]]></c>, asymmetric.
	/// </remarks>
	public bool IsLocalWing
		=> SplitMask is (true, var m1, var m2, var m3, var m4, var m5, var m6)
		&& m1 == m2 && m3 == m4 && m5 == m6 && m1 != m3 && m1 != m5 && m1 != m5;

	/// <summary>
	/// Indicates whether the chain is formed a H-Wing.
	/// </summary>
	/// <remarks>
	/// A valid pattern of H-Wing is <c><![CDATA[(x=y)-(y=z)-z=z]]></c>, asymmetric.
	/// </remarks>
	public bool IsHybridWing
		=> SplitMask is (true, var m1, var m2, var m3, var m4, var m5, var m6) && (
			m2 == m3 && m4 == m5 && m4 == m6 && m1 != m2 && m2 != m4 && m2 != m4
			|| m1 == m2 && m1 == m3 && m4 == m5 && m1 != m4 && m1 != m6 && m4 != m6
		);

	/// <inheritdoc/>
	public override bool ContainsOverlappedNodes
	{
		get
		{
			foreach (var nodePair in (from node in Span select node.Map).GetSubsets(2))
			{
				ref readonly var map1 = ref nodePair[0];
				ref readonly var map2 = ref nodePair[1];
				if (map1 & map2)
				{
					return true;
				}
			}
			return false;
		}
	}

	/// <inheritdoc/>
	public override int Length => _weakStart ? _nodes.Length - 2 : _nodes.Length;

	/// <inheritdoc/>
	public override int Complexity => _nodes.Length;

	/// <inheritdoc/>
	public override ReadOnlySpan<Link> Links
	{
		get
		{
			var span = Span;
			var result = new Link[Length - 1];
			for (var (linkIndex, i) = (0, 0); i < Length - 1; linkIndex++, i++)
			{
				result[i] = new(span[i], span[i + 1], LinkType.Unknown, Inferences[linkIndex & 1]);
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public override Node First => Span[0];

	/// <inheritdoc/>
	public override Node Last => Span[^1];

	/// <summary>
	/// Split mask for 6 nodes.
	/// </summary>
	private (bool, Mask, Mask, Mask, Mask, Mask, Mask)? SplitMask
#pragma warning disable format
		=> this switch
		{
			[
				{ Map.Digits: var m1 },
				{ Map.Digits: var m2 },
				{ Map.Digits: var m3 },
				{ Map.Digits: var m4 },
				{ Map.Digits: var m5 },
				{ Map.Digits: var m6 }
			] => IsPow2(m1) && IsPow2(m2) && IsPow2(m3) && IsPow2(m4) && IsPow2(m5) && IsPow2(m6)
				? (true, m1, m2, m3, m4, m5, m6)
				: (false, m1, m2, m3, m4, m5, m6),
			_ => null
		};
#pragma warning restore format

	/// <summary>
	/// Create a <see cref="ReadOnlySpan{T}"/> instance that holds valid <see cref="Node"/> instances to be used in a chain.
	/// </summary>
	private ReadOnlySpan<Node> Span => _nodes.AsReadOnlySpan()[_weakStart ? 1..^1 : ..];


	/// <inheritdoc/>
	public override Node this[int index] => Span[index];


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
	public bool Equals([NotNullWhen(true)] Chain? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected);

	/// <inheritdoc cref="ChainPattern.Equals(ChainPattern?, NodeComparison, ChainPatternComparison)"/>
	public bool Equals([NotNullWhen(true)] Chain? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
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
		switch (patternComparison)
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
				throw new ArgumentOutOfRangeException(nameof(patternComparison));
			}
		}
	}

	/// <inheritdoc/>
	public override bool Equals(ChainPattern? other) => Equals(other as Chain);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ChainPattern? other, NodeComparison nodeComparison, ChainPatternComparison patternComparison)
		=> Equals(other as Chain, nodeComparison, patternComparison);

	/// <inheritdoc/>
	public override int GetHashCode(NodeComparison nodeComparison, ChainPatternComparison patternComparison)
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
	public int CompareTo(Chain? other) => CompareTo(other, NodeComparison.IncludeIsOn);

	/// <inheritdoc/>
	public override int CompareTo(ChainPattern? other) => CompareTo(other as Chain);

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

	/// <inheritdoc/>
	public override string ToString(string? format, IFormatProvider? formatProvider)
	{
		var span = Span;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (0, 0); i < span.Length; linkIndex++, i++)
		{
			var inference = Inferences[linkIndex & 1];
			sb.Append(span[i].ToString(format, formatProvider));
			if (i != span.Length - 1)
			{
				sb.Append(inference.ConnectingNotation());
			}
		}
		return sb.ToString();
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<Node> Slice(int start, int length) => Span[start..(start + length)];

	/// <inheritdoc/>
	public override ConclusionSet GetConclusions(ref readonly Grid grid) => [.. GetConclusions(in grid, First, Last)];
}
