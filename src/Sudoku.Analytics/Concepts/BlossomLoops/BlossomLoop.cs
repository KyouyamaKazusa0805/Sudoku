namespace Sudoku.Concepts;

/// <summary>
/// Represents a blossom loop.
/// </summary>
/// <param name="burredLoop">Indicates the burred loop used.</param>
/// <param name="isCellType">Indicates whether the loop is cell type.</param>
/// <param name="krakenCellOrHouse">Indicates the kraken cell or house used.</param>
/// <param name="conclusions">Indicates the conclusions used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class BlossomLoop(
	[PrimaryConstructorParameter] Loop burredLoop,
	[PrimaryConstructorParameter] bool isCellType,
	[PrimaryConstructorParameter] int krakenCellOrHouse,
	[PrimaryConstructorParameter] params Conclusion[] conclusions
) :
	SortedDictionary<Node, WeakForcingChain>,
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
	/// Determines whether at least one branch contains at least one element satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool BranchesExist(Func<ChainOrLoop, bool> predicate)
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

		if (!BurredLoop.Equals(other.BurredLoop, nodeComparison, patternComparison))
		{
			return false;
		}

		using var e1 = Keys.GetEnumerator();
		using var e2 = other.Keys.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			var (kvp1, kvp2) = (e1.Current, e2.Current);
			var ((a1, b1), (a2, b2)) = (kvp1, kvp2);
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

		if (BurredLoop.CompareTo(other.BurredLoop, nodeComparison) is var r2 and not 0)
		{
			return r2;
		}

		using var e1 = Keys.GetEnumerator();
		using var e2 = other.Keys.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			if (e1.Current.CompareTo(e2.Current) is var r3 and not 0)
			{
				return r3;
			}
		}

		foreach (var key in Keys)
		{
			switch (this[key], other[key])
			{
				case var (c, d) when c.CompareTo(d, nodeComparison) is var r4 and not 0:
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
		foreach (var (branchStartNode, forcingChain) in this)
		{
			hashCode.Add(branchStartNode);
			hashCode.Add(BurredLoop.GetHashCode(nodeComparison, patternComparison));
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
		=> $"{ToBurredLoopString(format, formatProvider)}, {ToBranchesString(format, formatProvider)}";

	/// <inheritdoc cref="ToBranchesString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToBranchesString(IFormatProvider? formatProvider) => ToBranchesString(null, formatProvider);

	/// <summary>
	/// Formats the branches into a string.
	/// </summary>
	/// <param name="format">The format string value used by formatting a node.</param>
	/// <param name="formatProvider">The format provider.</param>
	/// <returns>A <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToBranchesString(string? format, IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return string.Join(
			", ",
			from kvp in this
			select $"{converter.CandidateConverter(kvp.Key.Map)}: {kvp.Value.ToString(format, converter)}"
		);
	}

	/// <inheritdoc cref="ToBurredLoopString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToBurredLoopString(IFormatProvider? formatProvider) => ToBurredLoopString(null, formatProvider);

	/// <summary>
	/// Formats the burred loop into a string.
	/// </summary>
	/// <param name="format">The format string value used by formatting a node.</param>
	/// <param name="formatProvider">The format provider.</param>
	/// <returns>A <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToBurredLoopString(string? format, IFormatProvider? formatProvider)
		=> BurredLoop.ToString(format, formatProvider);
}
