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
/// <seealso cref="AnalysisResult"/>
[method: SetsRequiredMembers]
internal sealed partial class SummaryViewBindableSource(
	[Property(Accessibility = "public required", Setter = "set")] string techniqueName,
	[Property(Accessibility = "public required", Setter = "set")] DifficultyLevel difficultyLevel,
	[Property(Accessibility = "public required", Setter = "set")] decimal totalDifficulty,
	[Property(Accessibility = "public required", Setter = "set")] decimal maximumDifficulty,
	[Property(Accessibility = "public required", Setter = "set")] int countOfSteps
)
{
	/// <summary>
	/// Creates the list of <see cref="SummaryViewBindableSource"/> as the result value,
	/// via the specified <paramref name="analysisResult"/> instance of <see cref="AnalysisResult"/> type.
	/// </summary>
	/// <param name="analysisResult">
	/// The <see cref="AnalysisResult"/> instance that is used for creating the result value.
	/// </param>
	/// <returns>The result list of <see cref="SummaryViewBindableSource"/>-typed elements.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle hasn't been solved.</exception>
	public static ObservableCollection<SummaryViewBindableSource> CreateListFrom(AnalysisResult analysisResult)
	{
		var pref = Application.Current.AsApp().Preference.TechniqueInfoPreferences;
		return analysisResult switch
		{
			{ IsSolved: true, StepsSpan: var steps } => [.. g(steps, pref)],
			{ IsPartiallySolved: true, StepsSpan: var steps } => [.. g(steps, pref)],
			_ => throw new InvalidOperationException(SR.ExceptionMessage("GridMustBeSolvedOrNotBad"))
		};


		static decimal r(Step step)
		{
			var pref = Application.Current.AsApp().Preference.TechniqueInfoPreferences;
			return pref.GetRating(step.Code) switch { { } v => v, _ => step.Difficulty } / pref.RatingScale;
		}

		static unsafe SummaryViewBindableSource[] g(ReadOnlySpan<Step> steps, TechniqueInfoPreferenceGroup pref)
		{
			var stepsSortedByName = new List<Step>();
			stepsSortedByName.AddRange(steps);
			stepsSortedByName.Sort(
				static (left, right) => left.DifficultyLevel.CompareTo(right.DifficultyLevel) is var difficultyLevelComparisonResult and not 0
					? difficultyLevelComparisonResult
					: left.Code.CompareTo(right.Code) is var codeComparisonResult and not 0
						? codeComparisonResult
						: StepMarshal.CompareName(left, right, App.CurrentCulture)
			);
			return [
				..
				from step in stepsSortedByName
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
					difficultyLevels.Aggregate(@delegate.EnumFlagMerger),
					stepGroupArray.SumUnsafe(&r),
					stepGroupArray.MaxUnsafe(&r),
					stepGroupArray.Length
				)
			];
		}
	}
}
