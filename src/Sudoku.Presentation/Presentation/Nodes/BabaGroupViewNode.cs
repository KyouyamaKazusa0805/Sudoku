namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a Baba group.
/// </summary>
public sealed partial class BabaGroupViewNode : ViewNode
{
	/// <summary>
	/// Initializes an <see cref="BabaGroupViewNode"/> instance via the specified identifier,
	/// the cell used, the unknown value character and the mask representing the digits used.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell used.</param>
	/// <param name="unknownValueChar">The character that represents the range of the unknown.</param>
	/// <param name="digitsMask">The mask representing digits used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BabaGroupViewNode(Identifier identifier, int cell, Utf8Char unknownValueChar, short digitsMask) : base(identifier)
		=> (Cell, UnknownValueChar, DigitsMask) = (cell, unknownValueChar, digitsMask);


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the digits used.
	/// </summary>
	public short DigitsMask { get; }

	/// <summary>
	/// Indicates the character that represents the unknown range.
	/// </summary>
	public Utf8Char UnknownValueChar { get; }

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(BabaGroupViewNode);

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();

	/// <summary>
	/// Indicates the digits mask string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(DigitsMask))]
	private string DigitsMaskString => Convert.ToString(DigitsMask, 2).ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is BabaGroupViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell == comparer.Cell && DigitsMask == comparer.DigitsMask && UnknownValueChar == comparer.UnknownValueChar;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(Cell), nameof(DigitsMask), nameof(UnknownValueChar))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString), nameof(DigitsMaskString), nameof(UnknownValueChar))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BabaGroupViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
