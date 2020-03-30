using System;
using System.Windows.Input;
using Sudoku.Drawing.Extensions;
using Sudoku.Forms.Drawing.Layers;
using w = System.Windows;
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

		private void ImageUndoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditUndo_Click(sender, e);

		private void ImageRedoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditRedo_Click(sender, e);

		private void ImageGeneratingIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			((Action<object, w::RoutedEventArgs>)(_comboBoxMode.SelectedIndex switch
			{
				0 => (sender, e) => MenuItemGenerateWithSymmetry_Click(sender, e),
				1 => (sender, e) => MenuItemGenerateHardPattern_Click(sender, e),
				_ => throw new NotImplementedException()
			}))(sender, e);
		}

		private void ImageSolve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemAnalyzeSolve_Click(sender, e);
	}
}
