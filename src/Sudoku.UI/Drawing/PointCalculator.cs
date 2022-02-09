using Windows.Foundation;

namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides the methods for the calculations on <see cref="Point"/>.
/// </summary>
/// <param name="PaneSize">Indicates the pane size.</param>
/// <param name="OutsideOffset">Indicates the outside offset.</param>
#if DEBUG
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
#endif
public readonly record struct PointCalculator(double PaneSize, double OutsideOffset) :
	IEquatable<PointCalculator>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<PointCalculator, PointCalculator>
#endif
{
	/// <summary>
	/// Indicates the epsilon that is used for the comparation between two <see cref="PointCalculator"/>s.
	/// </summary>
	public const double Epsilon = 1E-2;


	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public double GridSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => PaneSize - 2 * OutsideOffset;
	}

	/// <summary>
	/// Indicates the block size.
	/// </summary>
	public double BlockSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 3;
	}

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public double CellSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 9;
	}

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public double CandidateSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 27;
	}


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(PointCalculator other) =>
		PaneSize.NearlyEquals(other.PaneSize, Epsilon) && OutsideOffset.NearlyEquals(other.OutsideOffset, Epsilon);

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(PaneSize, OutsideOffset);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(PointCalculator)} {{ {nameof(PaneSize)} = {PaneSize:0.0}, {nameof(OutsideOffset)} = {OutsideOffset:0.0} }}";

	/// <summary>
	/// Gets the first point value of the horizontal border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Point HorizontalBorderLinePoint1(int i, BorderLineType weight) =>
		new(OutsideOffset + i * GridSize / (int)weight, OutsideOffset);

	/// <summary>
	/// Gets the second point value of the horizontal border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Point HorizontalBorderLinePoint2(int i, BorderLineType weight) =>
		new(OutsideOffset + i * GridSize / (int)weight, PaneSize - OutsideOffset);

	/// <summary>
	/// Gets the first point value of the vertical border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Point VerticalBorderLinePoint1(int i, BorderLineType weight) =>
		new(OutsideOffset, OutsideOffset + i * GridSize / (int)weight);

	/// <summary>
	/// Gets the second point value of the vertical border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Point VerticalBorderLinePoint2(int i, BorderLineType weight) =>
		new(PaneSize - OutsideOffset, OutsideOffset + i * GridSize / (int)weight);
}
