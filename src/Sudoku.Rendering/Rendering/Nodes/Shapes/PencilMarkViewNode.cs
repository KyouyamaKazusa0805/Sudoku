namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a pencil-mark view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class PencilMarkViewNode(Cell cell, string notation) :
	SingleCellMarkViewNode(WellKnownColorIdentifier.Normal, cell, Direction.None)
{
	/// <summary>
	/// Indicates the notation.
	/// </summary>
	[StringMember]
	public string Notation { get; } = notation;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is PencilMarkViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PencilMarkViewNode Clone() => new(Cell, Notation);
}
