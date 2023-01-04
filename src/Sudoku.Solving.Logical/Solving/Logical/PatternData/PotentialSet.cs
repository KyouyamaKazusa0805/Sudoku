#define MUTABLE_OPERATION

namespace Sudoku.Solving.Logical.PatternData;

/// <summary>
/// Defines a <see cref="Potential"/> collection using <see cref="HashSet{T}"/> as backing implementation.
/// </summary>
/// <seealso cref="Potential"/>
internal sealed class PotentialSet : HashSet<Potential>
{
	/// <summary>
	/// Initializes a <see cref="PotentialSet"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PotentialSet() : base(EqualityComparer.Instance)
	{
	}

	/// <summary>
	/// Initializes a <see cref="PotentialSet"/> instance via the specified <see cref="Potential"/> collection to be added.
	/// </summary>
	/// <param name="base">The collection of <see cref="Potential"/> instances.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PotentialSet(PotentialSet @base) : base(@base, EqualityComparer.Instance)
	{
	}


	/// <summary>
	/// Determines whether a <see cref="PotentialSet"/> object contains the specified element,
	/// comparing for properties <see cref="Potential.Candidate"/> and <see cref="Potential.IsOn"/>.
	/// </summary>
	/// <param name="base">The element to locate in the <see cref="PotentialSet"/> object.</param>
	/// <returns>
	/// <see langword="true"/> if the <see cref="PotentialSet"/> object contains the specified element; otherwise, <see langword="false"/>.
	/// </returns>
	/// <seealso cref="Potential.Candidate"/>
	/// <seealso cref="Potential.IsOn"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new bool Contains(Potential @base) => GetNullable(@base) is not null;

	/// <summary>
	/// <para>
	/// Try to get the target <see cref="Potential"/> instance whose internal value
	/// (i.e. properties <see cref="Potential.Candidate"/> and <see cref="Potential.IsOn"/>) are same as
	/// the specified one.
	/// </para>
	/// <para>
	/// Please note that this method will return an instance inside the current collection
	/// whose value equals to the specified one; however, property <see cref="Potential.Parents"/> may not be equal.
	/// </para>
	/// </summary>
	/// <param name="base">The value to be checked.</param>
	/// <returns>
	/// <para>
	/// The found value whose value is equal to <paramref name="base"/>; without checking for property <see cref="Potential.Parents"/>.
	/// </para>
	/// <para>If none found, <see langword="null"/> will be returned.</para>
	/// </returns>
	/// <seealso cref="Potential.Candidate"/>
	/// <seealso cref="Potential.IsOn"/>
	public Potential? GetNullable(Potential @base)
	{
		foreach (var potential in this)
		{
			if (potential == @base)
			{
				return potential;
			}
		}

		return null;
	}


	/// <summary>
	/// <para>
	/// Gets the <see cref="Potential"/> instances that both <paramref name="left"/> and <paramref name="right"/> contain,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="Potential"/>s mentioned above,
	/// then returns it.
	/// </para>
	/// <para><b>This operator can only be used on compound cases: </b><c><![CDATA[a &= b]]></c>.</para>
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PotentialSet operator &(PotentialSet left, PotentialSet right)
	{
#if MUTABLE_OPERATION
		left.IntersectWith(right);
		return left;
#else
		var result = new PotentialSet(left);
		result.IntersectWith(right);
		return result;
#endif
	}

	/// <summary>
	/// <para>
	/// Gets the <see cref="Potential"/> instances that comes from both collections <paramref name="left"/> and <paramref name="right"/>,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="Potential"/>s mentioned above,
	/// then returns it.
	/// </para>
	/// <para><b>This operator can only be used on compound cases: </b><c><![CDATA[a |= b]]></c>.</para>
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PotentialSet operator |(PotentialSet left, PotentialSet right)
	{
#if MUTABLE_OPERATION
		left.UnionWith(right);
		return left;
#else
		var result = new PotentialSet(left);
		result.UnionWith(right);
		return result;
#endif
	}

	/// <summary>
	/// <para>
	/// Gets the <see cref="Potential"/> instances that only one collection in <paramref name="left"/> and <paramref name="right"/> contains,
	/// and modifies the argument <paramref name="left"/>, replacing it with <see cref="Potential"/>s mentioned above,
	/// then returns it.
	/// </para>
	/// <para><b>This operator can only be used on compound cases: </b><c><![CDATA[a ^= b]]></c>.</para>
	/// </summary>
	/// <param name="left">The first collection to be participated in merging operation.</param>
	/// <param name="right">The second collection to be participated in merging operation.</param>
	/// <returns>Modified collection <paramref name="left"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PotentialSet operator ^(PotentialSet left, PotentialSet right)
	{
#if MUTABLE_OPERATION
		left.SymmetricExceptWith(right);
		return left;
#else
		var result = new PotentialSet(left);
		result.SymmetricExceptWith(right);
		return result;
#endif
	}
}

/// <summary>
/// Defines an equality comparer that compares to <see cref="Potential"/> instances.
/// </summary>
/// <seealso cref="Potential"/>
file sealed class EqualityComparer : IEqualityComparer<Potential>
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly EqualityComparer Instance = new();


	/// <summary>
	/// Initializes a <see cref="EqualityComparer"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private EqualityComparer()
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Potential x, Potential y) => x == y;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(Potential obj) => obj.GetHashCode();
}
