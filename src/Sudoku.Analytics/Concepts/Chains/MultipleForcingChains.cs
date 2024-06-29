namespace Sudoku.Concepts;

/// <summary>
/// Represents an instance that describes for cell forcing chains or region forcing chains (i.e. house forcing chains).
/// </summary>
/// <param name="conclusion">Indicates the final conclusion.</param>
/// <remarks>
/// <para>
/// By the way, this type is directly derived from <see cref="SortedDictionary{TKey, TValue}"/>
/// of key and value types <see cref="Candidate"/> and <see cref="StrongForcingChain"/> or <see cref="WeakForcingChain"/>
/// respectively.
/// </para>
/// <para>
/// For keys, <see cref="Candidate"/> describes for a candidate used as the start of a branch.
/// However, in fact this type can be replaced with other ones, in order to achieve the implementation goals
/// on generalized forcing chains logic.
/// </para>
/// <para>
/// For example, a UR forcing chains can be made key types to be <see cref="Node"/>
/// instead of <see cref="Candidate"/>.
/// </para>
/// <para>
/// Also, please note that the concept "Forcing Chains" is always represented as a plural noun,
/// i.e. the concept represents a list of chains, rather than "Forcing Chain".
/// </para>
/// </remarks>
/// <seealso cref="SortedDictionary{TKey, TValue}"/>
/// <seealso cref="Candidate"/>
/// <seealso cref="StrongForcingChain"/>
/// <seealso cref="WeakForcingChain"/>
/// <seealso cref="Node"/>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class MultipleForcingChains([PrimaryConstructorParameter] Conclusion conclusion) :
	SortedDictionary<Candidate, ChainOrLoop>,
	IAnyAllMethod<MultipleForcingChains, KeyValuePair<Candidate, ChainOrLoop>>,
	IComparable<MultipleForcingChains>,
	IComparisonOperators<MultipleForcingChains, MultipleForcingChains, bool>,
	IEquatable<MultipleForcingChains>,
	IEqualityOperators<MultipleForcingChains, MultipleForcingChains, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates whether the pattern is aimed to a cell, producing multiple branches.
	/// </summary>
	/// <remarks>
	/// This value is conflict with <see cref="IsHouseMultiple"/>. If this property is <see langword="true"/>,
	/// the property <see cref="IsHouseMultiple"/> will always return <see langword="false"/> and vice versa.
	/// </remarks>
	/// <seealso cref="IsHouseMultiple"/>
	public bool IsCellMultiple => Candidates.Cells.Count == 1;

	/// <summary>
	/// Indicates whether the pattern is aimed to a house, producing multiple branches.
	/// </summary>
	/// <remarks>
	/// This value is conflict with <see cref="IsCellMultiple"/>. If this property is <see langword="true"/>,
	/// the property <see cref="IsCellMultiple"/> will always return <see langword="false"/> and vice versa.
	/// </remarks>
	/// <seealso cref="IsCellMultiple"/>
	public bool IsHouseMultiple => IsPow2(Candidates.Digits);

	/// <summary>
	/// Indicates the complexity of the whole pattern.
	/// </summary>
	public int Complexity => BranchedComplexity.Sum();

	/// <summary>
	/// Indicates the complexity of each branch.
	/// </summary>
	public ReadOnlySpan<int> BranchedComplexity => (from kvp in Values select kvp.Length).ToArray();

	/// <summary>
	/// Returns a <see cref="CandidateMap"/> indicating all candidates used in this pattern, as the start.
	/// </summary>
	public CandidateMap Candidates => [.. Keys];


	/// <summary>
	/// Determines whether the collection contains at least one element satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Exists(Func<ChainOrLoop, bool> predicate)
	{
		foreach (var element in Values)
		{
			if (predicate(element))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether all elements in this collection satisfy the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool TrueForAll(Func<ChainOrLoop, bool> predicate)
	{
		foreach (var element in Values)
		{
			if (!predicate(element))
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(MultipleForcingChains? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <summary>
	/// Compares the value with the other one, to get which one is greater.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <returns>An <see cref="int"/> value indicating which instance is better.</returns>
	public int CompareTo(MultipleForcingChains? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return -1;
		}

		if (Count.CompareTo(other.Count) is var r1 and not 0)
		{
			return r1;
		}

		var (map1, map2) = (Candidates, other.Candidates);
		if (map1.CompareTo(in map2) is var r2 and not 0)
		{
			return r2;
		}

		foreach (var candidate in map1)
		{
			switch (this[candidate], other[candidate])
			{
				case (StrongForcingChain c, StrongForcingChain d) when c.CompareTo(d, nodeComparison) is var r3 and not 0:
				{
					return r3;
				}
				case (WeakForcingChain c, WeakForcingChain d) when c.CompareTo(d, nodeComparison) is var r3 and not 0:
				{
					return r3;
				}
			}
		}
		return 0;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] MultipleForcingChains? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <summary>
	/// Determines whether two <see cref="MultipleForcingChains"/> are considered equal.
	/// </summary>
	/// <param name="other">The other instance to be checked.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <param name="patternComparison">The chain comparison rule.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Equals([NotNullWhen(true)] MultipleForcingChains? other, NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Count != other.Count)
		{
			return false;
		}

		var (map1, map2) = (Candidates, other.Candidates);
		if (map1 != map2)
		{
			return false;
		}

		foreach (var candidate in map1)
		{
			var (chain1, chain2) = (this[candidate], other[candidate]);
			if (!chain1.Equals(chain2, nodeComparison, patternComparison))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Try to cast the current instance into a finned chain pattern.
	/// </summary>
	/// <param name="result">The chain pattern result.</param>
	/// <param name="fins">The fins of the chain.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the current instance can convert into a valid finned chain pattern.
	/// </returns>
	public bool TryCastToFinnedChain([NotNullWhen(true)] out ChainOrLoop? result, [NotNullWhen(true)] out CandidateMap? fins)
	{
		// Determine whether the pattern is an elimination.
		if (Conclusion.ConclusionType == Assignment)
		{
			goto ReturnFalse;
		}

		// Iterate on each branch, to get whether they can directly points to conclusion.
		var finsFound = CandidateMap.Empty;
		var krakenBranches = new List<ChainOrLoop>(2);
		foreach (var branch in Values)
		{
			if (branch is [.. { Length: 1 }, { Map: { PeerIntersection: var p } lastMap }] && p.Contains(Conclusion.Candidate))
			{
				finsFound |= lastMap;
				continue;
			}

			if (krakenBranches.Count >= 2)
			{
				goto ReturnFalse;
			}

			krakenBranches.Add(branch);
		}

		// Only if the number of kraken branches is exact 2.
		if (krakenBranches is not [[_, .. var firstKraken], [.. var secondKraken]])
		{
			goto ReturnFalse;
		}

		// If so, a finned chain is found. Now we should merge two branches into one, by rotating one of two branches,
		// and concat two branches.
		(result, fins) = (c([.. firstKraken.Reverse(), .. secondKraken], Conclusion), finsFound);
		return true;

	ReturnFalse:
		(result, fins) = (null, null);
		return false;


		static Chain c(ReadOnlySpan<Node> nodes, Conclusion conclusion)
		{
			// Find the node at the specified position in nodes.
			var i = 0;
			for (; i < nodes.Length; i++)
			{
				if (nodes[i].Map is [var c] && c == conclusion.Candidate)
				{
					break;
				}
			}
			if (i == nodes.Length)
			{
				throw new InvalidOperationException();
			}

			var isOn = conclusion.ConclusionType == Elimination;
			var currentNode = new Node(in nodes[i].Map, isOn, nodes[i].IsAdvanced);
			(var lastNode, i, isOn) = (currentNode, (i + 1) % nodes.Length, !isOn);
			for (var x = 0; x < nodes.Length; i = (i + 1) % nodes.Length, x++)
			{
				currentNode.Parent = new Node(in nodes[i].Map, isOn, nodes[i].IsAdvanced);
				currentNode = currentNode.Parent;
				isOn = !isOn;
			}
			return new(lastNode);
		}
	}

	/// <inheritdoc/>
	public override int GetHashCode() => GetHashCode(NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <summary>
	/// Calculates a hash code value used for comparison.
	/// </summary>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <param name="patternComparison">The chain comparison rule.</param>
	/// <returns>Hash code value.</returns>
	public int GetHashCode(NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
	{
		var hashCode = new HashCode();
		foreach (var (candidate, chain) in this)
		{
			hashCode.Add(candidate);
			hashCode.Add(chain.GetHashCode(nodeComparison, patternComparison));
		}
		return hashCode.ToHashCode();
	}

	/// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(", ", formatProvider);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return string.Join(
			", ",
			from kvp in this
			let candidate = kvp.Key
			let pattern = kvp.Value
			select $"{converter.CandidateConverter(candidate)}: {pattern.ToString(format, converter)}"
		);
	}

	/// <summary>
	/// Cast the current instance into a valid finned chain, or throws <see cref="InvalidOperationException"/>
	/// if the current instance cannot be converted.
	/// </summary>
	/// <param name="fins">The fins converted.</param>
	/// <returns>The pattern converted.</returns>
	/// <exception cref="InvalidOperationException">Throws when the current instance cannot be converted.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainOrLoop CastToFinnedChain(out CandidateMap fins)
	{
		if (TryCastToFinnedChain(out var result, out var f))
		{
			fins = f.Value;
			return result;
		}

		throw new InvalidOperationException(SR.ExceptionMessage("CannotCastFinnedChain"));
	}

	/// <summary>
	/// Collect views for the current chain.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="conclusion">The conclusion.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The views.</returns>
	public View[] GetViews(ref readonly Grid grid, Conclusion conclusion, ReadOnlySpan<ChainingRule> supportedRules)
	{
		var viewNodes = v(in grid, supportedRules);
		var result = new View[viewNodes.Length];
		for (var i = 0; i < viewNodes.Length; i++)
		{
			result[i] = [
				..
				from node in viewNodes[i]
				where node is not CandidateViewNode { Candidate: var c } || c != conclusion.Candidate
				select node
			];
		}
		foreach (var supportedRule in supportedRules)
		{
			supportedRule.MapViewNodes(in grid, this, result);
		}
		return result;


		ReadOnlySpan<ViewNode[]> v(ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules)
		{
			var result = new ViewNode[Count + 1][];
			ViewNode houseOrCellNode = IsCellMultiple
				? new CellViewNode(ColorIdentifier.Normal, this.First().Key / 9)
				: new HouseViewNode(ColorIdentifier.Normal, TrailingZeroCount(Candidates.Cells.SharedHouses));

			var i = 0;
			var globalView = new List<ViewNode>();
			foreach (var key in Keys)
			{
				var chain = this[key];
				var subview = new View();
				var j = 0;
				foreach (var node in chain)
				{
					var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
					foreach (var candidate in node.Map)
					{
						var currentViewNode = new CandidateViewNode(id, candidate);
						globalView.Add(currentViewNode);
						subview.Add(currentViewNode);
					}
				}

				j = 0;
				foreach (var link in chain.Links)
				{
					var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
					var currentViewNode = new ChainLinkViewNode(id, link.FirstNode.Map, link.SecondNode.Map, link.IsStrong);
					globalView.Add(currentViewNode);
					subview.Add(currentViewNode);
				}
				result[++i] = [houseOrCellNode, .. subview];
			}
			result[0] = [houseOrCellNode, .. globalView];
			return result;
		}
	}

	/// <inheritdoc/>
	bool IAnyAllMethod<MultipleForcingChains, KeyValuePair<Candidate, ChainOrLoop>>.Any() => Count != 0;

	/// <inheritdoc/>
	bool IAnyAllMethod<MultipleForcingChains, KeyValuePair<Candidate, ChainOrLoop>>.Any(Func<KeyValuePair<int, ChainOrLoop>, bool> predicate)
	{
		foreach (var kvp in this)
		{
			if (predicate(kvp))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	bool IAnyAllMethod<MultipleForcingChains, KeyValuePair<Candidate, ChainOrLoop>>.All(Func<KeyValuePair<int, ChainOrLoop>, bool> predicate)
	{
		foreach (var kvp in this)
		{
			if (!predicate(kvp))
			{
				return false;
			}
		}
		return true;
	}
}
