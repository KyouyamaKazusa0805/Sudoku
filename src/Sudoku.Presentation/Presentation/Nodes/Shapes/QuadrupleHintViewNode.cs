namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a quadruple hint view node.
/// </summary>
public sealed partial class QuadrupleHintViewNode : QuadrupleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="QuadrupleHintViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleHintViewNode(Identifier identifier, int topLeftCell, string s) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10), s)
	{
	}

	/// <summary>
	/// Initializes a <see cref="QuadrupleHintViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleHintViewNode(Identifier identifier, scoped in CellMap cells, string s) : base(identifier, cells)
	{
		Argument.ThrowIfFalse(s.Length == 4 && int.TryParse(s, out _), "The hint length must be 4.");

		Hint = s;
	}


	/// <summary>
	/// The hint string.
	/// </summary>
	public string Hint { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is QuadrupleHintViewNode comparer
		&& Identifier == comparer.Identifier && Cells == comparer.Cells && Hint == comparer.Hint;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cells), nameof(Hint))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(QuadrupleHintViewNode)}} { {{nameof(Cells)}} = {{Cells}}, {{nameof(Hint)}} = {{Hint}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override QuadrupleHintViewNode Clone() => new(Identifier, Cells, Hint);
}
