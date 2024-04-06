namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a Baba group.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell used.</param>
/// <param name="digitsMask">Indicates a mask that hold digits used.</param>
/// <param name="unknownValueChar">Indicates the character that represents the baba group name.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class BabaGroupViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember] Cell cell,
	[PrimaryConstructorParameter, StringMember] char unknownValueChar,
	[PrimaryConstructorParameter] Mask digitsMask
) : BasicViewNode(identifier)
{
	/// <summary>
	/// Initializes a <see cref="BabaGroupViewNode"/> instance via the specified values.
	/// </summary>
	/// <inheritdoc cref="BabaGroupViewNode(ColorIdentifier, Cell, char, Mask)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BabaGroupViewNode(Cell cell, char unknownValueChar, Mask digitsMask) :
		this(ColorIdentifier.Normal, cell, unknownValueChar, digitsMask)
	{
	}


	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	private string CellString => GlobalizedConverter.InvariantCultureConverter.CellConverter([Cell]);

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[StringMember(nameof(DigitsMask))]
	private string DigitsMaskString => Convert.ToString(DigitsMask, 2).ToString();


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell, out char unknownValueChar)
		=> (identifier, cell, unknownValueChar) = (Identifier, Cell, UnknownValueChar);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Mask digitsMask, out char unknownValueChar)
		=> ((identifier, cell, unknownValueChar), digitsMask) = (this, DigitsMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is BabaGroupViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
