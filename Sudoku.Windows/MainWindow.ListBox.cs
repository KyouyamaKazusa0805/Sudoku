using System;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Solving;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_listBoxPaths.SelectedIndex == -1)
			{
				//_puzzle = new(_initialPuzzle);
				//UpdateImageGrid();

				e.Handled = true;
				return;
			}

			if (
				sender is ListBox
				{
					SelectedItem: ListBoxItem { Content: KeyedTuple<string, int, TechniqueInfo> triplet }
				} && _analyisResult is { SolvingSteps: not null } and { StepGrids: not null })
			{
				var (_, n, s, _) = triplet;
				var techniqueInfo = _analyisResult.SolvingSteps[n];
				_currentTechniqueInfo = techniqueInfo;
				_currentPainter = _currentPainter with
				{
					Grid = _puzzle = _analyisResult.StepGrids[n],
					View = s.Views[_currentViewIndex = 0],
					Conclusions = techniqueInfo.Conclusions
				};

#if OBSOLETE
				_textBoxInfo.Text = techniqueInfo.ToFullString();
#else

				_textBoxInfo.Text = techniqueInfo.ToFullString(Settings.LanguageCode);
#endif

				UpdateImageGrid();
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxTechniques_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (
				sender is ListBox
				{
					SelectedItem: ListBoxItem { Content: KeyedTuple<string, TechniqueInfo, bool> triplet }
				})
			{
				if (triplet.Item3)
				{
					var info = triplet.Item2;
					_currentTechniqueInfo = info;
					_currentPainter = _currentPainter with
					{
						View = info.Views[_currentViewIndex = 0],
						Conclusions = info.Conclusions
					};

#if OBSOLETE
					_textBoxInfo.Text = info.ToFullString();
#else
					_textBoxInfo.Text = info.ToFullString(Settings.LanguageCode);
#endif

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
