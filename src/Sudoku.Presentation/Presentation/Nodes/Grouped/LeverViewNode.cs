namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a lever view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='identifier']"/></param>
/// <param name="headCell"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='headCell']"/></param>
/// <param name="tailCell">The tail cell.</param>
/// <param name="centerCell">The center cell.</param>
public sealed partial class LeverViewNode(
	Identifier identifier,
	int headCell,
	int tailCell,
	int centerCell
) : GroupedViewNode(identifier, headCell, ImmutableArray<int>.Empty)
{
	/// <summary>
	/// Indicates the tail cell.
	/// </summary>
	public int TailCell { get; } = tailCell;

	/// <summary>
	/// Indicates the center cell.
	/// </summary>
	public int CenterCell { get; } = centerCell;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LeverViewNode comparer
		&& Identifier == comparer.Identifier
		&& HeadCell == comparer.HeadCell && TailCell == comparer.TailCell && CenterCell == comparer.CenterCell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(HeadCell), nameof(TailCell), nameof(CenterCell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(TailCell), nameof(CenterCell))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LeverViewNode Clone() => new(Identifier, HeadCell, TailCell, CenterCell);
}
