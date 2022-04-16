namespace Sudoku.Concepts;

/// <summary>
/// Defines a range of cells.
/// </summary>
public readonly struct CellRange :
	IEquatable<CellRange>,
	IEqualityOperators<CellRange, CellRange>,
	ISimpleParseable<CellRange>
{
	/// <summary>
	/// Indicates the inner mask.
	/// </summary>
	private readonly short _mask;


	/// <summary>
	/// Initializes a <see cref="CellRange"/> instance via the specified start and end cell.
	/// </summary>
	/// <param name="start">The start cell.</param>
	/// <param name="end">The end cell.</param>
	/// <exception cref="ArgumentException">
	/// Throws when:
	/// <list type="bullet">
	/// <item>The argument <paramref name="start"/> is greater than <paramref name="end"/>.</item>
	/// <item>The argument <paramref name="start"/> is below than 0 or greater than 80.</item>
	/// <item>The argument <paramref name="end"/> is below than 0 or greater than 81.</item>
	/// </list>
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellRange(int start, int end) =>
		_mask = start <= end
			? start is >= 0 and < 81
				? end is >= 0 and <= 81
					? (short)(end << 7 | start)
					: throw new ArgumentException($"The argument '{nameof(end)}' must be between 0 and 81.", nameof(end))
				: throw new ArgumentException($"The argument '{nameof(start)}' must be between 0 and 80.", nameof(start))
			: throw new ArgumentException($"The argument '{nameof(start)}' must be less than '{nameof(end)}'");


	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	public int StartCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask >> 7 & 127;
	}

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	public int EndCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & 127;
	}


	/// <summary>
	/// Deconstruct the instance into multiple values.
	/// </summary>
	/// <param name="startCell">The start cell.</param>
	/// <param name="endCell">The end cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out int startCell, out int endCell) => (startCell, endCell) = (StartCell, EndCell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is CellRange comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(CellRange other) => _mask == other._mask;

	/// <summary>
	/// Determines whether the two <see cref="CellRange"/> instances are overlapping with each other.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool OverlapsWith(CellRange other) => (((Cells)this) & ((Cells)other)) is not [];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(_mask);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string s = StartCell == 0 ? string.Empty : (StartCell + 1).ToString();
		string e = EndCell switch { 81 => string.Empty, 80 => "^1", 79 => "^2", _ => EndCell.ToString() };
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
			throw new FormatException("The start cell index cannot contain index-negation operator '^'.");
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
			Unsafe.SkipInit(out result);
			return false;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(CellRange left, CellRange right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(CellRange left, CellRange right) => !(left == right);


	/// <summary>
	/// Implicitly cast from <see cref="Range"/> to <see cref="CellRange"/>.
	/// </summary>
	/// <param name="range">The range instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator CellRange(Range range) =>
		new(range.Start.GetOffset(81), range.End.GetOffset(81));

	/// <summary>
	/// Implicitly cast from <see cref="CellRange"/> to <see cref="Cells"/>.
	/// </summary>
	/// <param name="range">The cell range instance.</param>
	public static implicit operator Cells(CellRange range)
	{
		var result = Cells.Empty;
		for (int i = range.StartCell; i < range.EndCell; i++)
		{
			result.Add(i);
		}

		return result;
	}
}
