namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a chromatic pattern.
/// </summary>
/// <param name="block1Cells">Indicates the cells used in first block.</param>
/// <param name="block2Cells">Indicates the cells used in second block.</param>
/// <param name="block3Cells">Indicates the cells used in third block.</param>
/// <param name="block4Cells">Indicates the cells used in fourth block.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class ChromaticPattern(
	[PrimaryConstructorParameter] Cell[] block1Cells,
	[PrimaryConstructorParameter] Cell[] block2Cells,
	[PrimaryConstructorParameter] Cell[] block3Cells,
	[PrimaryConstructorParameter] Cell[] block4Cells
) : IEquatable<ChromaticPattern>, IEqualityOperators<ChromaticPattern, ChromaticPattern, bool>
{
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
	public bool Equals([NotNullWhen(true)] ChromaticPattern? other) => other is not null && Map == other.Map;
}
