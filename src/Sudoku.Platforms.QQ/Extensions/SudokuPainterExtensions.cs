namespace Sudoku.Drawing;

/// <summary>
/// Provides with extension methods <see cref="ISudokuPainter"/>.
/// </summary>
/// <seealso cref="ISudokuPainter"/>
public static class SudokuPainterExtensions
{
	/// <summary>
	/// Render the <see cref="IStep"/> instance onto the target painter.
	/// </summary>
	/// <param name="this">The <see cref="ISudokuPainter"/> instance.</param>
	/// <param name="step">The step.</param>
	/// <returns>The <see cref="ISudokuPainter"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform("windows")]
	public static ISudokuPainter WithRenderingStep(this ISudokuPainter @this, IStep step)
		=> step switch
		{
			{ Views: [var view], Conclusions: var conclusions } => @this.AddNodes(view).WithConclusions(conclusions).WithRenderingCandidates(true),
			{ Conclusions: var conclusions } => @this.WithConclusions(conclusions).WithRenderingCandidates(true)
		};
}
