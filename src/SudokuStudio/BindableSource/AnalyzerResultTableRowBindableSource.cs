using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using WinRT;

namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for the table-like grid controls to display techniques used,
/// using technique name to distinct them.
/// </summary>
/// <seealso cref="AnalyzerResult"/>
internal sealed class AnalyzerResultTableRowBindableSource
{
	/// <summary>
	/// Indicates the total difficulty of all steps.
	/// </summary>
	public required decimal TotalDifficulty { get; set; }

	/// <summary>
	/// Indicates the maximum difficulty in the steps.
	/// </summary>
	public required decimal MaximumDifficulty { get; set; }

	/// <summary>
	/// Indicates the number of steps that uses logic of the current technique.
	/// </summary>
	public required int CountOfSteps { get; set; }

	/// <summary>
	/// Indicates the technique name.
	/// </summary>
	public required string TechniqueName { get; set; }

	/// <summary>
	/// Indicates the difficulty level of the technique belonging to.
	/// </summary>
	public required DifficultyLevel DifficultyLevel { get; set; }


	/// <summary>
	/// Creates the list of <see cref="AnalyzerResultTableRowBindableSource"/> as the result value,
	/// via the specified <paramref name="analyzerResult"/> instance of <see cref="AnalyzerResult"/> type.
	/// </summary>
	/// <param name="analyzerResult">
	/// The <see cref="AnalyzerResult"/> instance that is used for creating the result value.
	/// </param>
	/// <returns>The result list of <see cref="AnalyzerResultTableRowBindableSource"/>-typed elements.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle hasn't been solved.</exception>
	public static IEnumerable<AnalyzerResultTableRowBindableSource> CreateListFrom(AnalyzerResult analyzerResult)
	{
		if (analyzerResult is not { IsSolved: true, Steps: var steps })
		{
			throw new InvalidOperationException("This method requires the puzzle having been solved, and has a unique solution.");
		}

		return
			from step in steps
			orderby step.DifficultyLevel, step.Code
			group step by step.Name into stepGroup
			let stepGroupArray = (Step[])[.. stepGroup]
			let difficultyLevels =
				from step in stepGroupArray
				group step by step.DifficultyLevel into stepGroupedByDifficultyLevel
				select stepGroupedByDifficultyLevel.Key into targetDifficultyLevel
				orderby targetDifficultyLevel
				select targetDifficultyLevel
			select new AnalyzerResultTableRowBindableSource
			{
				TechniqueName = stepGroup.Key,
				CountOfSteps = stepGroupArray.Length,
				DifficultyLevel = difficultyLevels.Aggregate(CommonMethods.EnumFlagMerger),
				TotalDifficulty = stepGroupArray.Sum(static step => step.Difficulty),
				MaximumDifficulty = stepGroupArray.Max(static step => step.Difficulty)
			};
	}
}
