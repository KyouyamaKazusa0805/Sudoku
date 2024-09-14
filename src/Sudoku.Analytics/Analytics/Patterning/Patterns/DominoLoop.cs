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

	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append($"{nameof(Map)} = [{Map}]");
		return true;
	}
}
