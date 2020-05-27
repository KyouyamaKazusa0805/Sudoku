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
			MessageBox.Show("This program does not support multiple programs opening at the same time.", "Info");

		/// <summary>
		/// Indicates the message for function being unavailable.
		/// </summary>
		public static void FunctionIsUnavailable() =>
			MessageBox.Show("The puzzle is invalid, so you cannot use this function.", "Warning");

		/// <summary>
		/// Indicates the message for wrong handling.
		/// </summary>
		public static void WrongHandling() =>
			MessageBox.Show("The current step is wrong due to wrong calculation. Please contact the author", "Info");

		/// <summary>
		/// Indicates the message for failed to backup the configurations.
		/// </summary>
		public static void FailedToBackupConfig() =>
			MessageBox.Show("Configuration file is failed to save due to internal error.", "Warning");

		/// <summary>
		/// Indicates the message for failed to load pictures.
		/// </summary>
		public static void FailedToLoadPicture() =>
			MessageBox.Show("Failed to initialize. Please restart the window and try again.", "Info");

		/// <summary>
		/// Indicates the message for failed to load pictures due to not having initialized.
		/// </summary>
		public static void FailedToLoadPictureDueToNotHavingInitialized() =>
			MessageBox.Show("The OCR tool hasn't been initialized yet.", "Info");

		/// <summary>
		/// Indicates the message for loading pictures.
		/// </summary>
		public static void NotSupportedForLoadingPicture() =>
			MessageBox.Show("Your machine cannot use image recognition.", "Info");

		/// <summary>
		/// Indicates the message for loading database.
		/// </summary>
		/// <param name="puzzlesCount">The number of puzzles successfully loading.</param>
		public static void LoadDatabase(int puzzlesCount) => MessageBox.Show($"Load {puzzlesCount} puzzles.", "Info");

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard when using multiple threads.
		/// </summary>
		public static void FailedToSaveToClipboardDueToAsyncCalling() =>
			MessageBox.Show(
				"Your clipboard is unavailable now, " +
				"because the program is running for generating or solving." +
				"Please close this program or wait for finishing and try later.",
				"Info");

		/// <summary>
		/// Indicates the message while loading puzzles.
		/// </summary>
		public static void FailedToLoadPuzzle() =>
			MessageBox.Show("The specified puzzle is invalid.", "Warning");

		/// <summary>
		/// Indicates the message for failed to load database.
		/// </summary>
		public static void FailedToLoadDatabase() =>
			MessageBox.Show("File is missing... Load failed >_<", "Warning");

		/// <summary>
		/// Indicates the message for failed to load settings.
		/// </summary>
		public static void FailedToLoadSettings() =>
			MessageBox.Show("Failed to load the settings.", "Warning");

		/// <summary>
		/// Indicates the message for failed to load recognition tools.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToLoadRecognitionTool(Exception ex) =>
			MessageBox.Show(
				$"Cannot to calculate{NewLine}  Source: {ex.Source}{NewLine}  Message: {ex.Message}",
				"Error", MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// Indicates the message for failed to save configurations.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveConfig(Exception ex) =>
			MessageBox.Show(
				$"The configuration file cannot be saved due to exception throws:{NewLine}{ex.Message}",
				"Warning");

		/// <summary>
		/// Indicates the message of throwing an exception while saving to clipboard.
		/// </summary>
		/// <param name="ex">The exception.</param>
		public static void FailedToSaveToClipboardDueToArgumentNullException(Exception ex) =>
			MessageBox.Show(
				$"Cannot save text to clipboard due to:{NewLine}{ex.Message}",
				"Warning");

		/// <summary>
		/// To show the message of an exception.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		public static void ShowExceptionMessage(Exception ex) => MessageBox.Show(ex.Message, "Warning");

		/// <summary>
		/// Indicates the message that sukakus cannot use this function.
		/// </summary>
		public static void SukakuCannotUseThisFunction() =>
			MessageBox.Show(
				"The puzzle is invalid or may be a sukaku. " +
				"If invalid, Please check your input and retry; " +
				"however if sukaku, this function cannot use because the specified puzzle" +
				"has no given or modifiable values.", "Info");

		/// <summary>
		/// Indicates the message for failed to load sukaku text.
		/// </summary>
		public static void FailedToPasteText() =>
			MessageBox.Show("The specified puzzle pasted from clipboard is invalid.", "Warning");

		/// <summary>
		/// Indicates the message for failed to check due to invalid puzzle.
		/// </summary>
		public static void FailedToCheckDueToInvaildPuzzle() => MessageBox.Show("The puzzle is invalid.", "Warning");

		/// <summary>
		/// Indicates the message that the puzzle does not exist any BUG digit.
		/// </summary>
		public static void DoesNotContainBugMultiple() =>
			MessageBox.Show("The puzzle is not a valid BUG pattern.", "Info");

		/// <summary>
		/// Indicates the message that the puzzle does not exist any backdoors of the level 0 or 1.
		/// </summary>
		public static void DoesNotContainBackdoor() =>
			MessageBox.Show(
				"The puzzle does not have any backdoors whose level is 0 or 1, " +
				"which means the puzzle can be solved difficultly with brute forces.", "Info");

		/// <summary>
		/// Indicates the message while generating with the technique filter.
		/// </summary>
		public static void NotSupportedWhileGeneratingWithFilter() =>
			MessageBox.Show("This function is for debugging use now. Sorry. >_<", "Info");

		/// <summary>
		/// Indicates the message while applying a puzzle.
		/// </summary>
		public static void FailedToApplyPuzzle() =>
			MessageBox.Show("The puzzle is invalid. Please check your input and retry.", "Info");

		/// <summary>
		/// Indicates the message for a puzzle having been solved.
		/// </summary>
		public static void PuzzleAlreadySolved() =>
			MessageBox.Show("The puzzle has already solved.", "Info");

		/// <summary>
		/// Indicates the message for failed to solve a puzzle.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		public static void FailedToSolveWithMessage(AnalysisResult analysisResult) =>
			MessageBox.Show(
				$"The puzzle cannot be solved because " +
				$"the solver has found a wrong conclusion to apply " +
				$"or else the puzzle has eliminated the correct value.{NewLine}" +
				$"You should check the puzzle or notify the author.{NewLine}" +
				$"Error technique step: {analysisResult.Additional}",
				"Warning");

		/// <summary>
		/// Indicates the message that you should solve the puzzle first.
		/// </summary>
		public static void YouShouldSolveFirst() => MessageBox.Show("You should solve the puzzle first.", "Info");

		/// <summary>
		/// Indicates the message that sukaku cannot use GSP checking.
		/// </summary>
		public static void SukakuCannotUseGspChecking() =>
			MessageBox.Show("The sukaku puzzle does not support this function now.", "Info");

		/// <summary>
		/// Indicates the message that the puzzle does not exist GSP hint.
		/// </summary>
		public static void DoesNotContainGsp() =>
			MessageBox.Show("The puzzle does not contain any Gurth's symmetrical placement.", "Info");

		/// <summary>
		/// Indicates the message that the step cannot be copied.
		/// </summary>
		public static void CannotCopyStep() =>
			MessageBox.Show("Cannot copy due to internal error. Please try later.", "Warning");

		/// <summary>
		/// Indicates the message while quitting.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileQuitting() =>
			MessageBox.Show("Are you sure to quit?", "Info", MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while loading and covering the database.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingAndCoveringDatabase() =>
			MessageBox.Show(
				"You have used a database at the previous time you use the program. " +
				"Do you want to load now?",
				"Info", MessageBoxButton.YesNo);
		
		/// <summary>
		/// Indicates the message while loading pictures.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileLoadingPicture() =>
			MessageBox.Show(
				$"Ensure your picture be clear.{NewLine}" +
				$"Due to the limitation of the OCR algorithm, " +
				$"the program can only recognize the puzzle picture with given values. " +
				$"All modifiable values will be treated as given ones " +
				$"because OCR engine cannot recognize the color of the value.{NewLine}" +
				$"If you are not sure, please click NO button.",
				"Info", MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message while clearing stack.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileClearingStack() =>
			MessageBox.Show(
				$"The steps will be cleared. " +
				$"If so, you cannot undo any steps to previous puzzle status.{Environment.NewLine}" +
				$"Do you want to clear anyway?", "Info", MessageBoxButton.YesNo);

		/// <summary>
		/// Indicates the message for clearing database while generating.
		/// </summary>
		/// <returns>The <see cref="MessageBoxResult"/>.</returns>
		public static MessageBoxResult AskWhileGeneratingWithDatabase() =>
			MessageBox.Show(
				"You are now working on the puzzle database. " +
				"If you click YES button, the generating will start, " +
				"but the database history will be cleared." +
				"A hard decision, isn't it?", "Info", MessageBoxButton.YesNo);
	}
}
