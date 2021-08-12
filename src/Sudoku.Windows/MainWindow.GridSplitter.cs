using System;
using System.Windows.Controls.Primitives;
using Sudoku.Windows.Extensions;

namespace Sudoku.Windows;

partial class MainWindow
{
	private void GridSplitterColumn01_DragDelta(object sender, DragDeltaEventArgs e)
	{
		_imageGrid.Height = _imageGrid.Width =
			Math.Min(_gridMain.ColumnDefinitions[0].ActualWidth, _gridMain.RowDefinitions[0].ActualHeight);
		Settings.GridSize = _gridMain.ColumnDefinitions[0].ActualWidth;
		_currentPainter = new(new(_imageGrid.RenderSize.ToDSizeF()), Settings, _puzzle);

		UpdateImageGrid();
	}

	private void GridSplitterColumn01_DragCompleted(object sender, DragCompletedEventArgs e)
	{
		DisplayDifficultyInfoAfterAnalyzed();
		UpdateImageGrid();
	}
}
