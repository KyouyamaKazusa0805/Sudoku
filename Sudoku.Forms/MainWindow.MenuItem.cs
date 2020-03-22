using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Sudoku.Data.Stepping;
using SudokuGrid = Sudoku.Data.Grid;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem menuItem)
			{
				if (_grid.HasUndoSteps)
				{
					_grid.Undo();
					UpdateImageGrid();
				}

				menuItem.IsEnabled = _grid.HasUndoSteps;
			}
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem menuItem)
			{
				if (_grid.HasRedoSteps)
				{
					_grid.Redo();
					UpdateImageGrid();
				}

				menuItem.IsEnabled = _grid.HasRedoSteps;
			}
		}

		private void MenuIteEditCopy_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetText(_grid.ToString());
			}
			catch (ArgumentNullException ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}", "Warning");
			}
		}

		private void MenuItemEditPaste_Click(object sender, RoutedEventArgs e)
		{
			string value = Clipboard.GetText();
			if (value is null)
			{
				e.Handled = true;
				return;
			}

			try
			{
				_grid = new UndoableGrid(SudokuGrid.Parse(value));

				UpdateImageGrid();
			}
			catch (ArgumentException)
			{
				MessageBox.Show("The specified value from clipboard is invalid grid string.", "Warning");
			}
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			_grid.Fix();

			UpdateImageGrid();
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			_grid.Unfix();

			UpdateImageGrid();
		}

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Save failed due to:{Environment.NewLine}{ex.Message}.", "Warning");
			}
		}

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMe().Show();
	}
}
