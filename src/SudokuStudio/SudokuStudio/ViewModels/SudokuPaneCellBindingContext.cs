namespace SudokuStudio.ViewModels;

/// <summary>
/// Defines a binding context that is used by <see cref="SudokuPaneCell"/>.
/// </summary>
/// <seealso cref="SudokuPaneCell"/>
internal sealed class SudokuPaneCellBindingContext : BindingContext
{
	private GridCellData _cellData = null!;


	/// <summary>
	/// Defines a cell data.
	/// </summary>
	public GridCellData CellData
	{
		get => _cellData;

		set => SetBackingField(ref _cellData, value, static (f, v) => f == v);
	}
}
