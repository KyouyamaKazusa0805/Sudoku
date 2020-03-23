using System.Windows;
using w = System.Windows;
using d = System.Drawing;
using SudokuGrid = Sudoku.Data.Grid;
using Sudoku.Data.Stepping;
using System.Collections.Generic;
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
					_pointConverter, s.Views[0], new Dictionary<int, d::Color>
					{
						[0] = d::Color.FromArgb(255, 134, 242, 128),
						[1] = d::Color.FromArgb(255, 255, 192, 89),
						[2] = d::Color.FromArgb(255, 177, 165, 243),
						[-1] = d::Color.FromArgb(255, 247, 165, 167),
						[-2] = d::Color.FromArgb(255, 134, 232, 208),
					}));

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
