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
				} && _analyisResult is { Steps: not null, StepGrids: not null })
			{
				var techniqueInfo = _analyisResult.Steps[n];
				_currentStepInfo = techniqueInfo;
				_currentViewIndex = 0;

				_currentPainter = _currentPainter with
				{
					Grid = _puzzle = _analyisResult.StepGrids[n],
					View = s.Views[0],
					Conclusions = techniqueInfo.Conclusions
				};

				_textBoxInfo.Text = techniqueInfo.ToString();

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
					_currentStepInfo = info;
					_currentViewIndex = 0;
					_currentPainter.View = views[0];
					_currentPainter.Conclusions = conclusions;

					_textBoxInfo.Text = info.ToString();

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
