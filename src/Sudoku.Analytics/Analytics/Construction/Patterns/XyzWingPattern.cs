namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents an XYZ-Wing pattern.
/// </summary>
/// <param name="pivot">Indicates the pivot cell.</param>
/// <param name="leafCell1">Indicates the leaf cell 1.</param>
/// <param name="leafCell2">Indicates the leaf cell 2.</param>
/// <param name="house1">Indicates the house 1.</param>
/// <param name="house2">Indicates the house 2.</param>
/// <param name="digitsMask">Indicates all digits.</param>
/// <param name="zDigit">Indicates the digit Z.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class XyzWingPattern(
	[Property, HashCodeMember] Cell pivot,
	[Property, HashCodeMember] Cell leafCell1,
	[Property, HashCodeMember] Cell leafCell2,
	[Property, HashCodeMember] House house1,
	[Property, HashCodeMember] House house2,
	[Property, HashCodeMember] Mask digitsMask,
	[Property] Digit zDigit
) :
	Pattern,
	IFormattable
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => true;

	/// <summary>
	/// Indicates the full pattern of cells.
	/// </summary>
	public CellMap Cells => Pivot.AsCellMap() + LeafCell1 + LeafCell2;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap cells, out Mask digitsMask, out Digit zDigit)
		=> (cells, digitsMask, zDigit) = (Cells, DigitsMask, ZDigit);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Cell pivot, out Cell leafCell1, out Cell leafCell2, out House house1, out House house2, out Mask digitsMask, out Digit zDigit)
		=> (pivot, leafCell1, leafCell2, house1, house2, (_, digitsMask, zDigit)) = (Pivot, LeafCell1, LeafCell2, House1, House2, this);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is XyzWingPattern comparer && Cells == comparer.Cells && DigitsMask == comparer.DigitsMask
		&& House1 == comparer.House1 && House2 == comparer.House2;

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var zDigitStr = converter.DigitConverter((Mask)(1 << ZDigit));
		return $@"{converter.CellConverter(Pivot.AsCellMap() + LeafCell1 + LeafCell2)}({DigitsMask}, {zDigitStr})";
	}

	/// <inheritdoc/>
	public override XyzWingPattern Clone() => new(Pivot, LeafCell1, LeafCell2, House1, House2, DigitsMask, ZDigit);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
