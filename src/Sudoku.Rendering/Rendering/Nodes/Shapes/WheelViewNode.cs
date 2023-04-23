namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a wheel view node.
/// </summary>
public sealed partial class WheelViewNode(ColorIdentifier identifier, Cell cell, string digits) :
	SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <summary>
	/// The digit string. The string is of length 4, as a four-number digit,
	/// describing 4 digits surrounding with the target cell in clockwise order.
	/// The first one is displayed on the top cell.
	/// </summary>
	public string DigitString { get; } = digits;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is WheelViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(CellString), nameof(DigitString))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WheelViewNode Clone() => new(Identifier, Cell, DigitString);
}
