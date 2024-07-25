namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that includes a list of houses and a list of digits, indicating the eliminations range.
/// </summary>
/// <remarks>
/// You can create a collection of this type by using collection expressions. By using a pair of values to create an instance.
/// For example, <c>[0, 24, 1, 24]</c> means digit 0 and 1 holds blocks 4 and 5 (24 = <c>0b0001_1000</c>, 4th and 5th bits are set 1).
/// </remarks>
[InlineArray(9)]
[CollectionBuilder(typeof(EliminationRange), nameof(Create))]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.EqualityOperators, IsLargeStructure = true)]
public partial struct EliminationRange :
	IBitwiseOperators<EliminationRange, EliminationRange, EliminationRange>,
	IEnumerable<KeyValuePair<Digit, HouseMask>>,
	IEquatable<EliminationRange>,
	IEqualityOperators<EliminationRange, EliminationRange, bool>,
	IFormattable,
	ILogicalOperators<EliminationRange>,
	IReadOnlyCollection<KeyValuePair<Digit, HouseMask>>,
	IToArrayMethod<EliminationRange, KeyValuePair<Digit, HouseMask>>
{
	/// <summary>
	/// Indicates the empty instance that contains no elements.
	/// </summary>
	public static readonly EliminationRange Empty = default;


	/// <summary>
	/// Indicates the internal mask field. The length of the buffer is 9,
	/// which represents distributions of each digit and their corresponding cover sets.
	/// </summary>
	[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private HouseMask _mask;


	/// <inheritdoc/>
	public readonly int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = 0;
			if (this[0] != 0) { result++; }
			if (this[1] != 0) { result++; }
			if (this[2] != 0) { result++; }
			if (this[3] != 0) { result++; }
			if (this[4] != 0) { result++; }
			if (this[5] != 0) { result++; }
			if (this[6] != 0) { result++; }
			if (this[7] != 0) { result++; }
			if (this[8] != 0) { result++; }
			return result;
		}
	}

	/// <summary>
	/// Indicates all houses covered.
	/// </summary>
	public readonly HouseMask Houses
	{
		get
		{
			var result = 0;
			for (var digit = 0; digit < 9; digit++)
			{
				result |= this[digit];
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates all digits used.
	/// </summary>
	public readonly Mask Digits
	{
		get
		{
			var result = (Mask)0;
			for (var digit = 0; digit < 9; digit++)
			{
				if (this[digit] != 0)
				{
					result |= (Mask)(1 << digit);
				}
			}
			return result;
		}
	}


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly EliminationRange other)
	{
		if (this[0] != other[0]) { return false; }
		if (this[1] != other[1]) { return false; }
		if (this[2] != other[2]) { return false; }
		if (this[3] != other[3]) { return false; }
		if (this[4] != other[4]) { return false; }
		if (this[5] != other[5]) { return false; }
		if (this[6] != other[6]) { return false; }
		if (this[7] != other[7]) { return false; }
		if (this[8] != other[8]) { return false; }
		return true;
	}

	/// <summary>
	/// Determines whether the collection contains at least one element of digit and houses satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool Exists(Func<HouseMask, Digit, bool> predicate)
	{
		for (var digit = 0; digit < 9; digit++)
		{
			if (predicate(this[digit], digit))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether all elements in the collection satisfy the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool TrueForAll(Func<HouseMask, Digit, bool> predicate)
	{
		for (var digit = 0; digit < 9; digit++)
		{
			if (!predicate(this[digit], digit))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determines whether the collection contains the specified digit.
	/// </summary>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool ContainsDigit(Digit digit) => this[digit] != 0;

	/// <inheritdoc/>
	public override readonly int GetHashCode()
	{
		var result = 0;
		result += this[0] ^ 0xDCEBFA << 17;
		result += this[1] << 17 ^ 0xAFBECD;
		result += this[2] << 17 ^ 0xDCEBFA;
		result += this[3] ^ 0xAFBECD << 17;
		result += this[4] ^ 0xDCEBFA << 17;
		result += this[5] << 17 ^ 0xAFBECD;
		result += this[6] << 17 ^ 0xDCEBFA;
		result += this[7] ^ 0xAFBECD << 17;
		result += this[8] ^ 0xDCEBFA << 17;
		return result;
	}

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public readonly string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		var result = new List<string>();
		for (var digit = 0; digit < 9; digit++)
		{
			if (this[digit] is var mask and not 0)
			{
				var cells = CellMap.Empty;
				foreach (var house in mask)
				{
					cells |= HousesMap[house];
				}
				result.Add($"{digit + 1}/{converter.HouseConverter(mask)}");
			}
		}
		return string.Join(", ", result);
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[UnscopedRef]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(in this);

	/// <inheritdoc/>
	public readonly KeyValuePair<Digit, HouseMask>[] ToDigitsArray()
	{
		var result = new List<KeyValuePair<Digit, HouseMask>>(9);
		for (var digit = 0; digit < 9; digit++)
		{
			if (this[digit] is var mask and not 0)
			{
				result.AddRef(new(digit, mask));
			}
		}
		return [.. result];
	}

	/// <summary>
	/// Add a new digit and houses into the current collection.
	/// </summary>
	/// <param name="digit">The digit to be added.</param>
	/// <param name="houses">The houses to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Digit digit, HouseMask houses) => this[digit] = houses;

	/// <summary>
	/// Removes the digit and its configured value from the collection.
	/// </summary>
	/// <param name="digit">The digit to be removed.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(Digit digit) => this[digit] = 0;

	/// <summary>
	/// Gets the houses covered for the specified digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="HouseMask"/> instance representing the houses covered.</returns>
	[UnscopedRef]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref HouseMask GetAtRef(Digit digit) => ref this[digit];

	/// <inheritdoc/>
	readonly bool IEquatable<EliminationRange>.Equals(EliminationRange other) => Equals(in other);

	/// <inheritdoc/>
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	readonly IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<Digit, HouseMask>>)this).GetEnumerator();

	/// <inheritdoc/>
	readonly IEnumerator<KeyValuePair<Digit, HouseMask>> IEnumerable<KeyValuePair<Digit, HouseMask>>.GetEnumerator()
	{
		for (var digit = 0; digit < 9; digit++)
		{
			if (this[digit] is var mask and not 0)
			{
				yield return new(digit, mask);
			}
		}
	}


	/// <summary>
	/// Creates an <see cref="EliminationRange"/> via collection expression.
	/// </summary>
	/// <param name="values">The values to be created.</param>
	/// <returns>An <see cref="EliminationRange"/> instance as result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the number of argument <paramref name="values"/> is not an even number, or greater than 18.
	/// </exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static EliminationRange Create(ReadOnlySpan<int> values)
	{
		if ((values.Length & 1) != 0)
		{
			throw new InvalidOperationException(SR.ExceptionMessage("LengthMustBeAnEven"));
		}
		if (values.Length > 18)
		{
			throw new InvalidOperationException(string.Format(SR.ExceptionMessage("LengthExceeded"), 18));
		}

		var result = Empty;
		for (var i = 0; i < values.Length; i += 2)
		{
			result.Add(values[i], values[i + 1]);
		}
		return result;
	}


	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_OnesComplement(TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EliminationRange operator ~(in EliminationRange value)
	{
		var result = value;
		result[0] = ~result[0] & ((1 << 27) - 1);
		result[1] = ~result[1] & ((1 << 27) - 1);
		result[2] = ~result[2] & ((1 << 27) - 1);
		result[3] = ~result[3] & ((1 << 27) - 1);
		result[4] = ~result[4] & ((1 << 27) - 1);
		result[5] = ~result[5] & ((1 << 27) - 1);
		result[6] = ~result[6] & ((1 << 27) - 1);
		result[7] = ~result[7] & ((1 << 27) - 1);
		result[8] = ~result[8] & ((1 << 27) - 1);
		return result;
	}

	/// <inheritdoc cref="ILogicalOperators{TSelf}.op_True(TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(in EliminationRange value) => value.Count != 0;

	/// <inheritdoc cref="ILogicalOperators{TSelf}.op_False(TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(in EliminationRange value) => value.Count == 0;

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EliminationRange operator &(in EliminationRange left, in EliminationRange right)
	{
		var result = left;
		result[0] &= right[0];
		result[1] &= right[1];
		result[2] &= right[2];
		result[3] &= right[3];
		result[4] &= right[4];
		result[5] &= right[5];
		result[6] &= right[6];
		result[7] &= right[7];
		result[8] &= right[8];
		return result;
	}

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EliminationRange operator |(in EliminationRange left, in EliminationRange right)
	{
		var result = left;
		result[0] |= right[0];
		result[1] |= right[1];
		result[2] |= right[2];
		result[3] |= right[3];
		result[4] |= right[4];
		result[5] |= right[5];
		result[6] |= right[6];
		result[7] |= right[7];
		result[8] |= right[8];
		return result;
	}

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_ExclusiveOr(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EliminationRange operator ^(in EliminationRange left, in EliminationRange right)
	{
		var result = left;
		result[0] ^= right[0];
		result[1] ^= right[1];
		result[2] ^= right[2];
		result[3] ^= right[3];
		result[4] ^= right[4];
		result[5] ^= right[5];
		result[6] ^= right[6];
		result[7] ^= right[7];
		result[8] ^= right[8];
		return result;
	}

	/// <inheritdoc/>
	static bool ILogicalOperators<EliminationRange>.operator !(EliminationRange value) => value.Count != 0;

	/// <inheritdoc/>
	static bool ILogicalOperators<EliminationRange>.operator true(EliminationRange value) => value.Count != 0;

	/// <inheritdoc/>
	static bool ILogicalOperators<EliminationRange>.operator false(EliminationRange value) => value.Count == 0;

	/// <inheritdoc/>
	static EliminationRange IBitwiseOperators<EliminationRange, EliminationRange, EliminationRange>.operator ~(EliminationRange value) => ~value;

	/// <inheritdoc/>
	static EliminationRange IBitwiseOperators<EliminationRange, EliminationRange, EliminationRange>.operator &(EliminationRange left, EliminationRange right)
		=> left & right;

	/// <inheritdoc/>
	static EliminationRange IBitwiseOperators<EliminationRange, EliminationRange, EliminationRange>.operator |(EliminationRange left, EliminationRange right)
		=> left | right;

	/// <inheritdoc/>
	static EliminationRange IBitwiseOperators<EliminationRange, EliminationRange, EliminationRange>.operator ^(EliminationRange left, EliminationRange right)
		=> left ^ right;

	/// <inheritdoc/>
	static EliminationRange ILogicalOperators<EliminationRange>.operator &(EliminationRange left, EliminationRange right) => left & right;

	/// <inheritdoc/>
	static EliminationRange ILogicalOperators<EliminationRange>.operator |(EliminationRange left, EliminationRange right) => left | right;

	/// <inheritdoc/>
	static EliminationRange ILogicalOperators<EliminationRange>.operator ^(EliminationRange left, EliminationRange right) => left ^ right;
}
