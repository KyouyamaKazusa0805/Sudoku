namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a lever view node.
/// </summary>
public sealed partial class LeverViewNode : GroupedViewNode
{
	/// <summary>
	/// Initializes a <see cref="LeverViewNode"/> via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="headCell">The head cell.</param>
	/// <param name="tailCell">The tail cell.</param>
	/// <param name="centerCell">The center cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LeverViewNode(Identifier identifier, int headCell, int tailCell, int centerCell) : base(identifier, headCell, ImmutableArray<int>.Empty)
		=> (TailCell, CenterCell) = (tailCell, centerCell);


	/// <summary>
	/// Indicates the tail cell.
	/// </summary>
	public int TailCell { get; }

	/// <summary>
	/// Indicates the center cell.
	/// </summary>
	public int CenterCell { get; }


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
