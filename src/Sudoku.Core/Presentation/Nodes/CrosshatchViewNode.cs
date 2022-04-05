namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a crosshatch.
/// </summary>
public sealed class CrosshatchViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="CrosshatchViewNode"/> instance via the specified identifier,
	/// start point, end point cells and the digit used.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="startPoint">The start point of the crosshatch.</param>
	/// <param name="endPoint">The end point of the crosshatch.</param>
	/// <param name="digit">The digit used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CrosshatchViewNode(Identifier identifier, in Cells startPoint, in Cells endPoint, int digit) :
		base(identifier)
	{
		Digit = digit;
		Start = startPoint;
		End = endPoint;
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; }

	/// <summary>
	/// Indicates the start point cells.
	/// </summary>
	public Cells Start { get; }

	/// <summary>
	/// Indicates the end point cells.
	/// </summary>
	public Cells End { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) =>
		other is CrosshatchViewNode comparer
			&& Identifier == comparer.Identifier
			&& Digit == comparer.Digit && Start == comparer.Start && End == comparer.End;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		HashCode.Combine(nameof(CrosshatchViewNode), Identifier, Digit, Start, End);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(CrosshatchViewNode)} {{ {nameof(Digit)} = {Digit + 1}, {nameof(Start)} = {Start}, {nameof(End)} = {End} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CrosshatchViewNode Clone() => new(Identifier, Start, End, Digit);
}
