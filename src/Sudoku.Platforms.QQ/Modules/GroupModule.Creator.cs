namespace Sudoku.Platforms.QQ.Modules;

partial class GroupModule
{
	/// <summary>
	/// The internal creator.
	/// </summary>
	protected static class Creator
	{
		/// <summary>
		/// The solver that only uses SSTS techniques to solve a puzzle.
		/// </summary>
		private static readonly LogicalSolver SstsOnly = CommonLogicalSolvers.SstsOnly;


		/// <summary>
		/// Creates a new <see cref="ISudokuPainter"/> instance via the specified library name and puzzle index.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <param name="libName">The library name.</param>
		/// <param name="index">The index of the puzzle.</param>
		/// <returns>An <see cref="ISudokuPainter"/> instance.</returns>
		[SupportedOSPlatform("windows")]
		public static ISudokuPainter CreateDefaultSudokuPainter(scoped in Grid grid, string libName, int index)
			=> ISudokuPainter.Create(1000)
				.WithCanvasOffset(20)
				.WithGrid(grid)
				.WithRenderingCandidates(SstsOnly.Solve(grid) is { IsSolved: false })
				.WithFontScale(1.0M, .4M)
				.WithFooterText($"@{libName} #{index + 1}");
	}
}
