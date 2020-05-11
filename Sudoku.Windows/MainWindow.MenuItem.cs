using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if SUDOKU_RECOGNIZING
using System.Drawing;
#endif
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Generating;
using static Sudoku.Windows.Constants.Processing;
using AnonymousType = System.Object;
using DColor = System.Drawing.Color;
using SudokuGrid = Sudoku.Data.Grid;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
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
			sw.Write(_puzzle.ToString("#"));
		}

		private void MenuItemFileOpenDatabase_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".sudokus",
				Filter = "Text file|*.txt|Sudoku database file|*.sudokus|All files|*.*",
				Multiselect = false,
				Title = "Open sudoku file from..."
			};

			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			using var sr = new StreamReader(Settings.CurrentPuzzleDatabase = _database = dialog.FileName);
			_puzzlesText = sr.ReadToEnd().Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

			MessageBox.Show($"Load {_puzzlesText.Length} puzzles.", "Info");
			if (_puzzlesText.Length == 0)
			{
				e.Handled = true;
				return;
			}

			LoadPuzzle(_puzzlesText[Settings.CurrentPuzzleNumber = 0].TrimEnd(Splitter));
			UpdateDatabaseControls(false, false, true, true);

			_textBoxJumpTo.IsEnabled = true;
			_labelPuzzleNumber.Content = $"1/{_puzzlesText.Length}";
		}

		private void MenuItemBackupConfig_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "scfg",
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

#if SUDOKU_RECOGNIZING
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#else
		private void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#endif
		{
#if SUDOKU_RECOGNIZING
			await internalOperation();

			async Task internalOperation()
			{
				if (_recognition is null)
				{
					MessageBox.Show("Failed to initialize. Please restart the window and try again.", "Info");

					e.Handled = true;
					return;
				}

				var dialog = new OpenFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = "PNG file|*.png|JPEG file|*.jpg;*.jpeg|BMP file|*.bmp|GIF file|*.gif",
					Multiselect = false,
					Title = "Load picture from..."
				};

				if (!(dialog.ShowDialog() is true))
				{
					e.Handled = true;
					return;
				}

				try
				{
					if (_recognition.ToolIsInitialized)
					{
						if (MessageBox.Show(
							$"Ensure your picture be clear.{Environment.NewLine}" +
							$"Due to the limitation of the OCR algorithm, " +
							$"the program can only recognize the puzzle picture with given values. " +
							$"All modifiable values will be treated as given ones " +
							$"because OCR engine cannot recognize the color of the value.{Environment.NewLine}" +
							$"If you are not sure, please click NO button.",
							"Info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							_textBoxInfo.Text = "Load picture successfully, now grab all digits in the picture, please wait...";
							using (var bitmap = new Bitmap(dialog.FileName))
							{
								var grid = (await Task.Run(() => _recognition.Recorgnize(bitmap))).ToMutable();
								grid.Fix();
								Puzzle = new UndoableGrid(grid);
							}

							_textBoxInfo.ClearValue(TextBox.TextProperty);

							UpdateUndoRedoControls();
							UpdateImageGrid();
						}
					}
					else
					{
						MessageBox.Show("The OCR tool has not recognized yet.", "Info");
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Warning");
				}
			}
#else
			MessageBox.Show("Your machine cannot use image recognition.", "Info");
#endif
		}

		private void MenuItemFileSavePicture_Click(object sender, RoutedEventArgs e) =>
			new PictureSavingPreferencesWindow(_puzzle, Settings).ShowDialog();

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Save failed due to:{NewLine}{ex.Message}.", "Warning");
			}
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) => Close();

		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _puzzle,
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
			if (_puzzle.HasUndoSteps)
			{
				_puzzle.Undo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle.HasRedoSteps)
			{
				_puzzle.Redo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		private void MenuItemEditRecomputeCandidates_Click(object sender, RoutedEventArgs e)
		{
			int[] z = new int[81];
			for (int cell = 0; cell < 81; cell++)
			{
				z[cell] = _puzzle[cell] + 1;
			}

			var grid = SudokuGrid.CreateInstance(z);
			if (new BitwiseSolver().Solve(grid.ToString(), null, 2) == 0)
			{
				MessageBox.Show(
					"The puzzle is invalid or may be a sukaku. " +
					"If invalid, Please check your input and retry; " +
					"however if sukaku, this function cannot use because the specified puzzle" +
					"has no given or modifiable values.", "Info");
				e.Handled = true;
				return;
			}

			_puzzle = new UndoableGrid(grid);
			_puzzle.Unfix();
			_puzzle.ClearStack();

			UpdateImageGrid();
			UpdateUndoRedoControls();
		}

		private void MenuItemEditCopy_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "0" : ".");

		private void MenuItemEditCopyCurrentGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");

		private void MenuItemEditCopyPmGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.PmGridCompatible ? "@:*!" : "@:*");

		private void MenuItemEditCopyHodokuLibrary_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string z = _puzzle.ToString(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");
				Clipboard.SetText($":0000:x:{z}{(z.Contains(':') ? "::" : ":::")}");
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
		}

		private void MenuItemEditCopyAsSukaku_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.PmGridCompatible ? "~" : "~@");

		private void MenuItemEditCopyAsExcel_Click(object sender, RoutedEventArgs e) => InternalCopy("%");

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

			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditPasteAsSukaku_Click(object sender, RoutedEventArgs e)
		{
			string puzzleStr = Clipboard.GetText();
			if (puzzleStr is null)
			{
				e.Handled = true;
				return;
			}

			try
			{
				Puzzle = new UndoableGrid(SudokuGrid.Parse(puzzleStr, GridParsingOption.Sukaku));

				_menuItemEditUndo.IsEnabled = _menuItemEditRedo.IsEnabled = false;
				UpdateImageGrid();
			}
			catch (ArgumentException)
			{
				MessageBox.Show("The specified puzzle is invalid.", "Warning");
			}

			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Fix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Unfix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
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
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemClearStack_Click(object sender, RoutedEventArgs e)
		{
			if ((_puzzle.HasUndoSteps || _puzzle.HasRedoSteps)
				&& MessageBox.Show(
					$"The steps will be cleared. " +
					$"If so, you cannot undo any steps to previous puzzle status.{Environment.NewLine}" +
					$"Do you want to clear anyway?", "Info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				_puzzle.ClearStack();

				UpdateUndoRedoControls();
			}
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

				if (!(_database is null)
					&& MessageBox.Show(
						"You are now working on the puzzle database. " +
						"If you click YES button, the generating will start, " +
						"but the database history will be cleared." +
						"A hard decision, isn't it?", "Info", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				{
					e.Handled = true;
					return;
				}

				// Disable relative database controls.
				Settings.CurrentPuzzleDatabase = _database = null;
				Settings.CurrentPuzzleNumber = -1;
				UpdateDatabaseControls(false, false, false, false);

				DisableGeneratingControls();

				// These two value should be assigned first, rather than 
				// inlining in the asynchronized environment.
				var symmetry = ((PrimaryElementTuple<string, SymmetryType>)_comboBoxSymmetry.SelectedItem).Value2;
				//var diff = (DifficultyLevel)_comboBoxDifficulty.SelectedItem;
				Puzzle = new UndoableGrid(await Task.Run(() => new BasicPuzzleGenerator().Generate(33, symmetry)));

				EnableGeneratingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				ClearItemSourcesWhenGeneratedOrSolving();
				UpdateImageGrid();
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (!(_database is null)
					&& MessageBox.Show(
						"You are now working on the puzzle database, isn't it? " +
						"If you click YES button, the generating will start, " +
						"but the database history will be cleared. " +
						"That's a hard decision, isn't it?", "Info", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				{
					e.Handled = true;
					return;
				}

				// Disable relative database controls.
				Settings.CurrentPuzzleDatabase = _database = null;
				Settings.CurrentPuzzleNumber = -1;
				UpdateDatabaseControls(false, false, false, false);
				_labelPuzzleNumber.ClearValue(ContentProperty);

				DisableGeneratingControls();

				int depth = _comboBoxBackdoorFilteringDepth.SelectedIndex;
				Puzzle = new UndoableGrid(
					await Task.Run(() => new HardPatternPuzzleGenerator().Generate(depth - 1)));

				EnableGeneratingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				ClearItemSourcesWhenGeneratedOrSolving();
				UpdateImageGrid();
			}
		}

		private void MenuItemAnalyzeSolve_Click(object sender, RoutedEventArgs e)
		{
			if (!applyNormal() && !applySukaku())
			{
				MessageBox.Show("The puzzle is invalid. Please check your input and retry.", "Info");
				return;
			}

			bool applySukaku()
			{
				var sb = new StringBuilder(SudokuGrid.EmptyString);
				string puzzleString = _puzzle.ToString("~");
				if (new SukakuBitwiseSolver().Solve(puzzleString, sb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				Puzzle = new UndoableGrid(SudokuGrid.Parse(sb.ToString()));
				UpdateImageGrid();
				return true;
			}

			bool applyNormal()
			{
				var sb = new StringBuilder(SudokuGrid.EmptyString);
				for (int cell = 0; cell < 81; cell++)
				{
					sb[cell] += (char)(_puzzle[cell] + 1);
				}

				if (new BitwiseSolver().Solve(sb.ToString(), sb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				Puzzle = new UndoableGrid(SudokuGrid.Parse(sb.ToString()));
				UpdateImageGrid();
				return true;
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		[SuppressMessage("", "IDE0050")]
		private async void MenuItemAnalyzeAnalyze_Click(object sender, RoutedEventArgs e)
		{
			if (!await internalOperation(false) && !await internalOperation(true))
			{
				MessageBox.Show("The puzzle is invalid. Please check your input and retry.", "Info");
				e.Handled = true;
				return;
			}

			async Task<bool> internalOperation(bool sukakuMode)
			{
				if (_puzzle.HasSolved)
				{
					MessageBox.Show("The puzzle has already solved.", "Info");
					return !(e.Handled = true);
				}

				var sb = new StringBuilder(SudokuGrid.EmptyString);
				if (sukakuMode)
				{
					string puzzleString = _puzzle.ToString("~");
					if (new SukakuBitwiseSolver().Solve(puzzleString, sb, 2) != 1)
					{
						return !(e.Handled = true);
					}
				}
				else
				{
					for (int cell = 0; cell < 81; cell++)
					{
						sb[cell] += (char)(_puzzle[cell] + 1);
					}

					if (new BitwiseSolver().Solve(sb.ToString(), null, 2) != 1)
					{
						
						return !(e.Handled = true);
					}
				}

				// Update status.
				ClearItemSourcesWhenGeneratedOrSolving();
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
				SwitchOnGeneratingComboBoxesDisplaying();

				if (_analyisResult.HasSolved)
				{
					_textBoxInfo.Text = _analyisResult.ToString(string.Empty);

					int i = 0;
					var pathList = new List<ListBoxItem>();
					foreach (var step in _analyisResult.SolvingSteps!)
					{
						var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
						var item = new ListBoxItem
						{
							Foreground = new SolidColorBrush(fore.ToWColor()),
							Background = new SolidColorBrush(back.ToWColor()),
							Content = new PrimaryElementTuple<int, TechniqueInfo>(i++, step, 2),
							BorderThickness = new Thickness()
						};
						pathList.Add(item);
					}
					_listBoxPaths.ItemsSource = pathList;

					// Gather the information.
					// GridView should list the instance with each property, not fields,
					// even if fields are public.
					// Therefore, here may use anonymous type is okay, but using value tuples
					// is bad.
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

					GridView view;
					_listViewSummary.ItemsSource = collection;
					_listViewSummary.View = view = new GridView();
					view.Columns.AddRange(new[]
					{
						createGridViewColumn("Technique", .6),
						createGridViewColumn("Count", .1),
						createGridViewColumn("Total", .2),
						createGridViewColumn("Max", .1)
					});
					view.AllowsColumnReorder = false;
				}
				else
				{
					MessageBox.Show(
						$"The puzzle cannot be solved because " +
						$"the solver has found a wrong conclusion to apply " +
						$"or else the puzzle has eliminated the correct value.{NewLine}" +
						$"You should check the puzzle or notify the author.{NewLine}" +
						$"Error technique step: {_analyisResult.Additional}",
						"Warning");
				}

				return true;
			}

			IEnumerable<IGrouping<string, TechniqueInfo>> getGroupedSteps()
			{
				return from solvingStep in _analyisResult.SolvingSteps!
					   orderby solvingStep.Difficulty
					   group solvingStep by solvingStep.Name;
			}

			GridViewColumn createGridViewColumn(string name, double widthScale)
			{
				return new GridViewColumn
				{
					Header = name,
					DisplayMemberBinding = new Binding(name),
					Width = _tabControlInfo.ActualWidth * widthScale - 4,
				};
			}
		}

		private void MenuItemAnalyzeSeMode_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.AnalyzeDifficultyStrictly = _menuItemAnalyzeSeMode.IsChecked = Settings.AnalyzeDifficultyStrictly ^= true;

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

		private void MenuItemAnalyzeBugN_Click(object sender, RoutedEventArgs e)
		{
			if (!_puzzle.IsValid(out _))
			{
				MessageBox.Show("The puzzle is invalid.", "Warning");
				e.Handled = true;
				return;
			}

			new BugNSearchWindow(_puzzle).ShowDialog();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemViewsShowBugN_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					MessageBox.Show("The puzzle is invalid.", "Warning");
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text =
					"The program is running as quickly as possible to search " +
					"all non-BUG candidates (true candidates). Please wait.";

				var trueCandidates = await Task.Run(() => new BugChecker(_puzzle).GetAllTrueCandidates(64));

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (trueCandidates.Count == 0)
				{
					MessageBox.Show("The puzzle is not a valid BUG pattern.", "Info");
					e.Handled = true;
					return;
				}

				_layerCollection.Add(
					new ViewLayer(
						_pointConverter,
						new View(
							null,
							new List<(int, int)>(from candidate in trueCandidates select (0, candidate)),
							null,
							null),
						null,
						Settings.PaletteColors, DColor.Empty, DColor.Empty, DColor.Empty));

				UpdateImageGrid();

				_textBoxInfo.Text = $"All true candidate(s): {new CandidateCollection(trueCandidates).ToString()}";
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemViewsBackdoorView_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					MessageBox.Show("The puzzle is invalid.", "Warning");
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text =
					"The program is running as quickly as possible to search " +
					"all backdoors (at level 0 or 1). Please wait.";

				var backdoors = new List<Conclusion>();
				for (int level = 0; level <= 1; level++)
				{
					foreach (var backdoor in
						await Task.Run(() => new BackdoorSearcher().SearchForBackdoorsExact(_puzzle, level)))
					{
						backdoors.AddRange(backdoor);
					}
				}

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (!backdoors.Any())
				{
					MessageBox.Show(
						"The puzzle does not have any backdoors whose level is 0 or 1, " +
						"which means the puzzle can be solved difficultly with brute forces.", "Info");
					e.Handled = true;
					return;
				}

				var highlightCandidates = new List<(int, int)>();
				int currentLevel = 0;
				foreach (var (_, candidate) in backdoors)
				{
					highlightCandidates.Add((currentLevel, candidate));

					currentLevel++;
				}

				_layerCollection.Add(
					new ViewLayer(
						_pointConverter,
						new View(
							null,
							new List<(int, int)>(
								from conclusion in backdoors
								where conclusion.ConclusionType == ConclusionType.Assignment
								select (0, conclusion.CellOffset * 9 + conclusion.Digit)),
							null,
							null),
						backdoors,
						Settings.PaletteColors, Settings.EliminationColor, DColor.Empty, DColor.Empty));

				UpdateImageGrid();

				_textBoxInfo.Text = $"All backdoor(s) at level 0 or 1: {new ConclusionCollection(backdoors).ToString()}";
			}
		}

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) => new AboutMeWindow().Show();

		private void MenuItemAboutSpecialThanks_Click(object sender, RoutedEventArgs e) =>
			new SpecialThanksWindow().Show();

		private void ContextListBoxPathsCopyCurrentStep_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
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
			if (sender is MenuItem)
			{
				var sb = new StringBuilder();
				foreach (string step in from object item in _listBoxPaths.Items select item.ToString())
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

		private void MenuItemImageGridSet1_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridSet2_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridSet3_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridSet4_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridSet5_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridSet6_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridSet7_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridSet8_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridSet9_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);

		private void MenuItemImageGridDelete1_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridDelete2_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridDelete3_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridDelete4_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridDelete5_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridDelete6_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridDelete7_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridDelete8_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridDelete9_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);
	}
}
