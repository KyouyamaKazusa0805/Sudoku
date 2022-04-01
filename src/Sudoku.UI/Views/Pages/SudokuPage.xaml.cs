using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Sudoku.Generating;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SudokuPage : Page
{
	/// <summary>
	/// Indicates the internal solver to solve a puzzle.
	/// </summary>
	private static readonly ManualSolver Solver = new();

	/// <summary>
	/// Indicates the internal puzzle generator.
	/// </summary>
	private static readonly HardPatternPuzzleGenerator Generator = new();


	/// <summary>
	/// Initializes a <see cref="SudokuPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPage() => InitializeComponent();


	/// <inheritdoc/>
	protected override async void OnKeyDown(KeyRoutedEventArgs e)
	{
		// Calls the base method.
		base.OnKeyDown(e);

		// Checks the status of the key-pressed data.
		await routeHandlerAsync(e);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		async Task routeHandlerAsync(KeyRoutedEventArgs e)
		{
			bool controlKeyIsDown = modifierKeyIsPressed(VirtualKey.Control);
			bool shiftKeyIsDown = modifierKeyIsPressed(VirtualKey.Shift);
			switch (e.Key)
			{
				case VirtualKey.O when controlKeyIsDown && EnsureUnsnapped():
				{
					await OpenFileAsync();
					break;
				}
				case VirtualKey.C when controlKeyIsDown:
				{
					CopySudokuCode();
					break;
				}
				case VirtualKey.V when controlKeyIsDown:
				{
					await PasteAsync();
					break;
				}
				case VirtualKey.S when controlKeyIsDown && EnsureUnsnapped():
				{
					await SaveFileAsync();
					break;
				}
				case VirtualKey.Tab when controlKeyIsDown:
				{
					FixGrid();
					break;
				}
				case VirtualKey.Tab when controlKeyIsDown && shiftKeyIsDown:
				{
					UnfixGrid();
					break;
				}
				case VirtualKey.Z when controlKeyIsDown:
				{
					Undo();
					break;
				}
				case VirtualKey.Y when controlKeyIsDown:
				{
					Redo();
					break;
				}
				case VirtualKey.H when controlKeyIsDown:
				{
					await GenerateAsync(_cButtonGenerate);
					break;
				}
			}

			// Make the property value 'false' to allow the handler continuously routes to the inner controls.
			e.Handled = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool modifierKeyIsPressed(VirtualKey key) =>
			InputKeyboardSource.GetKeyStateForCurrentThread(key).Flags(CoreVirtualKeyStates.Down);
	}

	/// <summary>
	/// Adds the initial sudoku-technique based <see cref="InfoBar"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitialAddSudokuTechniqueInfoBar() =>
		_cInfoBoard.AddMessage(
			InfoBarSeverity.Informational, Get("SudokuPage_InfoBar_Welcome"),
			Get("Link_SudokuTutorial"), Get("Link_SudokuTutorialDescription"));

	/// <summary>
	/// Clear the current sudoku grid, and revert the status to the empty grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ClearSudokuGrid()
	{
		_cPane.Grid = Grid.Empty;
		_cInfoBoard.AddMessage(InfoBarSeverity.Informational, Get("SudokuPage_InfoBar_ClearSuccessfully"));
	}

	/// <summary>
	/// Copy the string text that represents the current sudoku grid used, to the clipboard.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CopySudokuCode()
	{
		ref readonly var grid = ref _cPane.GridByReference();
		if (grid is { IsUndefined: true } or { IsEmpty: true })
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_CopyFailedDueToEmpty"));
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(grid.ToString("#"));
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Copy the snapshot of the sudoku grid control, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical awaitable instance that holds the task to copy the snapshot.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private async Task CopySnapshotAsync()
	{
		// Gets the snapshot of the control.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(_cPane);
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value for the current computer.
#if true
		const float dpi = 96;
#else
		float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
#endif

		// Creates the stream to store the output image data.
		await using var stream = new MemoryStream();
		var randomAccessStream = stream.AsRandomAccessStream();

		// Creates an encoder.
		var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
		encoder.SetPixelData(
			BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight,
			dpi, dpi, pixelBuffer.ToArray());

		// Copies the data to the data package.
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		var dataTransferManager = DataTransferManager.GetForCurrentView();
		var streamRef = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
		dataPackage.SetBitmap(streamRef);

		// Copies to the clipboard.
		Clipboard.SetContent(dataPackage);

		// Flushes the encoder.
		await encoder.FlushAsync();
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
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_CannotSnapTheSample"));
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
		var fop = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
		fop.FileTypeFilter.Add(CommonFileExtensions.Text);
		fop.AwareHandleOnWin32();

		var file = await fop.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		if (new FileInfo(filePath).Length == 0)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsEmpty"));
			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await FileIO.ReadTextAsync(file);
		if (string.IsNullOrWhiteSpace(content))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsEmpty"));
			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsInvalid"));
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_FilePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, Get("SudokuPage_InfoBar_FileOpenSuccessfully"));
	}

	/// <summary>
	/// Asynchronously saving the file using the current sudoku grid as the base content.
	/// </summary>
	/// <returns>
	/// The typical awaitable instance that holds the task to save the file to the local position.
	/// </returns>
	private async Task SaveFileAsync()
	{
		var fsp = new FileSavePicker
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
			SuggestedFileName = "Sudoku"
		};
		fsp.FileTypeChoices.Add(Get("FileExtension_TextDescription"), new List<string> { CommonFileExtensions.Text });
		fsp.AwareHandleOnWin32();

		var file = await fsp.PickSaveFileAsync();
		if (file is not { Name: var fileName })
		{
			return;
		}

		// Prevent updates to the remote version of the file until we finish making changes
		// and call CompleteUpdatesAsync.
		CachedFileManager.DeferUpdates(file);

		// Writes to the file.
		await FileIO.WriteTextAsync(file, _cPane.GridByReference().ToString("#"));

		// Let Windows know that we're finished changing the file so the other app can update
		// the remote version of the file.
		// Completing updates may require Windows to ask for user input.
		var status = await CachedFileManager.CompleteUpdatesAsync(file);
		if (status == FileUpdateStatus.Complete)
		{
			string a = Get("SudokuPage_InfoBar_SaveSuccessfully1");
			string b = Get("SudokuPage_InfoBar_SaveSuccessfully2");
			_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");
		}
		else
		{
			string a = Get("SudokuPage_InfoBar_SaveFailed1");
			string b = Get("SudokuPage_InfoBar_SaveFailed2");
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, $"{a}{fileName}{b}");
		}
	}

	/// <summary>
	/// To paste the text via the clipboard asynchonously.
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
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_PasteIsInvalid"));
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_PastePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, Get("SudokuPage_InfoBar_PasteSuccessfully"));
	}

	/// <summary>
	/// Try to generate a sudoku puzzle, to display onto the sudoku pane.
	/// </summary>
	/// <param name="button">The button.</param>
	/// <returns>The typical awaitable instance that holds the task to generate the puzzle.</returns>
	private async Task GenerateAsync(AppBarButton button)
	{
		// Disable the control to prevent re-invocation.
		button.IsEnabled = false;

		// Generate the puzzle.
		// The generation may be slow, so we should use asynchronous invocation instead of the synchronous one.
		// TODO: May allow the user cancelling the puzzle-generating operation.
		var grid = await Task.Run(static () => Generator.Generate());

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
		string part1 = Get("SudokuPage_InfoBar_GeneratingSuccessfully1");
		string part2 = Get("SudokuPage_InfoBar_GeneratingSuccessfully2");
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{part1}\r\n{part2}{grid.GivensCount}");
	}

	/// <summary>
	/// Gets the solution of the grid.
	/// </summary>
	private void GetSolution()
	{
		// Gets the grid and its solution, then check it.
		ref readonly var grid = ref _cPane.GridByReference();
		if (grid is not { IsValid: true, Solution: { IsUndefined: false } solution })
		{
			return;
		}

		// Add message to tell the user the grid has been successfully solved.
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, Get("SudokuPage_InfoBar_GetSolutionSuccessfully"));

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
		// Disable the control to prevent re-invocation.
		button.IsEnabled = false;

		// Solve the puzzle using the manual solver.
		var analysisResult = await Task.Run(() => (ManualSolverResult)Solver.Solve(_cPane.GridByReference()));

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
			string firstPart = Get("SudokuPage_InfoBar_AnalyzeFailedDueTo1");
			string secondPart = Get(failedReason switch
			{
				FailedReason.UserCancelled => "SudokuPage_InfoBar_AnalyzeFailedDueToUserCancelling",
				FailedReason.NotImplemented => "SudokuPage_InfoBar_AnalyzeFailedDueToNotImplemented",
				FailedReason.ExceptionThrown => "SudokuPage_InfoBar_AnalyzeFailedDueToExceptionThrown",
				FailedReason.WrongStep => "SudokuPage_InfoBar_AnalyzeFailedDueToWrongStep",
				FailedReason.PuzzleIsTooHard => "SudokuPage_InfoBar_AnalyzeFailedDueToPuzzleTooHard",
				_ => throw new InvalidOperationException("The specified failed reason is not supported.")
			});
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, $"{firstPart}{secondPart}{wrongStep}");
		}
	}


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Loaded(object sender, RoutedEventArgs e) =>
		InitialAddSudokuTechniqueInfoBar();

	/// <summary>
	/// Triggers when the inner collection of the control <see cref="_cInfoBoard"/> is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void InfoBoard_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
		UpdateIsEnabledStatus();

	/// <summary>
	/// Indicates the event trigger callback method that determines
	/// whether the current window status can execute the following operation.
	/// </summary>
	private void CommandOpenOrSaveSudokuFile_CanExecuteRequested(
		XamlUICommand sender, CanExecuteRequestedEventArgs args) => EnsureUnsnapped();

	/// <summary>
	/// Indicates the event trigger callback method that executes opening sudoku file.
	/// </summary>
	private async void CommandOpenSudokuFile_ExecuteRequestedAsync(
		XamlUICommand sender, ExecuteRequestedEventArgs args) => await OpenFileAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// copying the string text representing as the current sudoku grid.
	/// </summary>
	private void CommandCopySudokuGridText_ExecuteRequested(
		XamlUICommand sender, ExecuteRequestedEventArgs args) => CopySudokuCode();

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// copying the snapshot of the sudoku grid control.
	/// </summary>
#if true
	private void CommandCopyControlSnapshot_ExecuteRequestedAsync(
		XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
	}
#else
	private async void CommandCopyControlSnapshot_ExecuteRequestedAsync(
		XamlUICommand sender, ExecuteRequestedEventArgs args) => await CopySnapshotAsync();
#endif

	/// <summary>
	/// Indicates the event trigger callback method that executes
	/// parsing the string text representing as a sudoku grid from the clipboard.
	/// </summary>
	private async void CommandPasteSudokuGridText_ExecuteRequestedAsync(
		XamlUICommand sender, ExecuteRequestedEventArgs args) => await PasteAsync();

	/// <summary>
	/// Indicates the event trigger callback method that executes saving sudoku file.
	/// </summary>
	private async void CommandSaveSudokuFile_ExecuteRequestedAsync(
		XamlUICommand sender, ExecuteRequestedEventArgs args) => await SaveFileAsync();

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
	private void CommandReset_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		ResetGrid();

	/// <summary>
	/// Indicates the event trigger callback method that executes fixing digits.
	/// </summary>
	private void CommandFix_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => FixGrid();

	/// <summary>
	/// Indicates the event trigger callback method that executes unfixing digits.
	/// </summary>
	private void CommandUnfix_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		UnfixGrid();

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
	private void CommandClearMessages_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		ClearMessages();

	/// <summary>
	/// Indicates the event trigger callback method that executes generating a puzzle.
	/// </summary>
	private async void CommandGenerate_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		await GenerateAsync(_cButtonGenerate);

	/// <summary>
	/// Indicates the event trigger callback method that gets the solution of the puzzle.
	/// </summary>
	private void CommandGetSolution_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		GetSolution();

	/// <summary>
	/// Indicates the event trigger callback method that analyzes the puzzle.
	/// </summary>
	private async void CommandAnalysis_ExecuteRequestedAsync(XamlUICommand sender, ExecuteRequestedEventArgs args) =>
		await AnalyzeAsync(_cButtonAnalyze);
}
