namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">
	/// The instance to trigger the event. The argument is always the <see langword="this"/> reference.
	/// </param>
	/// <param name="e">The event arguments provided for the event.</param>
	private partial void This_Loaded([Discard] object sender, [Discard] RoutedEventArgs e) =>
		DispatcherQueue.TryEnqueue(
			async () =>
			{
				GridCanvas = await SudokuGridCanvas.CreateAsync(Preference, MainSudokuGrid);

				LoadingRing.IsActive = false;
			}
		);
}
