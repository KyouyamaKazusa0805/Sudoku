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
	public UnknownViewNode(Identifier identifier, int cell, byte unknownValueChar, short digitsMask) :
		base(identifier)
	{
		Cell = cell;
		UnknownValueChar = unknownValueChar;
		DigitsMask = digitsMask;
	}


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the character that represents the unknown range.
	/// </summary>
	public byte UnknownValueChar { get; }

	/// <summary>
	/// Indicates the digits used.
	/// </summary>
	public short DigitsMask { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) =>
		other is UnknownViewNode comparer
			&& Identifier == comparer.Identifier
			&& Cell == comparer.Cell && DigitsMask == comparer.DigitsMask
			&& UnknownValueChar == comparer.UnknownValueChar;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		HashCode.Combine(nameof(UnknownViewNode), Identifier, Cell, DigitsMask, UnknownValueChar);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(UnknownViewNode)} {{ {nameof(Cell)} = {Cells.Empty + Cell}, {nameof(DigitsMask)} = {new DigitCollection(DigitsMask).ToString()}, {nameof(UnknownValueChar)} = '{(char)UnknownValueChar}' }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override UnknownViewNode Clone() => new(Identifier, Cell, UnknownValueChar, DigitsMask);
}
