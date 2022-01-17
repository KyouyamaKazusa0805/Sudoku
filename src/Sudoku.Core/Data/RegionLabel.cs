namespace Sudoku.Data;

/// <summary>
/// Indicates the region label.
/// </summary>
/// <param name="RegionKind">
/// The <see cref="byte"/> value as the eigenvalue of the type. All possible values are:
/// <list type="table">
/// <item>
/// <term>0</term>
/// <description>The link is <see cref="RegionLabels.Block"/>.</description>
/// </item>
/// <item>
/// <term>1</term>
/// <description>The link is <see cref="RegionLabels.Row"/>.</description>
/// </item>
/// <item>
/// <term>2</term>
/// <description>The link is <see cref="RegionLabels.Column"/>.</description>
/// </item>
/// </list>
/// </param>
public readonly record struct RegionLabel(byte RegionKind) : IRegionLabel<RegionLabel>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(RegionLabel other) => RegionKind == other.RegionKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => RegionKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(RegionLabel other) =>
		RegionKind > other.RegionKind ? 1 : RegionKind < other.RegionKind ? -1 : 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		RegionKind switch
		{
			0 => nameof(RegionLabels.Block),
			1 => nameof(RegionLabels.Row),
			2 => nameof(RegionLabels.Column),
			_ => throw new ArgumentOutOfRangeException(nameof(RegionKind))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj) =>
		obj is not RegionLabel comparer
			? throw new ArgumentException($"The specified argument must be of type '{nameof(RegionLabel)}'.", nameof(obj))
			: CompareTo(comparer);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void ToRegion(int cell, int* ptr)
	{
		ptr[0] = IRegionLabel<RegionLabel>.BlockTable[cell];
		ptr[1] = IRegionLabel<RegionLabel>.RowTable[cell];
		ptr[2] = IRegionLabel<RegionLabel>.ColumnTable[cell];
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToRegion(int cell, RegionLabel label) =>
		label.RegionKind switch
		{
			0 => IRegionLabel<RegionLabel>.BlockTable[cell],
			1 => IRegionLabel<RegionLabel>.RowTable[cell],
			2 => IRegionLabel<RegionLabel>.ColumnTable[cell]
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RegionLabel ToLabel(int region) => (RegionLabel)(byte)(region / 9);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(RegionLabel left, RegionLabel right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(RegionLabel left, RegionLabel right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(RegionLabel left, RegionLabel right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(RegionLabel left, RegionLabel right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RegionLabel operator ++(RegionLabel label) =>
		label.RegionKind switch
		{
			0 => RegionLabels.Row,
			1 => RegionLabels.Column,
			_ => throw new InvalidOperationException("The value is invalid.")
		};


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator RegionLabel(byte value) => new(value);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator byte(RegionLabel regionLabel) => regionLabel.RegionKind;
}
