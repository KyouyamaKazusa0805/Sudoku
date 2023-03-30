namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a figure view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="ViewNode(Identifier)"/></param>
/// <param name="cell">The cell.</param>
public abstract partial class FigureViewNode(Identifier identifier, int cell) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; } = cell;


	[DeconstructionMethod]
	public partial void Deconstruct(out int cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] ViewNode? other) => other is FigureViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public sealed override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell))]
	public sealed override partial string ToString();
}
