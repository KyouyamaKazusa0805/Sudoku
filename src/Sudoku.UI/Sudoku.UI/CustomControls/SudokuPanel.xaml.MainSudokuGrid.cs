namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	/// <summary>
	/// Triggers when the specified object received a file that dragged-and-dropped here.
	/// </summary>
	/// <param name="sender">The instance to trigger this event.</param>
	/// <param name="e">The event data provided.</param>
	private async partial void MainSudokuGrid_DragEnterAsync([Discard] object sender, DragEventArgs e)
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
#if !WINDOWS_APP
		catch (TypeInitializationException ex) when (ex is { InnerException: AssemblyFailedToLoadException })
		{
			// For the consideration of the compatibility, the resource dictionary in project 'Sudoku.Core'
			// must throw an exception here, because the interaction type 'TextResources' is deprecated here.
			
			// This case is just a trick. This project is always a Windows UI project.
			throw;
		}
#endif
#if DEBUG
		catch
		{
		}
#endif


		async Task i(CancellationToken cancellationToken = default)
		{
			// Directly gets the 'e.DataView' value.
			// Argument 'sender' holds the same value as the property 'e.OriginalSource' holds. Just check one.
			if (e is not { DataView: var view, OriginalSource: Grid })
			{
				FailedDragPuzzleFile_FileFormatFailed.IsOpen = true;
				return;
			}

			var puzzle = SudokuGrid.Undefined;

			// Checks whether the drag data is a file.
			if (view.Contains(StandardDataFormats.StorageItems))
			{
				// Checks whether the drag item contains only one file.
				// User can drag multiple files, but we only allows the user drag one file.
				if (await view.GetStorageItemsAsync() is not { Count: 1 } items)
				{
					FailedDragPuzzleFile_MultipleFilesSelected.IsOpen = true;
					return;
				}

				// Checks whether the drag file type and the path.
				if (items[0] is not StorageFile { FileType: TextFormat or SudokuFormat, Path: var path })
				{
					FailedDragPuzzleFile_FileFormatFailed.IsOpen = true;
					return;
				}

				// Here we got the file path. Now get the specified file content, and parse the inner value.
				string content = await File.ReadAllTextAsync(path, cancellationToken);
				if (!SudokuGrid.TryParse(content, out var result))
				{
					FailedDragPuzzleFile_FileFormatFailed.IsOpen = true;
					return;
				}

				puzzle = result;
			}
			// Checks whether the drag data is a text content.
			else if (view.Contains(StandardDataFormats.Text))
			{
				// Checks whether the drag text isn't empty or null.
				string? text = await view.GetTextAsync();
				if (string.IsNullOrWhiteSpace(text))
				{
					FailedDragPuzzleText_TextFailed.IsOpen = true;
					return;
				}

				// Here we got the valid text content. Now parse the value to a sudoku type instance.
				if (!SudokuGrid.TryParse(text, out var result))
				{
					FailedDragPuzzleText_TextFailed.IsOpen = true;
					return;
				}

				puzzle = result;
			}
			else
			{
				// Other drag data types are invalid.
				FailedDragPuzzleFile_FileFormatFailed.IsOpen = true;
				return;
			}

			try
			{
				// Try to load sudoku puzzle.
				// If found any possible cases, just check them, and them report the error cases.
				GridCanvas.LoadSudoku(puzzle);
			}
			catch (InvalidPuzzleException ex)
			{
				// Show the teaching tip to display the error information.
				(
					ex.Reason switch
					{
						var r when r == UiResources.Current.TeachingTip_FailedDragPuzzleFile_Content_UndefinedFailed =>
							FailedDragPuzzleFile_UndefinedFailed,

						var r when r == UiResources.Current.TeachingTip_FailedDragPuzzleFile_Content_UniquenessFailed =>
							FailedDragPuzzleFile_UniquenessFailed,

#if DEBUG
						var r when r == UiResources.Current.TeachingTip_FailedDragPuzzleFile_Content_DebuggerUndefinedFailed1 =>
							FailedDragPuzzleFile_DebuggerUndefinedFailed,
#endif

						_ => null
					}
				).Open();
			}
		}
	}
}
