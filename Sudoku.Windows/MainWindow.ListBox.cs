using System;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data.Stepping;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Windows.Constants;

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
				var (n, s) = pair;
				var techniqueInfo = _analyisResult!.SolvingSteps![n];
				_currentPainter.Grid = _puzzle = new UndoableGrid(_analyisResult.StepGrids![n]);
				_currentPainter.View = s.Views[0];
				_currentPainter.Conclusions = techniqueInfo.Conclusions;
				_textBoxInfo.Text = techniqueInfo.ToFullString();

				UpdateImageGrid();
			}
		}

		private void ListBoxTechniques_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox && listBox.SelectedIndex != -1
				&& listBox.SelectedItem is ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<string, TechniqueInfo, bool> triplet)
			{
				var (_, info, isEnabled) = triplet;
				if (isEnabled)
				{
					_currentPainter.View = info.Views[0];
					_currentPainter.Conclusions = info.Conclusions;
					_textBoxInfo.Text = info.ToFullString();

					UpdateImageGrid();
				}
			}
		}

		private void ContextMenuListBoxTechniquesApply_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem && _listBoxTechniques.SelectedIndex != -1
				&& _listBoxTechniques.SelectedItem is ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<string, TechniqueInfo, bool> triplet)
			{
				var (_, info, isEnabled) = triplet;
				if (isEnabled)
				{
					if (!Settings.MainManualSolver.CheckConclusionValidityAfterSearched
						|| CheckConclusionsValidity(new BitwiseSolver().Solve(_puzzle).Solution!, info.Conclusions))
					{
						info.ApplyTo(_puzzle);

						_currentPainter.View = null;
						_currentPainter.Grid = _puzzle;

						_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
						_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
						_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

						UpdateImageGrid();
					}
					else
					{
						Messagings.WrongHandling();
					}
				}
			}
		}
	}
}
