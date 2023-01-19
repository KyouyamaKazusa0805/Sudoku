namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about puzzle details displayed.
/// </summary>
internal static class SudokuGridConversion
{
	/// <summary>
	/// Defines a solver.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	public static string GetPuzzleHintsCount(Grid grid)
		=> grid switch
		{
			{ IsUndefined: true } => GetString("AnalyzePage_UndefinedGrid"),
			{ IsEmpty: true } => GetString("AnalyzePage_EmptyGrid"),
			{ GivensCount: var givens } => givens.ToString()
		};

	public static string GetPuzzleCode(Grid grid)
		=> grid switch
		{
			{ IsUndefined: true } => GetString("AnalyzePage_UndefinedGrid"),
			{ IsEmpty: true } => GetString("AnalyzePage_EmptyGrid"),
			_ => grid.ToString("#")
		};

	public static unsafe string GetPuzzleUniqueness(Grid grid)
	{
		if (grid.IsUndefined)
		{
			return GetString("AnalyzePage_PuzzleHasNoSolution");
		}

		if (grid.IsEmpty)
		{
			return GetString("AnalyzePage_PuzzleHasMultipleSolutions");
		}

		var hasNoGivenCells = grid.GivensCount == 0;
		var str = hasNoGivenCells ? grid.ToString("!") : grid.ToString();
		return GetString(
			Solver.Solve(str, null, 2) switch
			{
				0 => "AnalyzePage_PuzzleHasNoSolution",
				1 => hasNoGivenCells ? "AnalyzePage_PuzzleHasUniqueSolutionButUnfixed" : "AnalyzePage_PuzzleHasUniqueSolution",
				_ => "AnalyzePage_PuzzleHasMultipleSolutions"
			}
		);
	}

	public static unsafe string GetIsMinimal(Grid grid)
	{
		if (grid is { IsUndefined: true } or { IsEmpty: true })
		{
			return GetString("AnalyzePage_MinimalResult_NotUniquePuzzle");
		}

		var hasNoGivenCells = grid.GivensCount == 0;
		var str = hasNoGivenCells ? grid.ToString("!") : grid.ToString();
		var solutions = Solver.Solve(str, null, 2);
		if (solutions != 1)
		{
			return GetString("AnalyzePage_MinimalResult_NotUniquePuzzle");
		}

		if (!MinimalPuzzleChecker.IsMinimal(grid, out var firstCandidateMakePuzzleNotMinimal))
		{
			return string.Format(
				GetString(
					hasNoGivenCells
						? "AnalyzePage_MinimalResult_AtLeastOneHintCanBeRemoved2"
						: "AnalyzePage_MinimalResult_AtLeastOneHintCanBeRemoved"
				),
				RxCyNotation.ToCandidateString(firstCandidateMakePuzzleNotMinimal)
			);
		}

		return GetString(GetString(hasNoGivenCells ? "AnalyzePage_MinimalResult_Yes2" : "AnalyzePage_MinimalResult_Yes"));
	}
}
