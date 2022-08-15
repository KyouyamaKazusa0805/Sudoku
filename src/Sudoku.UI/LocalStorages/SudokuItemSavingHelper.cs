namespace Sudoku.UI.LocalStorages;

/// <summary>
/// Represents a list of methods that handles and saves the files with supported sudoku formats.
/// </summary>
internal static class SudokuItemSavingHelper
{
	/// <summary>
	/// Saves with plain text format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> PlainTextSaveAsync(StorageFile file, SudokuPage page)
	{
		if (
#pragma warning disable IDE0055
			(file, page) is not (
				{ Name: var fileName, Path: var filePath },
				{ _cPane.Grid: var grid, _cInfoBoard: var board }
			)
#pragma warning restore IDE0055
		)
		{
			return false;
		}

		string code = grid.ToString("#");

		await SioFile.WriteAllTextAsync(filePath, code);

		string a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
		string b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
		board.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");

		return true;
	}

	/// <summary>
	/// Saves with picture format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> PictureSaveAsync(StorageFile file, SudokuPage page)
	{
		if ((file, page) is not ({ Name: var fileName }, { _cPane: var pane, _cInfoBoard: var board }))
		{
			return false;
		}

		await pane.RenderToAsync(file);

		string a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
		string b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
		board.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");

		return true;
	}

	/// <summary>
	/// Saves with drawing data format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> DrawingDataSaveAsync(StorageFile file, SudokuPage page)
	{
		if (file is not { Path: var filePath, Name: var fileName })
		{
			return false;
		}

		switch (page._cPane.GetDisplayableUnit())
		{
			case null:
			{
				// Just return because the view is empty.
				return true;
			}
			case UserDefinedDisplayable displayable:
			{
				string json = Serialize(displayable, CommonSerializerOptions.CamelCasing);

				await SioFile.WriteAllTextAsync(filePath, json);

				string a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
				string b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
				page._cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");

				return true;
			}
			default:
			{
				// Specified view is not supported.
				string theFile = R["SudokuPage_InfoBar_SaveFailed1"]!;
				string isFailedToBeSaved = R["SudokuPage_InfoBar_SaveFailed2"]!;
				string theReasonIs = R["ReasonIs"]!;
				string currentDisplayableTypeIsNotSupported = R["SudokuPage_InfoBar_SaveFailedReason_CurrentDisplayableTypeIsNotSupported"]!;
				page._cInfoBoard.AddMessage(
					InfoBarSeverity.Error,
					$"""
					{theFile}{fileName}{isFailedToBeSaved}{theReasonIs}
					{currentDisplayableTypeIsNotSupported}
					"""
				);

				return false;
			}
		}
	}
}
