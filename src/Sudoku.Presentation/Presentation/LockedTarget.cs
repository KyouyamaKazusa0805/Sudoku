namespace Sudoku.Presentation;

/// <summary>
/// Defines the data structure that stores a set of cells and a digit, indicating the information
/// about the locked candidate node.
/// </summary>
public readonly struct LockedTarget : IEquatable<LockedTarget>, IEqualityOperators<LockedTarget, LockedTarget, bool>
{
	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cell and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cell">Indicates the cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedTarget(int digit, int cell) : this(digit, CellsMap[cell])
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cells and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cells">Indicates the cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedTarget(int digit, scoped in CellMap cells) => (Digit, Cells) = (digit, cells);


	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	[JsonIgnore]
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; init; }

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; init; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is LockedTarget comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in LockedTarget other) => Digit == other.Digit && Cells == other.Cells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Cells, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(LockedTarget)}} { {{nameof(Digit)}} = {{Digit + 1}}, {{nameof(Cells)}} = {{Cells}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<LockedTarget>.Equals(LockedTarget other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in LockedTarget left, scoped in LockedTarget right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in LockedTarget left, scoped in LockedTarget right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget, bool>.operator ==(LockedTarget left, LockedTarget right)
		=> left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget, bool>.operator !=(LockedTarget left, LockedTarget right)
		=> left != right;
}
