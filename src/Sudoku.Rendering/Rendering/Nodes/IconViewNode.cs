namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines an icon view node that applies to a cell, indicating the icon of the cell. The icons can be used on some sudoku variants.
/// </summary>
/// <param name="identifier"><inheritdoc cref="ViewNode(ColorIdentifier)"/></param>
/// <param name="cell">The cell.</param>
[GetHashCode]
[ToString]
public abstract partial class IconViewNode(ColorIdentifier identifier, [DataMember, HashCodeMember, StringMember] Cell cell) : ViewNode(identifier)
{
	[DeconstructionMethod]
	public partial void Deconstruct(out Cell cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] ViewNode? other) => other is IconViewNode comparer && Cell == comparer.Cell;
}
