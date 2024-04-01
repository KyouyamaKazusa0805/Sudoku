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
	{
		var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
		return analyzerResult switch
		{
			{ IsSolved: true, InterimSteps: var steps } => [
				..
				from step in steps
				orderby step.DifficultyLevel, step.Code
				group step by step.GetName(App.CurrentCulture) into stepGroup
				let stepGroupArray = (Step[])[.. stepGroup]
				let difficultyLevels =
					from step in stepGroupArray
					let code = step.Code
					group step by pref.GetDifficultyLevelOrDefault(code) into stepGroupedByDifficultyLevel
					select stepGroupedByDifficultyLevel.Key into targetDifficultyLevel
					orderby targetDifficultyLevel
					select targetDifficultyLevel
				select new SummaryViewBindableSource(
					stepGroup.Key,
					difficultyLevels.Aggregate(CommonMethods.EnumFlagMerger),
					stepGroupArray.Sum(r),
					stepGroupArray.Max(r),
					stepGroupArray.Length
				)
			],
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("GridMustBeSolvedOrUnique"))
		};


		static decimal r(Step step)
		{
			var pref = ((App)Application.Current).Preference.TechniqueInfoPreferences;
			return pref.GetRating(step.Code) is { } integerValue
				? integerValue / pref.RatingScale
				: step.Difficulty * TechniqueInfoPreferenceGroup.RatingScaleDefaultValue / pref.RatingScale;
		}
	}
}
