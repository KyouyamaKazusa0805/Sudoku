namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a grouped view node.
/// </summary>
public abstract partial class GroupedViewNode : ViewNode
{
	/// <summary>
	/// Assigns the property <see cref="ViewNode.Identifier"/> with target value.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected GroupedViewNode(Identifier identifier) : base(identifier)
	{
	}


	/// <summary>
	/// Indicates the head cell.
	/// </summary>
	public int HeadCell { get; }

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public ImmutableArray<int> Cells { get; }


	[GeneratedDeconstruction]
	public partial void Deconstruct([GeneratedDeconstructionArgument(nameof(HeadCell))] out int head, out ImmutableArray<int> cells);

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
