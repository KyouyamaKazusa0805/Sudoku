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

	/// <summary>
	/// Render candidates with specified option.
	/// </summary>
	/// <param name="this">The <see cref="ISudokuPainter"/> instance.</param>
	/// <param name="candidatePrinting">The printing options.</param>
	/// <param name="difficultyLevel">The difficulty level.</param>
	/// <returns>The <see cref="ISudokuPainter"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when argument <paramref name="candidatePrinting"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform("windows")]
	internal static ISudokuPainter WithRenderingCandidates(
		this ISudokuPainter @this,
		CandidatePrintingOptions candidatePrinting,
		DifficultyLevel difficultyLevel
	)
	{
		@this.WithRenderingCandidates(
			candidatePrinting switch
			{
				CandidatePrintingOptions.Never => false,
				CandidatePrintingOptions.PrintIfPuzzleIndirect => difficultyLevel >= DifficultyLevel.Hard,
				CandidatePrintingOptions.Always => true,
				_ => throw new ArgumentOutOfRangeException(nameof(candidatePrinting))
			}
		);

		return @this;
	}
}
