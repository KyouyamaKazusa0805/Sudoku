using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Drawing.Layers;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using w = System.Windows;
using Grid = System.Windows.Controls.Grid;
using Sudoku.Data.Extensions;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				AddExtension = true,
				DefaultExt = "sudoku",
				Filter = "Text file|*.txt|Sudoku file|*.sudoku|All files|*.*",
				Multiselect = false,
				Title = "Open file from..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sr = new StreamReader(dialog.FileName);
			LoadPuzzle(sr.ReadToEnd());
		}

		private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "sudoku",
				Filter = "Text file|*.txt|Sudoku file|*.sudoku",
				Title = "Save file to..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sw = new StreamWriter(dialog.FileName);
			sw.Write(_grid.ToString("#"));
		}

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Save failed due to:{Environment.NewLine}{ex.Message}.",
					"Warning");
			}
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _grid,
					Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true));

			UpdateImageGrid();
		}

		private void MenuItemOptionsSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(this);
			if (!(settingsWindow.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.CoverBy(settingsWindow.Settings);
			UpdateControls();
		}

		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (_grid.HasUndoSteps)
			{
				_grid.Undo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = _grid.HasUndoSteps;
			_menuItemRedo.IsEnabled = _grid.HasRedoSteps;
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (_grid.HasRedoSteps)
			{
				_grid.Redo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = _grid.HasUndoSteps;
			_menuItemRedo.IsEnabled = _grid.HasRedoSteps;
		}

		private void MenuItemEditCopy_Click(object sender, RoutedEventArgs e) => InternalCopy(null);

		private void MenuItemEditCopyCurrentGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy("#");

		private void MenuItemEditCopyPmGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy("@");

		private void MenuItemEditCopyHodokuLibrary_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetText(_grid.ToString(GridOutputOptions.HodokuCompatible));
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
		}

		private void MenuItemEditPaste_Click(object sender, RoutedEventArgs e)
		{
			string puzzleStr = Clipboard.GetText();
			if (puzzleStr is null)
			{
				// 'value' is not null always.
				e.Handled = true;
				return;
			}

			LoadPuzzle(puzzleStr);
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			_grid.Fix();

			UpdateImageGrid();
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			_grid.Unfix();

			UpdateImageGrid();
		}

		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_grid.Reset();

			UpdateImageGrid();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemModeSolve_Click(object sender, RoutedEventArgs e)
		{
			_listBoxPaths.Items.Clear();
			_gridSummary.Children.Clear();

			_textBoxInfo.Text = "Solving...";
			var analysisResult = await Task.Run(() =>
			{
				return new ManualSolver
				{
					FastSearch = Settings.FastSearch,
					AnalyzeDifficultyStrictly = Settings.SeMode
				}.Solve(_grid);
			});

			_gridSummary.RowDefinitions.Clear();
			_textBoxInfo.Text = string.Empty;
			if (analysisResult.HasSolved)
			{
				foreach (var step in analysisResult.SolvingSteps!)
				{
					_listBoxPaths.Items.Add(step);
				}

				var collection = new List<(string?, int, decimal?, decimal?)>();
				decimal summary = 0, summaryMax = 0;
				int summaryCount = 0;
				foreach (var techniqueGroup in GetGroupedSteps())
				{
					string name = techniqueGroup.Key;
					int count = techniqueGroup.Count();
					decimal total = 0, maximum = 0;
					foreach (var step in techniqueGroup)
					{
						summary += step.Difficulty;
						summaryCount++;
						total += step.Difficulty;
						maximum = Math.Max(step.Difficulty, maximum);
						summaryMax = Math.Max(step.Difficulty, maximum);
					}

					collection.Add((name, count, total, maximum));
				}

				collection.Add((null, summaryCount, summary, summaryMax));

				_gridSummary.RowDefinitions.Add(
					new w.Controls.RowDefinition
					{
						Height = new GridLength(FontSize, GridUnitType.Auto)
					});
				_gridSummary.Children.Add(
					CreateLabelInGrid(
						"Technique", HorizontalAlignment.Left, VerticalAlignment.Center, 0, 0));
				_gridSummary.Children.Add(
					CreateLabelInGrid(
						"Count", HorizontalAlignment.Center, VerticalAlignment.Center, 0, 1));
				_gridSummary.Children.Add(
					CreateLabelInGrid(
						"Total", HorizontalAlignment.Center, VerticalAlignment.Center, 0, 2));
				_gridSummary.Children.Add(
					CreateLabelInGrid(
						"Max", HorizontalAlignment.Center, VerticalAlignment.Center, 0, 3));

				int i = 1;
				foreach (ITuple quadruple in collection)
				{
					_gridSummary.RowDefinitions.Add(
						new w::Controls.RowDefinition
						{
							Height = new GridLength(FontSize, GridUnitType.Auto)
						});
					for (int j = 0; j < 4; j++)
					{
						_gridSummary.Children.Add(
							CreateLabelInGrid(
								quadruple[j].NullableToString(),
								j != 0 ? HorizontalAlignment.Center : HorizontalAlignment.Left,
								VerticalAlignment.Center,
								i,
								j));
					}

					i++;
				}
			}
			else
			{
				MessageBox.Show("The puzzle cannot be solved due to internal error.", "Warning");
			}

			IEnumerable<IGrouping<string, TechniqueInfo>> GetGroupedSteps()
			{
				(_, _, var solvingSteps) = analysisResult;
				return from solvingStep in solvingSteps!
					   orderby solvingStep.Difficulty
					   group solvingStep by solvingStep.Name;
			}

			static w::Controls.Label CreateLabelInGrid(
				string content, HorizontalAlignment horizontalAlignment,
				VerticalAlignment verticalAlignment, int row, int column)
			{
				var z = new w::Controls.Label
				{
					Content = content,
					HorizontalAlignment = horizontalAlignment,
					VerticalAlignment = verticalAlignment
				};
				z.SetValue(Grid.RowProperty, row);
				z.SetValue(Grid.ColumnProperty, column);
				return z;
			}
		}

		private void MenuItemModeSeMode_Click(object sender, RoutedEventArgs e) =>
			_menuItemModeSeMode.IsChecked = Settings.SeMode ^= true;

		private void MenuItemModeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_menuItemModeFastSearch.IsChecked = Settings.FastSearch ^= true;

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMeWindow().Show();
	}
}
