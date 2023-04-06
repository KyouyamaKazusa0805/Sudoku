namespace Sudoku.Text.Parsing;

/// <summary>
/// Defines a range of cells.
/// </summary>
/// <param name="mask">The mask to be set.</param>
public readonly partial struct CellRange(short mask) :
	IEquatable<CellRange>,
	IEqualityOperators<CellRange, CellRange, bool>,
	ISimpleParsable<CellRange>
{
	/// <summary>
	/// Indicates the inner mask.
	/// </summary>
	private readonly short _mask = mask;


	/// <summary>
	/// Initializes a <see cref="CellRange"/> instance via the specified value controlling the range.
	/// </summary>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <exception cref="ArgumentException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>The argument <paramref name="min"/> is greater than <paramref name="max"/>.</item>
	/// <item>The argument <paramref name="min"/> is below than 0 or greater than 80.</item>
	/// <item>The argument <paramref name="max"/> is below than 0 or greater than 81.</item>
	/// </list>
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellRange(int min, int max) :
		this(
			min <= max
				? min is >= 0 and < 81
					? max is >= 0 and <= 81
						? (short)(max << 7 | min)
						: throw new ArgumentException($"The argument '{nameof(max)}' must be between 0 and 81.", nameof(max))
					: throw new ArgumentException($"The argument '{nameof(min)}' must be between 0 and 80.", nameof(min))
				: throw new ArgumentException($"The argument '{nameof(min)}' must be less than '{nameof(max)}'")
		)
	{
	}


	/// <summary>
	/// Indicates the minimum value.
	/// </summary>
	public int MinValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 7 & 127;
	}

	/// <summary>
	/// Indicates the maximum value.
	/// </summary>
	public int MaxValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 127;
	}


	[DeconstructionMethod]
	public partial void Deconstruct(out int minValue, out int maxValue);

	[GeneratedOverridingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CellRange other) => _mask == other._mask;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(_mask))]
	public override partial int GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var s = MinValue == 0 ? string.Empty : (MinValue + 1).ToString();
		var e = MaxValue switch { 81 => string.Empty, 80 => "^1", 79 => "^2", _ => MaxValue.ToString() };
		return $"{s}..{e}";
	}


	/// <inheritdoc/>
	public static CellRange Parse(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			throw new FormatException("The string being parsed cannot be empty or only contain whitespaces.");
		}

		if (!str.Contains(".."))
		{
			throw new FormatException("The range operator '..' expected.");
		}

		if (str.Split("..") is not [var startStr, var endStr])
		{
			throw new FormatException("Expect 2 cell indices to be parsed.");
		}

		if (startStr.Contains('^'))
		{
			throw new FormatException("The min cell index cannot contain index-negation operator '^'.");
		}

		return new(
			string.IsNullOrWhiteSpace(startStr) ? 0 : int.Parse(startStr),
			string.IsNullOrWhiteSpace(endStr)
				? 81
				: endStr.Trim() is ['^', .. var otherChars]
					? int.Parse(otherChars)
					: throw new FormatException("The potential cell index string cannot be parsed as a real index value.")
		);
	}

	/// <inheritdoc/>
	public static bool TryParse(string str, [NotNullWhen(true)] out CellRange result)
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
	public static bool operator ==(CellRange left, CellRange right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CellRange left, CellRange right) => !left.Equals(right);


	/// <summary>
	/// Implicitly cast from <see cref="Range"/> to <see cref="CellRange"/>.
	/// </summary>
	/// <param name="range">The range instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellRange(Range range) => new(range.Start.GetOffset(81), range.End.GetOffset(81));
}
