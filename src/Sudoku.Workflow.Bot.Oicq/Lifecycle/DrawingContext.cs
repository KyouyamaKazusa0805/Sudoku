namespace Sudoku.Workflow.Bot.Oicq.Lifecycle;

/// <summary>
/// The drawing context.
/// </summary>
internal sealed class DrawingContext
{
	/// <summary>
	/// The grid to be operated with.
	/// </summary>
	public Grid Puzzle { get; set; } = Grid.Empty;

	/// <summary>
	/// The painter.
	/// </summary>
	public ISudokuPainter? Painter { get; set; }
}
