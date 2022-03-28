namespace Sudoku.Presentation;

/// <summary>
/// Defines the data structure that stores a set of cells and a digit, indicating the information
/// about the locked candidate node.
/// </summary>
public readonly struct LockedTarget :
	IEquatable<LockedTarget>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<LockedTarget, LockedTarget>
#if FEATURE_GENEIC_MATH_IN_ARG
	,
	IValueEqualityOperators<LockedTarget, LockedTarget>
#endif
#endif
{
	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cells and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cells">Indicates the cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedTarget(int digit, in Cells cells)
	{
		Digit = digit;
		Cells = cells;
	}


	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; }

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public Cells Cells { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is LockedTarget comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in LockedTarget other) => Digit == other.Digit && Cells == other.Cells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Cells, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Locked target: {Digit + 1}{Cells}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<LockedTarget>.Equals(LockedTarget other) => Equals(other);


	/// <summary>
	/// Determines whether two <see cref="LockedTarget"/> instances are considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in LockedTarget left, in LockedTarget right) => left.Equals(right);

	/// <summary>
	/// Determines whether two <see cref="LockedTarget"/> instances are not considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in LockedTarget left, in LockedTarget right) => !(left == right);

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget>.operator ==(LockedTarget left, LockedTarget right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<LockedTarget, LockedTarget>.operator !=(LockedTarget left, LockedTarget right) =>
		left != right;

#if FEATURE_GENERIC_MATH_IN_ARG
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<LockedTarget, LockedTarget>.operator ==(LockedTarget left, in LockedTarget right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<LockedTarget, LockedTarget>.operator ==(in LockedTarget left, LockedTarget right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<LockedTarget, LockedTarget>.operator !=(LockedTarget left, in LockedTarget right) =>
		left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<LockedTarget, LockedTarget>.operator !=(in LockedTarget left, LockedTarget right) =>
		left != right;
#endif
#endif
}
