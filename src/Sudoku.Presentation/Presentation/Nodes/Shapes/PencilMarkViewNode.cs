namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a pencil-mark view node.
/// </summary>
public sealed partial class PencilMarkViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="PencilMarkViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="notation">The notation.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PencilMarkViewNode(int cell, string notation) : base(DisplayColorKind.Normal, cell, Direction.None) => Notation = notation;


	/// <summary>
	/// Indicates the notation.
	/// </summary>
	public string Notation { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is PencilMarkViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Notation == comparer.Notation;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Notation))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Notation))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PencilMarkViewNode Clone() => new(Cell, Notation);
}
