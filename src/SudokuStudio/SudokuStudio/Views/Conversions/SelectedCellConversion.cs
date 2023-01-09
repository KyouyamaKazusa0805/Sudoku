namespace SudokuStudio.Views.Conversions;

internal static class SelectedCellConversion
{
	public static int SelectedCellToGridRow_Block(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Block)][0] / 9 + 2;

	public static int SelectedCellToGridRow_Row(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Row)][0] / 9 + 2;

	public static int SelectedCellToGridRow_Column(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Column)][0] / 9 + 2;

	public static int SelectedCellToGridColumn_Block(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Block)][^1] % 9 + 2;

	public static int SelectedCellToGridColumn_Row(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Row)][^1] % 9 + 2;

	public static int SelectedCellToGridColumn_Column(int selectedCell)
		=> selectedCell == -1 ? 2 : HousesMap[selectedCell.ToHouseIndex(HouseType.Column)][^1] % 9 + 2;

	public static Visibility SelectedCellToVisibility(int selectedCell) => selectedCell == -1 ? Visibility.Collapsed : Visibility.Visible;
}
