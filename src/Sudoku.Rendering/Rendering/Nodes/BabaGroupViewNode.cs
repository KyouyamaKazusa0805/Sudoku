namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a Baba group.
/// </summary>
//[method: JsonConstructor]
public sealed partial class BabaGroupViewNode : BasicViewNode//(ColorIdentifier identifier, Cell cell, Utf8Char unknownValueChar, Mask digitsMask) : BasicViewNode(identifier)
{
#pragma warning disable CS1591
	[JsonConstructor]
	public BabaGroupViewNode(ColorIdentifier identifier, Cell cell, Utf8Char unknownValueChar, Mask digitsMask) : base(identifier)
	{
		Cell = cell;
		DigitsMask = digitsMask;
		UnknownValueChar = unknownValueChar;
	}
#pragma warning restore CS1591


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public Cell Cell { get; }// = cell;

	/// <summary>
	/// Indicates the digits used.
	/// </summary>
	public Mask DigitsMask { get; }// = digitsMask;

	/// <summary>
	/// Indicates the character that represents the unknown range.
	/// </summary>
	public Utf8Char UnknownValueChar { get; }// = unknownValueChar;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[GeneratedDisplayName(nameof(DigitsMask))]
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

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString), nameof(DigitsMaskString), nameof(UnknownValueChar))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
