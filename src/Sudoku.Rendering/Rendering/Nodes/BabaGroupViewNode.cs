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
	[PrimaryConstructorParameter, StringMember] Utf8Char unknownValueChar,
	[PrimaryConstructorParameter] Mask digitsMask
) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	private string CellString => RxCyNotation.ToCellString(Cell);

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[StringMember(nameof(DigitsMask))]
	private string DigitsMaskString => Convert.ToString(DigitsMask, 2).ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Utf8Char unknownValueChar);

	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Mask digitsMask, out Utf8Char unknownValueChar);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is BabaGroupViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
