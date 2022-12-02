namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines an average bar view node.
/// </summary>
public sealed partial class AverageBarViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes an <see cref="AverageBarViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="type">The adjacent cell type.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AverageBarViewNode(Identifier identifier, int cell, AdjacentCellType type) : base(identifier, cell, Direction.None) => Type = type;


	/// <summary>
	/// Indicates the adjacent cell type.
	/// </summary>
	public AdjacentCellType Type { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is AverageBarViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Type == comparer.Type;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Type))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Type))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override AverageBarViewNode Clone() => new(Identifier, Cell, Type);
}
