namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>K9</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="K9Notation" path="/remarks"/>
/// </remarks>
public sealed record K9Format : ICellMapFormatter
{
	/// <inheritdoc cref="ICellMapFormatter.Instance"/>
	public static readonly K9Format Default = new();


	/// <inheritdoc/>
	static ICellMapFormatter ICellMapFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => K9Notation.ToCellsString(cellMap);
}
