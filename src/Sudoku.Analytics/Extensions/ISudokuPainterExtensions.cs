namespace Sudoku.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="ISudokuPainter"/>.
/// </summary>
/// <seealso cref="ISudokuPainter"/>
public static class ISudokuPainterExtensions
{
	/// <summary>
	/// Sets <see cref="Conclusion"/>s and <see cref="View"/>s for the specified step.
	/// </summary>
	/// <param name="this">The sudoku painter instance.</param>
	/// <param name="step">The step instance.</param>
	/// <returns>Argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform("Windows")]
	public static ISudokuPainter WithStep(this ISudokuPainter @this, Step step)
		=> @this
			.WithConclusions(step.Conclusions)
			.WithNodes(from view in step.Views from node in view select node);
}
