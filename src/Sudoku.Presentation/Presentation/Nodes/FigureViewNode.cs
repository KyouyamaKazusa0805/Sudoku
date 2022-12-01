namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a figure view node.
/// </summary>
public abstract partial class FigureViewNode : ViewNode
{
	/// <summary>
	/// Assigns the values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected FigureViewNode(Identifier identifier, int cell) : base(identifier) => Cell = cell;


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; }


	/// <inheritdoc/>
	protected sealed override string TypeIdentifier => GetType().Name;


	[GeneratedDeconstruction]
	public partial void Deconstruct(out int cell);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is FigureViewNode comparer && Identifier == comparer.Identifier && Cell == comparer.Cell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell))]
	public sealed override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell))]
	public sealed override partial string ToString();
}
