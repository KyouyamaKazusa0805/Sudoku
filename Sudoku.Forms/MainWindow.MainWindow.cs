using System.Windows.Input;
using Sudoku.Drawing.Extensions;
using Sudoku.Forms.Extensions;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (ReferenceEquals(sender, this))
			{
				// Get the current cell.
				var pt = Mouse.GetPosition(_imageGrid);
				var (x, y) = pt;
				if (x < 0 || x > _imageGrid.Width || y < 0 || y > _imageGrid.Height)
				{
					e.Handled = true;
					return;
				}

				// Get all cases for being pressed keys.
				if (e.Key >= Key.D0 && e.Key <= Key.D9)
				{
					int cell = _pointConverter.GetCellOffset(pt.ToDPointF());
					_grid[cell] = e.Key - Key.D1;

					UpdateImageGrid();
				}
			}
		}
	}
}
