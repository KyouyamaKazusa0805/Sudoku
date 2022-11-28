namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a wheel view node.
/// </summary>
public sealed partial class WheelViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="WheelViewNode"/> instance via specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digits">The four-number digit, in clockwise order. The first one is displayed on the top cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WheelViewNode(Identifier identifier, int cell, string digits) : base(identifier, cell, Direction.None)
	{
		Argument.ThrowIfFalse(cell / 9 is not (0 or 8) || cell % 9 is not (0 or 8), $"The wheel center cannot lie on boundary row or column.");
		Argument.ThrowIfFalse(int.TryParse(digits, out _) && digits.Length == 4, $"The argument '{nameof(digits)}' must be a four-digit value.");

		DigitString = digits;
	}


	/// <summary>
	/// The digit string. The string is of length 4, as a four-number digit, describing 4 digits surroundding with the target cell
	/// in clockwise order. The first one is displayed on the top cell.
	/// </summary>
	public string DigitString { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is WheelViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && DigitString == comparer.DigitString;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(DigitString))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(WheelViewNode)}} { {{nameof(Cell)}} = {{CellsMap[Cell]}}, {{nameof(DigitString)}} = "{{DigitString}}" }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WheelViewNode Clone() => new(Identifier, Cell, DigitString);
}
