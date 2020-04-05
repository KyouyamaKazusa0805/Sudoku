using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Data.Extensions;
using Sudoku.Drawing.Extensions;
using Sudoku.Forms.Drawing.Layers;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Image image))
			{
				e.Handled = true;
				return;
			}

			int getCell() => _pointConverter.GetCellOffset(e.GetPosition(image).ToDPointF());
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
			((Action<object, RoutedEventArgs>)(_comboBoxMode.SelectedIndex switch
			{
				0 => (sender, e) => MenuItemGenerateWithSymmetry_Click(sender, e),
				1 => (sender, e) => MenuItemGenerateHardPattern_Click(sender, e),
				_ => throw new NotImplementedException()
			}))(sender, e);
		}

		private void ImageSolve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemAnalyzeSolve_Click(sender, e);

		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			_currentRightClickPos = e.GetPosition(_imageGrid);

			// Disable all menu items.
			for (int i = 0; i < 9; i++)
			{
				((MenuItem)GetType()
					.GetField($"_menuItemImageGridSet{i + 1}", BindingFlags.NonPublic | BindingFlags.Instance)!
					.GetValue(this)!
				).IsEnabled = false;
				((MenuItem)GetType()
					.GetField($"_menuItemImageGridDelete{i + 1}", BindingFlags.NonPublic | BindingFlags.Instance)!
					.GetValue(this)!
				).IsEnabled = false;
			}

			// Then enable some of them.
			foreach (int i in
				_puzzle.GetCandidatesReversal(
					_pointConverter.GetCellOffset(
						_currentRightClickPos.ToDPointF())
					).GetAllSets())
			{
				((MenuItem)GetType()
					.GetField($"_menuItemImageGridSet{i + 1}", BindingFlags.NonPublic | BindingFlags.Instance)!
					.GetValue(this)!
				).IsEnabled = true;
				((MenuItem)GetType()
					.GetField($"_menuItemImageGridDelete{i + 1}", BindingFlags.NonPublic | BindingFlags.Instance)!
					.GetValue(this)!
				).IsEnabled = true;
			}
		}
	}
}
