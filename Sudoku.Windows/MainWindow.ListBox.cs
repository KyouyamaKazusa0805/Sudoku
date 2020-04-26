using System;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Layers;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;

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

			if (!(
				sender is ListBox b && b.SelectedItem is ListBoxItem listBoxItem
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
					_puzzle = new UndoableGrid(_analyisResult.StepGrids![n]),
					Settings.ShowCandidates));
			_layerCollection.Add(
				new ViewLayer(
					_pointConverter, s.Views[0], techniqueInfo.Conclusions, Settings.PaletteColors,
					Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));
			_textBoxInfo.Text = techniqueInfo.ToString();

			UpdateImageGrid();
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
					_layerCollection.Add(
						new ViewLayer(
							_pointConverter, info.Views[0], info.Conclusions, Settings.PaletteColors,
							Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));
					_textBoxInfo.Text = info.ToString();

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
					if (Settings.CheckConclusionValidityAfterSearched
						? CheckConclusionsValidity(new BitwiseSolver().Solve(_puzzle).Solution!, info.Conclusions)
						: true)
					{
						info.ApplyTo(_puzzle);

						_layerCollection.Remove(typeof(ViewLayer).Name);
						_layerCollection.Add(
							new ValueLayer(
								_pointConverter, Settings.ValueScale, Settings.CandidateScale,
								Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
								Settings.GivenFontName, Settings.ModifiableFontName, Settings.CandidateFontName,
								_puzzle, Settings.ShowCandidates));

						_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
						_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
						_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

						UpdateImageGrid();
					}
					else
					{
						MessageBox.Show(
							"The current step is wrong due to wrong calculation. " +
							"Please contact the author", "Info");
					}
				}
			}
		}
	}
}
