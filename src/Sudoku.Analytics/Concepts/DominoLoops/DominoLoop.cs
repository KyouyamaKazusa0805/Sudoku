namespace Sudoku.Concepts;

/// <summary>
/// Represents a domino loop pattern.
/// </summary>
/// <param name="Cells">Indicates the cells used.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public readonly partial record struct DominoLoop(Cell[] Cells)
{
	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [.. Cells];


	/// <inheritdoc/>
	public bool Equals(DominoLoop other) => Map == other.Map;

	private bool PrintMembers(StringBuilder stringBuilder)
	{
		stringBuilder.Append($"{nameof(Map)} = [{Map}]");
		return true;
	}
}
