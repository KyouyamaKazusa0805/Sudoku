namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a wheel view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digits">
/// The digit string. The string is of length 4, as a four-number digit,
/// describing 4 digits surrounding with the target cell in clockwise order.
/// The first one is displayed on the top cell.
/// </param>
[GetHashCode]
[ToString]
public sealed partial class WheelViewNode(
	ColorIdentifier identifier,
	Cell cell,
	[PrimaryConstructorParameter(GeneratedMemberName = "DigitString"), StringMember] string digits
) : SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is WheelViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WheelViewNode Clone() => new(Identifier, Cell, DigitString);
}
