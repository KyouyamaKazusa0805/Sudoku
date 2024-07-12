namespace Sudoku.Concepts;

public partial struct CellMap : IBinaryInteger<CellMap>, ISignedNumber<CellMap>, IBinaryNumber<CellMap>
{
	/// <summary>
	/// Indicates the possible maximal value in <see cref="double"/> representation.
	/// </summary>
	private const double MaxValueDouble = 2417851639229258349412352D;

	/// <summary>
	/// Indicates the size.
	/// </summary>
	private const int Size = 16;


	/// <summary>
	/// Indicates the max bits on numeric representation.
	/// </summary>
	private static readonly UInt128 MaxValueUInt128 = new(0x1FFFFUL, ulong.MaxValue);


	/// <summary>
	/// Indicates the back numeric value to be used.
	/// </summary>
	private readonly Int128 NumericValue
	{
		get
		{
			var one = Int128.One;
			var result = Int128.Zero;
			foreach (var offset in Offsets)
			{
				result |= one << offset;
			}
			return result;
		}
	}


	/// <inheritdoc/>
	static int INumberBase<CellMap>.Radix => 2;

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.One => CreateByNumericValue(1);

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.Zero => Empty;

	/// <inheritdoc/>
	static CellMap IMultiplicativeIdentity<CellMap, CellMap>.MultiplicativeIdentity => CreateByNumericValue(1);

	/// <inheritdoc/>
	static CellMap ISignedNumber<CellMap>.NegativeOne => Full;

	/// <inheritdoc/>
	static CellMap IBinaryNumber<CellMap>.AllBitsSet => Full;


	/// <inheritdoc/>
	readonly bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> NumericValue.TryFormat(destination, out charsWritten, format, provider);

	/// <inheritdoc/>
	readonly bool IUtf8SpanFormattable.TryFormat(Span<byte> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> NumericValue.TryFormat(destination, out charsWritten, format, provider);

	/// <inheritdoc/>
	readonly bool IBinaryInteger<CellMap>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
		=> ((IBinaryInteger<Int128>)NumericValue).TryWriteBigEndian(destination, out bytesWritten);

	/// <inheritdoc/>
	readonly bool IBinaryInteger<CellMap>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
		=> ((IBinaryInteger<Int128>)NumericValue).TryWriteLittleEndian(destination, out bytesWritten);

	/// <inheritdoc/>
	readonly int IComparable.CompareTo(object? obj) => obj is CellMap value ? CompareTo(in value) : -1;

	/// <inheritdoc/>
	readonly int IBinaryInteger<CellMap>.GetByteCount() => Size;

	/// <inheritdoc/>
	readonly int IBinaryInteger<CellMap>.GetShortestBitLength() => (Size << 3) - LeadingZeroCountAsInt32(NumericValue);


	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via a <see cref="Int128"/> as numeric value.
	/// </summary>
	/// <param name="numericValue">The numeric value.</param>
	/// <returns>A valid <see cref="CellMap"/> instance.</returns>
	public static CellMap CreateByNumericValue(Int128 numericValue)
	{
		var result = Empty;
		foreach (var element in (UInt128)numericValue & MaxValueUInt128)
		{
			result.Add(element);
		}
		return result;
	}

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via a <see cref="Int128"/> as numeric value.
	/// </summary>
	/// <param name="numericValue">The numeric value.</param>
	/// <returns>A valid <see cref="CellMap"/> instance.</returns>
	/// <exception cref="OverflowException">Throws when the numeric value has exceeded bits of position greater than 81.</exception>
	public static CellMap CreateByNumericValueChecked(Int128 numericValue)
	{
		if ((UInt128)numericValue >= MaxValueUInt128)
		{
			throw new OverflowException();
		}

		var result = Empty;
		foreach (var element in numericValue)
		{
			result.Add(element);
		}
		return result;
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsCanonical(CellMap value) => true;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsComplexNumber(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsEvenInteger(CellMap value) => Int128.IsEvenInteger((Int128)value);

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsFinite(CellMap value) => true;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsImaginaryNumber(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsInfinity(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsInteger(CellMap value) => true;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsNaN(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsNegative(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsNegativeInfinity(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsNormal(CellMap value) => value.NumericValue != 0;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsOddInteger(CellMap value) => Int128.IsOddInteger((Int128)value);

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsPositive(CellMap value) => Int128.IsPositive((Int128)value);

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsPositiveInfinity(CellMap value) => false;

	/// <inheritdoc/>
	static bool IBinaryNumber<CellMap>.IsPow2(CellMap value) => Int128.IsPow2((Int128)value);

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsRealNumber(CellMap value) => true;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsSubnormal(CellMap value) => false;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.IsZero(CellMap value) => value.NumericValue == 0;

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertFromChecked<TOther>(TOther value, out CellMap result)
	{
		if (typeof(TOther) == typeof(Int128))
		{
			var actualValue = (Int128)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualValue = (UInt128)(object)value;
			result = CreateByNumericValueChecked(checked((Int128)actualValue));
			return true;
		}
		else if (typeof(TOther) == typeof(double))
		{
			var actualValue = (double)(object)value;
			result = CreateByNumericValueChecked(checked((Int128)actualValue));
			return true;
		}
		else if (typeof(TOther) == typeof(Half))
		{
			var actualValue = (Half)(object)value;
			result = CreateByNumericValueChecked(checked((Int128)actualValue));
			return true;
		}
		else if (typeof(TOther) == typeof(short))
		{
			var actualValue = (short)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(int))
		{
			var actualValue = (int)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(long))
		{
			var actualValue = (long)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(nint))
		{
			var actualValue = (nint)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(sbyte))
		{
			var actualValue = (sbyte)(object)value;
			result = CreateByNumericValueChecked(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(float))
		{
			var actualValue = (float)(object)value;
			result = CreateByNumericValueChecked(checked((Int128)actualValue));
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertFromSaturating<TOther>(TOther value, out CellMap result)
	{
		if (typeof(TOther) == typeof(Int128))
		{
			var actualValue = (Int128)(object)value;
			result = CreateByNumericValue(actualValue >= (Int128)MaxValueUInt128 ? (Int128)MaxValueUInt128 : actualValue <= 0 ? (Int128)0 : actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualValue = (UInt128)(object)value;
			result = CreateByNumericValue((Int128)(actualValue >= MaxValueUInt128 ? MaxValueUInt128 : actualValue));
			return true;
		}
		else if (typeof(TOther) == typeof(double))
		{
			var actualValue = (double)(object)value;
			result = CreateByNumericValue(actualValue >= MaxValueDouble ? (Int128)MaxValueUInt128 : actualValue <= 0 ? 0 : (Int128)actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(Half))
		{
			var actualValue = (Half)(object)value;
			result = CreateByNumericValue(
				actualValue == Half.PositiveInfinity
					? Int128.MaxValue
					: actualValue <= (Half)0 ? Int128.MinValue : (Int128)actualValue
			);
			return true;
		}
		else if (typeof(TOther) == typeof(short))
		{
			var actualValue = (short)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(int))
		{
			var actualValue = (int)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(long))
		{
			var actualValue = (long)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(nint))
		{
			var actualValue = (nint)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(sbyte))
		{
			var actualValue = (sbyte)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(float))
		{
			var actualValue = (float)(object)value;
			result = CreateByNumericValue((Int128)(actualValue >= MaxValueDouble ? MaxValueUInt128 : actualValue <= 0 ? 0 : (UInt128)actualValue));
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertFromTruncating<TOther>(TOther value, out CellMap result)
	{
		if (typeof(TOther) == typeof(Int128))
		{
			var actualValue = (Int128)(object)value;
			result = CreateByNumericValue(actualValue >= (Int128)MaxValueUInt128 ? (Int128)MaxValueUInt128 : actualValue <= 0 ? (Int128)0 : actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualValue = (UInt128)(object)value;
			result = CreateByNumericValue((Int128)(actualValue >= MaxValueUInt128 ? MaxValueUInt128 : actualValue));
			return true;
		}
		else if (typeof(TOther) == typeof(double))
		{
			var actualValue = (double)(object)value;
			result = CreateByNumericValue(actualValue >= MaxValueDouble ? Int128.MaxValue : actualValue <= 0 ? Int128.MinValue : (Int128)actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(Half))
		{
			var actualValue = (Half)(object)value;
			result = CreateByNumericValue(actualValue == Half.PositiveInfinity ? Int128.MaxValue : actualValue <= (Half)0 ? Int128.MinValue : (Int128)actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(short))
		{
			var actualValue = (short)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(int))
		{
			var actualValue = (int)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(long))
		{
			var actualValue = (long)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(nint))
		{
			var actualValue = (nint)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(sbyte))
		{
			var actualValue = (sbyte)(object)value;
			result = CreateByNumericValue(actualValue);
			return true;
		}
		else if (typeof(TOther) == typeof(float))
		{
			var actualValue = (double)(object)value;
			result = CreateByNumericValue(actualValue >= MaxValueDouble ? Int128.MaxValue : actualValue <= 0 ? Int128.MinValue : (Int128)actualValue);
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertToChecked<TOther>(CellMap value, [MaybeNullWhen(false)] out TOther result)
	{
		var numericValue = value.NumericValue;
		if (typeof(TOther) == typeof(Int128))
		{
			var actualResult = checked((UInt128)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualResult = checked((UInt128)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(byte))
		{
			var actualResult = checked((byte)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(char))
		{
			var actualResult = checked((char)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(decimal))
		{
			var actualResult = checked((decimal)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ushort))
		{
			var actualResult = checked((ushort)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(uint))
		{
			var actualResult = checked((uint)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ulong))
		{
			var actualResult = checked((ulong)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(nuint))
		{
			var actualResult = checked((nuint)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertToSaturating<TOther>(CellMap value, [MaybeNullWhen(false)] out TOther result)
	{
		var numericValue = value.NumericValue;
		if (typeof(TOther) == typeof(Int128))
		{
			var actualResult = checked((UInt128)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualResult = numericValue <= 0 ? UInt128.MinValue : (UInt128)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(byte))
		{
			var actualResult = numericValue >= byte.MaxValue
				? byte.MaxValue
				: numericValue <= byte.MinValue ? byte.MinValue : (byte)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(char))
		{
			var actualResult = numericValue >= char.MaxValue
				? char.MaxValue
				: numericValue <= char.MinValue ? char.MinValue : (char)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(decimal))
		{
			var actualResult = numericValue >= new Int128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)
				? decimal.MaxValue
				: numericValue <= new Int128(0xFFFF_FFFF_0000_0000, 0x0000_0000_0000_0001)
					? decimal.MinValue
					: (decimal)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ushort))
		{
			var actualResult = numericValue >= ushort.MaxValue
				? ushort.MaxValue
				: numericValue <= ushort.MinValue ? ushort.MinValue : (ushort)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(uint))
		{
			var actualResult = numericValue >= uint.MaxValue
				? uint.MaxValue
				: numericValue <= uint.MinValue ? uint.MinValue : (uint)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ulong))
		{
			var actualResult = numericValue >= ulong.MaxValue
				? ulong.MaxValue
				: numericValue <= ulong.MinValue ? ulong.MinValue : (ulong)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(nuint))
		{
			var actualResult = numericValue >= nuint.MaxValue
				? nuint.MaxValue
				: numericValue <= nuint.MinValue ? nuint.MinValue : (nuint)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryConvertToTruncating<TOther>(CellMap value, [MaybeNullWhen(false)] out TOther result)
	{
		var numericValue = value.NumericValue;
		if (typeof(TOther) == typeof(Int128))
		{
			var actualResult = checked((UInt128)numericValue);
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(UInt128))
		{
			var actualResult = (UInt128)numericValue <= 0 ? 0 : numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(byte))
		{
			var actualResult = (byte)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(char))
		{
			var actualResult = (char)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(decimal))
		{
			var actualResult = numericValue >= new Int128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)
				? decimal.MaxValue
				: numericValue <= new Int128(0xFFFF_FFFF_0000_0000, 0x0000_0000_0000_0001)
					? decimal.MinValue
					: (decimal)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ushort))
		{
			var actualResult = (ushort)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(uint))
		{
			var actualResult = (uint)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(ulong))
		{
			var actualResult = (ulong)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else if (typeof(TOther) == typeof(nuint))
		{
			var actualResult = (nuint)numericValue;
			result = (TOther)(object)actualResult;
			return true;
		}
		else
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out CellMap result)
	{
		var returnValue = Int128.TryParse(s, style, provider, out var r);
		result = CreateByNumericValue(r);
		return returnValue;
	}

	/// <inheritdoc/>
	static bool INumberBase<CellMap>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out CellMap result)
	{
		var returnValue = Int128.TryParse(s, style, provider, out var r);
		result = CreateByNumericValue(r);
		return returnValue;
	}

	/// <inheritdoc/>
	static bool ISpanParsable<CellMap>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CellMap result)
	{
		var returnValue = Int128.TryParse(s, provider, out var r);
		result = CreateByNumericValue(r);
		return returnValue;
	}

	/// <inheritdoc/>
	static bool IParsable<CellMap>.TryParse(string? s, IFormatProvider? provider, out CellMap result)
	{
		var returnValue = Int128.TryParse(s, provider, out var r);
		result = CreateByNumericValue(r);
		return returnValue;
	}

	/// <inheritdoc/>
	static bool IBinaryInteger<CellMap>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out CellMap value)
	{
		var result = tryReadBigEndian<Int128>(source, isUnsigned, out var v);
		value = CreateByNumericValue(v);
		return result;


		static bool tryReadBigEndian<T>(ReadOnlySpan<byte> source, bool isUnsigned, out T value) where T : IBinaryInteger<T>
			=> T.TryReadBigEndian(source, isUnsigned, out value);
	}

	/// <inheritdoc/>
	static bool IBinaryInteger<CellMap>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out CellMap value)
	{
		var result = tryReadLittleEndian<Int128>(source, isUnsigned, out var v);
		value = CreateByNumericValue(v);
		return result;


		static bool tryReadLittleEndian<T>(ReadOnlySpan<byte> source, bool isUnsigned, out T value) where T : IBinaryInteger<T>
			=> T.TryReadLittleEndian(source, isUnsigned, out value);
	}

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.Abs(CellMap value) => CreateByNumericValue(Int128.Abs(value.NumericValue));

	/// <inheritdoc/>
	static CellMap IBinaryNumber<CellMap>.Log2(CellMap value) => CreateByNumericValue(Int128.Log2((Int128)value));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.MaxMagnitude(CellMap x, CellMap y)
		=> CreateByNumericValue(Int128.MaxMagnitude((Int128)x, (Int128)y));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.MaxMagnitudeNumber(CellMap x, CellMap y)
		=> CreateByNumericValue(Int128.MaxMagnitude((Int128)x, (Int128)y));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.MinMagnitude(CellMap x, CellMap y)
		=> CreateByNumericValue(Int128.MinMagnitude((Int128)x, (Int128)y));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.MinMagnitudeNumber(CellMap x, CellMap y)
		=> CreateByNumericValue(Int128.MinMagnitude((Int128)x, (Int128)y));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
		=> CreateByNumericValue(Int128.Parse(s, style, provider));

	/// <inheritdoc/>
	static CellMap INumberBase<CellMap>.Parse(string s, NumberStyles style, IFormatProvider? provider)
		=> CreateByNumericValue(Int128.Parse(s, style, provider));

	/// <inheritdoc/>
	static CellMap ISpanParsable<CellMap>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
		=> CreateByNumericValue(Int128.Parse(s, provider));

	/// <inheritdoc/>
	static CellMap IParsable<CellMap>.Parse(string s, IFormatProvider? provider) => CreateByNumericValue(Int128.Parse(s, provider));

	/// <inheritdoc/>
	static CellMap IBinaryInteger<CellMap>.PopCount(CellMap value) => CreateByNumericValue(Int128.PopCount(value.NumericValue));

	/// <inheritdoc/>
	static CellMap IBinaryInteger<CellMap>.TrailingZeroCount(CellMap value)
		=> CreateByNumericValue(Int128.TrailingZeroCount(value.NumericValue));

	/// <summary>
	/// Calculates leading zeros count, try casting the value as two parts of bits.
	/// </summary>
	/// <param name="value">The values.</param>
	/// <returns>The final value.</returns>
	private static int LeadingZeroCountAsInt32(Int128 value)
		=> value >> 64 == 0 ? 64 + LeadingZeroCount((ulong)(value & ulong.MaxValue)) : LeadingZeroCount((ulong)(value >> 64));


	/// <inheritdoc/>
	static CellMap IUnaryPlusOperators<CellMap, CellMap>.operator +(CellMap value) => value;

	/// <inheritdoc/>
	static CellMap IAdditionOperators<CellMap, CellMap, CellMap>.operator +(CellMap left, CellMap right)
		=> CreateByNumericValue(left.NumericValue + right.NumericValue);

	/// <inheritdoc/>
	static CellMap IAdditionOperators<CellMap, CellMap, CellMap>.operator checked +(CellMap left, CellMap right)
		=> CreateByNumericValueChecked(checked(left.NumericValue + right.NumericValue));

	/// <inheritdoc/>
	static CellMap IUnaryNegationOperators<CellMap, CellMap>.operator -(CellMap value) => CreateByNumericValue(-value.NumericValue);

	/// <inheritdoc/>
	static CellMap IUnaryNegationOperators<CellMap, CellMap>.operator checked -(CellMap value)
		=> CreateByNumericValueChecked(checked(-value.NumericValue));

	/// <inheritdoc/>
	static CellMap ISubtractionOperators<CellMap, CellMap, CellMap>.operator -(CellMap left, CellMap right)
		=> CreateByNumericValue(left.NumericValue - right.NumericValue);

	/// <inheritdoc/>
	static CellMap ISubtractionOperators<CellMap, CellMap, CellMap>.operator checked -(CellMap left, CellMap right)
		=> CreateByNumericValueChecked(checked(left.NumericValue - right.NumericValue));

	/// <inheritdoc/>
	static CellMap IIncrementOperators<CellMap>.operator ++(CellMap value) => CreateByNumericValue(value.NumericValue + 1);

	/// <inheritdoc/>
	static CellMap IIncrementOperators<CellMap>.operator checked ++(CellMap value)
		=> CreateByNumericValueChecked(checked(value.NumericValue + 1));

	/// <inheritdoc/>
	static CellMap IDecrementOperators<CellMap>.operator --(CellMap value) => CreateByNumericValue(value.NumericValue - 1);

	/// <inheritdoc/>
	static CellMap IDecrementOperators<CellMap>.operator checked --(CellMap value)
		=> CreateByNumericValueChecked(checked(value.NumericValue - 1));

	/// <inheritdoc/>
	static CellMap IMultiplyOperators<CellMap, CellMap, CellMap>.operator *(CellMap left, CellMap right)
		=> CreateByNumericValue(left.NumericValue * right.NumericValue);

	/// <inheritdoc/>
	static CellMap IMultiplyOperators<CellMap, CellMap, CellMap>.operator checked *(CellMap left, CellMap right)
		=> CreateByNumericValueChecked(checked(left.NumericValue * right.NumericValue));

	/// <inheritdoc/>
	static CellMap IDivisionOperators<CellMap, CellMap, CellMap>.operator /(CellMap left, CellMap right)
		=> CreateByNumericValue(left.NumericValue / right.NumericValue);

	/// <inheritdoc/>
	static CellMap IDivisionOperators<CellMap, CellMap, CellMap>.operator checked /(CellMap left, CellMap right)
		=> CreateByNumericValueChecked(checked(left.NumericValue / right.NumericValue));

	/// <inheritdoc/>
	static CellMap IShiftOperators<CellMap, int, CellMap>.operator <<(CellMap value, int shiftAmount)
		=> CreateByNumericValue(value.NumericValue << shiftAmount);

	/// <inheritdoc/>
	static CellMap IShiftOperators<CellMap, int, CellMap>.operator >>(CellMap value, int shiftAmount)
		=> CreateByNumericValue(value.NumericValue >> shiftAmount);

	/// <inheritdoc/>
	static CellMap IShiftOperators<CellMap, int, CellMap>.operator >>>(CellMap value, int shiftAmount)
		=> CreateByNumericValue(value.NumericValue >>> shiftAmount);


	/// <summary>
	/// Creates a <see cref="Int128"/> instance as its numeric representation via the specified <see cref="CellMap"/> instance.
	/// </summary>
	/// <param name="this">A <see cref="CellMap"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Int128(in CellMap @this) => @this.NumericValue;

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance as its bit representation via the specified <see cref="Int128"/> instance.
	/// </summary>
	/// <param name="value">An <see cref="Int128"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(Int128 value) => CreateByNumericValue(value);

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance as its bit representation via the specified <see cref="Int128"/> instance;
	/// with overflow checking - throws <see cref="OverflowException"/> if the argument uses exceeded bits
	/// whose position is greater than 81.
	/// </summary>
	/// <param name="value">An <see cref="Int128"/> instance.</param>
	/// <exception cref="OverflowException">Throws when the value uses exceeded bits whose position is greater than 81.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked CellMap(Int128 value) => CreateByNumericValueChecked(value);
}
