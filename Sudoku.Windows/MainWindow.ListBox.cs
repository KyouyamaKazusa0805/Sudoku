using System;
using System.Windows.Controls;
using Sudoku.DocComments;
using InfoTriplet = System.KeyedTuple<string, Sudoku.Solving.Manual.StepInfo, bool>;
using StepTriplet = System.KeyedTuple<string, int, Sudoku.Solving.Manual.StepInfo>;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_listBoxPaths.SelectedIndex == -1)
			{
				e.Handled = true;
				return;
			}

			if (
				sender is ListBox
				{
					SelectedItem: ListBoxItem { Content: StepTriplet { Item2: var n, Item3: var s } }
				} && _analyisResult is { SolvingSteps: not null, StepGrids: not null })
			{
				var techniqueInfo = _analyisResult.SolvingSteps[n];
				_currentTechniqueInfo = techniqueInfo;
				_currentViewIndex = 0;
				_currentPainter = _currentPainter with
				{
					Grid = _puzzle = _analyisResult.StepGrids[n],
					View = s.Views[0],
					Conclusions = techniqueInfo.Conclusions
				};

				_textBoxInfo.Text = techniqueInfo.ToFullString(Settings.LanguageCode);

				UpdateImageGrid();
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxTechniques_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox { SelectedItem: ListBoxItem { Content: InfoTriplet triplet } })
			{
				if (triplet.Item3 && triplet.Item2 is var info and var (_, _, _, conclusions, views))
				{
					_currentTechniqueInfo = info;
					_currentViewIndex = 0;
					_currentPainter = _currentPainter with { View = views[0], Conclusions = conclusions };

					_textBoxInfo.Text = info.ToFullString(Settings.LanguageCode);

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
