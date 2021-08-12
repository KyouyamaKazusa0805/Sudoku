using System.Windows;
using Sudoku.Data;

namespace Sudoku.Windows;

partial class App
{
	private partial Window ShowMainWindowDefault() => new MainWindow();

	private partial Window? ShowMainWindowWithGridCode(string str)
	{
		var targetGrid = str switch
		{
			"%empty" => SudokuGrid.Empty,
			_ when SudokuGrid.TryParse(str, out var grid) => grid,
			_ => SudokuGrid.Undefined
		};

		return targetGrid.IsUndefined ? null : new MainWindow(targetGrid);
	}

#if AUTHOR_RESERVED && DEBUG
	private partial Window ShowMainWindowWithDynamic() => new MainWindow(true);
#endif
}
