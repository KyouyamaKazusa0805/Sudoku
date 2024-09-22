namespace Sudoku.Analytics.Patterning.Chaining;

/// <summary>
/// Represents a chain.
/// </summary>
/// <param name="lastNode"><inheritdoc/></param>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class Chain(Node lastNode) : NamedChain(lastNode, false)
{
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

	/// <summary>
	/// Indicates whether the chain is an implicit loop,
	/// which means the start and end nodes are in a same house, of a same digit; or in a same cell.
	/// </summary>
	public bool IsImplicitLoop
		=> WeakStart && ValidNodes is [{ Map: [var first] }, .., { Map: [var last] }]
		&& (first / 9 == last / 9 || first % 9 == last % 9 && ((first / 9).AsCellMap() + last / 9).InOneHouse(out _));

	/// <inheritdoc/>
	public override int Complexity => _nodes.Length;

	/// <inheritdoc/>
	protected override int WeakStartIdentity => 0;

	/// <inheritdoc/>
	protected override int LoopIdentity => 1;

	/// <inheritdoc/>
	protected override ReadOnlySpan<Node> ValidNodes => _nodes.AsReadOnlySpan()[WeakStart ? 1..^1 : ..];

	/// <summary>
	/// Indicates whether the chain starts with weak link.
	/// </summary>
	private bool WeakStart => _nodes[^1].IsOn;

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
			] => Mask.IsPow2(m1) && Mask.IsPow2(m2) && Mask.IsPow2(m3) && Mask.IsPow2(m4) && Mask.IsPow2(m5) && Mask.IsPow2(m6)
				? (true, m1, m2, m3, m4, m5, m6)
				: (false, m1, m2, m3, m4, m5, m6),
			_ => null
		};
#pragma warning restore format


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
	public bool Equals([NotNullWhen(true)] Chain? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <inheritdoc cref="ChainOrLoop.Equals(ChainOrLoop?, NodeComparison, ChainOrLoopComparison)"/>
	public bool Equals([NotNullWhen(true)] Chain? other, NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
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
			case ChainOrLoopComparison.Undirected:
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
			case ChainOrLoopComparison.Directed:
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
	public override bool Equals([NotNullWhen(true)] ChainOrLoop? other, NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
		=> Equals(other as Chain, nodeComparison, patternComparison);

	/// <inheritdoc/>
	public override int GetHashCode(NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
	{
		var span = ValidNodes;
		switch (patternComparison)
		{
			case ChainOrLoopComparison.Undirected:
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
			case ChainOrLoopComparison.Directed:
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
	public int CompareTo(Chain? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <inheritdoc/>
	public override int CompareTo(ChainOrLoop? other) => CompareTo(other as Chain);

	/// <inheritdoc cref="CompareTo(Chain?)"/>
	public int CompareTo(Chain? other, NodeComparison nodeComparison)
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

	/// <inheritdoc/>
	public override ConclusionSet GetConclusions(ref readonly Grid grid) => [.. GetConclusions(in grid, First, Last)];
}
