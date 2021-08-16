#pragma warning disable IDE1006

using Sudoku.Data.Stepping;

namespace Sudoku.Windows;

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

	private async void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
	{
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
								var grid = await _recognition.RecognizeAsync(bitmap);
								grid.Fix();
								Puzzle = grid;
							}

							_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
							_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
							_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);

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
	}

	private void MenuItemFileSavePicture_Click(object sender, RoutedEventArgs e) =>
		new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

	private void MenuItemFileLoadDrawing_Click(object sender, RoutedEventArgs e)
	{
		var dialog = new OpenFileDialog
		{
			DefaultExt = "drawings",
			Filter = (string)LangSource["FilterLoadingDrawingContents"],
			Multiselect = false,
			Title = (string)LangSource["TitleLoadingDrawingContents"]
		};

		if (dialog.ShowDialog() is true)
		{
			try
			{
				_currentPainter.CustomView =
					JsonSerializer.Deserialize<MutableView>(
						File.ReadAllText(dialog.FileName), _serializerOptions
					) ?? throw new JsonException("The custom view is currently null.");

				UpdateImageGrid();
			}
			catch
			{
				Messagings.FailedToLoadDrawingContents();
			}
		}
	}

	private void MenuItemFileSaveDrawing_Click(object sender, RoutedEventArgs e)
	{
		var customView = _currentPainter.CustomView;
		if (customView is null)
		{
			Messagings.FailedToSaveDrawingContentsDueToEmpty();
			return;
		}

		var dialog = new SaveFileDialog
		{
			AddExtension = true,
			CheckPathExists = true,
			DefaultExt = "drawings",
			Filter = (string)LangSource["FilterSavingDrawingContents"],
			Title = (string)LangSource["TitleSavingDrawingContents"]
		};

		if (dialog.ShowDialog() is true)
		{
			try
			{
				string json = JsonSerializer.Serialize(customView, _serializerOptions);
				File.WriteAllText(dialog.FileName, json);

				Messagings.SaveSuccess();
			}
			catch
			{
				Messagings.FailedToSaveDrawingContents();
			}
		}
	}

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
		if (!_puzzle.IsValid())
		{
			Messagings.FailedToCheckDueToInvalidPuzzle();
			e.Handled = true;
			return;
		}

		// Save puzzle picture.
		new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

		// Save puzzle text.
		MenuItemFileSave_Click(sender, e);

		// Solve the puzzle.
		MenuItemAnalyzeSolve_Click(sender, e);

		// Save solution picture.
		new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

		// Save solution text.
		MenuItemFileSave_Click(sender, e);
	}

	private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) => Close();

	private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
	{
		Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true;

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

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
	{
		if (_puzzle.HasRedoSteps)
		{
			_puzzle.Redo();
			UpdateImageGrid();
		}

		UpdateUndoRedoControls();

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemEditRecomputeCandidates_Click(object sender, RoutedEventArgs e)
	{
		int[] z = new int[81];
		for (int cell = 0; cell < 81; cell++)
		{
			z[cell] = _puzzle[cell] + 1;
		}

		var grid = new SudokuGrid(z, GridCreatingOption.MinusOne);
		if (new UnsafeBitwiseSolver().Solve(grid.ToString(null, null), null, 2) == 0)
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

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
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
			SystemClipboard.DataObject = $":0000:x:{z}{(z.Contains(':') ? "::" : ":::")}";
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

	private void MenuItemEditCopyAsOpenSudoku_Click(object sender, RoutedEventArgs e) => InternalCopy("^");

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
		try
		{
			string puzzleStr = SystemClipboard.Text;
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

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
	{
		_puzzle.Fix();

		UpdateImageGrid();
		_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
		_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
	{
		_puzzle.Unfix();

		UpdateImageGrid();
		_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
		_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
	{
		_currentPainter = _currentPainter with { Grid = _puzzle = _initialPuzzle, View = null };

		UpdateImageGrid();
		_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
		_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private void MenuItemClearStack_Click(object sender, RoutedEventArgs e)
	{
		if (_puzzle is { HasUndoSteps: true } or { HasRedoSteps: true }
			&& Messagings.AskWhileClearingStack() == MessageBoxResult.Yes)
		{
			_puzzle.ClearStack();

			UpdateUndoRedoControls();

#if AUTHOR_RESERVED && DEBUG
			if (_enableDynamicChecking)
			{
				if (!Directory.Exists(CachePath))
				{
					return;
				}

				if (!File.Exists(TempFilePath))
				{
					return;
				}

				try
				{
					File.Delete(TempFilePath);
				}
				catch
				{
				}
			}
#endif
		}
	}

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

#if AUTHOR_RESERVED && DEBUG
		if (_enableDynamicChecking)
		{
			if (!Directory.Exists(CachePath))
			{
				return;
			}

			if (!File.Exists(TempFilePath))
			{
				return;
			}

			try
			{
				File.Delete(TempFilePath);
			}
			catch
			{
			}
		}
#endif
	}

	private async void MenuItemGenerateWithSymmetry_Click(object sender, RoutedEventArgs e)
	{
		CancellationTokenSource? cts = null;
		try
		{
			cts = new();
			await internalOperation(cts);
		}
		catch (OperationCanceledException)
		{
			EnableGeneratingControls();
			SwitchOnGeneratingComboBoxesDisplaying();
			ClearItemSourcesWhenGeneratedOrSolving();
		}
		finally
		{
			cts?.Dispose();
		}

		async Task internalOperation(CancellationTokenSource cts)
		{
			if (_database is null || Messagings.AskWhileGeneratingWithDatabase() == MessageBoxResult.Yes)
			{
				// Disable relative database controls.
				Settings.CurrentPuzzleDatabase = _database = null;
				Settings.CurrentPuzzleNumber = -1;
				UpdateDatabaseControls(false, false, false, false);

				DisableGeneratingControls();

				// These two value should be assigned first, rather than 
				// inlining in the asynchronous environment.
				var dialog = new ProgressWindow { CancellationTokenSource = cts };
				dialog.Show();

				int si = _comboBoxSymmetry.SelectedIndex;
				var symmetry = si == 0 ? SymmetryType.None : (SymmetryType)(1 << si - 1);
				//var diff = (DifficultyLevel)_comboBoxDifficulty.SelectedItem;
				Puzzle =
					await new BasicPuzzleGenerator().GenerateAsync(
						33, symmetry, dialog.DefaultReporting, Settings.LanguageCode, cts.Token
					) ?? throw new OperationCanceledException();

				dialog.Close();

				EnableGeneratingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				ClearItemSourcesWhenGeneratedOrSolving();
				UpdateImageGrid();

#if AUTHOR_RESERVED && DEBUG
				if (_enableDynamicChecking)
				{
					if (!Directory.Exists(CachePath))
					{
						return;
					}

					if (!File.Exists(TempFilePath))
					{
						return;
					}

					try
					{
						File.Delete(TempFilePath);
					}
					catch
					{
					}
				}
#endif
			}
		}
	}

	private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
	{
		CancellationTokenSource? cts = null;
		try
		{
			cts = new();
			await internalOperation(cts);
		}
		catch (OperationCanceledException)
		{
			EnableGeneratingControls();
			SwitchOnGeneratingComboBoxesDisplaying();
			ClearItemSourcesWhenGeneratedOrSolving();
		}
		finally
		{
			cts?.Dispose();
		}

		async Task internalOperation(CancellationTokenSource cts)
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
				var dialog = new ProgressWindow { CancellationTokenSource = cts };
				dialog.Show();

				int index = _comboBoxBackdoorFilteringDepth.SelectedIndex;
				Settings.GeneratingDifficultyLevelSelectedIndex = _comboBoxDifficulty.SelectedIndex;

				Puzzle =
					await new HardPatternPuzzleGenerator().GenerateAsync(
						index - 1,
						dialog.DefaultReporting,
						(DifficultyLevel)Settings.GeneratingDifficultyLevelSelectedIndex,
						Settings.LanguageCode,
						cts.Token
					) ?? throw new OperationCanceledException();

				dialog.Close();

				EnableGeneratingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				ClearItemSourcesWhenGeneratedOrSolving();
				UpdateImageGrid();

#if AUTHOR_RESERVED && DEBUG
				if (_enableDynamicChecking)
				{
					if (!Directory.Exists(CachePath))
					{
						return;
					}

					if (!File.Exists(TempFilePath))
					{
						return;
					}

					try
					{
						File.Delete(TempFilePath);
					}
					catch
					{
					}
				}
#endif
			}
		}
	}

	private async void MenuItemGenerateWithTechniqueFiltering_Click(object sender, RoutedEventArgs e)
	{
		CancellationTokenSource? cts = null;
		try
		{
			cts = new();
			await internalOperation(cts);
		}
		catch (OperationCanceledException)
		{
			EnableGeneratingControls();
			SwitchOnGeneratingComboBoxesDisplaying();
			ClearItemSourcesWhenGeneratedOrSolving();
		}
		finally
		{
			cts?.Dispose();
		}

		async Task internalOperation(CancellationTokenSource cts)
		{
			DisableGeneratingControls();

			var dialog = new ProgressWindow { CancellationTokenSource = cts };
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
						filter, dialog.DefaultReporting, Settings.LanguageCode, cts.Token
					) ?? throw new OperationCanceledException();
			}

		Last:
			dialog.Close();

			EnableGeneratingControls();
			SwitchOnGeneratingComboBoxesDisplaying();
			ClearItemSourcesWhenGeneratedOrSolving();
			UpdateImageGrid();

#if AUTHOR_RESERVED && DEBUG
			if (_enableDynamicChecking)
			{
				if (!Directory.Exists(CachePath))
				{
					return;
				}

				if (!File.Exists(TempFilePath))
				{
					return;
				}

				try
				{
					File.Delete(TempFilePath);
				}
				catch
				{
				}
			}
#endif
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
			var sb = new ValueStringBuilder(SudokuGrid.EmptyString);
			if (
				new UnsafeBitwiseSolver().Solve(
					_puzzle.ToString($"~{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}"),
					ref sb,
					2
				) != 1
			)
			{
				return !(e.Handled = true);
			}

			var grid = SudokuGrid.Parse(sb.ToString());
			grid.Unfix();

			Puzzle = grid;
			UpdateImageGrid();

#if AUTHOR_RESERVED && DEBUG
			if (_enableDynamicChecking)
			{
				if (!Directory.Exists(CachePath))
				{
					goto Return;
				}

				if (!File.Exists(TempFilePath))
				{
					goto Return;
				}

				try
				{
					File.Delete(TempFilePath);
				}
				catch
				{
				}
			}

		Return:
			return true;
#else
			return true;
#endif
		}

		bool applyNormal()
		{
			var solutionSb = new ValueStringBuilder(stackalloc char[81]);
			string puzzleStr = _puzzle.ToString("0+");
			if (new UnsafeBitwiseSolver().Solve(_puzzle.ToString(), ref solutionSb, 2) != 1)
			{
				return !(e.Handled = true);
			}

			var newSb = new ValueStringBuilder(300);
			for (int i = 0, cell = 0, length = puzzleStr.Length; i < length; cell++)
			{
				char c = solutionSb[cell];
				switch (puzzleStr[i])
				{
					case '+':
					{
						newSb.Append('+');
						newSb.Append(c);
						i += 2;
						break;
					}
					case '0':
					{
						newSb.Append('+');
						newSb.Append(c);
						i++;
						break;
					}
					default:
					{
						newSb.Append(c);
						i++;
						break;
					}
				}
			}

			Puzzle = SudokuGrid.Parse(newSb.ToString());
			UpdateImageGrid();

#if AUTHOR_RESERVED && DEBUG
			if (_enableDynamicChecking)
			{
				if (!Directory.Exists(CachePath))
				{
					goto Return;
				}

				if (!File.Exists(TempFilePath))
				{
					goto Return;
				}

				try
				{
					File.Delete(TempFilePath);
				}
				catch
				{
				}
			}
		Return:
			return true;
#else
			return true;
#endif
		}
	}

	private async void MenuItemAnalyzeAnalyze_Click(object sender, RoutedEventArgs e)
	{
		if (_puzzle == SudokuGrid.Empty)
		{
			Messagings.AnalyzeEmptyGrid();
			e.Handled = true;
			return;
		}

		CancellationTokenSource? cts = null;
		try
		{
			cts = new();
			if (!await internalOperation(cts))
			{
				Messagings.FailedToApplyPuzzle();
				e.Handled = true;
				return;
			}
		}
		catch (OperationCanceledException)
		{
			// If here, the task has been already cancelled.
			EnableSolvingControls();
			SwitchOnGeneratingComboBoxesDisplaying();
			DisplayDifficultyInfoAfterAnalyzed();
		}
		finally
		{
			cts?.Dispose();
		}

		async Task<bool> internalOperation(CancellationTokenSource cts)
		{
			for (int i = 0; i < 2; i++)
			{
				bool sukakuMode = i != 0;
				if (_puzzle.IsSolved)
				{
					Messagings.PuzzleAlreadySolved();
					continue;
				}

				if (!isValidGrid(_puzzle, sukakuMode))
				{
					continue;
				}

				// Update status.
				ClearItemSourcesWhenGeneratedOrSolving();
				_textBoxInfo.Text = (string)LangSource["WhileSolving"];
				DisableSolvingControls();

				// Run the solver asynchronously, during solving you can do other work.
				var dialog = new ProgressWindow { CancellationTokenSource = cts };
				dialog.Show();

				if (_puzzle.GivensCount == 0)
				{
					_puzzle.Fix();
				}

				if (!Settings.SolveFromCurrent && !sukakuMode)
				{
					_puzzle.Reset();
				}

				_puzzle.ClearStack();

				_analyisResult =
					await _manualSolver.SolveAsync(
						(SudokuGrid)_puzzle, dialog.DefaultReporting, Settings.LanguageCode, cts.Token
					) ?? throw new OperationCanceledException();

				UpdateImageGrid();

				dialog.Close();

				// Solved. Now update the technique summary.
				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				DisplayDifficultyInfoAfterAnalyzed();

				return true;
			}

			return false;
		}

		static bool isValidGrid(UndoableGrid grid, bool sukakuMode)
		{
			if (sukakuMode)
			{
				if (new UnsafeBitwiseSolver().Solve(grid.ToString("~"), null, 2) != 1)
				{
					return false;
				}
			}
			else
			{
				var sb = new ValueStringBuilder(SudokuGrid.EmptyString);
				for (int cell = 0; cell < 81; cell++)
				{
					sb[cell] += (char)(grid[cell] + 1);
				}

				if (new UnsafeBitwiseSolver().Solve(sb.ToString(), null, 2) != 1)
				{
					return false;
				}
			}

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

		new ExportAnalysisResultWindow(_analyisResult, Settings).Show();
	}

	private void MenuItemAnalyzeBackdoor_Click(object sender, RoutedEventArgs e) =>
		new BackdoorWindow((SudokuGrid)_puzzle).ShowDialog();

	private void MenuItemAnalyzeBugN_Click(object sender, RoutedEventArgs e)
	{
		if (!_puzzle.IsValid())
		{
			Messagings.FailedToCheckDueToInvalidPuzzle();
			e.Handled = true;
			return;
		}

		new BugNSearchWindow((SudokuGrid)_puzzle).ShowDialog();
	}

	private void MenuItemToolsBatch_Click(object sender, RoutedEventArgs e) =>
		new BatchWindow(Settings).Show();

	private unsafe void MenuItemTransformMirrorLeftRight_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.MirrorLeftRight);

	private unsafe void MenuItemTransformMirrorTopBotton_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.MirrorTopBottom);

	private unsafe void MenuItemTransformMirrorDiagonal_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.MirrorDiagonal);

	private unsafe void MenuItemTransformMirrorAntidiagonal_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.MirrorAntidiagonal);

	private unsafe void MenuItemTransformRotateClockwise_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.RotateClockwise);

	private unsafe void MenuItemTransformRotateCounterclockwise_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.RotateCounterclockwise);

	private unsafe void MenuItemTransformRotatePi_Click(object sender, RoutedEventArgs e) =>
		Transform(&SudokuGridTransformations.RotatePi);

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

			var candidateOffsets = new DrawingInfo[trueCandidates.Count];
			int i = 0;
			foreach (int candidate in trueCandidates)
			{
				candidateOffsets[i++] = new(0, candidate);
			}

			_currentPainter.View = new() { Candidates = candidateOffsets };
			_currentPainter.Conclusions = null;

			UpdateImageGrid();

			_textBoxInfo.Text = $"{LangSource["AllTrueCandidates"]}{new Candidates(trueCandidates)}";
		}
	}

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

			_currentPainter.View = new()
			{
				Candidates = (
					from backdoor in backdoors
					where backdoor.ConclusionType == ConclusionType.Assignment
					select new DrawingInfo(0, backdoor.Cell * 9 + backdoor.Digit)
				).ToArray()
			};
			_currentPainter.Conclusions = backdoors;

			UpdateImageGrid();

			_textBoxInfo.Text = $"{LangSource["AllBackdoorsAtLevel0Or1"]}{new ConclusionCollection(backdoors).ToString()}";
		}
	}

	private unsafe void MenuItemViewsGspView_Click(object sender, RoutedEventArgs e)
	{
		if (Enumerable.Range(0, 81).All(i => _puzzle.GetStatus(i) != CellStatus.Given))
		{
			Messagings.SukakuCannotUseGspChecking();
			e.Handled = true;
			return;
		}

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
				if ((A: series[i], B: mapping[i]) is (A: false, B: { } j)
					&& (series[i] = true) & (series[j] = true))
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

			_currentPainter.View = new() { Cells = cellOffsets, Candidates = info.Views[0].Candidates };
		}
		else
		{
			_currentPainter.View = new() { Candidates = info.Views[0].Candidates! };
		}

		_currentPainter.Conclusions = info.Conclusions;
		_textBoxInfo.Text = info.Formatize();

		UpdateImageGrid();
	}

	private void MenuItemLanguagesChinese_Click(object sender, RoutedEventArgs e) =>
		ChangeLanguage(CountryCode.ZhCn);

	private void MenuItemLanguagesEnglish_Click(object sender, RoutedEventArgs e) =>
		ChangeLanguage(CountryCode.EnUs);

	private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) => new AboutMeWindow().Show();

	private void MenuItemAboutSpecialThanks_Click(object sender, RoutedEventArgs e) =>
		new SpecialThanksWindow().Show();

	private void MenuItemAboutImplementedTechniques_Click(object sender, RoutedEventArgs e) =>
		new TechniquesWindow().Show();
}
