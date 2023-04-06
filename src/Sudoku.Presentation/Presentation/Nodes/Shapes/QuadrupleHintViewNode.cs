namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a quadruple hint view node.
/// </summary>
public sealed partial class QuadrupleHintViewNode(Identifier identifier, scoped in CellMap cells, string s) :
	QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="QuadrupleHintViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleHintViewNode(Identifier identifier, int topLeftCell, string s) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10), s)
	{
	}


	/// <summary>
	/// The hint string.
	/// </summary>
	public string Hint { get; } = s;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is QuadrupleHintViewNode comparer && Cells == comparer.Cells;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cells))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cells), nameof(Hint))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override QuadrupleHintViewNode Clone() => new(Identifier, Cells, Hint);
}
