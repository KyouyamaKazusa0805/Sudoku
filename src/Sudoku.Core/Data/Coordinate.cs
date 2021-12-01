extern alias extended;

namespace Sudoku.Data;

/// <summary>
/// Defines a coordinate used in a sudoku grid.
/// </summary>
/// <param name="Cell">
/// Indicates the cell value used. The possible values are between 0 and 80. You can't assign a value out of this range;
/// otherwise, a <see cref="InvalidOperationException"/> or <see cref="ArgumentOutOfRangeException"/> will be thrown.
/// </param>
[AutoGetHashCode(nameof(Cell))]
[AutoEquality(nameof(Cell))]
public readonly partial record struct Coordinate(byte Cell)
: IEqualityOperators<Coordinate, Coordinate>
, IComparisonOperators<Coordinate, Coordinate>
, IEquatable<Coordinate>
, IValueEquatable<Coordinate>
, IComparable<Coordinate>
, IValueComparable<Coordinate>
, IFormattable
, IMinMaxValue<Coordinate>
, extended::System.IParseable<Coordinate>
{
	/// <summary>
	/// Indicates the undefined <see cref="Coordinate"/> instance that stands for an invalid <see cref="Coordinate"/> value.
	/// </summary>
	public static readonly Coordinate Undefined = new(byte.MaxValue);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	public static readonly Coordinate MinValue;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly Coordinate MaxValue = new(80);


	/// <inheritdoc/>
	static Coordinate IMinMaxValue<Coordinate>.MinValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => MinValue;
	}

	/// <inheritdoc/>
	static Coordinate IMinMaxValue<Coordinate>.MaxValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => MaxValue;
	}


	/// <inheritdoc/>
	public int CompareTo(Coordinate other) => Cell.CompareTo(other.Cell);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(null, null);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format) => ToString(format, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		formatProvider.HasFormatted(this, format, out string? result)
			? result
			: format switch
			{
				null or "rc" => $"r{Cell / 9 + 1}c{Cell % 9 + 1}",
				"RC" => $"R{Cell / 9 + 1}C{Cell % 9 + 1}",
				"RCB" => $"R{Cell / 9 + 1}C{Cell % 9 + 1}B{RegionCalculator.ToRegion(Cell, RegionLabel.Block)}",
				"rcb" => $"r{Cell / 9 + 1}c{Cell % 9 + 1}b{RegionCalculator.ToRegion(Cell, RegionLabel.Block)}",
				{ Length: 1 } when format[0] is var formatChar => formatChar switch
				{
					'N' => $"R{Cell / 9 + 1}C{Cell % 9 + 1}",
					'n' => $"r{Cell / 9 + 1}c{Cell % 9 + 1}",
					'R' => $"R{Cell / 9 + 1}",
					'r' => $"r{Cell / 9 + 1}",
					'C' => $"C{Cell % 9 + 1}",
					'c' => $"c{Cell % 9 + 1}",
					'B' => $"B{RegionCalculator.ToRegion(Cell, RegionLabel.Block)}",
					'b' => $"b{RegionCalculator.ToRegion(Cell, RegionLabel.Block)}",
					_ => throw new FormatException($"The specified format '{formatChar}' is invalid or not supported.")
				},
				_ => throw new FormatException($"The specified format '{format}' is invalid or not supported.")
			};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IValueComparable<Coordinate>.CompareTo(in Coordinate other) => CompareTo(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj) =>
		obj is not Coordinate { Cell: var cell }
			? throw new ArgumentException("The specified argument type is invalid.", nameof(obj))
			: Cell.CompareTo(cell);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Coordinate Parse(string? str)
	{
		Nullability.ThrowIfNull(str);

		if (string.IsNullOrWhiteSpace(str))
		{
			goto ThrowFormatException;
		}

		string copied = str.Trim();
		if (rcb(copied, out byte c)) { return new(c); }
		if (k9(copied, out c)) { return new(c); }

	ThrowFormatException:
		throw new FormatException("The specified string is invalid to parse.");


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool rcb(string str, [DiscardWhen(false)] out byte cell)
		{
			if (str.Length != 4)
			{
				goto DefaultReturn;
			}

			if (str[0] is not ('R' or 'r') || str[2] is not ('C' or 'c'))
			{
				goto DefaultReturn;
			}

			if (!char.IsDigit(str[1]) || !char.IsDigit(str[2]))
			{
				goto DefaultReturn;
			}

			cell = (byte)((str[1] - '1') * 9 + (str[3] - '1'));
			return true;

		DefaultReturn:
			cell = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool k9(string str, [DiscardWhen(false)] out byte cell)
		{
			if (str.Length != 2)
			{
				goto DefaultReturn;
			}

			if (str[0] is not (>= 'a' and <= 'i' or >= 'A' and <= 'I') || !char.IsDigit(str[1]))
			{
				goto DefaultReturn;
			}

			char start = str[0] is >= 'A' and <= 'I' ? 'A' : 'a';
			cell = (byte)((str[0] - start) * 9 + str[1] - '1');
			return true;

		DefaultReturn:
			cell = default;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse(string? str, [DiscardWhen(false)] out Coordinate result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (Exception ex) when (ex is FormatException or ArgumentNullException)
		{
			result = Undefined;
			return false;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(Coordinate left, Coordinate right) => left.Cell >= right.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(Coordinate left, Coordinate right) => left.Cell <= right.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(Coordinate left, Coordinate right) => left.Cell > right.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(Coordinate left, Coordinate right) => left.Cell < right.Cell;


	/// <summary>
	/// Implicit conversion from <see cref="Coordinate"/> to <see cref="byte"/>.
	/// </summary>
	/// <param name="coordinate">The <see cref="Coordinate"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator byte(Coordinate coordinate) => coordinate.Cell;


	/// <summary>
	/// Explicit conversion from <see cref="byte"/> to <see cref="Coordinate"/>.
	/// </summary>
	/// <param name="cell">The cell value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Coordinate(byte cell) => new(cell);
}
