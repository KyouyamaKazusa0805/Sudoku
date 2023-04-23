namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell.
/// </summary>
public sealed partial class CellViewNode(ColorIdentifier identifier, Cell cell) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell highlighted.
	/// </summary>
	[JsonInclude]
	public Cell Cell { get; } = cell;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Cell cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellViewNode Clone() => new(Identifier, Cell);
}
