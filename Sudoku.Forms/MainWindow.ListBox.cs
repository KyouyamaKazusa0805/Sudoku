using System;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Layers;
using Sudoku.Solving;
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
				_puzzle = new UndoableGrid((SudokuGrid)_initialPuzzle);
				UpdateImageGrid();

				e.Handled = true;
				return;
			}

			if (!(
				sender is w::Controls.ListBox b
				&& b.SelectedItem is w::Controls.ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<int, TechniqueInfo> pair))
			{
				e.Handled = true;
				return;
			}

			var (n, s) = pair;
			var techniqueInfo = _analyisResult!.SolvingSteps![n];
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName, Settings.CandidateFontName,
					_puzzle = new UndoableGrid((SudokuGrid)_analyisResult.StepGrids![n]),
					Settings.ShowCandidates));
			_layerCollection.Add(
				new ViewLayer(
					_pointConverter, s.Views[0], techniqueInfo.Conclusions, Settings.PaletteColors,
					Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));
			_textBoxInfo.Text = techniqueInfo.ToString();

			UpdateImageGrid();
		}

		private void ListBoxTechniques_SelectionChanged(object sender, w::Controls.SelectionChangedEventArgs e)
		{
			if (sender is w::Controls.ListBox listBox
				&& listBox.SelectedIndex != -1
				&& listBox.SelectedItem is w::Controls.ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<string, TechniqueInfo, bool> triplet)
			{
				var (_, info, isEnabled) = triplet;
				if (isEnabled)
				{
					_layerCollection.Add(
					new ViewLayer(
						_pointConverter, info.Views[0], info.Conclusions, Settings.PaletteColors,
						Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));
					_textBoxInfo.Text = info.ToString();

					UpdateImageGrid();
				}
			}
		}
	}
}
