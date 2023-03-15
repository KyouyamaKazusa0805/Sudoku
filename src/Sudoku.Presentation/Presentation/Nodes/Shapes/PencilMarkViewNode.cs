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
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is PencilMarkViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Notation == comparer.Notation;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Notation))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Notation))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PencilMarkViewNode Clone() => new(Cell, Notation);
}
