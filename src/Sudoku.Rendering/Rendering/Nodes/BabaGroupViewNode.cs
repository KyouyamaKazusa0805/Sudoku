namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a Baba group.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell">Indicates the cell used.</param>
/// <param name="digitsMask">Indicates a mask that hold digits used.</param>
/// <param name="unknownValueChar">Indicates the character that represents the baba group name.</param>
[method: JsonConstructor]
public sealed partial class BabaGroupViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter] Cell cell,
	[PrimaryConstructorParameter] Utf8Char unknownValueChar,
	[PrimaryConstructorParameter] Mask digitsMask
) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[ToStringIdentifier(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[ToStringIdentifier(nameof(DigitsMask))]
	private string DigitsMaskString => Convert.ToString(DigitsMask, 2).ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Utf8Char unknownValueChar);

	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Cell cell, out Mask digitsMask, out Utf8Char unknownValueChar);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is BabaGroupViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, "Identifier", "CellString", "DigitsMaskString", "UnknownValueChar")]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
