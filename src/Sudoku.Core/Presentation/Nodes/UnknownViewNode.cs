namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a unknown.
/// </summary>
public sealed class UnknownViewNode : ViewNode
{
	/// <summary>
	/// Initializes an <see cref="UnknownViewNode"/> instance via the specified identifier,
	/// the cell used, the unknown value character and the mask representing the digits used.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell used.</param>
	/// <param name="unknownValueChar">The character that represents the range of the unknown.</param>
	/// <param name="digitsMask">The mask representing digits used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UnknownViewNode(Identifier identifier, int cell, Utf8Char unknownValueChar, short digitsMask) :
		base(identifier) => (Cell, UnknownValueChar, DigitsMask) = (cell, unknownValueChar, digitsMask);


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
	protected override string TypeIdentifier => nameof(UnknownViewNode);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is UnknownViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell == comparer.Cell && DigitsMask == comparer.DigitsMask
		&& UnknownValueChar == comparer.UnknownValueChar;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> HashCode.Combine(TypeIdentifier, Identifier, Cell, DigitsMask, UnknownValueChar);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(UnknownViewNode)}} { {{nameof(Identifier)}} = {{Identifier}}, {{nameof(Cell)}} = {{Cells.Empty + Cell}}, {{nameof(DigitsMask)}} = {{Convert.ToString(DigitsMask, 2)}}, {{nameof(UnknownValueChar)}} = {{UnknownValueChar}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override UnknownViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
