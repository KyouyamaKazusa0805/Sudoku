using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Drawing.Layers;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				AddExtension = true,
				DefaultExt = "sudoku",
				Filter = "Text file|*.txt|Sudoku file|*.sudoku|All files|*.*",
				Multiselect = false,
				Title = "Open file from..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sr = new StreamReader(dialog.FileName);
			LoadPuzzle(sr.ReadToEnd());
		}

		private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "sudoku",
				Filter = "Text file|*.txt|Sudoku file|*.sudoku",
				Title = "Save file to..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sw = new StreamWriter(dialog.FileName);
			sw.Write(_grid.ToString("#"));
		}

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Save failed due to:{Environment.NewLine}{ex.Message}.",
					"Warning");
			}
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _grid,
					Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true));

			UpdateImageGrid();
		}

		private void MenuItemOptionsSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(this);
			if (!(settingsWindow.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.CoverBy(settingsWindow.Settings);
			UpdateControls();
		}

		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (_grid.HasUndoSteps)
			{
				_grid.Undo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = _grid.HasUndoSteps;
			_menuItemRedo.IsEnabled = _grid.HasRedoSteps;
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (_grid.HasRedoSteps)
			{
				_grid.Redo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = _grid.HasUndoSteps;
			_menuItemRedo.IsEnabled = _grid.HasRedoSteps;
		}

		private void MenuItemEditCopy_Click(object sender, RoutedEventArgs e) => InternalCopy(null);

		private void MenuItemEditCopyCurrentGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy("#");

		private void MenuItemEditCopyPmGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy("@");

		private void MenuItemEditCopyHodokuLibrary_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetText(_grid.ToString(GridOutputOptions.HodokuCompatible));
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
		}

		private void MenuItemEditPaste_Click(object sender, RoutedEventArgs e)
		{
			string puzzleStr = Clipboard.GetText();
			if (puzzleStr is null)
			{
				// 'value' is not null always.
				e.Handled = true;
				return;
			}

			LoadPuzzle(puzzleStr);
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

		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_grid.Reset();

			UpdateImageGrid();
		}

		private void MenuItemModeSeMode_Click(object sender, RoutedEventArgs e) =>
			_menuItemModeSeMode.IsChecked = Settings.SeMode ^= true;

		private void MenuItemModeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_menuItemModeFastSearch.IsChecked = Settings.FastSearch ^= true;

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMeWindow().Show();
	}
}
