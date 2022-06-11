namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell.
/// </summary>
[AutoOverridesToString(nameof(Identifier), nameof(Cell))]
[AutoOverridesGetHashCode(nameof(TypeIdentifier), nameof(Identifier), nameof(Cell))]
[AutoDeconstruction(nameof(Identifier), nameof(Cell))]
public sealed partial class CellViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="CellViewNode"/> instance via the identifier and the highlight cell.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellViewNode(Identifier identifier, int cell) : base(identifier) => Cell = cell;


	/// <summary>
	/// Indicates the cell highlighted.
	/// </summary>
	public int Cell { get; }

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CellViewNode);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CellViewNode comparer && Identifier == comparer.Identifier && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellViewNode Clone() => new(Identifier, Cell);
}
