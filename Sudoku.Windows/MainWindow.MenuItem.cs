#pragma warning disable IDE1006

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Generating;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Windows.Constants;
using static Sudoku.Windows.Constants.Processings;
using Grid = Sudoku.Data.Grid;
using Sudoku.Drawing;
using Sudoku.Extensions;
#if SUDOKU_RECOGNIZING
using System.Drawing;
#endif

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = "sudoku",
				Filter = (string)LangSource["FilterLoadingPuzzles"],
				Multiselect = false,
				Title = (string)LangSource["TitleLoadingPuzzles"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sr = new StreamReader(dialog.FileName);
				LoadPuzzle(sr.ReadToEnd());
			}
		}

		private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "sudoku",
				Filter = (string)LangSource["FilterSavingPuzzles"],
				Title = (string)LangSource["TitleSavingPuzzles"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sw = new StreamWriter(dialog.FileName);
				sw.Write(_puzzle.ToString("#"));
			}
		}

		private void MenuItemFileOpenDatabase_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = "sudokus",
				Filter = (string)LangSource["FilterOpeningDatabase"],
				Multiselect = false,
				Title = (string)LangSource["TitleOpeningDatabase"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sr = new StreamReader(Settings.CurrentPuzzleDatabase = _database = dialog.FileName);
				_puzzlesText = sr.ReadToEnd().SplitByNewLine();

				Messagings.LoadDatabase(_puzzlesText.Length);

				if (_puzzlesText.Length != 0)
				{
					LoadPuzzle(_puzzlesText[Settings.CurrentPuzzleNumber = 0].TrimEndNewLine());
					UpdateDatabaseControls(false, false, true, true);

					_textBoxJumpTo.IsEnabled = true;
					_labelPuzzleNumber.Content = $"1/{_puzzlesText.Length}";
				}
			}
		}

		private void MenuItemBackupConfig_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "scfg",
				Filter = (string)LangSource["FilterBackupingConfigurations"],
				Title = (string)LangSource["TitleBackupingConfigurations"]
			};

			if (dialog.ShowDialog() is true)
			{
				try
				{
					SaveConfig(dialog.FileName);
				}
				catch
				{
					Messagings.FailedToBackupConfig();
				}
			}
		}

#if SUDOKU_RECOGNIZING
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
					Messagings.FailedToLoadPicture();

					e.Handled = true;
					return;
				}

				var dialog = new OpenFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = (string)LangSource["FilterOpeningPictures"],
					Multiselect = false,
					Title = (string)LangSource["TitleOpeningPictures"]
				};

				if (dialog.ShowDialog() is true)
				{
					try
					{
						if (_recognition.ToolIsInitialized)
						{
							if (Messagings.AskWhileLoadingPicture() == MessageBoxResult.Yes)
							{
								_textBoxInfo.Text = (string)LangSource["TextOpeningPictures"];
								using (var bitmap = new Bitmap(dialog.FileName))
								{
									var grid = await _recognition.RecorgnizeAsync(bitmap);
									grid.Fix();
									Puzzle = new(grid);
								}

								UpdateUndoRedoControls();
								UpdateImageGrid();
							}
						}
						else
						{
							Messagings.FailedToLoadPictureDueToNotHavingInitialized();
						}
					}
					catch (Exception ex)
					{
						Messagings.ShowExceptionMessage(ex);
					}

					_textBoxInfo.ClearValue(TextBox.TextProperty);
				}
			}
#else
			Messagings.NotSupportedForLoadingPicture();
#endif
		}

		private void MenuItemFileSavePicture_Click(object sender, RoutedEventArgs e) =>
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SystemClipboard.Image = (BitmapSource)_imageGrid.Source;
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}

		private void MenuItemFileSaveBatch_Click(object sender, RoutedEventArgs e)
		{
			if (!_puzzle.IsValid(out _))
			{
				Messagings.FailedToCheckDueToInvalidPuzzle();
				e.Handled = true;
				return;
			}

			// Batch.
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog(); // Save puzzle picture.
			MenuItemFileSave_Click(sender, e); // Save puzzle text.
			MenuItemAnalyzeSolve_Click(sender, e); // Solve the puzzle.
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog(); // Save solution picture.
			MenuItemFileSave_Click(sender, e); // Save solution text.
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) => Close();

		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true;
			_currentPainter.Grid = _puzzle;

			UpdateImageGrid();
		}

		private void MenuItemOptionsSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(Settings, _manualSolver);
			if (settingsWindow.ShowDialog() is not true)
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

			var grid = Grid.CreateInstance(z);
			if (new UnsafeBitwiseSolver().Solve(grid.ToString(), null, 2) == 0)
			{
				Messagings.SukakuCannotUseThisFunction();

				e.Handled = true;
				return;
			}

			_puzzle = new(grid);
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
				SystemClipboard.Text = $":0000:x:{z}{(z.Contains(':') ? "::" : ":::")}";
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}

		private void MenuItemEditCopyAsSukaku_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(
				$"~{(Settings.PmGridCompatible ? string.Empty : "@")}" +
				$"{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}");

		private void MenuItemEditCopyAsExcel_Click(object sender, RoutedEventArgs e) => InternalCopy("%");

		private void MenuItemEditPaste_Click(object sender, RoutedEventArgs e)
		{
			if (SystemClipboard.Text is var puzzleStr)
			{
				LoadPuzzle(puzzleStr);

				_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
				_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			}
		}

		private void MenuItemEditPasteAsSukaku_Click(object sender, RoutedEventArgs e)
		{
			if (SystemClipboard.Text is var puzzleStr)
			{
				try
				{
					Puzzle = new(Grid.Parse(puzzleStr, GridParsingOption.Sukaku));

					_menuItemEditUndo.IsEnabled = _menuItemEditRedo.IsEnabled = false;
					UpdateImageGrid();
				}
				catch (ArgumentException)
				{
					Messagings.FailedToPasteText();
				}

				_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
				_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			}
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
			_currentPainter.Grid = _puzzle = new(_initialPuzzle);
			_currentPainter.View = null;

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemClearStack_Click(object sender, RoutedEventArgs e)
		{
			if ((_puzzle.HasUndoSteps || _puzzle.HasRedoSteps)
				&& Messagings.AskWhileClearingStack() == MessageBoxResult.Yes)
			{
				_puzzle.ClearStack();

				UpdateUndoRedoControls();
			}
		}

		private void MenuItemEditClear_Click(object sender, RoutedEventArgs e)
		{
			Puzzle = new(Grid.Empty);
			_analyisResult = null;

			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			_textBoxInfo.ClearValue(TextBox.TextProperty);

			_tabControlInfo.SelectedIndex = 0;

			UpdateImageGrid();
		}

		private async void MenuItemGenerateWithSymmetry_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (_database is null || Messagings.AskWhileGeneratingWithDatabase() == MessageBoxResult.Yes)
				{
					// Disable relative database controls.
					Settings.CurrentPuzzleDatabase = _database = null;
					Settings.CurrentPuzzleNumber = -1;
					UpdateDatabaseControls(false, false, false, false);

					DisableGeneratingControls();

					// These two value should be assigned first, rather than 
					// inlining in the asynchronized environment.
					var dialog = new ProgressWindow();
					dialog.Show();

					int si = _comboBoxSymmetry.SelectedIndex;
					var symmetry = si == 0 ? SymmetryType.None : (SymmetryType)(1 << si - 1);
					//var diff = (DifficultyLevel)_comboBoxDifficulty.SelectedItem;
					Puzzle = new(
						await new BasicPuzzleGenerator().GenerateAsync(
							33, symmetry, dialog.DefaultReporting, Settings.LanguageCode));

					dialog.CloseAnyway();

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

		private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (_database is null || Messagings.AskWhileGeneratingWithDatabase() == MessageBoxResult.Yes)
				{
					// Disable relative database controls.
					Settings.CurrentPuzzleDatabase = _database = null;
					Settings.CurrentPuzzleNumber = -1;
					UpdateDatabaseControls(false, false, false, false);
					_labelPuzzleNumber.ClearValue(ContentProperty);

					DisableGeneratingControls();

					// Here two variables cannot be moved into the lambda expression
					// because the lambda expression will be executed in asynchornized way.
					var dialog = new ProgressWindow();
					dialog.Show();

					int index = _comboBoxBackdoorFilteringDepth.SelectedIndex;
					Settings.GeneratingDifficultyLevelSelectedIndex = _comboBoxDifficulty.SelectedIndex;

					Puzzle =
						new(
							await new HardPatternPuzzleGenerator().GenerateAsync(
								index - 1,
								dialog.DefaultReporting,
								(DifficultyLevel)Settings.GeneratingDifficultyLevelSelectedIndex,
								Settings.LanguageCode
							)
						);

					dialog.CloseAnyway();

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

		private async void MenuItemGenerateWithTechniqueFiltering_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				DisableGeneratingControls();

				var dialog = new ProgressWindow();
				dialog.Show();

				var window = new TechniqueViewWindow();
				if (window.ShowDialog() is true)
				{
					var filter = window.ChosenTechniques;
					if (filter.Count == 0)
					{
						e.Handled = true;
						goto Last;
					}

					Puzzle =
						new(
							await new TechniqueFilteringPuzzleGenerator().GenerateAsync(
								filter, dialog.DefaultReporting, Settings.LanguageCode
							)
						);
				}

			Last:
				dialog.CloseAnyway();

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
				Messagings.FailedToApplyPuzzle();
				e.Handled = true;
				return;
			}

			bool applySukaku()
			{
				var sb = new StringBuilder(Grid.EmptyString);
				if (new UnsafeBitwiseSolver().Solve(
					_puzzle.ToString($"~{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}"), sb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				var grid = Grid.Parse(sb.ToString());
				grid.Unfix();

				Puzzle = new(grid);
				UpdateImageGrid();
				return true;
			}

			bool applyNormal()
			{
				var solutionSb = new StringBuilder();
				string puzzleStr = _puzzle.ToString("0+");
				if (new UnsafeBitwiseSolver().Solve(_puzzle.ToString(), solutionSb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				var newSb = new StringBuilder();
				for (int i = 0, cell = 0, length = puzzleStr.Length; i < length; cell++)
				{
					char c = solutionSb[cell];
					_ = puzzleStr[i] switch
					{
						'+' => (newSb.Append($"+{c}"), i += 2),
						'0' => (newSb.Append($"+{c}"), i++),
						_ => (newSb.Append(c), i++)
					};
				}

				Puzzle = new(Grid.Parse(newSb.ToString()));
				UpdateImageGrid();
				return true;
			}
		}

		private async void MenuItemAnalyzeAnalyze_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle == Grid.Empty)
			{
				Messagings.AnalyzeEmptyGrid();
				e.Handled = true;
				return;
			}

			if (!await internalOperation(false) && !await internalOperation(true))
			{
				Messagings.FailedToApplyPuzzle();
				e.Handled = true;
				return;
			}

			async Task<bool> internalOperation(bool sukakuMode)
			{
				if (_puzzle.HasSolved)
				{
					Messagings.PuzzleAlreadySolved();
					return !(e.Handled = true);
				}

				var sb = new StringBuilder(Grid.EmptyString);
				if (sukakuMode)
				{
					string puzzleString = $"{_puzzle:~}";
					if (new UnsafeBitwiseSolver().Solve(puzzleString, sb, 2) != 1)
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

					if (new UnsafeBitwiseSolver().Solve(sb.ToString(), null, 2) != 1)
					{
						return !(e.Handled = true);
					}
				}

				// Update status.
				ClearItemSourcesWhenGeneratedOrSolving();
				_textBoxInfo.Text = (string)LangSource["WhileSolving"];
				DisableSolvingControls();

				// Run the solver asynchronizedly, during solving you can do other work.
				var dialog = new ProgressWindow();
				dialog.Show();

				_analyisResult =
					await Task.Run(() =>
					{
						if (_puzzle.GivensCount == 0)
						{
							_puzzle.Fix();
						}

						if (!Settings.SolveFromCurrent && !sukakuMode)
						{
							_puzzle.Reset();
						}

						_puzzle.ClearStack();

						return _manualSolver.Solve(_puzzle, dialog.DefaultReporting, Settings.LanguageCode);
					});

				UpdateImageGrid();

				dialog.CloseAnyway();

				// Solved. Now update the technique summary.
				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				DisplayDifficultyInfoAfterAnalyzed();

				return true;
			}
		}

		private void MenuItemAnalyzeSeMode_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.AnalyzeDifficultyStrictly = _menuItemAnalyzeSeMode.IsChecked ^= true;

		private void MenuItemAnalyzeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.FastSearch = _menuItemAnalyzeFastSearch.IsChecked ^= true;

		private void MenuItemCheckConclusionValidityAfterSearched_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckConclusionValidityAfterSearched = _menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked ^= true;

		private void MenuItemCheckGurthSymmetricalPlacement_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckGurthSymmetricalPlacement = _menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked ^= true;

		private void MenuItemShowFullHouses_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableFullHouse = _menuItemAnalyzeShowFullHouses.IsChecked ^= true;

		private void MenuItemShowLastDigits_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableLastDigit = _menuItemAnalyzeShowLastDigits.IsChecked ^= true;

		private void MenuItemOptimizeApplyingOrder_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.OptimizedApplyingOrder = _menuItemAnalyzeOptimizeApplyingOrder.IsChecked ^= true;

		private void MenuItemUseCalculationPriority_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.UseCalculationPriority = _menuItemAnalyzeUseCalculationPriority.IsChecked ^= true;

		private void MenuItemExport_Click(object sender, RoutedEventArgs e)
		{
			if (_analyisResult is null)
			{
				Messagings.YouShouldSolveFirst();
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
				Messagings.FailedToCheckDueToInvalidPuzzle();
				e.Handled = true;
				return;
			}

			new BugNSearchWindow(_puzzle).ShowDialog();
		}

		private unsafe void MenuItemTransformMirrorLeftRight_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.MirrorLeftRight);

		private unsafe void MenuItemTransformMirrorTopBotton_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.MirrorTopBottom);

		private unsafe void MenuItemTransformMirrorDiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.MirrorDiagonal);

		private unsafe void MenuItemTransformMirrorAntidiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.MirrorAntidiagonal);

		private unsafe void MenuItemTransformRotateClockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.RotateClockwise);

		private unsafe void MenuItemTransformRotateCounterclockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.RotateCounterclockwise);

		private unsafe void MenuItemTransformRotatePi_Click(object sender, RoutedEventArgs e) =>
			Transform(&GridTransformations.RotatePi);

		private async void MenuItemViewsShowBugN_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingTrueCandidates"];

				var trueCandidates = await new BugChecker(_puzzle).GetAllTrueCandidatesAsync(64);

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (trueCandidates.Count == 0)
				{
					Messagings.DoesNotContainBugMultiple();
					e.Handled = true;
					return;
				}

				_currentPainter.View =
					new((from candidate in trueCandidates select new DrawingInfo(0, candidate)).ToArray());
				_currentPainter.Conclusions = null;

				UpdateImageGrid();

				_textBoxInfo.Text = $"{LangSource["AllTrueCandidates"]}{new SudokuMap(trueCandidates)}";
			}
		}

		private async void MenuItemViewsBackdoorView_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingBackdoorsLevel0Or1"];

				var backdoors = new List<Conclusion>();
				for (int level = 0; level <= 1; level++)
				{
					foreach (var backdoor in await new BackdoorSearcher().SearchForBackdoorsExactAsync(_puzzle, level))
					{
						backdoors.AddRange(backdoor);
					}
				}

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (backdoors.Count == 0)
				{
					Messagings.DoesNotContainBackdoor();
					e.Handled = true;
					return;
				}

				var highlightCandidates = new List<DrawingInfo>();
				int currentLevel = 0;
				foreach (var (_, candidate) in backdoors)
				{
					highlightCandidates.Add(new(currentLevel, candidate));

					currentLevel++;
				}

				_currentPainter.View = new((
					from backdoor in backdoors
					where backdoor.ConclusionType == ConclusionType.Assignment
					select new DrawingInfo(0, backdoor.Cell * 9 + backdoor.Digit)).ToArray());
				_currentPainter.Conclusions = backdoors;

				UpdateImageGrid();

				_textBoxInfo.Text =
					$"{LangSource["AllBackdoorsAtLevel0Or1"]}" +
					$"{new ConclusionCollection(backdoors).ToString()}";
			}
		}

		private void MenuItemViewsGspView_Click(object sender, RoutedEventArgs e)
		{
			if (Enumerable.Range(0, 81).All(i => _puzzle.GetStatus(i) != CellStatus.Given))
			{
				Messagings.SukakuCannotUseGspChecking();
				e.Handled = true;
				return;
			}

			if (new GspTechniqueSearcher().GetOne(_puzzle) is not GspTechniqueInfo info)
			{
				Messagings.DoesNotContainGsp();
				e.Handled = true;
				return;
			}

			bool[] series = new bool[9];
			int?[] mapping = info.MappingTable;
			var cellOffsets = new List<DrawingInfo>();
			for (int i = 0, p = 0; i < 9; i++)
			{
				if (series[i])
				{
					continue;
				}

				int? value = mapping[i];
				if (value is null)
				{
					continue;
				}

				int j = value.Value;
				(series[i], series[j]) = (true, true);
				for (int cell = 0; cell < 81; cell++)
				{
					int cellValue = _puzzle[cell];
					if (cellValue == i || cellValue == j)
					{
						cellOffsets.Add(new(p, cell));
					}
				}

				p++;
			}

			_textBoxInfo.Text = info.ToString();
			_currentPainter.View = new(cellOffsets, info.Views[0].Candidates, null, null);
			_currentPainter.Conclusions = info.Conclusions;

			UpdateImageGrid();
		}

		private void MenuItemLanguagesChinese_Click(object sender, RoutedEventArgs e) => ChangeLanguage("zh-cn");

		private void MenuItemLanguagesEnglish_Click(object sender, RoutedEventArgs e) => ChangeLanguage("en-us");

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) => new AboutMeWindow().Show();

		private void MenuItemAboutSpecialThanks_Click(object sender, RoutedEventArgs e) =>
			new SpecialThanksWindow().Show();

		private void MenuItemAboutImplementedTechniques_Click(object sender, RoutedEventArgs e) =>
			new TechniquesWindow().Show();
	}
}
