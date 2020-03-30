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
		/// <summary>
		/// The item source property.
		/// </summary>
		private static DependencyProperty ItemsSourceProperty => w::Controls.ItemsControl.ItemsSourceProperty;


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

			UpdateUndoRedoControls();
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (Puzzle.HasRedoSteps)
			{
				Puzzle.Redo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
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

			_listBoxPaths.ClearValue(ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsSourceProperty);
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			Puzzle.Fix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsSourceProperty);
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			Puzzle.Unfix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsSourceProperty);
		}

		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName, Settings.CandidateFontName,
					_puzzle = new UndoableGrid(_initialPuzzle), Settings.ShowCandidates));
			_layerCollection.Remove(typeof(ViewLayer).Name);

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsSourceProperty);
		}

		private void MenuItemEditClear_Click(object sender, RoutedEventArgs e)
		{
			Puzzle = new UndoableGrid(SudokuGrid.Empty);

			UpdateImageGrid();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateWithSymmetry_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				//if (_comboBoxDifficulty.SelectedIndex == 0)
				//{
				//	MessageBox.Show(
				//		"We may not allow you to generate the puzzle whose difficulty is unknown.", "Warning");
				//
				//	e.Handled = true;
				//	return;
				//}
				//
				//if (_comboBoxDifficulty.SelectedIndex >= 5
				//	&& MessageBox.Show(
				//		"You selected a difficulty that generate a puzzle will be too slow " +
				//		"and you may not cancel the operation when generating. " +
				//		"Would you like to generate anyway?", "Info", MessageBoxButton.YesNo
				//	) != MessageBoxResult.Yes)
				//{
				//	e.Handled = true;
				//	return;
				//}

				DisableGeneratingControls();

				// These two value should be assigned first, rather than 
				// inlining in the asynchronized environment.
				var symmetry = (SymmetricalType)_comboBoxSymmetry.SelectedItem;
				//var diff = (DifficultyLevel)_comboBoxDifficulty.SelectedItem;
				var puzzle = await Task.Run(() => new BasicPuzzleGenerator().Generate(36, symmetry));

				EnableGeneratingControls();

				Puzzle = new UndoableGrid(puzzle);
				_listBoxPaths.ClearValue(ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsSourceProperty);

				UpdateImageGrid();
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				DisableGeneratingControls();

				var puzzle = await Task.Run(new HardPatternPuzzleGenerator().Generate);

				EnableGeneratingControls();

				Puzzle = new UndoableGrid(puzzle);
				_listBoxPaths.ClearValue(ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsSourceProperty);

				UpdateImageGrid();
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		[SuppressMessage("", "IDE0050")]
		private async void MenuItemAnalyzeSolve_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				// Update status.
				_listBoxPaths.ClearValue(ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsSourceProperty);
				_textBoxInfo.Text = "Solving, please wait. During solving you can do some other work...";
				DisableSolvingControls();

				// Run the solver asynchronizedly, during solving you can do other work.
				_analyisResult = await Task.Run(() =>
				{
					if (!Settings.SolveFromCurrent)
					{
						_puzzle.Reset();
						_puzzle.ClearStack();
					}

					return _manualSolver.Solve(_puzzle);
				});

				// Solved. Now update the technique summary.
				EnableSolvingControls();

				if (_analyisResult.HasSolved)
				{
					_textBoxInfo.Text = _analyisResult.ToString(string.Empty);

					int i = 0;
					var pathList = new List<w::Controls.ListBoxItem>();
					foreach (var step in _analyisResult.SolvingSteps!)
					{
						var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
						var item = new w::Controls.ListBoxItem
						{
							Foreground = new w::Media.SolidColorBrush(fore.ToWColor()),
							Background = new w::Media.SolidColorBrush(back.ToWColor()),
							Content = new PrimaryElementTuple<int, TechniqueInfo>(i++, step, 2),
							BorderThickness = new Thickness()
						};
						pathList.Add(item);
					}
					_listBoxPaths.ItemsSource = pathList;

					var collection = new List<AnonymousType>();
					decimal summary = 0, summaryMax = 0;
					int summaryCount = 0;
					foreach (var techniqueGroup in getGroupedSteps())
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

						collection.Add(new { Technique = name, Count = count, Total = total, Max = maximum });
					}

					collection.Add(new
					{
						Technique = default(string?),
						Count = summaryCount,
						Total = summary,
						Max = summaryMax
					});

					w::Controls.GridView view;
					_listViewSummary.ItemsSource = collection;
					_listViewSummary.View = view = new w::Controls.GridView();
					view.Columns.Add(createGridViewColumn("Technique"));
					view.Columns.Add(createGridViewColumn("Count"));
					view.Columns.Add(createGridViewColumn("Total"));
					view.Columns.Add(createGridViewColumn("Max"));
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
			}

			IEnumerable<IGrouping<string, TechniqueInfo>> getGroupedSteps()
			{
				return from solvingStep in _analyisResult.SolvingSteps!
					   orderby solvingStep.Difficulty
					   group solvingStep by solvingStep.Name;
			}

			static w::Controls.GridViewColumn createGridViewColumn(string name)
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
			_manualSolver.CheckConclusionValidityAfterSearched = _menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked = Settings.CheckConclusionValidityAfterSearched ^= true;

		private void MenuItemCheckGurthSymmetricalPlacement_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckGurthSymmetricalPlacement = _menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked = Settings.CheckGurthSymmetricalPlacement ^= true;

		private void MenuItemShowFullHouses_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableFullHouse = _menuItemAnalyzeShowFullHouses.IsChecked = Settings.EnableFullHouse ^= true;

		private void MenuItemShowLastDigits_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableLastDigit = _menuItemAnalyzeShowLastDigits.IsChecked = Settings.EnableLastDigit ^= true;

		private void MenuItemOptimizeApplyingOrder_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.OptimizedApplyingOrder = _menuItemAnalyzeOptimizeApplyingOrder.IsChecked = Settings.OptimizedApplyingOrder ^= true;

		private void MenuItemUseCalculationPriority_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.UseCalculationPriority = _menuItemAnalyzeUseCalculationPriority.IsChecked = Settings.UseCalculationPriority ^= true;

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
