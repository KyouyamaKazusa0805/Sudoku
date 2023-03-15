namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a grouped view node.
/// </summary>
/// <param name="identifier">The identifier.</param>
/// <param name="headCell">The head cell.</param>
/// <param name="cells">The cells.</param>
public abstract partial class GroupedViewNode(Identifier identifier, int headCell, ImmutableArray<int> cells) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the head cell. If the node does not use this property, assign -1.
	/// </summary>
	public int HeadCell { get; } = headCell;

	/// <summary>
	/// Indicates the cells used. If the node does not use this property, assign <see cref="ImmutableArray{T}.Empty"/>.
	/// </summary>
	/// <seealso cref="ImmutableArray{T}.Empty"/>
	public ImmutableArray<int> Cells { get; } = cells;


	[DeconstructionMethod]
	public partial void Deconstruct([DeconstructionMethodArgument(nameof(HeadCell))] out int head, out ImmutableArray<int> cells);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is GroupedViewNode comparer
		&& Identifier == comparer.Identifier && HeadCell == comparer.HeadCell && Cells.SequenceEqual(comparer.Cells);

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(HeadCell), nameof(Cells))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(Cells))]
	public override partial string ToString();
}
