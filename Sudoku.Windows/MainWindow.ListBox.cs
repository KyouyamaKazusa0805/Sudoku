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

			if (sender is ListBox
			{
				SelectedItem: ListBoxItem { Content: PriorKeyedTuple<string, int, TechniqueInfo> triplet }
			})
			{
				_cacheAllSteps = null; // Remove older steps cache while updating paths.

				var (_, n, s, _) = triplet;
				var techniqueInfo = _analyisResult!.SolvingSteps![n];
				_currentTechniqueInfo = techniqueInfo;
				_currentPainter.Grid = _puzzle = new UndoableGrid(_analyisResult.StepGrids![n]);
				_currentPainter.View = s.Views[_currentViewIndex = 0];
				_currentPainter.Conclusions = techniqueInfo.Conclusions;
				_textBoxInfo.Text = techniqueInfo.ToFullString();

				UpdateImageGrid();
			}
		}

		private void ListBoxTechniques_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox
			{
				SelectedItem: ListBoxItem { Content: PriorKeyedTuple<string, TechniqueInfo, bool> triplet }
			})
			{
				if (triplet.Item3)
				{
					var info = triplet.Item2;
					_currentTechniqueInfo = info;
					_currentPainter.View = info.Views[_currentViewIndex = 0];
					_currentPainter.Conclusions = info.Conclusions;
					_textBoxInfo.Text = info.ToFullString();

					UpdateImageGrid();
				}
			}
			else
			{
				_contextMenuTechniques = null;
			}
		}
	}
}
