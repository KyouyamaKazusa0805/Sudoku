namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell highlighted.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class CellViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter, HashCodeMember] Cell cell) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	private string CellString => CoordinateConverter.InvariantCultureInstance.CellConverter(in Cell.AsCellMap());


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell) => (identifier, cell) = (Identifier, Cell);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	public override CellViewNode Clone() => new(Identifier, Cell);
}
