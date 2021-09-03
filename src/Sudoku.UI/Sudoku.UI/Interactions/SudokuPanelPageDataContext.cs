namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="SudokuPanelPage"/>.
/// </summary>
/// <seealso cref="SudokuPanelPage"/>
public sealed class SudokuPanelPageDataContext : IDataContext<SudokuPanelPageDataContext>
{
	/// <inheritdoc/>
	public static SudokuPanelPageDataContext CreateInstance() => new();
}
