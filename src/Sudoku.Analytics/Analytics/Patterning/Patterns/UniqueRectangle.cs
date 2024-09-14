namespace Sudoku.Concepts;

/// <summary>
/// Represents a Unique Rectangle.
/// </summary>
/// <param name="cells">The cells.</param>
/// <param name="digitsMask">The digits mask.</param>
/// <param name="otherDigitsMask">The other digits mask.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class UniqueRectangle(
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] Mask otherDigitsMask
) : IEquatable<UniqueRectangle>, IEqualityOperators<UniqueRectangle, UniqueRectangle, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap cells, out Mask digitsMask) => (cells, digitsMask) = (Cells, DigitsMask);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap cells, out Mask digitsMask, out Mask otherDigitsMask)
		=> ((cells, digitsMask), otherDigitsMask) = (this, OtherDigitsMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] UniqueRectangle? other)
		=> other is not null && Cells == other.Cells && DigitsMask == other.DigitsMask && OtherDigitsMask == other.OtherDigitsMask;

	/// <summary>
	/// Try to get all candidates used in the pattern.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All candidates.</returns>
	public CandidateMap GetAllCandidates(ref readonly Grid grid)
	{
		var result = CandidateMap.Empty;
		foreach (var cell in Cells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & OtherDigitsMask))
			{
				result.Add(cell * 9 + digit);
			}
		}
		return result;
	}
}
