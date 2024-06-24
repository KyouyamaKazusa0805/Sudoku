namespace Sudoku.Concepts;

/// <summary>
/// Represents a blossom loop pattern that starts with a house or a candidate, and ends with a hosue or a candidate.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class BlossomLoop :
	SortedDictionary<BlossomLoopEntry, ChainOrLoop>,
	IAnyAllMethod<BlossomLoop, KeyValuePair<BlossomLoopEntry, ChainOrLoop>>,
	IComparable<BlossomLoop>,
	IComparisonOperators<BlossomLoop, BlossomLoop, bool>,
	IEquatable<BlossomLoop>,
	IEqualityOperators<BlossomLoop, BlossomLoop, bool>,
	IFormattable
{
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
	public CandidateMap StartCandidates => [.. from pair in Keys select pair.Start];

	/// <summary>
	/// Returns a <see cref="CandidateMap"/> indicating all candidates used in this pattern, as the end.
	/// </summary>
	public CandidateMap EndCandidates => [.. from pair in Keys select pair.End];


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
	public bool Equals([NotNullWhen(true)] BlossomLoop? other)
		=> Equals(other, NodeComparison.IgnoreIsOn, ChainOrLoopComparison.Undirected);

	/// <summary>
	/// Determines whether two <see cref="BlossomLoop"/> are considered equal.
	/// </summary>
	/// <param name="other">The other instance to be checked.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <param name="patternComparison">The chain comparison rule.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Equals([NotNullWhen(true)] BlossomLoop? other, NodeComparison nodeComparison, ChainOrLoopComparison patternComparison)
	{
		if (other is null)
		{
			return false;
		}

		if (Count != other.Count)
		{
			return false;
		}

		using var e1 = Keys.GetEnumerator();
		using var e2 = other.Keys.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			var kvp1 = e1.Current;
			var kvp2 = e2.Current;
			var ((a1, _, b1, _), (a2, _, b2, _)) = (kvp1, kvp2);
			if (a1 != a2 || b1 != b2)
			{
				return false;
			}

			var (chain1, chain2) = (this[kvp1], other[kvp2]);
			if (!chain1.Equals(chain2, nodeComparison, patternComparison))
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(BlossomLoop? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <summary>
	/// Compares the value with the other one, to get which one is greater.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="nodeComparison">The node comparison rule.</param>
	/// <returns>An <see cref="int"/> value indicating which instance is better.</returns>
	public int CompareTo(BlossomLoop? other, NodeComparison nodeComparison)
	{
		if (other is null)
		{
			return -1;
		}

		if (Count.CompareTo(other.Count) is var r1 and not 0)
		{
			return r1;
		}

		using var e1 = Keys.GetEnumerator();
		using var e2 = other.Keys.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			var (a1, _, b1, _) = e1.Current;
			var (a2, _, b2, _) = e2.Current;
			if (a1.CompareTo(a2) is var r2 and not 0)
			{
				return r2;
			}
			if (b1.CompareTo(b2) is var r3 and not 0)
			{
				return r3;
			}
		}

		foreach (var key in Keys)
		{
			switch (this[key], other[key])
			{
				case (StrongForcingChain c, StrongForcingChain d) when c.CompareTo(d, nodeComparison) is var r4 and not 0:
				{
					return r4;
				}
				case (WeakForcingChain c, WeakForcingChain d) when c.CompareTo(d, nodeComparison) is var r4 and not 0:
				{
					return r4;
				}
			}
		}
		return 0;
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
		foreach (var (key, chain) in this)
		{
			hashCode.Add(key.Start);
			hashCode.Add(key.End);
			hashCode.Add(chain.GetHashCode(nodeComparison, patternComparison));
		}
		return hashCode.ToHashCode();
	}

	/// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return string.Join(
			", ",
			from kvp in this
			let start = kvp.Key.Start
			let end = kvp.Key.End
			let pattern = kvp.Value
			select $"{converter.CandidateConverter(start)} - {converter.CandidateConverter(end)}: {pattern.ToString(format, converter)}"
		);
	}

	/// <inheritdoc/>
	bool IAnyAllMethod<BlossomLoop, KeyValuePair<BlossomLoopEntry, ChainOrLoop>>.Any() => Count != 0;

	/// <inheritdoc/>
	bool IAnyAllMethod<BlossomLoop, KeyValuePair<BlossomLoopEntry, ChainOrLoop>>.Any(Func<KeyValuePair<BlossomLoopEntry, ChainOrLoop>, bool> predicate)
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
	bool IAnyAllMethod<BlossomLoop, KeyValuePair<BlossomLoopEntry, ChainOrLoop>>.All(Func<KeyValuePair<BlossomLoopEntry, ChainOrLoop>, bool> predicate)
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
