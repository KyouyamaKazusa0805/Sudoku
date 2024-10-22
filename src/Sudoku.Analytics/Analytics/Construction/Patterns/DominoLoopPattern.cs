namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a domino loop pattern.
/// </summary>
/// <param name="cells">Indicates the cells used.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode | TypeImplFlags.Object_ToString)]
public sealed partial class DominoLoopPattern([Property] Cell[] cells) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;

	/// <inheritdoc/>
	public override PatternType Type => PatternType.DominoLoop;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [.. Cells];

	[StringMember(nameof(Map))]
	private string MapString => Map.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other) => other is DominoLoopPattern comparer && Map == comparer.Map;

	/// <inheritdoc/>
	public override DominoLoopPattern Clone() => new(Cells);
}
