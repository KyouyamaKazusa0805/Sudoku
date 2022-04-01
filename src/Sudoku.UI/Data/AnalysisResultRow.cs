using System.Collections.Generic;
using System.Linq;
using Sudoku.Solving.Manual;

namespace Sudoku.UI.Data;

/// <summary>
/// Defines a row that used for displaying the information about a technique,
/// whose inner data is from the <see cref="ManualSolverResult"/> instance.
/// </summary>
/// <seealso cref="ManualSolverResult"/>
public sealed class AnalysisResultRow
{
	/// <summary>
	/// Indicates the total difficulty of all steps.
	/// </summary>
	public decimal TotalDifficulty { get; set; }

	/// <summary>
	/// Indicates the maximum difficulty in the steps.
	/// </summary>
	public decimal MaximumDifficulty { get; set; }

	/// <summary>
	/// Indicates the number of steps that uses logic of the current technique.
	/// </summary>
	public int CountOfSteps { get; set; }

	/// <summary>
	/// Indicates the technique name.
	/// </summary>
	public string TechniqueName { get; set; } = null!;

	/// <summary>
	/// Indicates the difficulty level of the techinque belonging to.
	/// </summary>
	public DifficultyLevel DifficultyLevel { get; set; }


	/// <summary>
	/// Creates the list of <see cref="AnalysisResultRow"/> as the result value,
	/// via the specified <paramref name="analysisResult"/> instance of <see cref="ManualSolverResult"/> type.
	/// </summary>
	/// <param name="analysisResult">
	/// The <see cref="ManualSolverResult"/> instance that is used for creating the result value.
	/// </param>
	/// <returns>The result list of <see cref="AnalysisResultRow"/>-typed elements.</returns>
	public static IEnumerable<AnalysisResultRow> CreateListFrom(ManualSolverResult analysisResult)
	{
		var stepDifficultySelector = static (Step step) => step.Difficulty;
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
			select new AnalysisResultRow
			{
				TechniqueName = stepGroup.Key,
				CountOfSteps = stepGroupArray.Length,
				DifficultyLevel = difficultyLevels.Aggregate(static (interim, next) => interim | next),
				TotalDifficulty = stepGroupArray.Sum(stepDifficultySelector),
				MaximumDifficulty = stepGroupArray.Max(stepDifficultySelector)
			};
	}
}
