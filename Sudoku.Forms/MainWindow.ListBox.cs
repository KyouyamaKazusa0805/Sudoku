using System.Windows;
using w = System.Windows;
using d = System.Drawing;
using SudokuGrid = Sudoku.Data.Grid;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Layers;

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

			if (!(sender is w::Controls.ListBox b && b.SelectedItem is InfoPair pair))
			{
				e.Handled = true;
				return;
			}

			var (n, s) = pair;
			Puzzle = new UndoableGrid((SudokuGrid)_analyisResult!.StepGrids![n]);
			_layerCollection.Add(
				new ViewLayer(
					_pointConverter, s.Views[0], _analyisResult!.SolvingSteps![n].Conclusions,
					Settings.ColorDictionary, Settings.EliminationColor, Settings.CannibalismColor));

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
