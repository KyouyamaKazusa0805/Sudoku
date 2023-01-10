namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about selected cells.
/// </summary>
internal static class SelectedCellConversion
{
	public static int SelectedCellToGridRow_Block(int selectedCell)
		=> selectedCell switch
		{
			-1 => 2,
			_ => selectedCell.ToHouseIndex(HouseType.Block) switch
			{
				0 or 1 or 2 => 2,
				3 or 4 or 5 => 5,
				6 or 7 or 8 => 8
			}
		};

	public static int SelectedCellToGridColumn_Block(int selectedCell)
		=> selectedCell switch
		{
			-1 => 2,
			_ => selectedCell.ToHouseIndex(HouseType.Block) switch
			{
				0 or 3 or 6 => 2,
				1 or 4 or 7 => 5,
				2 or 5 or 8 => 8
			}
		};

	public static int SelectedCellToGridRow_Row(int selectedCell)
		=> selectedCell == -1 ? 2 : selectedCell.ToHouseIndex(HouseType.Row) - 9 + 2;

	public static int SelectedCellToGridColumn_Column(int selectedCell)
		=> selectedCell == -1 ? 2 : selectedCell.ToHouseIndex(HouseType.Column) - 18 + 2;

	public static Visibility SelectedCellToVisibility(int selectedCell) => selectedCell == -1 ? Visibility.Collapsed : Visibility.Visible;
}
