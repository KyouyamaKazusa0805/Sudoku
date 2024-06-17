namespace Sudoku.Concepts;

/// <summary>
/// Represents an instance that describes for multiple forcing chains.
/// </summary>
/// <param name="conclusion">Indicates the final conclusion.</param>
/// <remarks>
/// <para>
/// By the way, this type is directly derived from <see cref="SortedDictionary{TKey, TValue}"/>
/// of key and value types <see cref="Candidate"/> and <see cref="WeakChain"/> respectively.
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
/// <seealso cref="Node"/>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators)]
public sealed partial class MultipleForcingChains([PrimaryConstructorParameter] Conclusion conclusion) :
	SortedDictionary<Candidate, WeakChain>,
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
	public bool IsCellMultiple => CandidatesUsed.Cells.Count == 1;

	/// <summary>
	/// Indicates whether the pattern is aimed to a house, producing multiple branches.
	/// </summary>
	/// <remarks>
	/// This value is conflict with <see cref="IsCellMultiple"/>. If this property is <see langword="true"/>,
	/// the property <see cref="IsCellMultiple"/> will always return <see langword="false"/> and vice versa.
	/// </remarks>
	/// <seealso cref="IsCellMultiple"/>
	public bool IsHouseMultiple => IsPow2(CandidatesUsed.Digits);

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
	public CandidateMap CandidatesUsed => [.. Keys];


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

		var (map1, map2) = (CandidatesUsed, other.CandidatesUsed);
		if (map1.CompareTo(in map2) is var r2 and not 0)
		{
			return r2;
		}

		foreach (var candidate in map1)
		{
			var (chain1, chain2) = (this[candidate], other[candidate]);
			if (chain1.CompareTo(chain2, nodeComparison) is var r3 and not 0)
			{
				return r3;
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

		var (map1, map2) = (CandidatesUsed, other.CandidatesUsed);
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
		var converter = formatProvider switch
		{
			CultureInfo c => CoordinateConverter.GetConverter(c),
			CoordinateConverter c => c,
			_ => CoordinateConverter.InvariantCultureConverter
		};
		return string.Join(
			", ",
			from kvp in this
			let candidate = kvp.Key
			let pattern = kvp.Value
			select $"{converter.CandidateConverter(candidate)}: {pattern.ToString(format, converter)}"
		);
	}
}
