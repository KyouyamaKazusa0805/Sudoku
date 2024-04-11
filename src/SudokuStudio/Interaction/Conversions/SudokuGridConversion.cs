namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about puzzle details displayed.
/// </summary>
internal static class SudokuGridConversion
{
	/// <summary>
	/// Defines a solver.
	/// </summary>
	private static readonly ThreadLocal<BitwiseSolver> Solver = new(static () => new());


	public static bool GetFixedButtonAvailability(Grid grid) => grid.ModifiablesCount != 0;

	public static bool GetUnfixedButtonAvailability(Grid grid) => grid.GivensCount != 0;

	public static string GetPuzzleHintsCount(Grid grid)
		=> grid switch
		{
			{ IsUndefined: true } => ResourceDictionary.Get("AnalyzePage_UndefinedGrid", App.CurrentCulture),
			{ IsEmpty: true } => ResourceDictionary.Get("AnalyzePage_EmptyGrid", App.CurrentCulture),
			{ GivensCount: var givens } => givens.ToString()
		};

	public static string GetPuzzleCode(Grid grid)
	{
		var character = ((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter;
		return grid switch
		{
			{ IsUndefined: true } => ResourceDictionary.Get("AnalyzePage_UndefinedGrid", App.CurrentCulture),
			{ IsEmpty: true } => ResourceDictionary.Get("AnalyzePage_EmptyGrid", App.CurrentCulture),
			_ => grid.ToString($"#{character}")
		};
	}

	public static unsafe string GetPuzzleUniqueness(Grid grid)
	{
		if (grid.IsUndefined)
		{
			return ResourceDictionary.Get("AnalyzePage_PuzzleHasNoSolution", App.CurrentCulture);
		}

		if (grid.IsEmpty)
		{
			return ResourceDictionary.Get("AnalyzePage_PuzzleHasMultipleSolutions", App.CurrentCulture);
		}

		var character = ((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter;
		var hasNoGivenCells = grid.GivensCount == 0;
		var str = hasNoGivenCells ? grid.ToString($"!{character}") : grid.ToString();
		return ResourceDictionary.Get(
			Solver.Value!.SolveString(str, null, 2) switch
			{
				0 => "AnalyzePage_PuzzleHasNoSolution",
				1 => hasNoGivenCells ? "AnalyzePage_PuzzleHasUniqueSolutionButUnfixed" : "AnalyzePage_PuzzleHasUniqueSolution",
				_ => "AnalyzePage_PuzzleHasMultipleSolutions"
			},
			App.CurrentCulture
		);
	}

	public static string GetIsMinimal(Grid grid)
	{
		if (grid is { IsUndefined: true } or { IsEmpty: true })
		{
			return ResourceDictionary.Get("AnalyzePage_MinimalResult_NotUniquePuzzle", App.CurrentCulture);
		}

		if (!Solver.Value!.CheckValidity(grid.ToString()))
		{
			return ResourceDictionary.Get("AnalyzePage_MinimalResult_NotUniquePuzzle", App.CurrentCulture);
		}

		if (!grid.CheckMinimal(out var firstCandidateMakePuzzleNotMinimal))
		{
			return string.Format(
				ResourceDictionary.Get("AnalyzePage_MinimalResult_AtLeastOneHintCanBeRemoved", App.CurrentCulture),
				GlobalizedConverter.InvariantCultureConverter.CandidateConverter([firstCandidateMakePuzzleNotMinimal])
			);
		}

		return ResourceDictionary.Get("AnalyzePage_MinimalResult_Yes", App.CurrentCulture);
	}

	public static FontFamily GetFont(FontFamily givenFont, FontFamily modifiable, CellState state)
		=> state == CellState.Modifiable ? modifiable : givenFont;
}
