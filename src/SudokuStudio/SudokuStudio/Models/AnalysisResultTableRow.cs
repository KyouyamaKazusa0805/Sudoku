namespace SudokuStudio.Models;

/// <summary>
/// Defines a row of analysis result table.
/// </summary>
internal sealed class AnalysisResultTableRow
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
	/// Creates the list of <see cref="AnalysisResultRow"/> as the result value,
	/// via the specified <paramref name="analysisResult"/> instance of <see cref="LogicalSolverResult"/> type.
	/// </summary>
	/// <param name="analysisResult">
	/// The <see cref="LogicalSolverResult"/> instance that is used for creating the result value.
	/// </param>
	/// <returns>The result list of <see cref="AnalysisResultRow"/>-typed elements.</returns>
	public static IEnumerable<AnalysisResultTableRow> CreateListFrom(LogicalSolverResult analysisResult)
	{
		return
			from step in analysisResult.Steps
			orderby step.DifficultyLevel, step.TechniqueCode
			group step by step.Name into stepGroup
			let stepGroupArray = stepGroup.ToArray()
			let difficultyLevels =
				from step in stepGroupArray
				group step by step.DifficultyLevel into stepGroupedByDifficultyLevel
				select stepGroupedByDifficultyLevel.Key into targetDifficultyLevel
				orderby targetDifficultyLevel
				select targetDifficultyLevel
			select new AnalysisResultTableRow
			{
				TechniqueName = stepGroup.Key,
				CountOfSteps = stepGroupArray.Length,
				DifficultyLevel = difficultyLevels.Aggregate(aggregateFunc),
				TotalDifficulty = stepGroupArray.Sum(selector),
				MaximumDifficulty = stepGroupArray.Max(selector)
			};


		static decimal selector(IStep step) => step.Difficulty;

		static DifficultyLevel aggregateFunc(DifficultyLevel interim, DifficultyLevel next) => interim | next;
	}
}
