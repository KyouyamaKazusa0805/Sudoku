using System;
using System.Windows;
using Sudoku.Solving;
using static Sudoku.Windows.Constants.Processings;

namespace Sudoku.Windows.Constants
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
		public static void OnlyOpenOneProgram() =>
			MessageBox.Show(
				(string)Application.Current.Resources["OnlyOpenOneProgram"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for function being unavailable.
		/// </summary>
		public static void FunctionIsUnavailable() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FunctionIsUnavailable"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for wrong handling.
		/// </summary>
		public static void WrongHandling() =>
			MessageBox.Show(
				(string)Application.Current.Resources["WrongHandling"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to backup the configurations.
		/// </summary>
		public static void FailedToBackupConfig() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToBackupConfig"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load pictures.
		/// </summary>
		public static void FailedToLoadPicture() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadPicture"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to load globalization files.
		/// </summary>
		public static void FailedToLoadGlobalizationFile() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadGlobalizationFile"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load pictures due to not having initialized.
		/// </summary>
		public static void FailedToLoadPictureDueToNotHavingInitialized() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadPictureDueToNotHavingInitialized"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for loading pictures.
		/// </summary>
		public static void NotSupportedForLoadingPicture() =>
			MessageBox.Show(
				(string)Application.Current.Resources["NotSupportedForLoadingPicture"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for loading database.
		/// </summary>
		/// <param name="puzzlesCount">The number of puzzles successfully loading.</param>
		public static void LoadDatabase(int puzzlesCount) =>
			MessageBox.Show(
				Application.Current.Resources["LoadInfo"] +
				$" {puzzlesCount} {Application.Current.Resources["LoadPuzzles"]}",
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard when using multiple threads.
		/// </summary>
		public static void FailedToSaveToClipboardDueToAsyncCalling() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToSaveToClipboardDueToAsyncCalling"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while loading puzzles.
		/// </summary>
		public static void FailedToLoadPuzzle() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadPuzzle"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load database.
		/// </summary>
		public static void FailedToLoadDatabase() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadDatabase"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load settings.
		/// </summary>
		public static void FailedToLoadSettings() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToLoadSettings"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to load recognition tools.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToLoadRecognitionTool(Exception ex) =>
			MessageBox.Show(
				$"{Application.Current.Resources["CannotCalculate"]}{NewLine}" +
				$"  {Application.Current.Resources["Source"]}{ex.Source}{NewLine}" +
				$"  {Application.Current.Resources["Message"]}{ex.Message}",
				(string)Application.Current.Resources["CaptionError"],
				MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// Indicates the message for failed to save configurations.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveConfig(Exception ex) =>
			MessageBox.Show(
				$"{Application.Current.Resources["FailedToSaveConfig"]}{NewLine}" +
				$"  {Application.Current.Resources["Message"]}{ex.Message}",
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveToClipboardDueToArgumentNullException(Exception ex) =>
			MessageBox.Show(
				$"{Application.Current.Resources["FailedToSaveToClipboardDueToArgumentNullException"]}{NewLine}" +
				$"  {Application.Current.Resources["Message"]}{ex.Message}",
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// To show the message of an exception.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		public static void ShowExceptionMessage(Exception ex) =>
			MessageBox.Show(ex.Message, (string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that sukakus cannot use this function.
		/// </summary>
		public static void SukakuCannotUseThisFunction() =>
			MessageBox.Show(
				(string)Application.Current.Resources["SukakuCannotUseThisFunction"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to load sukaku text.
		/// </summary>
		public static void FailedToPasteText() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToPasteText"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message for failed to check due to invalid puzzle.
		/// </summary>
		public static void FailedToCheckDueToInvaildPuzzle() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToCheckDueToInvaildPuzzle"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that the puzzle does not exist any BUG digit.
		/// </summary>
		public static void DoesNotContainBugMultiple() =>
			MessageBox.Show(
				(string)Application.Current.Resources["DoesNotContainBugMultiple"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the puzzle does not exist any backdoors of the level 0 or 1.
		/// </summary>
		public static void DoesNotContainBackdoor() =>
			MessageBox.Show(
				(string)Application.Current.Resources["DoesNotContainBackdoor"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while generating with the technique filter.
		/// </summary>
		public static void NotSupportedWhileGeneratingWithFilter() =>
			MessageBox.Show(
				(string)Application.Current.Resources["NotSupportedWhileGeneratingWithFilter"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message while applying a puzzle.
		/// </summary>
		public static void FailedToApplyPuzzle() =>
			MessageBox.Show(
				(string)Application.Current.Resources["FailedToApplyPuzzle"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for a puzzle having been solved.
		/// </summary>
		public static void PuzzleAlreadySolved() =>
			MessageBox.Show(
				(string)Application.Current.Resources["PuzzleAlreadySolved"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message for failed to solve a puzzle.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		public static void FailedToSolveWithMessage(AnalysisResult analysisResult) =>
			MessageBox.Show(
				$"{Application.Current.Resources["FailedToSolveWithMessage1"]}{NewLine}" +
				$"{Application.Current.Resources["FailedToSolveWithMessage2"]}{NewLine}" +
				$"{Application.Current.Resources["FailedToSolveWithMessage3"]}{analysisResult.Additional}",
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message that you should solve the puzzle first.
		/// </summary>
		public static void YouShouldSolveFirst() =>
			MessageBox.Show(
				(string)Application.Current.Resources["YouShouldSolveFirst"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that sukaku cannot use GSP checking.
		/// </summary>
		public static void SukakuCannotUseGspChecking() =>
			MessageBox.Show(
				(string)Application.Current.Resources["SukakuCannotUseGspChecking"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the puzzle does not exist GSP hint.
		/// </summary>
		public static void DoesNotContainGsp() =>
			MessageBox.Show(
				(string)Application.Current.Resources["DoesNotContainGsp"],
				(string)Application.Current.Resources["CaptionInfo"]);

		/// <summary>
		/// Indicates the message that the step cannot be copied.
		/// </summary>
		public static void CannotCopyStep() =>
			MessageBox.Show(
				(string)Application.Current.Resources["CannotCopyStep"],
				(string)Application.Current.Resources["CaptionWarning"]);

		/// <summary>
		/// Indicates the message while quitting.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileQuitting() =>
			MessageBox.Show(
				(string)Application.Current.Resources["AskWhileQuitting"],
				(string)Application.Current.Resources["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while loading and covering the database.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingAndCoveringDatabase() =>
			MessageBox.Show(
				(string)Application.Current.Resources["AskWhileLoadingAndCoveringDatabase"],
				(string)Application.Current.Resources["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while loading pictures.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingPicture() =>
			MessageBox.Show(
				$"{Application.Current.Resources["AskWhileLoadingPicture1"]}{NewLine}" +
				$"{Application.Current.Resources["AskWhileLoadingPicture2"]}{NewLine}" +
				Application.Current.Resources["AskWhileLoadingPicture3"],
				(string)Application.Current.Resources["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while clearing stack.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileClearingStack() =>
			MessageBox.Show(
				$"{Application.Current.Resources["AskWhileClearingStack1"]}{NewLine}" +
				Application.Current.Resources["AskWhileClearingStack2"],
				(string)Application.Current.Resources["CaptionInfo"],
				MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message for clearing database while generating.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileGeneratingWithDatabase() =>
			MessageBox.Show(
				(string)Application.Current.Resources["AskWhileGeneratingWithDatabase"],
				(string)Application.Current.Resources["CaptionInfo"],
				MessageBoxButton.YesNo);
	}
}
