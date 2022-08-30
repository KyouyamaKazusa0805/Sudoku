namespace Sudoku.Solving;

/// <summary>
/// Encapsulates a conclusion representation while solving in logic.
/// </summary>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
/// will be greater; if those two hold same conclusion type, but one of those two holds
/// the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <seealso cref="ConclusionType.Elimination"/>
[AutoDeconstruction(nameof(ConclusionType), nameof(Candidate))]
[AutoDeconstruction(nameof(ConclusionType), nameof(Cell), nameof(Digit))]
public readonly partial struct Conclusion :
	IComparable<Conclusion>,
	IComparisonOperators<Conclusion, Conclusion>,
	IEqualityOperators<Conclusion, Conclusion>,
	IEquatable<Conclusion>
{
	/// <summary>
	/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
	/// The bits distribution is like:
	/// <code><![CDATA[
	/// 16       8       0
	///  |-------|-------|
	///  |     |---------|
	/// 16    10         0
	///        |   used  |
	/// ]]></code>
	/// </summary>
	private readonly int _mask;


	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int candidate) : this(((int)type << 10) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int cell, int digit) : this(((int)type << 10) + cell * 9 + digit)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Conclusion"/> instance with the specified mask.
	/// </summary>
	/// <param name="mask">The mask value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Conclusion(int mask) => _mask = mask;


	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _mask & (1 << 10) - 1;
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="ConclusionType.Assignment"/>,
	/// this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(_mask >> 10 & 1);
	}


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTo(scoped ref Grid grid)
	{
		switch (ConclusionType)
		{
			case ConclusionType.Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case ConclusionType.Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Conclusion comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => _mask.CompareTo(_mask);

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _mask;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{CellMap.Empty + Cell}{ConclusionType.GetNotation()}{Digit + 1}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj)
		=> obj is Conclusion comparer
			? CompareTo(comparer)
			: throw new ArgumentException($"The argument must be of type '{nameof(Conclusion)}'", nameof(obj));


	/// <summary>
	/// Determines whether two <see cref="Conclusion"/> instances are considered equal on inner value.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two instances are equal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Conclusion left, Conclusion right) => left.Equals(right);

	/// <summary>
	/// Determines whether two <see cref="Conclusion"/> instances are not equal on inner value.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two instances are not equal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Conclusion left, Conclusion right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(Conclusion left, Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(Conclusion left, Conclusion right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(Conclusion left, Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(Conclusion left, Conclusion right) => left.CompareTo(right) <= 0;
}
