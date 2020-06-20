using System;
using System.Windows.Controls.Primitives;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void GridSplitterColumn01_DragDelta(object sender, DragDeltaEventArgs e)
		{
			_imageGrid.Height = _imageGrid.Width =
				Math.Min(_gridMain.ColumnDefinitions[0].ActualWidth, _gridMain.RowDefinitions[0].ActualHeight);
			_currentPainter.PointConverter = new PointConverter(_imageGrid.RenderSize);

			UpdateImageGrid();
		}
	}
}
