namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for the table-like grid controls to display techniques used,
/// using technique name to distinct them.
/// </summary>
/// <param name="techniqueName">Indicates the name of the technique.</param>
/// <param name="difficultyLevel">Indicates the difficulty level of the technique.</param>
/// <param name="totalDifficulty">Indicates the total difficulty of the group of steps.</param>
/// <param name="maximumDifficulty">Indicates the maximum difficulty of the group of steps.</param>
/// <param name="countOfSteps">Indicates the number of steps in this group.</param>
/// <seealso cref="AnalyzerResult"/>
[method: SetsRequiredMembers]
internal sealed partial class SummaryViewBindableSource(
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] string techniqueName,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] DifficultyLevel difficultyLevel,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] decimal totalDifficulty,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] decimal maximumDifficulty,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] int countOfSteps
)
{
	/// <summary>
	/// Creates the list of <see cref="SummaryViewBindableSource"/> as the result value,
	/// via the specified <paramref name="analyzerResult"/> instance of <see cref="AnalyzerResult"/> type.
	/// </summary>
	/// <param name="analyzerResult">
	/// The <see cref="AnalyzerResult"/> instance that is used for creating the result value.
	/// </param>
	/// <returns>The result list of <see cref="SummaryViewBindableSource"/>-typed elements.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle hasn't been solved.</exception>
	public static ObservableCollection<SummaryViewBindableSource> CreateListFrom(AnalyzerResult analyzerResult)
		=> analyzerResult switch
		{
			{ IsSolved: true, Steps: var steps } => [
				..
				from step in steps
				orderby step.DifficultyLevel, step.Code
				group step by step.GetName(App.CurrentCulture) into stepGroup
				let stepGroupArray = (Step[])[.. stepGroup]
				let difficultyLevels =
					from step in stepGroupArray
					group step by step.DifficultyLevel into stepGroupedByDifficultyLevel
					select stepGroupedByDifficultyLevel.Key into targetDifficultyLevel
					orderby targetDifficultyLevel
					select targetDifficultyLevel
				select new SummaryViewBindableSource(
					stepGroup.Key,
					difficultyLevels.Aggregate(CommonMethods.EnumFlagMerger),
					stepGroupArray.Sum(static step => step.Difficulty),
					stepGroupArray.Max(static step => step.Difficulty),
					stepGroupArray.Length
				)
			],
			_ => throw new InvalidOperationException("This method requires the puzzle having been solved, and has a unique solution.")
		};
}
