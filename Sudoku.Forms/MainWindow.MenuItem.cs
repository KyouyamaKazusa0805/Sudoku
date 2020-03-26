using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Drawing.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Generating;
using AnonymousType = System.Object;
using SudokuGrid = Sudoku.Data.Grid;
using w = System.Windows;

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
				Title = "Open sudoku file from..."
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
				Title = "Save sudoku file to..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sw = new StreamWriter(dialog.FileName);
			sw.Write(Puzzle.ToString("#"));
		}

		private void MenuItemBackupConfig_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "sudoku",
				Filter = "Configuration file|*.scfg",
				Title = "Save configuration file to..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			try
			{
				SaveConfig(dialog.FileName);
			}
			catch (Exception)
			{
				MessageBox.Show("Configuration file is failed to save due to internal error.", "Warning");
			}
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
					Settings.CandidateFontName, Puzzle,
					Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true));

			UpdateImageGrid();
		}

		private void MenuItemOptionsSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(Settings, _manualSolver);
			if (!(settingsWindow.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.CoverBy(settingsWindow.Settings);
			UpdateControls();
			UpdateImageGrid();
		}

		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (Puzzle.HasUndoSteps)
			{
				Puzzle.Undo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = Puzzle.HasUndoSteps;
			_menuItemRedo.IsEnabled = Puzzle.HasRedoSteps;
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (Puzzle.HasRedoSteps)
			{
				Puzzle.Redo();
				UpdateImageGrid();
			}

			_menuItemUndo.IsEnabled = Puzzle.HasUndoSteps;
			_menuItemRedo.IsEnabled = Puzzle.HasRedoSteps;
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
				Clipboard.SetText(Puzzle.ToString(GridOutputOptions.HodokuCompatible));
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

			_listBoxPaths.Items.Clear();
			_listViewSummary.Items.Clear();
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			Puzzle.Fix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			Puzzle.Unfix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName, Settings.CandidateFontName,
					_puzzle = new UndoableGrid((SudokuGrid)_initialPuzzle), Settings.ShowCandidates));
			_layerCollection.Remove(typeof(ViewLayer).Name);

			UpdateImageGrid();
			_listBoxPaths.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
		{
			DisableGeneratingControls();

			var puzzle = await Task.Run(new HardPatternPuzzleGenerator().Generate);

			EnableGeneratingControls();

			Puzzle = new UndoableGrid((SudokuGrid)puzzle);
			_listBoxPaths.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);

			UpdateImageGrid();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemAnalyzeSolve_Click(object sender, RoutedEventArgs e)
		{
			// Update status.
			_listBoxPaths.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);

			DisableSolvingControls();

			// Run the solver asynchronizedly, during solving you can do other work.
			_analyisResult = await Task.Run(() =>
			{
				if (!Settings.SolveFromCurrent)
				{
					_puzzle.Reset();
				}

				return _manualSolver.Solve(_puzzle);
			});

			// Solved. Now update the technique summary.
			EnableSolvingControls();

			if (_analyisResult.HasSolved)
			{
				int i = 0;
				var pathList = new List<w::Controls.ListBoxItem>();
				foreach (var step in _analyisResult.SolvingSteps!)
				{
					var item = new w::Controls.ListBoxItem();
					var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
					item.Foreground = new w::Media.SolidColorBrush(fore.ToWColor());
					item.Background = new w::Media.SolidColorBrush(back.ToWColor());
					item.Content = new PrimaryElementTuple<int, TechniqueInfo>(i++, step, 2);
					pathList.Add(item);
				}
				_listBoxPaths.ItemsSource = pathList;

				var collection = new List<AnonymousType>();
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

					collection.Add(
						new { Technique = name, Count = count, Total = total, Max = maximum });
				}

				collection.Add(
					new
					{
						Technique = default(string?),
						Count = summaryCount,
						Total = summary,
						Max = summaryMax
					});

				w::Controls.GridView view;
				_listViewSummary.ItemsSource = collection;
				_listViewSummary.View = view = new w::Controls.GridView();
				view.Columns.Add(CreateGridViewColumn("Technique"));
				view.Columns.Add(CreateGridViewColumn("Count"));
				view.Columns.Add(CreateGridViewColumn("Total"));
				view.Columns.Add(CreateGridViewColumn("Max"));
				view.AllowsColumnReorder = false;
			}
			else
			{
				MessageBox.Show(
					$"The puzzle cannot be solved. The cases are below:{Environment.NewLine}" +
					$"    Case 1: The puzzle has no or multiple solutions.{Environment.NewLine}" +
					$"    Case 2: The solver has found a wrong conclusion to apply.{Environment.NewLine}" +
					$"You should check the program or notify the author first.",
					"Warning");
			}

			IEnumerable<IGrouping<string, TechniqueInfo>> GetGroupedSteps()
			{
				(_, _, var solvingSteps) = _analyisResult;
				return from solvingStep in solvingSteps!
					   orderby solvingStep.Difficulty
					   group solvingStep by solvingStep.Name;
			}

			static w::Controls.GridViewColumn CreateGridViewColumn(string name)
			{
				return new w::Controls.GridViewColumn
				{
					Header = name,
					DisplayMemberBinding = new Binding(name)
				};
			}
		}

		private void MenuItemAnalyzeSeMode_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.AnalyzeDifficultyStrictly = _menuItemAnalyzeSeMode.IsChecked = Settings.SeMode ^= true;

		private void MenuItemAnalyzeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.FastSearch = _menuItemAnalyzeFastSearch.IsChecked = Settings.FastSearch ^= true;

		private void MenuItemCheckConclusionValidityAfterSearched_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckConclusionValidityAfterSearched = _menuItemCheckConclusionValidityAfterSearched.IsChecked = Settings.CheckConclusionValidityAfterSearched ^= true;

		private void MenuItemCheckGurthSymmetricalPlacement_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckGurthSymmetricalPlacement = _menuItemCheckGurthSymmetricalPlacement.IsChecked = Settings.CheckGurthSymmetricalPlacement ^= true;

		private void MenuItemDisableSlowTechniques_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.DisableSlowTechniques = _menuItemDisableSlowTechniques.IsChecked = Settings.DisableSlowTechniques ^= true;

		private void MenuItemShowFullHouses_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableFullHouse = _menuItemShowFullHouses.IsChecked = Settings.EnableFullHouse ^= true;

		private void MenuItemShowLastDigits_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableLastDigit = _menuItemShowLastDigits.IsChecked = Settings.EnableLastDigit ^= true;

		private void MenuItemOptimizeApplyingOrder_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.OptimizedApplyingOrder = _menuItemOptimizeApplyingOrder.IsChecked = Settings.OptimizedApplyingOrder ^= true;

		private void MenuItemUseCalculationPriority_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.UseCalculationPriority = _menuItemUseCalculationPriority.IsChecked = Settings.UseCalculationPriority ^= true;

		private void MenuItemExport_Click(object sender, RoutedEventArgs e)
		{
			if (_analyisResult is null)
			{
				MessageBox.Show("You should solve the puzzle first.", "Information");
				e.Handled = true;
				return;
			}

			new ExportAnalysisResultWindow(_analyisResult).Show();
		}

		private void MenuItemAnalyzeBackdoor_Click(object sender, RoutedEventArgs e) =>
			new BackdoorWindow(_puzzle).ShowDialog();

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMeWindow().Show();

		private void ContextListBoxPathsCopyCurrentStep_Click(object sender, RoutedEventArgs e)
		{
			if (sender is w::Controls.MenuItem)
			{
				try
				{
					Clipboard.SetText(_listBoxPaths.SelectedItem.ToString());
				}
				catch
				{
					MessageBox.Show("Cannot copy due to internal error, please try later.", "Warning");
				}
			}
		}

		private void ContextListBoxPathsCopyAllSteps_Click(object sender, RoutedEventArgs e)
		{
			if (sender is w::Controls.MenuItem)
			{
				var sb = new StringBuilder();
				foreach (var step in from object item in _listBoxPaths.Items select item.ToString())
				{
					sb.AppendLine(step);
				}

				try
				{
					Clipboard.SetText(sb.ToString());
				}
				catch
				{
					MessageBox.Show("Cannot copy due to internal error, please try later.", "Warning");
				}
			}
		}
	}
}
