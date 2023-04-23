namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines an icon view node that applies to a cell, indicating the icon of the cell. The icons can be used on some sudoku variants.
/// </summary>
/// <param name="identifier"><inheritdoc cref="ViewNode(ColorIdentifier)"/></param>
/// <param name="cell">The cell.</param>
public abstract partial class IconViewNode(ColorIdentifier identifier, Cell cell) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public Cell Cell { get; } = cell;


	[DeconstructionMethod]
	public partial void Deconstruct(out Cell cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] ViewNode? other) => other is IconViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public sealed override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell))]
	public sealed override partial string ToString();
}
