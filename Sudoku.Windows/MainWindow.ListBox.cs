using System;
using System.Windows.Controls;
using Sudoku.Data.Stepping;
using Sudoku.Solving;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void ListBoxPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_listBoxPaths.SelectedIndex == -1)
			{
				_puzzle = new UndoableGrid(_initialPuzzle);
				UpdateImageGrid();

				e.Handled = true;
				return;
			}

			if (sender is ListBox b && b.SelectedItem is ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<int, TechniqueInfo> pair)
			{
				_cacheAllSteps = null;

				var (n, s) = pair;
				var techniqueInfo = _analyisResult!.SolvingSteps![n];
				_currentPainter.Grid = _puzzle = new UndoableGrid(_analyisResult.StepGrids![n]);
				_currentPainter.View = s.Views[0];
				_currentPainter.Conclusions = techniqueInfo.Conclusions;
				_textBoxInfo.Text = techniqueInfo.ToFullString();

				UpdateImageGrid();
			}
		}
	}
}
