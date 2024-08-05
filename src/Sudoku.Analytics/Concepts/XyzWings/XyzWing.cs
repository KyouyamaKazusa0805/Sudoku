namespace Sudoku.Concepts;

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
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators)]
public sealed partial class XyzWing(
	[PrimaryConstructorParameter, HashCodeMember] Cell pivot,
	[PrimaryConstructorParameter, HashCodeMember] Cell leafCell1,
	[PrimaryConstructorParameter, HashCodeMember] Cell leafCell2,
	[PrimaryConstructorParameter, HashCodeMember] House house1,
	[PrimaryConstructorParameter, HashCodeMember] House house2,
	[PrimaryConstructorParameter, HashCodeMember] Mask digitsMask,
	[PrimaryConstructorParameter] Digit zDigit
) :
	IEquatable<XyzWing>,
	IEqualityOperators<XyzWing, XyzWing, bool>,
	IFormattable
{
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] XyzWing? other)
		=> other is not null && Cells == other.Cells && DigitsMask == other.DigitsMask
		&& House1 == other.House1 && House2 == other.House2;

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var zDigitStr = converter.DigitConverter((Mask)(1 << ZDigit));
		return $@"{converter.CellConverter(Pivot.AsCellMap() + LeafCell1 + LeafCell2)}({DigitsMask}, {zDigitStr})";
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
