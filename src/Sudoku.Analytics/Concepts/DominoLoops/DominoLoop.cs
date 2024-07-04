namespace Sudoku.Concepts;

/// <summary>
/// Represents a domino loop pattern.
/// </summary>
/// <param name="Cells">Indicates the cells used.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public readonly partial record struct DominoLoop(Cell[] Cells)
{
	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [.. Cells];

	[StringMember]
	private string MapString => Map.ToString();


	/// <inheritdoc/>
	public bool Equals(DominoLoop other) => Map == other.Map;
}
