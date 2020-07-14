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
				&& listBoxItem.Content is PrimaryElementTuple<string, int, TechniqueInfo> triplet)
			{
				_cacheAllSteps = null; // Remove older steps cache while updating paths.

				var (_, n, s) = triplet;
				var techniqueInfo = _analyisResult!.SolvingSteps![n];
				_currentTechniqueInfo = techniqueInfo;
				_currentPainter.Grid = _puzzle = new UndoableGrid(_analyisResult.StepGrids![n]);
				_currentPainter.View = s.Views[_currentViewIndex = 0];
				_currentPainter.Conclusions = techniqueInfo.Conclusions;
				_textBoxInfo.Text = techniqueInfo.ToFullString();

				UpdateImageGrid();
			}
		}
	}
}
