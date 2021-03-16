using System;
using System.Windows;
using Sudoku.Data;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using static System.Environment;
using static Sudoku.Windows.MainWindow;

namespace Sudoku.Windows
{
	/// <summary>
	/// Provides all messages used for <see cref="MessageBox"/> instance.
	/// </summary>
	/// <seealso cref="MessageBox"/>
	internal static class Messagings
	{
		/// <summary>
		/// Indicates the message for only opening one program.
		/// </summary>
		public static void YouCanOnlyOpenOneProgram() =>
			MessageBox.Show(
				(string)LangSource["OnlyOpenOneProgram"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for function being unavailable.
		/// </summary>
		public static void FunctionIsUnavailable() =>
			MessageBox.Show(
				(string)LangSource["FunctionIsUnavailable"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for wrong handling.
		/// </summary>
		/// <param name="errorInfo">The error information.</param>
		/// <param name="grid">The grid.</param>
		public static void WrongHandling(StepInfo errorInfo, in SudokuGrid grid) =>
			ErrorInfoWindow.Create(errorInfo, grid).ShowDialog();

		/// <summary>
		/// Indicates the message for failed to backup the configurations.
		/// </summary>
		public static void FailedToBackupConfig() =>
			MessageBox.Show(
				(string)LangSource["FailedToBackupConfig"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load pictures.
		/// </summary>
		public static void FailedToLoadPicture() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadPicture"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to load globalization files.
		/// </summary>
		public static void FailedToLoadGlobalizationFile() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadGlobalizationFile"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load pictures due to not having initialized.
		/// </summary>
		public static void FailedToLoadPictureDueToNotHavingInitialized() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadPictureDueToNotHavingInitialized"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for loading pictures.
		/// </summary>
		public static void NotSupportedForLoadingPicture() =>
			MessageBox.Show(
				(string)LangSource["NotSupportedForLoadingPicture"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for loading database.
		/// </summary>
		/// <param name="puzzlesCount">The number of puzzles successfully loading.</param>
		public static void LoadDatabase(int puzzlesCount) =>
			MessageBox.Show(
				LangSource["LoadInfo"] +
				$" {puzzlesCount.ToString()} {LangSource["LoadPuzzles"]}",
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard when using multiple threads.
		/// </summary>
		public static void FailedToSaveToClipboardDueToAsyncCalling() =>
			MessageBox.Show(
				(string)LangSource["FailedToSaveToClipboardDueToAsyncCalling"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while loading puzzles.
		/// </summary>
		public static void FailedToLoadPuzzle() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadPuzzle"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load database.
		/// </summary>
		public static void FailedToLoadDatabase() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadDatabase"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load settings.
		/// </summary>
		public static void FailedToLoadSettings() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadSettings"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load drawing contents.
		/// </summary>
		public static void FailedToLoadDrawingContents() =>
			MessageBox.Show(
				(string)LangSource["FailedToLoadDrawingContents"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to save drawing contents.
		/// </summary>
		public static void FailedToSaveDrawingContents() =>
			MessageBox.Show(
				(string)LangSource["FailedToSaveDrawingContents"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to save drawing contents due to the empty content.
		/// </summary>
		public static void FailedToSaveDrawingContentsDueToEmpty() =>
			MessageBox.Show(
				(string)LangSource["FailedToSaveDrawingContentsDueToEmpty"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to load recognition tools.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToLoadRecognitionTool(Exception ex) =>
			MessageBox.Show(
				$"{LangSource["CannotCalculate"]}{NewLine}" +
				$"  {LangSource["Source"]}{ex.Source}{NewLine}" +
				$"  {LangSource["Message"]}{ex.Message}",
				(string)LangSource["CaptionError"],
				MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// Indicates the message for failed to save configurations.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveConfig(Exception ex) =>
			MessageBox.Show(
				$"{LangSource["FailedToSaveConfig"]}{NewLine}" +
				$"  {LangSource["Message"]}{ex.Message}",
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveToClipboardDueToArgumentNullException(Exception ex) =>
			MessageBox.Show(
				$"{LangSource["FailedToSaveToClipboardDueToArgumentNullException"]}{NewLine}" +
				$"  {LangSource["Message"]}{ex.Message}",
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// To show the message of an exception.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		public static void ShowExceptionMessage(Exception ex) =>
			MessageBox.Show(ex.Message, (string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that sukakus can't use this function.
		/// </summary>
		public static void SukakuCannotUseThisFunction() =>
			MessageBox.Show(
				(string)LangSource["SukakuCannotUseThisFunction"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that succeed to save a file.
		/// </summary>
		public static void SaveSuccess() =>
			MessageBox.Show(
				(string)LangSource["SaveSuccess"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to load sukaku text.
		/// </summary>
		public static void FailedToPasteText() =>
			MessageBox.Show(
				(string)LangSource["FailedToPasteText"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to check due to invalid puzzle.
		/// </summary>
		public static void FailedToCheckDueToInvalidPuzzle() =>
			MessageBox.Show(
				(string)LangSource["FailedToCheckDueToInvalidPuzzle"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that the puzzle doesn't exist any BUG digit.
		/// </summary>
		public static void DoesNotContainBugMultiple() =>
			MessageBox.Show(
				(string)LangSource["DoesNotContainBugMultiple"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the puzzle doesn't exist any backdoors of the level 0 or 1.
		/// </summary>
		public static void DoesNotContainBackdoor() =>
			MessageBox.Show(
				(string)LangSource["DoesNotContainBackdoor"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while generating with the technique filter.
		/// </summary>
		public static void NotSupportedWhileGeneratingWithFilter() =>
			MessageBox.Show(
				(string)LangSource["NotSupportedWhileGeneratingWithFilter"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while applying a puzzle.
		/// </summary>
		public static void FailedToApplyPuzzle() =>
			MessageBox.Show(
				(string)LangSource["FailedToApplyPuzzle"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for a puzzle having been solved.
		/// </summary>
		public static void PuzzleAlreadySolved() =>
			MessageBox.Show(
				(string)LangSource["PuzzleAlreadySolved"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to solve a puzzle.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		public static void FailedToSolveWithMessage(AnalysisResult analysisResult)
		{
			switch (analysisResult.Additional)
			{
				case WrongStepException { InvalidGrid: var grid, WrongStep: { } info }:
				{
					ErrorInfoWindow.Create(info, grid).ShowDialog();

					break;
				}
				default:
				{
					MessageBox.Show(
						$"{LangSource["FailedToSolveWithMessage1"]}{NewLine}" +
						$"{LangSource["FailedToSolveWithMessage2"]}{NewLine}",
						(string)LangSource["CaptionWarning"]);

					break;
				}
			}
		}

		/// <summary>
		/// Indicates the message that you should solve the puzzle first.
		/// </summary>
		public static void YouShouldSolveFirst() =>
			MessageBox.Show(
				(string)LangSource["YouShouldSolveFirst"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that sukaku can't use GSP checking.
		/// </summary>
		public static void SukakuCannotUseGspChecking() =>
			MessageBox.Show(
				(string)LangSource["SukakuCannotUseGspChecking"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the puzzle doesn't exist GSP hint.
		/// </summary>
		public static void DoesNotContainGsp() =>
			MessageBox.Show(
				(string)LangSource["DoesNotContainGsp"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the step can't be copied.
		/// </summary>
		public static void CannotCopyStep() =>
			MessageBox.Show(
				(string)LangSource["CannotCopyStep"],
				(string)LangSource["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that warn you to check your input.
		/// </summary>
		public static void CheckInput() =>
			MessageBox.Show(
				(string)LangSource["CheckInput"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that warn you to check the format string.
		/// </summary>
		public static void CheckFormatString() =>
			MessageBox.Show(
				(string)LangSource["CheckFormatString"],
				(string)LangSource["CaptionInfo"]);

		public static void InvalidFilter() =>
			MessageBox.Show(
				(string)LangSource["InvalidFilter"],
				(string)LangSource["CaptionInfo"]);

		public static void AnalyzeEmptyGrid() =>
			MessageBox.Show(
				(string)LangSource["AnalyzeEmptyGrid"],
				(string)LangSource["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while quitting.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileQuitting() =>
			MessageBox.Show(
				(string)LangSource["AskWhileQuitting"],
				(string)LangSource["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while loading and covering the database.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingAndCoveringDatabase() =>
			MessageBox.Show(
				(string)LangSource["AskWhileLoadingAndCoveringDatabase"],
				(string)LangSource["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while loading pictures.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingPicture() =>
			MessageBox.Show(
				$"{LangSource["AskWhileLoadingPicture1"]}{NewLine}" +
				$"{LangSource["AskWhileLoadingPicture2"]}{NewLine}" +
				LangSource["AskWhileLoadingPicture3"],
				(string)LangSource["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while clearing stack.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileClearingStack() =>
			MessageBox.Show(
				$"{LangSource["AskWhileClearingStack1"]}{NewLine}" +
				LangSource["AskWhileClearingStack2"],
				(string)LangSource["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message for clearing database while generating.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileGeneratingWithDatabase() =>
			MessageBox.Show(
				(string)LangSource["AskWhileGeneratingWithDatabase"],
				(string)LangSource["CaptionInfo"],
				MessageBoxButton.YesNo);
	}
}
