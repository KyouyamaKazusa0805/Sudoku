namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	private partial void This_Loaded([Discard] object sender, [Discard] RoutedEventArgs e) =>
		DispatcherQueue.TryEnqueue(
			async () =>
			{
				GridCanvas = await SudokuGridCanvas.CreateAsync(Preference, MainSudokuGrid);

				LoadingRing.IsActive = false;
			}
		);
}
