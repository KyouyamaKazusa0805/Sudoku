namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that uses <b>RxCy</b> rule to format a <see cref="CellMap"/> instance.
/// </summary>
/// <remarks>
/// <inheritdoc cref="RxCyNotation" path="/remarks"/>
/// </remarks>
public sealed record RxCyFormat : ICellMapFormatter
{
	/// <inheritdoc cref="ICellMapFormatter.Instance"/>
	public static readonly RxCyFormat Default = new();


	/// <inheritdoc/>
	static ICellMapFormatter ICellMapFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap) => RxCyNotation.ToCellsString(cellMap);
}
