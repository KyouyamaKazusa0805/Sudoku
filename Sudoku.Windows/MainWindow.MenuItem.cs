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
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Generating;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Windows.Constants;
using System.Extensions;
#if SUDOKU_RECOGNITION
using System.Drawing;
#endif

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
#if SUDOKU_RECOGNITION
		private async void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#else
		private void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#endif
		{
#if SUDOKU_RECOGNITION
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
						if (_recognition.IsInitialized)
						{
							if (Messagings.AskWhileLoadingPicture() == MessageBoxResult.Yes)
							{
								_textBoxInfo.Text = (string)LangSource["TextOpeningPictures"];
								using (var bitmap = new Bitmap(dialog.FileName))
								{
									var grid = await _recognition.RecorgnizeAsync(bitmap);
									grid.Fix();
									Puzzle = grid;
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemFileSavePicture_Click(object sender, RoutedEventArgs e) =>
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemFileSaveBatch_Click(object sender, RoutedEventArgs e)
		{
			if (!((SudokuGrid)_puzzle).IsValid())
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) => Close();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true;
			//_currentPainter = _currentPainter with { Grid = _puzzle };

			UpdateImageGrid();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle.HasUndoSteps)
			{
				_puzzle.Undo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle.HasRedoSteps)
			{
				_puzzle.Redo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditRecomputeCandidates_Click(object sender, RoutedEventArgs e)
		{
			int[] z = new int[81];
			for (int cell = 0; cell < 81; cell++)
			{
				z[cell] = _puzzle[cell] + 1;
			}

			var grid = SudokuGrid.CreateInstance(z, GridCreatingOption.MinusOne);
			if (new UnsafeBitwiseSolver().Solve(grid.ToString(), null, 2) == 0)
			{
				Messagings.SukakuCannotUseThisFunction();

				e.Handled = true;
				return;
			}

			_puzzle = grid;
			_puzzle.Unfix();
			_puzzle.ClearStack();

			UpdateImageGrid();
			UpdateUndoRedoControls();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopy_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "0" : ".");

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopyCurrentGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopyPmGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.PmGridCompatible ? "@:*!" : "@:*");

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopyHodokuLibrary_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string z = _puzzle.ToString(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");
				SystemClipboard.DataObject = $":0000:x:{z}{(z.Contains(':') ? "::" : ":::")}";
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopyAsSukaku_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(
				$"~{(Settings.PmGridCompatible ? string.Empty : "@")}" +
				$"{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}");

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditCopyAsExcel_Click(object sender, RoutedEventArgs e) => InternalCopy("%");

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditPasteAsSukaku_Click(object sender, RoutedEventArgs e)
		{
			if (SystemClipboard.Text is var puzzleStr)
			{
				try
				{
					Puzzle = SudokuGrid.Parse(puzzleStr, GridParsingOption.Sukaku);

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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Fix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Unfix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_currentPainter = _currentPainter with { Grid = _puzzle = _initialPuzzle, View = null };

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemClearStack_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle is { HasUndoSteps: true } or { HasRedoSteps: true }
				&& Messagings.AskWhileClearingStack() == MessageBoxResult.Yes)
			{
				_puzzle.ClearStack();

				UpdateUndoRedoControls();
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemEditClear_Click(object sender, RoutedEventArgs e)
		{
			Puzzle = SudokuGrid.Empty;
			_analyisResult = null;

			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			_textBoxInfo.ClearValue(TextBox.TextProperty);

			_tabControlInfo.SelectedIndex = 0;

			UpdateImageGrid();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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
					Puzzle =
						await new BasicPuzzleGenerator().GenerateAsync(
							33, symmetry, dialog.DefaultReporting, Settings.LanguageCode
						);

					dialog.CloseAnyway();

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

					// Here two variables can't be moved into the lambda expression
					// because the lambda expression will be executed in asynchornized way.
					var dialog = new ProgressWindow();
					dialog.Show();

					int index = _comboBoxBackdoorFilteringDepth.SelectedIndex;
					Settings.GeneratingDifficultyLevelSelectedIndex = _comboBoxDifficulty.SelectedIndex;

					Puzzle =
						await new HardPatternPuzzleGenerator().GenerateAsync(
							index - 1,
							dialog.DefaultReporting,
							(DifficultyLevel)Settings.GeneratingDifficultyLevelSelectedIndex,
							Settings.LanguageCode
						);

					dialog.CloseAnyway();

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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
						await new TechniqueFilteringPuzzleGenerator().GenerateAsync(
							filter, dialog.DefaultReporting, Settings.LanguageCode
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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
				var sb = new StringBuilder(SudokuGrid.EmptyString);
				if (new UnsafeBitwiseSolver().Solve(
					_puzzle.ToString($"~{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}"), sb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				var grid = SudokuGrid.Parse(sb.ToString());
				grid.Unfix();

				Puzzle = grid;
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
						'+' => (newSb.Append('+').Append(c), i += 2),
						'0' => (newSb.Append('+').Append(c), i++),
						_ => (newSb.Append(c), i++)
					};
				}

				Puzzle = SudokuGrid.Parse(newSb.ToString());
				UpdateImageGrid();
				return true;
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void MenuItemAnalyzeAnalyze_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle == SudokuGrid.Empty)
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

				var sb = new StringBuilder(SudokuGrid.EmptyString);
				if (sukakuMode)
				{
					string puzzleString = _puzzle.ToString("~");
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

				if (_puzzle.GivensCount == 0)
				{
					_puzzle.Fix();
				}

				if ((Settings.SolveFromCurrent, sukakuMode) == (false, false))
				{
					_puzzle.Reset();
				}

				_puzzle.ClearStack();

				_analyisResult =
					await _manualSolver.SolveAsync(
						(SudokuGrid)_puzzle, dialog.DefaultReporting, Settings.LanguageCode
					);

				UpdateImageGrid();

				dialog.CloseAnyway();

				// Solved. Now update the technique summary.
				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				DisplayDifficultyInfoAfterAnalyzed();

				return true;
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAnalyzeSeMode_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.AnalyzeDifficultyStrictly = _menuItemAnalyzeSeMode.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAnalyzeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.FastSearch = _menuItemAnalyzeFastSearch.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemCheckConclusionValidityAfterSearched_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckConclusionValidityAfterSearched = _menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemCheckGurthSymmetricalPlacement_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckGurthSymmetricalPlacement = _menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemShowFullHouses_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableFullHouse = _menuItemAnalyzeShowFullHouses.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemShowLastDigits_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableLastDigit = _menuItemAnalyzeShowLastDigits.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemOptimizeApplyingOrder_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.OptimizedApplyingOrder = _menuItemAnalyzeOptimizeApplyingOrder.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemUseCalculationPriority_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.UseCalculationPriority = _menuItemAnalyzeUseCalculationPriority.IsChecked ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemExport_Click(object sender, RoutedEventArgs e)
		{
			if (_analyisResult is null)
			{
				Messagings.YouShouldSolveFirst();
				e.Handled = true;
				return;
			}

			new ExportAnalysisResultWindow(_analyisResult, Settings).Show();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAnalyzeBackdoor_Click(object sender, RoutedEventArgs e) =>
			new BackdoorWindow((SudokuGrid)_puzzle).ShowDialog();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAnalyzeBugN_Click(object sender, RoutedEventArgs e)
		{
			var valueGrid = (SudokuGrid)_puzzle;
			if (!valueGrid.IsValid())
			{
				Messagings.FailedToCheckDueToInvalidPuzzle();
				e.Handled = true;
				return;
			}

			new BugNSearchWindow(valueGrid).ShowDialog();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemToolsBatch_Click(object sender, RoutedEventArgs e) =>
			new BatchWindow(Settings).Show();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformMirrorLeftRight_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.MirrorLeftRight);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformMirrorTopBotton_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.MirrorTopBottom);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformMirrorDiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.MirrorDiagonal);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformMirrorAntidiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.MirrorAntidiagonal);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformRotateClockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.RotateClockwise);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformRotateCounterclockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.RotateCounterclockwise);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemTransformRotatePi_Click(object sender, RoutedEventArgs e) =>
			Transform(&SudokuGridTransformations.RotatePi);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void MenuItemViewsShowBugN_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				var valueGrid = (SudokuGrid)_puzzle;
				if (!valueGrid.IsValid())
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingTrueCandidates"];

				var trueCandidates = await new BugChecker(valueGrid).GetAllTrueCandidatesAsync(64);

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (trueCandidates.Count == 0)
				{
					Messagings.DoesNotContainBugMultiple();
					e.Handled = true;
					return;
				}

				_currentPainter = _currentPainter with
				{
					View = new((from candidate in trueCandidates select new DrawingInfo(0, candidate)).ToArray()),
					Conclusions = null
				};

				UpdateImageGrid();

				_textBoxInfo.Text = $"{LangSource["AllTrueCandidates"]}{new SudokuMap(trueCandidates)}";
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void MenuItemViewsBackdoorView_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				var valueGrid = (SudokuGrid)_puzzle;
				if (!valueGrid.IsValid())
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingBackdoorsLevel0Or1"];

				var backdoors = new List<Conclusion>();

				async Task r(int level)
				{
					foreach (var backdoor in
						await new BackdoorSearcher().SearchForBackdoorsExactAsync(valueGrid, level))
					{
						backdoors.AddRange(backdoor);
					}
				}

				await r(0);
				await r(1);

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

				_currentPainter = _currentPainter with
				{
					View =
						new(
						(
							from backdoor in backdoors
							where backdoor.ConclusionType == ConclusionType.Assignment
							select new DrawingInfo(0, backdoor.Cell * 9 + backdoor.Digit)
						).ToArray()),
					Conclusions = backdoors
				};

				UpdateImageGrid();

				_textBoxInfo.Text = $"{LangSource["AllBackdoorsAtLevel0Or1"]}{new ConclusionCollection(backdoors).ToString()}";
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private unsafe void MenuItemViewsGspView_Click(object sender, RoutedEventArgs e)
		{
			if (Enumerable.Range(0, 81).All(&internalChecking, this))
			{
				Messagings.SukakuCannotUseGspChecking();
				e.Handled = true;
				return;
			}

			static bool internalChecking(int i, in MainWindow @this) =>
				@this._puzzle.GetStatus(i) != CellStatus.Given;

			if (new GspStepSearcher().GetOne((SudokuGrid)_puzzle) is not GspStepInfo info)
			{
				Messagings.DoesNotContainGsp();
				e.Handled = true;
				return;
			}

			bool[] series = new bool[9];
			if (info.MappingTable is { } mapping)
			{
				var cellOffsets = new List<DrawingInfo>();
				for (int i = 0, p = 0; i < 9; i++)
				{
					if ((series[i], mapping[i]) is (false, { } j) && (series[i] = true) & (series[j] = true))
					{
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
				}

				_currentPainter = _currentPainter with
				{
					View = new(cellOffsets, info.Views[0].Candidates, null, null),
					Conclusions = info.Conclusions
				};
			}
			else
			{
				_currentPainter = _currentPainter with
				{
					View = new(info.Views[0].Candidates!),
					Conclusions = info.Conclusions
				};
			}

			_textBoxInfo.Text = info.ToString();
			
			UpdateImageGrid();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemLanguagesChinese_Click(object sender, RoutedEventArgs e) =>
			ChangeLanguage(CountryCode.ZhCn);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemLanguagesEnglish_Click(object sender, RoutedEventArgs e) =>
			ChangeLanguage(CountryCode.EnUs);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) => new AboutMeWindow().Show();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAboutSpecialThanks_Click(object sender, RoutedEventArgs e) =>
			new SpecialThanksWindow().Show();

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemAboutImplementedTechniques_Click(object sender, RoutedEventArgs e) =>
			new TechniquesWindow().Show();
	}
}
