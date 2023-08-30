namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a generating strategy items provider.
/// </summary>
public sealed class GeneratingStrategyItemsProvider : IRunningStrategyItemsProvider
{
	private const string GeneratingStrategyPage_DifficultyLevel = nameof(GeneratingStrategyPage_DifficultyLevel);

	private const string GeneratingStrategyPage_Symmetry = nameof(GeneratingStrategyPage_Symmetry);

	private const string GeneratingStrategyPage_TechniquesMustIncluded = nameof(GeneratingStrategyPage_TechniquesMustIncluded);

	private const string GeneratingStrategyPage_IsMinimalPuzzle = nameof(GeneratingStrategyPage_IsMinimalPuzzle);

	private const string GeneratingStrategyPage_FirstAssignmentAttribute = nameof(GeneratingStrategyPage_FirstAssignmentAttribute);


	/// <inheritdoc/>
	public IList<RunningStrategyItem> Items
		=> [
			new(
				GeneratingStrategyPage_DifficultyLevel,
				DifficultyLevelConversion.GetName(((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel),
				new(
					GeneratingStrategyPage_DifficultyLevel,
					static obj => ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)obj!
				)
			),
			new(
				GeneratingStrategyPage_Symmetry,
				GetString($"SymmetricType_{((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern}"),
				new(
					GeneratingStrategyPage_Symmetry,
					static obj => ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)obj!
				)
			),
			new(
				GeneratingStrategyPage_TechniquesMustIncluded,
				((App)Application.Current).Preference.UIPreferences.SelectedTechnique switch
				{
					Technique.None => GetString("TechniqueSelector_NoTechniqueSelected"),
					var n => $"{n.GetName()} ({n.GetEnglishName()})"
				},
				new(
					GeneratingStrategyPage_TechniquesMustIncluded,
					static obj => ((App)Application.Current).Preference.UIPreferences.SelectedTechnique = (Technique)obj!
				)
			),
			new(
				GeneratingStrategyPage_IsMinimalPuzzle,
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal.ToString(),
				new(
					GeneratingStrategyPage_IsMinimalPuzzle,
					static obj => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal = (bool)obj!
				)
			),
			new(
				GeneratingStrategyPage_FirstAssignmentAttribute,
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl switch
				{
					true => GetString("GeneratingStrategyPage_PearlPuzzle"),
					false => GetString("GeneratingStrategyPage_NormalPuzzle"),
					//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
				},
				new(
					GeneratingStrategyPage_FirstAssignmentAttribute,
					static obj => ((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl = (bool)obj!
				)
			)
		];
}
