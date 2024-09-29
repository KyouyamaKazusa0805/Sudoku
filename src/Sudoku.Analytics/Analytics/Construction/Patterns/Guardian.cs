namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a guardian technique.
/// </summary>
/// <param name="loopCells">Indicates the cells used in this whole guardian loop.</param>
/// <param name="guardians">Indicates the extra cells that is used as guardians.</param>
/// <param name="digit">Indicates the digit used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class Guardian(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap loopCells,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap guardians,
	[PrimaryConstructorParameter, HashCodeMember] Digit digit
) : IEquatable<Guardian>, IEqualityOperators<Guardian, Guardian, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap loopCells, out CellMap guardians, out Digit digit)
		=> (loopCells, guardians, digit) = (LoopCells, Guardians, Digit);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] Guardian? other)
		=> other is not null && LoopCells == other.LoopCells && Guardians == other.Guardians && Digit == other.Digit;
}
