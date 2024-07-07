namespace Sudoku.Concepts;

/// <summary>
/// Represents a blossom loop.
/// </summary>
/// <param name="conclusions">Indicates the conclusions used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class BlossomLoop([PrimaryConstructorParameter] params Conclusion[] conclusions) :
	SortedDictionary<Candidate, WeakForcingChain>,
	IComparable<BlossomLoop>,
	IComparisonOperators<BlossomLoop, BlossomLoop, bool>,
	IEquatable<BlossomLoop>,
	IEqualityOperators<BlossomLoop, BlossomLoop, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates whether the loop is cell type.
	/// </summary>
	public bool IsCellType => !IsPow2(Entries.Digits);

	/// <summary>
	/// Indicates the complexity of the whole pattern.
	/// </summary>
	public int Complexity => BranchedComplexity.Sum();

	/// <summary>
	/// Indicates the entry candidates that start each branch.
	/// </summary>
	public CandidateMap Entries => [.. Keys];

	/// <summary>
	/// Indicates the complexity of each branch.
	/// </summary>
	public ReadOnlySpan<int> BranchedComplexity => (from chain in Values select chain.Length).ToArray();


	/// <summary>
	/// Determines whether at least one branch contains at least one element satisfying the specified condition.
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
			var (kvp1, kvp2) = (e1.Current, e2.Current);
			if (kvp1 != kvp2)
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
			if (e1.Current.CompareTo(e2.Current) is var r2 and not 0)
			{
				return r2;
			}
		}

		foreach (var key in Keys)
		{
			switch (this[key], other[key])
			{
				case var (c, d) when c.CompareTo(d, nodeComparison) is var r3 and not 0:
				{
					return r3;
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
		foreach (var (branchStartNode, forcingChain) in this)
		{
			hashCode.Add(branchStartNode);
			hashCode.Add(forcingChain.GetHashCode(nodeComparison, patternComparison));
		}
		return hashCode.ToHashCode();
	}

	/// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return string.Join(
			", ",
			from kvp in this
			select $"{converter.CandidateConverter(kvp.Key)}: {kvp.Value.ToString(format, converter)}"
		);
	}
}
