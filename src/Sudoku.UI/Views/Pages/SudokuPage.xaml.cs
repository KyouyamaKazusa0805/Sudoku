namespace Sudoku.UI.Views.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[Page(DisplayTitle = false)]
public sealed partial class SudokuPage : Page
{
	/// <summary>
	/// Indicates the object that is only used for synchronize an operation, especially a long-time operation.
	/// </summary>
	private static readonly object SyncRoot = new();

	/// <summary>
	/// Indicates the internal solver to solve a puzzle.
	/// </summary>
	private static readonly ManualSolver Solver = new();

	/// <summary>
	/// Indicates the fast solver.
	/// </summary>
	private static readonly BitwiseSolver FastSolver = new();

	/// <summary>
	/// Indicates the internal puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Indicates whether the
	/// </summary>
	private bool _isFirstLoad = true;

	/// <summary>
	/// Indicates the print manager instance.
	/// </summary>
	private PrintManager _printManager;

	/// <summary>
	/// Indicates the document of the print.
	/// </summary>
	private PrintDocument _printDocument;

	/// <summary>
	/// Indicates the print document source.
	/// </summary>
	private IPrintDocumentSource _printDocumentSource;


	/// <summary>
	/// Initializes a <see cref="SudokuPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPage()
	{
		InitializeComponent();
		RegisterPrint();
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		// Calls the base method.
		base.OnKeyDown(e);

		// Checks the status of the key-pressed data.
		routeHandler(e);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void routeHandler(KeyRoutedEventArgs e)
		{
			(
				(Action?)(
					ModifierKeyDownData.FromCurrentState() switch
					{
						(true, true, false) => e.Key switch { Key.Tab => FixGrid, _ => null },
						(true, _, false) => e.Key switch
						{
							Key.O when EnsureUnsnapped() => async () => await OpenFileAsync(),
							Key.S when EnsureUnsnapped() => async () => await SaveFileAsync(),
							Key.C => CopySudokuCode,
							Key.V => async () => await PasteAsync(),
							Key.Tab => FixGrid,
							Key.Z => Undo,
							Key.Y => Redo,
							Key.H => () => GenerateAsync(_cButtonGenerate),
							_ => null
						},
						(false, false, false) => e.Key switch
						{
							(Key)189 => SetPreviousView,
							(Key)187 => SetNextView,
							Key.Escape => ClearViews,
							_ => null
						},
						_ => null
					}
				)
			)?.Invoke();

			// Make the property value 'false' to allow the handler continuously routes to the inner controls.
			e.Handled = false;
		}
	}

	/// <summary>
	/// To register the printing operation.
	/// </summary>
	[MemberNotNull(nameof(_printManager), nameof(_printDocumentSource), nameof(_printDocument))]
	private void RegisterPrint()
	{
		// Register for PrintTaskRequested event.
		nint hWnd = WindowNative.GetWindowHandle(((App)Application.Current).RuntimeInfo.MainWindow);

		// Registers a print manager.
		_printManager = PrintManagerInterop.GetForWindow(hWnd);
		_printManager.PrintTaskRequested += printTaskRequested;

		// Build a PrintDocument and register for callbacks.
		_printDocument = new();
		_printDocumentSource = _printDocument.DocumentSource;
		_printDocument.Paginate += (_, _) => _printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
		_printDocument.GetPreviewPage += (_, e) => _printDocument.SetPreviewPage(e.PageNumber, _cPane);
		_printDocument.AddPages += printDocAddPages;


		void printTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
		{
			// Create the PrintTask.
			// Defines the title and delegate for PrintTaskSourceRequested.
			var printTask = args.Request.CreatePrintTask("Print", e => e.SetSource(_printDocumentSource));

			// Handle PrintTask.Completed to catch failed print jobs.
			printTask.Completed += printTaskCompleted;
		}

		void printTaskCompleted(PrintTask __, PrintTaskCompletedEventArgs args)
			=> _ = args.Completion == PrintTaskCompletion.Failed && DispatcherQueue.TryEnqueue(showErrorAsync);

		async void showErrorAsync()
			=> await SimpleControlFactory.CreateErrorDialog(this, R["PrintFailedTitle"]!, R["PrintFailed"]!).ShowAsync();

		void printDocAddPages(object _, AddPagesEventArgs __)
		{
			_printDocument.AddPage(_cPane);

			// Indicate that all of the print pages have been provided.
			_printDocument.AddPagesComplete();
		}
	}

	/// <summary>
	/// Adds the initial sudoku-technique based <see cref="InfoBar"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitialAddSudokuTechniqueInfoBar()
	{
		if (_isFirstLoad)
		{
			_cInfoBoard.AddMessage(
				InfoBarSeverity.Informational,
				R["SudokuPage_InfoBar_Welcome"]!,
				R["Link_SudokuTutorial"]!,
				R["Link_SudokuTutorialDescription"]!
			);

			_isFirstLoad = false;
		}
	}

	/// <summary>
	/// Clear the current sudoku grid, and revert the status to the empty grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ClearSudokuGrid()
	{
		_cPane.Grid = Grid.Empty;
		_cInfoBoard.AddMessage(InfoBarSeverity.Informational, R["SudokuPage_InfoBar_ClearSuccessfully"]!);
	}

	/// <summary>
	/// Copy the string text that represents the current sudoku grid used, to the clipboard.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CopySudokuCode()
	{
		string format = ((App)Application.Current).UserPreference.PlaceholderIsZero ? "0+:" : ".+:";
		CopySudokuCode(format);
	}

	/// <summary>
	/// Copy the string text that represents the current sudoku grid used, to the clipboard, with the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CopySudokuCode(string? format)
	{
		var grid = _cPane.Grid;
		if (grid is { IsUndefined: true } or { IsEmpty: true })
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_CopyFailedDueToEmpty"]!);
			return;
		}

		CopySudokuCode(grid, format);
	}

	/// <summary>
	/// Copy the string text that represents the current sudoku grid used, to the clipboard,
	/// with the specified grid and the format.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="format">The format.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CopySudokuCode(scoped in Grid grid, string? format)
	{
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(grid.ToString(format));
		Clipboard.SetContent(dataPackage);
	}

	/// <inheritdoc cref="SudokuGrid.SetPreviousView"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetPreviousView()
	{
		_cPane.SetPreviousView();

		_cPipsPager.SelectedPageIndex = _cPane.GetViewIndex();
	}

	/// <inheritdoc cref="SudokuGrid.SetNextView"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetNextView()
	{
		_cPane.SetNextView();

		_cPipsPager.SelectedPageIndex = _cPane.GetViewIndex();
	}

	/// <summary>
	/// Copy the snapshot of the sudoku grid control, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical awaitable instance that holds the task to copy the snapshot.
	/// </returns>
	/// <remarks>
	/// The code is referenced from
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L120">here</see>
	/// and
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L182">here</see>.
	/// </remarks>
	private async Task CopySnapshotAsync()
	{
		// Creates the stream to store the output image data.
		var stream = new InMemoryRandomAccessStream();

		// Gets the snapshot of the control.
		await _cPane.RenderToAsync(stream);

		// Copies the data to the data package.
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
		dataPackage.SetBitmap(streamRef);

		// Copies to the clipboard.
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Update the status of the property <see cref="Control.IsEnabled"/>
	/// of the control <see cref="_cClearInfoBars"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateIsEnabledStatus() => _cClearInfoBars.IsEnabled = _cInfoBoard.Any;

	/// <summary>
	/// Clear the messages.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ClearMessages() => _cInfoBoard.ClearMessages();

	/// <summary>
	/// Undo the step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Undo() => _cPane.UndoStep();

	/// <summary>
	/// Redo the step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Redo() => _cPane.RedoStep();

	/// <summary>
	/// Fix the grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FixGrid() => _cPane.FixGrid();

	/// <summary>
	/// Unfix the grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UnfixGrid() => _cPane.UnfixGrid();

	/// <summary>
	/// Reset the grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ResetGrid() => _cPane.ResetGrid();

	/// <summary>
	/// Emits the information "File is empty".
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EmitFileIsEmptyInfo()
		=> _cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_FileIsEmpty"]!);

	/// <summary>
	/// Emits the information "File is too large".
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EmitFileIsTooLarge()
		=> _cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_FileIsTooLarge"]!);

	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool EnsureUnsnapped()
	{
		// FilePicker APIs will not work if the application is in a snapped state.
		// If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
		bool unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["SudokuPage_InfoBar_CannotSnapTheSample"]!);
		}

		return unsnapped;
	}

	/// <summary>
	/// Asynchronously opening the file, and get the inner content to be parsed to a <see cref="Grid"/> result
	/// to display.
	/// </summary>
	/// <returns>
	/// The typical awaitable instance that holds the task to open the file from the local position.
	/// </returns>
	private async Task OpenFileAsync()
	{
		var fop = new FileOpenPicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.AddFileTypeFilter(CommonFileExtensions.Text)
			.AddFileTypeFilter(CommonFileExtensions.Sudoku)
			.WithAwareHandleOnWin32();

		var file = await fop.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		var fileInfo = new FileInfo(filePath);
		switch (fileInfo.Length)
		{
			case 0:
			{
				EmitFileIsEmptyInfo();
				return;
			}
			case > 1024:
			{
				EmitFileIsTooLarge();
				return;
			}
			default:
			{
				// Checks the validity of the file, and reads the whole content.
				string content = await FileIO.ReadTextAsync(file);
				if (string.IsNullOrWhiteSpace(content))
				{
					_cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_FileIsEmpty"]!);
					return;
				}

				// Checks the file content.
				if (!Grid.TryParse(content, out var grid))
				{
					_cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_FileIsInvalid"]!);
					return;
				}

				// Checks the validity of the parsed grid.
				if (!grid.IsValid)
				{
					_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["SudokuPage_InfoBar_FilePuzzleIsNotUnique"]!);
				}

				// Loads the grid.
				_cPane.Grid = grid;
				_cInfoBoard.AddMessage(InfoBarSeverity.Success, R["SudokuPage_InfoBar_FileOpenSuccessfully"]!);

				break;
			}
		}
	}

	/// <summary>
	/// Asynchronously saving the file using the current sudoku grid as the base content.
	/// </summary>
	/// <returns>
	/// The typical awaitable instance that holds the task to save the file to the local position.
	/// </returns>
	private async Task SaveFileAsync()
	{
		var fsp = new FileSavePicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.WithSuggestedFileName(R["Sudoku"]!)
			.AddFileTypeChoice(R["FileExtension_TextDescription"]!, CommonFileExtensions.Text)
			.AddFileTypeChoice(R["FileExtension_Picture"]!, CommonFileExtensions.PortablePicture)
			.AddFileTypeChoice(R["FileExtension_SudokuGridDescription"]!, CommonFileExtensions.Sudoku)
			.AddFileTypeChoice(R["FileExtension_DrawingData"]!, CommonFileExtensions.DrawingData)
			.WithAwareHandleOnWin32();

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath, Name: var fileName } file)
		{
			return;
		}

		switch (SioPath.GetExtension(filePath))
		{
			case CommonFileExtensions.Text:
			case CommonFileExtensions.Sudoku:
			{
				// Prevent updates to the remote version of the file until we finish making changes
				// and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);

				// Writes to the file.
				await FileIO.WriteTextAsync(file, _cPane.Grid.ToString("#"));

				// Let Windows know that we're finished changing the file so the other app can update
				// the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				var status = await CachedFileManager.CompleteUpdatesAsync(file);
				reportUserOutputResult(status, fileName);

				break;
			}
			case CommonFileExtensions.PortablePicture:
			{
				// Prevent updates to the remote version of the file until we finish making changes
				// and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);

				// Writes to the file.
				// Render to an image at the current system scale and retrieve pixel contents.
				await _cPane.RenderToAsync(file);

				// Let Windows know that we're finished changing the file so the other app can update
				// the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				var drawingDataStatus = await CachedFileManager.CompleteUpdatesAsync(file);
				reportUserOutputResult(drawingDataStatus, fileName);

				break;
			}
			case CommonFileExtensions.DrawingData:
			{
				// Prevent updates to the remote version of the file until we finish making changes
				// and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);

				// Writes to the file.
				await FileIO.WriteTextAsync(
					file,
					$"""
					{R["JsonConfigHeader"]!}
					{_cPane.GetDrawingData()}
					"""
				);

				// Let Windows know that we're finished changing the file so the other app can update
				// the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				var drawingDataStatus = await CachedFileManager.CompleteUpdatesAsync(file);
				reportUserOutputResult(drawingDataStatus, fileName);

				if (((App)Application.Current).UserPreference.AlsoSavePictureWhenSaveDrawingData)
				{
					string? picturePath = SioPath.ChangeExtension(filePath, CommonFileExtensions.PortablePicture);
					if (picturePath is null)
					{
						// The path is null.
						break;
					}

					if (SioFile.Exists(picturePath))
					{
						// The file has already existed. We should break the method
						// to avoid the file being overwritten.
						break;
					}

					// Gets the file, but fast close the file.
					var tempStream = SioFile.Create(picturePath);
					tempStream.Close();

					// Creates the storage file instance.
					var pictureFile = await StorageFile.GetFileFromPathAsync(picturePath);

					// Prevent updates to the remote version of the file until we finish making changes
					// and call CompleteUpdatesAsync.
					CachedFileManager.DeferUpdates(pictureFile);

					// Writes to the file.
					// Render to an image at the current system scale and retrieve pixel contents.
					await _cPane.RenderToAsync(pictureFile);

					// Let Windows know that we're finished changing the file so the other app can update
					// the remote version of the file.
					// Completing updates may require Windows to ask for user input.
					var pictureStatus = await CachedFileManager.CompleteUpdatesAsync(pictureFile);
					reportUserOutputResult(pictureStatus, SioPath.GetFileName(picturePath)!);
				}

				break;
			}


			void reportUserOutputResult(FileUpdateStatus status, string fileName)
			{
				if (status == FileUpdateStatus.Complete)
				{
					string a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
					string b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
					_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");
				}
				else
				{
					string a = R["SudokuPage_InfoBar_SaveFailed1"]!;
					string b = R["SudokuPage_InfoBar_SaveFailed2"]!;
					_cInfoBoard.AddMessage(InfoBarSeverity.Error, $"{a}{fileName}{b}");
				}
			}
		}
	}

	/// <summary>
	/// To paste the text via the clipboard asynchronously.
	/// </summary>
	/// <returns>The typical awaitable instance that holds the task to paste the sudoku grid text.</returns>
	private async Task PasteAsync()
	{
		var dataPackageView = Clipboard.GetContent();
		if (!dataPackageView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		string gridStr = await dataPackageView.GetTextAsync();
		if (!Grid.TryParse(gridStr, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, R["SudokuPage_InfoBar_PasteIsInvalid"]!);
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["SudokuPage_InfoBar_PastePuzzleIsNotUnique"]!);
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, R["SudokuPage_InfoBar_PasteSuccessfully"]!);
	}

	/// <summary>
	/// Try to generate a sudoku puzzle, to display onto the sudoku pane.
	/// </summary>
	/// <param name="button">The button.</param>
	/// <returns>The typical awaitable instance that holds the task to generate the puzzle.</returns>
	private async void GenerateAsync(AppBarButton button)
	{
		// Disable the control to prevent re-invocation.
		button.IsEnabled = false;

		// Generate the puzzle.
		// The generation may be slow, so we should use asynchronous invocation instead of the synchronous one.
		// TODO: May allow the user canceling the puzzle-generating operation.
		var grid = await Task.Run(static () => { lock (SyncRoot) { return Generator.Generate(); } });

		// Enable the control.
		button.IsEnabled = true;

		// Check the status of the grid.
		if (grid.IsUndefined)
		{
			return;
		}

		// Sets the grid to update the view.
		_cPane.Grid = grid;

		// Append the info to the board.
		string part1 = R["SudokuPage_InfoBar_GeneratingSuccessfully1"]!;
		string part2 = R["SudokuPage_InfoBar_GeneratingSuccessfully2"]!;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{part1}\r\n{part2}{grid.GivensCount}");
	}

	/// <summary>
	/// Gets the solution of the grid.
	/// </summary>
	private void GetSolution()
	{
		// Gets the grid and its solution, then check it.
		if (_cPane.Grid is not { IsValid: true, Solution: { IsUndefined: false } solution })
		{
			return;
		}

		// Add message to tell the user the grid has been successfully solved.
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, R["SudokuPage_InfoBar_GetSolutionSuccessfully"]!);

		// Update the view.
		_cPane.ReplaceGridUndoable(solution);
	}

	/// <summary>
	/// To analyze the current sudoku grid.
	/// </summary>
	/// <param name="button">The button.</param>
	/// <returns>The typical awaitable instance that holds the task to analyze the puzzle.</returns>
	private async Task AnalyzeAsync(AppBarButton button)
	{
		switch (_cPane.Grid)
		{
			case { IsUndefined: true } or { IsEmpty: true }:
			{
				// Disallow user to analyze empty or undefined grid.
				return;
			}
			case { IsValid: false }:
			{
				// Disallow user to analyze non-unique solution.
				_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["SudokuPage_InfoBar_AnalyzeFailedDueToInvalidPuzzle"]!);

				return;
			}
			case var grid:
			{
				// Disable the control to prevent re-invocation.
				button.IsEnabled = false;

				// Solve the puzzle using the manual solver.
				var analysisResult = await Task.Run(() => { lock (SyncRoot) { return Solver.Solve(grid); } });

				// Enable the control.
				button.IsEnabled = true;

				// Displays the analysis result info.
				if (analysisResult.IsSolved)
				{
					_cInfoBoard.AddMessage(analysisResult);
				}
				else
				{
					var failedReason = analysisResult.FailedReason;
					var wrongStep = analysisResult.WrongStep;
					string firstPart = R["SudokuPage_InfoBar_AnalyzeFailedDueTo1"]!;
					string secondPart = R[
						failedReason switch
						{
							FailedReason.UserCancelled => "SudokuPage_InfoBar_AnalyzeFailedDueToUserCancelling",
							FailedReason.NotImplemented => "SudokuPage_InfoBar_AnalyzeFailedDueToNotImplemented",
							FailedReason.ExceptionThrown => "SudokuPage_InfoBar_AnalyzeFailedDueToExceptionThrown",
							FailedReason.WrongStep => "SudokuPage_InfoBar_AnalyzeFailedDueToWrongStep",
							FailedReason.PuzzleIsTooHard => "SudokuPage_InfoBar_AnalyzeFailedDueToPuzzleTooHard",
						}
					]!;
					_cInfoBoard.AddMessage(InfoBarSeverity.Warning, $"{firstPart}{secondPart}{wrongStep}");
				}

				break;
			}
		}
	}

	/// <summary>
	/// To mask the grid. All given and modifiable values will be hidden and displayed as an ellipse.
	/// </summary>
	private void Mask() => _cPane.Mask();

	/// <summary>
	/// To unmask the grid. All given and modifiable values will be hidden and displayed as a real digit.
	/// </summary>
	private void Unmask() => _cPane.Unmask();

	/// <summary>
	/// To show the candidates.
	/// </summary>
	private void ShowCandidates() => _cPane.ShowCandidates();

	/// <summary>
	/// To hide the candidates.
	/// </summary>
	private void HideCandidates() => _cPane.HideCandidates();

	/// <summary>
	/// Clear all views.
	/// </summary>
	private void ClearViews() => _cPane.ClearViews();

	/// <summary>
	/// Checks whether the current puzzle is minimal.
	/// </summary>
	private void CheckMinimal()
	{
		const string link = "https://sunnieshine.github.io/Sudoku/terms/minimal-puzzle";
		string linkDescription = R["MinimalPuzzle"]!;

		switch (_cPane.Grid)
		{
			case { IsValid: false }:
			{
				f(InfoBarSeverity.Warning, R["CheckMinimalFalied_NotUniquePuzzle"]!);
				break;
			}
			case var grid when !MinimalPuzzleChecker.IsMinimal(grid, out int firstFoundCandidateMakePuzzleNotMinimal):
			{
				string a = R["CheckMinimalFailed_NotMinimal1"]!;
				string b = R["CheckMinimalFailed_NotMinimal2"]!;
				string resultStr = RxCyNotation.ToCandidateString(firstFoundCandidateMakePuzzleNotMinimal);
				f(InfoBarSeverity.Informational, $"{a}{resultStr}{b}");

				break;
			}
			default:
			{
				f(InfoBarSeverity.Success, R["CheckMinimalSuccessful"]!);
				break;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void f(InfoBarSeverity s, string i) => _cInfoBoard.AddMessage(s, i, link, linkDescription);
		}
	}

	/// <summary>
	/// Checks whether the current puzzle is ittouryu.
	/// </summary>
	private void CheckIttouryu()
	{
		const string link = "https://sunnieshine.github.io/Sudoku/terms/ittouryu-puzzle";
		string linkDescription = R["IttouryuPuzzle"]!;

		switch (_cPane.Grid)
		{
			case { IsValid: false }:
			{
				f(InfoBarSeverity.Warning, R["CheckIttouryuFailed_NotUniquePuzzle"]!);
				break;
			}
			case var grid:
			{
				if (IttouryuPuzzleChecker.IsIttouryu(grid, out _))
				{
					f(InfoBarSeverity.Success, R["CheckIttouryuSuccessful"]!);
				}
				else
				{
					f(InfoBarSeverity.Informational, R["CheckIttouryuFailed_Invalid"]!);
				}

				break;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void f(InfoBarSeverity s, string i) => _cInfoBoard.AddMessage(s, i, link, linkDescription);
		}
	}

	/// <summary>
	/// Finds the missing digit for the current grid.
	/// </summary>
	private void FindMissingDigit()
	{
		var grid = _cPane.Grid;
		switch (FastSolver.Solve(grid, out _))
		{
			case var validity and not false:
			{
				_cInfoBoard.AddMessage(
					InfoBarSeverity.Warning,
					R[validity is true ? "FindMissingDigitFailed_UniquePuzzle" : "FindMissingDigitFailed_NoSolution"]!
				);

				break;
			}
			case false:
			{
				if (MissingDigitsSearcher.GetMissingDigits(grid) is not (var foundCandidates and not (null or [])))
				{
					_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["FindMissingDigitFailed_CannotFound"]!);
					return;
				}

				string candidatesStr = string.Join(
					R["Token_Comma2"]!,
					from candidate in foundCandidates
					select RxCyNotation.ToCandidateString(candidate)
				);
				_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{R["FindMissingDigitSuccessful"]!}{candidatesStr}");

				break;
			}
		}
	}

	/// <summary>
	/// Finds all true candidates in the current grid.
	/// </summary>
	private async Task FindTrueCandidatesAsync()
	{
		var grid = _cPane.Grid;
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, R["FindTrueCandidateFailed_NotUniquePuzzle"]!);
			return;
		}

		_cCommandFindTrueCandidates.IsEnabled = false;

		var i = new TrueCandidatesSearcher(grid);
		var trueCandidates = await Task.Run(() => { lock (SyncRoot) { return i.GetAllTrueCandidates(64); } });

		_cCommandFindTrueCandidates.IsEnabled = true;

		if (trueCandidates is [])
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Informational, R["FindTrueCandidateFailed_NotBugPattern"]!);
			return;
		}

		string a = R["FindTrueCandidateSuccessful1"]!;
		string b = R["FindTrueCandidateSuccessful2"]!;
		string c = RxCyNotation.ToCandidatesString(
			trueCandidates,
			RxCyNotationOptions.Default with { Separator = R["Token_Comma2"]! }
		);

		_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{trueCandidates.Count}{b}{c}");
	}

	/// <summary>
	/// To print the grid.
	/// </summary>
	/// <remarks>
	/// The code is referenced from <see href="https://github.com/marb2000/PrintSample">this sample</see>.
	/// </remarks>
	private async Task PrintAsync()
	{
		var mainWindow = ((App)Application.Current).RuntimeInfo.MainWindow;
		if (Supportability.Printer)
		{
			try
			{
				// Show print UI.
				nint hWnd = WindowNative.GetWindowHandle(mainWindow);
				await PrintManagerInterop.ShowPrintUIForWindowAsync(hWnd);
			}
			catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x80040155U))
			{
				// System.Runtime.InteropServices.COMException: 'Interface not registered (0x80040155)'.
				// This issue will be raised due to not having been implemented on printing operations.
				// Please visit this issue link for more details.
				// https://github.com/microsoft/microsoft-ui-xaml/issues/4419
				var dialog = SimpleControlFactory.CreateErrorDialog(this, R["PrintFailedTitle"]!, R["PrintFailed_InterfaceNotRegistered"]!);
				await dialog.ShowAsync();
			}
			catch
			{
				// Printing cannot proceed at this time.
				var dialog = SimpleControlFactory.CreateErrorDialog(this, R["PrintFailedTitle"]!, R["PrintFailed_CannotProceed"]!);
				await dialog.ShowAsync();
			}
		}
		else
		{
			// Printing is not supported on this device.
			var dialog = SimpleControlFactory.CreateErrorDialog(this, R["PrintFailedTitle"]!, R["PrintFailed_NotSupported"]!);
			await dialog.ShowAsync();
		}
	}

	/// <summary>
	/// Sets the manual step.
	/// </summary>
	/// <param name="e">The manual step.</param>
	private void SetManualStep(ManualStep e)
	{
		if (e is not (var grid, { Views.Length: var viewLength and not 0 } step))
		{
			return;
		}

		// Bug fix: This property assignment will reset the displayable unit, so we should put the statement firstly.
		_cPane.Grid = grid;
		_cPane.SetDisplayableUnit(step);

		bool isMultipleViews = viewLength > 1;
		_cPipsPager.SelectedPageIndex = 0;
		_cPipsPager.Visibility = isMultipleViews ? Visibility.Visible : Visibility.Collapsed;
		if (isMultipleViews)
		{
			_cPipsPager.NumberOfPages = viewLength;
		}
	}


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Loaded(object sender, RoutedEventArgs e) => InitialAddSudokuTechniqueInfoBar();

	/// <summary>
	/// Triggers when the current page is unloaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Unloaded(object sender, RoutedEventArgs e)
	{
		// TODO: Dispose the release if worth.
	}

	/// <summary>
	/// Triggers when the pane is refreshed the sudoku grid.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Pane_GridRefreshed(object sender, object? e)
	{
		_cPipsPager.SelectedPageIndex = -1;
		_cPipsPager.Visibility = Visibility.Collapsed;
	}

	/// <summary>
	/// Triggers when a file is dropped to the target sudoku pane.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Pane_SuccessfullyReceivedDroppedFile(object? sender, object? e)
		=> _cInfoBoard.AddMessage(InfoBarSeverity.Success, R["SudokuPage_InfoBar_FileDragAndDropSuccessfully"]!);

	/// <summary>
	/// Triggers when failed to receive the dropped file.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Pane_FailedReceivedDroppedFile(object sender, FailedReceivedDroppedFileEventArgs e)
		=> (
			(Action)(
				e.Reason switch
				{
					FailedReceivedDroppedFileReason.FileIsEmpty => EmitFileIsEmptyInfo,
					FailedReceivedDroppedFileReason.FileIsTooLarge => EmitFileIsTooLarge,
				}
			)
		).Invoke();

	/// <summary>
	/// Triggers when the current pips pager is updated its selected index.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void PipsPager_SelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
		=> _cPane.SkipToViewIndex(sender.SelectedPageIndex);

	/// <summary>
	/// Triggers when the inner collection of the control <see cref="_cInfoBoard"/> is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void InfoBoard_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		=> UpdateIsEnabledStatus();

	/// <summary>
	/// Triggers when the inner collection of the control <see cref="_cInfoBoard"/> is changed its step chosen,
	/// when the info bar message binds with a <see cref="ManualSolverResult"/>.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void InfoBoard_ChosenStepChanged(object sender, ManualStep e) => SetManualStep(e);

	/// <summary>
	/// Indicates the event trigger callback method that determines
	/// whether the current window status can execute the following operation.
	/// </summary>
	private void CommandOpenOrSaveSudokuFile_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
		=> EnsureUnsnapped();

	/// <summary>
	/// Indicates the event trigger callback method that executes opening sudoku file.
	/// </summary>
	private async void CommandOpenSudokuFile_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await OpenFileAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// copying the string text representing as the current sudoku grid.
	/// </summary>
	private void CommandCopySudokuGridText_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> CopySudokuCode();

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// copying the snapshot of the sudoku grid control.
	/// </summary>
	private async void CommandCopyControlSnapshot_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await CopySnapshotAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// parsing the string text representing as a sudoku grid from the clipboard.
	/// </summary>
	private async void CommandPasteSudokuGridText_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await PasteAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes saving sudoku file.
	/// </summary>
	private async void CommandSaveSudokuFile_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await SaveFileAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes returning back to the empty grid.
	/// </summary>
	private void CommandReturnEmptyGrid_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		if (args.Parameter is Button { Parent: StackPanel { Parent: FlyoutPresenter { Parent: Popup f } } })
		{
			f.IsOpen = false;
		}

		ClearSudokuGrid();
	}

	/// <summary>
	/// Indicates the event trigger callback method that executes resetting the grid to the initial status.
	/// </summary>
	private void CommandReset_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => ResetGrid();

	/// <summary>
	/// Indicates the event trigger callback method that executes fixing digits.
	/// </summary>
	private void CommandFix_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => FixGrid();

	/// <summary>
	/// Indicates the event trigger callback method that executes unfixing digits.
	/// </summary>
	private void CommandUnfix_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => UnfixGrid();

	/// <summary>
	/// Indicates the event trigger callback method that executes undoing a step.
	/// </summary>
	private void CommandUndo_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => Undo();

	/// <summary>
	/// Indicates the event trigger callback method that executes redoing a step.
	/// </summary>
	private void CommandRedo_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => Redo();

	/// <summary>
	/// Indicates the event trigger callback method that executes clearing all messages.
	/// </summary>
	private void CommandClearMessages_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> ClearMessages();

	/// <summary>
	/// Indicates the event trigger callback method that executes generating a puzzle.
	/// </summary>
	private void CommandGenerate_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> GenerateAsync(_cButtonGenerate);

	/// <summary>
	/// Indicates the event trigger callback method that gets the solution of the puzzle.
	/// </summary>
	private void CommandGetSolution_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> GetSolution();

	/// <summary>
	/// Indicates the event trigger callback method that analyzes the puzzle.
	/// </summary>
	private async void CommandAnalysis_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await AnalyzeAsync(_cButtonAnalyze);

	/// <summary>
	/// Indicates the event trigger callback method that mask the grid.
	/// </summary>
	private void ComamndMask_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => Mask();

	/// <summary>
	/// Indicates the event trigger callback method that unmask the grid.
	/// </summary>
	private void ComamndUnmask_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => Unmask();

	/// <summary>
	/// Indicates the event trigger callback method that shows the candidates temporarily.
	/// </summary>
	private void CommandShowCandidates_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> ShowCandidates();

	/// <summary>
	/// Indicates the event trigger callback method that hides the candidates permanently.
	/// </summary>
	private void CommandHideCandidates_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> HideCandidates();

	/// <summary>
	/// Indicates the event trigger callback method that check minimal.
	/// </summary>
	private void CheckMinimal_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => CheckMinimal();

	/// <summary>
	/// Indicates the event trigger callback method that check ittouryu.
	/// </summary>
	private void CheckIttouryu_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => CheckIttouryu();

	/// <summary>
	/// Indicates the event trigger callback method that find missing digit.
	/// </summary>
	private void CommandFindMissingDigit_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> FindMissingDigit();

	/// <summary>
	/// Indicates the event trigger callback method that find true candidates.
	/// </summary>
	private async void FindTrueCandidates_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await FindTrueCandidatesAsync();

	/// <summary>
	/// Indicates the event trigger callback method that prints the puzzle.
	/// </summary>
	private async void ComamndPrint_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args)
		=> await PrintAsync();

	/// <summary>
	/// Indicates the event trigger callback method that copies the initial grid.
	/// </summary>
	private void CopyInitial_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(((App)Application.Current).UserPreference.PlaceholderIsZero ? "0" : ".");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current grid.
	/// </summary>
	private void CopyCurrent_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(((App)Application.Current).UserPreference.PlaceholderIsZero ? "0+:" : ".+:");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current grid
	/// and treat all values as given ones forcedly.
	/// </summary>
	private void CopyCurrentIgnoreModifiables_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(((App)Application.Current).UserPreference.PlaceholderIsZero ? "0!:" : ".!:");

	/// <summary>
	/// Indicates the event trigger callback method that copies the solution grid.
	/// </summary>
	private void CopySolution_Click(object sender, RoutedEventArgs e) => CopySudokuCode(_cPane.Grid.Solution, null);

	/// <summary>
	/// Indicates the event trigger callback method that copies the initial grid, with multiple-line format.
	/// </summary>
	private void CopyInitialMultilined_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(((App)Application.Current).UserPreference.PlaceholderIsZero ? "@0*" : "@.*");

	/// <summary>
	/// Indicates the event trigger callback method that copies the solution grid, with multiple-line format.
	/// </summary>
	private void CopySolutionMultilined_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(_cPane.Grid.Solution, "@");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current pencil-marked grid.
	/// </summary>
	private void CopyPmCurrent_Click(object sender, RoutedEventArgs e) => CopySudokuCode("@:");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current pencil-marked grid,
	/// and treat all values as given ones forcedly.
	/// </summary>
	private void CopyPmCurrentIgnoreModifiable_Click(object sender, RoutedEventArgs e) => CopySudokuCode("@!");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current sukaku grid.
	/// </summary>
	private void CopySukakuCurrent_Click(object sender, RoutedEventArgs e)
		=> CopySudokuCode(((App)Application.Current).UserPreference.PlaceholderIsZero ? "@~0" : "@~.");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current Excel grid.
	/// </summary>
	private void CopyExcelCurrent_Click(object sender, RoutedEventArgs e) => CopySudokuCode("%");

	/// <summary>
	/// Indicates the event trigger callback method that copies the current open-sudoku grid.
	/// </summary>
	private void CopyOpenSudokuCurrent_Click(object sender, RoutedEventArgs e) => CopySudokuCode("^");
}
