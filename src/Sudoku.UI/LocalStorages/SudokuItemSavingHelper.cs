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
		if ((file, page) is not (
#pragma warning disable format
				{ Name: var fileName, Path: var filePath },
				{ _cPane.Grid: var grid, _cInfoBoard: var board }
			))
#pragma warning restore format
		{
			return false;
		}

		var code = grid.ToString("#");

		await SioFile.WriteAllTextAsync(filePath, code);

		var a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
		var b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
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
		if ((file, page) is not ({ Path: var filePath, Name: var fileName }, { _cPane: var pane, _cInfoBoard: var board }))
		{
			return false;
		}

		await pane.RenderToAsync(file);

		var a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
		var b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
		board.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");

		if (((App)Application.Current).UserPreference.AlsoSavePictureWhenSaveDrawingData)
		{
			filePath = SioPath.ChangeExtension(filePath, CommonFileExtensions.DrawingData);
			await DrawingDataSaveAsync(filePath, fileName, page);
		}

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
		=> file is { Path: var filePath, Name: var fileName } && await DrawingDataSaveAsync(filePath, fileName, page);

	/// <summary>
	/// Saves with drawing data format.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="fileName">The file name.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	private static async Task<bool> DrawingDataSaveAsync(string filePath, string fileName, SudokuPage page)
	{
		switch (page._cPane.GetDisplayableUnit())
		{
			case null:
			{
				// Just return because the view is empty.
				return true;
			}
			case UserDefinedVisual visual:
			{
				var json = Serialize(visual, CamelCasing);

				await SioFile.WriteAllTextAsync(filePath, json);

				var a = R["SudokuPage_InfoBar_SaveSuccessfully1"]!;
				var b = R["SudokuPage_InfoBar_SaveSuccessfully2"]!;
				page._cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");

				return true;
			}
			default:
			{
				// Specified view is not supported.
				var theFile = R["SudokuPage_InfoBar_SaveFailed1"]!;
				var isFailedToBeSaved = R["SudokuPage_InfoBar_SaveFailed2"]!;
				var theReasonIs = R["ReasonIs"]!;
				var currentDisplayableTypeIsNotSupported = R["SudokuPage_InfoBar_SaveFailedReason_CurrentDisplayableTypeIsNotSupported"]!;
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
