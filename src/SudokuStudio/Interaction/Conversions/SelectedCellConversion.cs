using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Sudoku.Concepts;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about selected cells.
/// </summary>
internal static class SelectedCellConversion
{
	public static Offset SelectedCellToGridRow_Block(Cell selectedCell)
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

	public static Offset SelectedCellToGridColumn_Block(Cell selectedCell)
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

	public static Offset SelectedCellToGridRow_Row(Cell selectedCell)
		=> selectedCell == -1 ? 2 : selectedCell.ToHouseIndex(HouseType.Row) - 9 + 2;

	public static Offset SelectedCellToGridColumn_Column(Cell selectedCell)
		=> selectedCell == -1 ? 2 : selectedCell.ToHouseIndex(HouseType.Column) - 18 + 2;

	public static Visibility SelectedCellToVisibility(Cell selectedCell, bool displayCursors)
		=> (displayCursors, selectedCell) switch
		{
			(false, _) => Visibility.Collapsed,
			(_, >= 0 and < 81) => Visibility.Visible,
			_ => Visibility.Collapsed
		};

	public static Brush GetCursorColor(Color color) => new SolidColorBrush(color);
}
