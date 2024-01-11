namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell highlighted.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class CellViewNode(ColorIdentifier identifier, [RecordParameter, HashCodeMember] Cell cell) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the mode that the bound view node will be displayed.
	/// The default value is <see cref="RenderingMode.PencilmarkModeOnly"/>, which means only pencilmark mode the node will be displayed.
	/// </summary>
	public RenderingMode RenderingMode { get; init; } = RenderingMode.PencilmarkModeOnly;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	private string CellString => GlobalizedConverter.InvariantCultureConverter.CellConverter([Cell]);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell) => (identifier, cell) = (Identifier, Cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellViewNode Clone() => new(Identifier, Cell);
}
