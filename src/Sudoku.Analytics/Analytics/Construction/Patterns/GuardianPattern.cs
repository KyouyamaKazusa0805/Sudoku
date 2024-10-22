namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a guardian technique.
/// </summary>
/// <param name="loopCells">Indicates the cells used in this whole guardian loop.</param>
/// <param name="guardians">Indicates the extra cells that is used as guardians.</param>
/// <param name="digit">Indicates the digit used.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode)]
public sealed partial class GuardianPattern(
	[Property, HashCodeMember] ref readonly CellMap loopCells,
	[Property, HashCodeMember] ref readonly CellMap guardians,
	[Property, HashCodeMember] Digit digit
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;

	/// <inheritdoc/>
	public override PatternType Type => PatternType.Guardian;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap loopCells, out CellMap guardians, out Digit digit)
		=> (loopCells, guardians, digit) = (LoopCells, Guardians, Digit);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is GuardianPattern comaprer
		&& LoopCells == comaprer.LoopCells && Guardians == comaprer.Guardians && Digit == comaprer.Digit;

	/// <inheritdoc/>
	public override GuardianPattern Clone() => new(LoopCells, Guardians, Digit);
}
