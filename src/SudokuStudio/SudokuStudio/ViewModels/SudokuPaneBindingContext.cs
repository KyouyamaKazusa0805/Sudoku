namespace SudokuStudio.ViewModels;

/// <summary>
/// Provides with a binding context that is used by <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
internal sealed class SudokuPaneBindingContext : BindingContext
{
	/// <summary>
	/// Indicates the cells.
	/// </summary>
	private GridCellData[] _cells = null!;


	/// <summary>
	/// Indicates the internal cells used.
	/// </summary>
	public GridCellData[] Cells
	{
		get => _cells;

		set => SetBackingField(ref _cells, value, static (_, _) => true, static v => v.Length == 81);
	}
}
