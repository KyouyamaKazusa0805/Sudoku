namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
	/// <summary>
	/// Indicates the format that is the text one.
	/// </summary>
	private const string TextFormat = ".txt";

	/// <summary>
	/// Indicates the format that is the sudoku puzzle one.
	/// </summary>
	private const string SudokuFormat = ".sudoku";


	/// <summary>
	/// Initializes a <see cref="SudokuPanel"/> instance.
	/// </summary>
	public SudokuPanel()
	{
		InitializeComponent();

		GridCanvas = new(MainSudokuGrid);
	}


	/// <summary>
	/// Indicates the grid canvas.
	/// </summary>
	public SudokuGridCanvas GridCanvas { get; }


	/// <summary>
	/// Triggers when the specified object received a file that dragged-and-dropped here.
	/// </summary>
	/// <param name="_"></param>
	/// <param name="e">The event data provided.</param>
	private async void MainSudokuGrid_DragEnterAsync([Discard] object _, DragEventArgs e)
	{
		try
		{
			await i();
		}
#if DEBUG
		catch (ArgumentException ex) when (ex.HResult == unchecked(-2147024809))
		{
			// This exception thrown may suggests that the resource dictionary or the page
			// has already contained the same key or name (i.e. points to the 'x:Key' or 'x:Name' property).
			throw;
		}
		catch (COMException ex) when (ex.HResult == unchecked(-2147483634))
		{
			// This exception thrown may suggests that we incorrectly use the 'IAsyncOperation<>.GetResults()'.
			// Here we must use 'await IAsyncOperation<>' to execute something.
			throw;
		}
#endif
#if !WIN_UI_PROJECT
		catch (TypeInitializationException ex) when (ex is { InnerException: AssemblyFailedToLoadException })
		{
			// For the consideration of the compatibility, the resource dictionary in project 'Sudoku.Core'
			// must throw an exception here, because the interaction type 'TextResources' is deprecated here.
			throw;
		}
#endif
		catch
		{
		}


		async Task i(CancellationToken cancellationToken = default)
		{
			// The argument 'sender' holds the same value as the property 'e.OriginalSource' holds,
			// so just check one.
			if (e is not { DataView: var view, OriginalSource: Grid })
			{
				goto ShowInvalidFileFormatDialog;
			}

			if (!view.Contains(StandardDataFormats.StorageItems))
			{
				goto ShowInvalidFileFormatDialog;
			}

			if (await view.GetStorageItemsAsync() is not { Count: not 0 } items)
			{
				goto ShowInvalidFileFormatDialog;
			}

			if (items[0] is not StorageFile { FileType: TextFormat or SudokuFormat, Path: var path })
			{
				goto ShowInvalidFileFormatDialog;
			}

			string content = await File.ReadAllTextAsync(path, cancellationToken);
			if (!SudokuGrid.TryParse(content, out var result))
			{
				goto ShowInvalidFileFormatDialog;
			}

			try
			{
				// Try to load sudoku.
				// If found any possible cases, just check them, and them report the error cases.
				GridCanvas.LoadSudoku(result);

				goto Successful;
			}
			catch (InvalidPuzzleException ex)
			when (ex.Reason == UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UndefinedFailed)
			{
				await FailedDragPuzzleFile_UndefinedFailed.ShowAsync();

				goto ExceptionThrownButHandled;
			}
			catch (InvalidPuzzleException ex)
			when (ex.Reason == UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UniquenessFailed)
			{
				await FailedDragPuzzleFile_UniquenessFailed.ShowAsync();

				goto ExceptionThrownButHandled;
			}
#if DEBUG
			catch (InvalidPuzzleException ex)
			when (ex.Reason == UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_DebuggerUndefinedFailed1)
			{
				await FailedDragPuzzleFile_DebuggerUndefinedFailed.ShowAsync();

				goto ExceptionThrownButHandled;
			}
#endif

		ShowInvalidFileFormatDialog:
			await FailedDragPuzzleFile_FileFormatFailed.ShowAsync();
			return;

		Successful:
		ExceptionThrownButHandled:
			return;
		}
	}
}
