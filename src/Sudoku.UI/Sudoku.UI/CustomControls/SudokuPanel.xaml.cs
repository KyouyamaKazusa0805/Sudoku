namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
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
		while (true)
		{
			try
			{
				await i();

				break;
			}
#if !WIN_UI_PROJECT
			catch (Exception ex) when (ex is AssemblyFailedToLoadException or TypeInitializationException)
			{
				// For the consideration of the compatibility, the resource dictionary in the project
				// 'Sudoku.Core' must throws an exception here.
				continue;
			}
#endif
			catch
			{
				break;
			}
		}


		async Task i(CancellationToken cancellationToken = default)
		{
			if (e is not { DataView: var view, OriginalSource: Grid })
			{
				goto OriginalSourceIsNotOfTypeGrid;
			}

			if (!view.Contains(StandardDataFormats.StorageItems))
			{
				goto ViewDoesNotContainStorageItems;
			}

			if (await view.GetStorageItemsAsync() is not { Count: not 0 } items)
			{
				goto ItemsListIsNullOrEmpty;
			}

			if (items[0] is not StorageFile { FileType: ".txt" or ".sudoku", Path: var path })
			{
				goto FirstItemIsNotValidStorageFile;
			}

			string content = await File.ReadAllTextAsync(path, cancellationToken);
			if (!SudokuGrid.TryParse(content, out var result))
			{
				goto ContentIsNotValidPuzzle;
			}

			try
			{
				GridCanvas.LoadSudoku(result);

				goto NoErrorEncountered;
			}
			catch (InvalidPuzzleException ex)
			{
				string? r = ex.Reason;
				Func<string, string, bool> e = string.Equals;

				if (e(r, UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UndefinedFailed))
				{
					await UiResources.Current.FailedDragPuzzleFile_UndefinedFailed.ShowAsync();
				}
#if DEBUG
				else if (e(r, UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_DebuggerUndefinedFailed1))
				{
					await UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_DebuggerUndefinedFailed1.ShowAsync();
				}
#endif
				else if (e(r, UiResources.Current.ContentDialog_FailedDragPuzzleFile_Content_UniquenessFailed))
				{
					await UiResources.Current.FailedDragPuzzleFile_UniquenessFailed.ShowAsync();
				}

				goto ErrorEncounteredButHandled;
			}

		OriginalSourceIsNotOfTypeGrid:
		ViewDoesNotContainStorageItems:
		ItemsListIsNullOrEmpty:
		FirstItemIsNotValidStorageFile:
		ContentIsNotValidPuzzle:
			await UiResources.Current.FailedDragPuzzleFile_FileFormatFailed.ShowAsync();

		NoErrorEncountered:
		ErrorEncounteredButHandled:
			;
		}
	}
}
