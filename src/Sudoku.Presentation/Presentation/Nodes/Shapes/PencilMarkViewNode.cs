namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a pencil-mark view node.
/// </summary>
public sealed partial class PencilMarkViewNode(int cell, string notation) : SingleCellMarkViewNode(DisplayColorKind.Normal, cell, Direction.None)
{
	/// <summary>
	/// Indicates the notation.
	/// </summary>
	public string Notation { get; } = notation;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is PencilMarkViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Notation))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PencilMarkViewNode Clone() => new(Cell, Notation);
}
