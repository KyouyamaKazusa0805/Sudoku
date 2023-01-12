namespace Sudoku.Text;

/// <summary>
/// Defines a digit range.
/// </summary>
public readonly partial struct DigitRange : IEquatable<DigitRange>, IEqualityOperators<DigitRange, DigitRange, bool>, ISimpleParsable<DigitRange>
{
	/// <summary>
	/// Indicates an instance representing a meaningless case.
	/// </summary>
	public static readonly DigitRange Empty = (DigitRange)"";

	/// <summary>
	/// Indicates an instance representing a case that all digits (1-9) will be removed.
	/// </summary>
	public static readonly DigitRange AllDigitsRemoving = (DigitRange)"!-";


	/// <summary>
	/// Initializes a <see cref="DigitRange"/> instance via the specified digit, as an assignment.
	/// </summary>
	/// <param name="digit">The digit to be assigned.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DigitRange(int digit) : this((short)(1 << digit), Assignment)
	{
	}

	/// <summary>
	/// Initializes a <see cref="DigitRange"/> instance via the mask and conclusion type.
	/// </summary>
	/// <param name="digits">The digits mask.</param>
	/// <param name="type">The type.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DigitRange(short digits, ConclusionType type) => (DigitsMask, ConclusionType) = (digits, type);


	/// <summary>
	/// Indicates the digits used to be removed.
	/// </summary>
	public short DigitsMask { get; }

	/// <summary>
	/// Indicates the conclusion type.
	/// </summary>
	public ConclusionType ConclusionType { get; }

	/// <summary>
	/// Indicates the number of digits stored in the mask <see cref="DigitsMask"/>.
	/// </summary>
	/// <seealso cref="DigitsMask"/>
	private int NumbersCount => PopCount((uint)DigitsMask);


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DigitRange other) => ConclusionType == other.ConclusionType && DigitsMask == other.DigitsMask;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(DigitsMask), nameof(ConclusionType))]
	public override partial int GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString()
	{
		if (this == Empty)
		{
			return string.Empty;
		}

		if (this == AllDigitsRemoving)
		{
			return "!-";
		}

		var listOfDigits = new List<int>(9);
		foreach (var digit in DigitsMask)
		{
			listOfDigits.Add(digit + 1);
		}

		return ConclusionType == Assignment ? listOfDigits[0].ToString() : $"!{string.Concat(listOfDigits)}";
	}


	/// <inheritdoc/>
	public static bool TryParse(string str, out DigitRange result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DigitRange Parse(string str)
	{
		return str switch
		{
			[] => Empty,
			[var digit and >= '1' and <= '9'] => new(digit - '1'),
			['!', var digit and >= '1' and <= '9'] => new((short)(1 << digit - '1'), Elimination),
			['!', '-'] => AllDigitsRemoving,
			['!', var digit, '-'] => (digit != '9') switch
			{
				true => new(maskRange(digit - '1', 8), Elimination),
				_ => throw new FormatException("The lower bound cannot be 9.")
			},
			['!', '-', var digit] => (digit != '1') switch
			{
				true => new(maskRange(0, digit - '1'), Elimination),
				_ => throw new FormatException("The upper bound cannot be 1.")
			},
			['!', var d1, '-', var d2] => (d1 < d2) switch
			{
				true => new(maskRange(d1 - '1', d2 - '1'), Elimination),
				_ => throw new FormatException("The lower bound is greater than upper bound, which is invalid.")
			},
			['!', .. var digits] => separateMaskRange(digits) switch
			{
				{ } mask => new(mask, Elimination),
				_ => throw new FormatException("The number range contains unexpected characters.")
			},
			_ => throw new FormatException($"The specified text cannot be parsed into target type '{nameof(DigitRange)}'.")
		};


		static short maskRange(int d1, int d2)
		{
			var result = (short)0;
			for (var temp = d1; temp <= d2; temp++)
			{
				result |= (short)(1 << temp);
			}

			return result;
		}

		static short? separateMaskRange(string digits)
		{
			var result = (short)0;
			foreach (var digit in digits)
			{
				if (digit is not (>= '1' and <= '9'))
				{
					return null;
				}

				result |= (short)(1 << digit - '1');
			}

			return result;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DigitRange left, DigitRange right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DigitRange left, DigitRange right) => !(left == right);


	/// <summary>
	/// Explicit cast from <see cref="string"/> to <see cref="DigitRange"/>.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator DigitRange(string s) => Parse(s);
}
