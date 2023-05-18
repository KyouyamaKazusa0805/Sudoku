namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a cell.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell highlighted.</param>
[method: JsonConstructor]
public sealed partial class CellViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter] Cell cell) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the mode that the bound view node will be displayed.
	/// The default value is <see cref="RenderingMode.PencilmarkModeOnly"/>, which means only pencilmark mode the node will be displayed.
	/// </summary>
	public RenderingMode RenderingMode { get; init; } = RenderingMode.PencilmarkModeOnly;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[ToStringIdentifier(nameof(Cell))]
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
