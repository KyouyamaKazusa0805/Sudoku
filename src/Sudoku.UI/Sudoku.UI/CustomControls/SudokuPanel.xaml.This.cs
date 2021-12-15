namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	private partial void This_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		DispatcherQueue.TryEnqueue(
			async () =>
			{
				GridCanvas = await SudokuGridCanvas.CreateAsync(Preference, MainSudokuGrid);

				LoadingRing.IsActive = false;
			}
		);
}
