namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a chromatic pattern.
/// </summary>
/// <param name="block1Cells">Indicates the cells used in first block.</param>
/// <param name="block2Cells">Indicates the cells used in second block.</param>
/// <param name="block3Cells">Indicates the cells used in third block.</param>
/// <param name="block4Cells">Indicates the cells used in fourth block.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public sealed partial class ChromaticPatternPattern(
	[Property] Cell[] block1Cells,
	[Property] Cell[] block2Cells,
	[Property] Cell[] block3Cells,
	[Property] Cell[] block4Cells
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;

	/// <summary>
	/// Indicates all cells used.
	/// </summary>
	public CellMap Map => [.. Block1Cells, .. Block2Cells, .. Block3Cells, .. Block4Cells];

	[HashCodeMember]
	private int HashCode => Map.GetHashCode();


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Cell[] block1Cells, out Cell[] block2Cells, out Cell[] block3Cells, out Cell[] block4Cells)
		=> (block1Cells, block2Cells, block3Cells, block4Cells) = (Block1Cells, Block2Cells, Block3Cells, Block4Cells);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is ChromaticPatternPattern comparer && Map == comparer.Map;

	/// <inheritdoc/>
	public override ChromaticPatternPattern Clone() => new(Block1Cells, Block2Cells, Block3Cells, Block4Cells);
}
