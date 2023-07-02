namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a quadruple hint view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class QuadrupleHintViewNode(
	ColorIdentifier identifier,
	scoped in CellMap cells,
	[PrimaryConstructorParameter(GeneratedMemberName = "Hint"), StringMember] string s
) : QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="QuadrupleHintViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleHintViewNode(ColorIdentifier identifier, Cell topLeftCell, string s) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10), s)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is QuadrupleHintViewNode comparer && Cells == comparer.Cells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override QuadrupleHintViewNode Clone() => new(Identifier, Cells, Hint);
}
