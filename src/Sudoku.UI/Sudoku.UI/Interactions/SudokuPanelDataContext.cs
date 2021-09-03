namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="SudokuPanel"/>.
/// </summary>
/// <seealso cref="SudokuPanel"/>
public sealed class SudokuPanelDataContext : IDataContext<SudokuPanelDataContext>
{
	/// <inheritdoc/>
	public static SudokuPanelDataContext CreateInstance() => new();
}
