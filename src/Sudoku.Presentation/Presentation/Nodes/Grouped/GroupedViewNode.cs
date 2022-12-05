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
	/// <param name="headCell">The head cell.</param>
	/// <param name="cells">The cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected GroupedViewNode(Identifier identifier, int headCell, ImmutableArray<int> cells) : base(identifier)
		=> (HeadCell, Cells) = (headCell, cells);


	/// <summary>
	/// Indicates the head cell. If the node does not use this property, assign -1.
	/// </summary>
	public int HeadCell { get; }

	/// <summary>
	/// Indicates the cells used. If the node does not use this property, assign <see cref="ImmutableArray{T}.Empty"/>.
	/// </summary>
	/// <seealso cref="ImmutableArray{T}.Empty"/>
	public ImmutableArray<int> Cells { get; }

	/// <inheritdoc/>
	protected sealed override string TypeIdentifier => GetType().Name;


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
