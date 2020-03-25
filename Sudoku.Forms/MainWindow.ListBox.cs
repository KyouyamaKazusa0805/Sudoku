using System.Windows;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Layers;
using SudokuGrid = Sudoku.Data.Grid;
using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ListBoxPaths_SelectionChanged(object sender, w::Controls.SelectionChangedEventArgs e)
		{
			if (_listBoxPaths.SelectedIndex == -1)
			{
				Puzzle = new UndoableGrid((SudokuGrid)_initialPuzzle);
				UpdateImageGrid();

				e.Handled = true;
				return;
			}

			if (!(
				sender is w::Controls.ListBox b
				&& b.SelectedItem is w::Controls.ListBoxItem listBoxItem
				&& listBoxItem.Content is InfoPair pair))
			{
				e.Handled = true;
				return;
			}

			var (n, s) = pair;
			Puzzle = new UndoableGrid((SudokuGrid)_analyisResult!.StepGrids![n]);
			var techniqueInfo = _analyisResult.SolvingSteps![n];
			_layerCollection.Add(
				new ViewLayer(
					_pointConverter, s.Views[0], techniqueInfo.Conclusions, Settings.ColorDictionary,
					Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));
			_textBoxInfo.Text = techniqueInfo.ToString();

			UpdateImageGrid();
		}

		private void ListBoxPaths_LostFocus(object sender, RoutedEventArgs e)
		{
			if (_initialPuzzle is null)
			{
				e.Handled = true;
				return;
			}

			Puzzle = new UndoableGrid((SudokuGrid)_initialPuzzle);
			UpdateImageGrid();
		}
	}
}
