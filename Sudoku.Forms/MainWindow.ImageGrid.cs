using System;
using System.Windows.Input;
using Sudoku.Drawing.Extensions;
using Sudoku.Forms.Drawing.Layers;
using ImageControl = System.Windows.Controls.Image;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is ImageControl imageControl))
			{
				e.Handled = true;
				return;
			}

			int getCell() => _pointConverter.GetCellOffset(e.GetPosition(imageControl).ToDPointF());
			if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				// Select a region of cells.
				int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.SetAt(0);
				int currentClickedCell = getCell();
				int r1 = cell / 9, c1 = cell % 9;
				int r2 = currentClickedCell / 9, c2 = currentClickedCell % 9;
				int minRow = Math.Min(r1, r2), minColumn = Math.Min(c1, c2);
				int maxRow = Math.Max(r1, r2), maxColumn = Math.Max(c1, c2);
				for (int r = minRow; r <= maxRow; r++)
				{
					for (int c = minColumn; c <= maxColumn; c++)
					{
						_focusedCells.Add(r * 9 + c);
					}
				}
			}
			else if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				// Multi-select.
				_focusedCells.Add(getCell());
			}
			else
			{
				_focusedCells.Clear();
				_focusedCells.Add(getCell());
			}

			_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

			UpdateImageGrid();
		}
	}
}
