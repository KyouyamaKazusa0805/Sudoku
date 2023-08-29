namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a generating strategy items provider.
/// </summary>
public sealed class GeneratingStrategyItemsProvider : IRunningStrategyItemsProvider
{
	/// <inheritdoc/>
	public IList<RunningStrategyItem> Items
		=> [
			new(
				GetString("GeneratingStrategyPage_DifficultyLevel"),
				DifficultyLevelConversion.GetName(((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel)
			),
			new(
				GetString("GeneratingStrategyPage_Symmetry"),
				GetString($"SymmetricType_{((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern}")
			),
			new(
				GetString("GeneratingStrategyPage_TechniquesMustIncluded"),
				((App)Application.Current).Preference.UIPreferences.SelectedTechnique switch
				{
					Technique.None => GetString("TechniqueSelector_NoTechniqueSelected"),
					var n => $"{n.GetName()} ({n.GetEnglishName()})"
				}
			),
			new(
				GetString("GeneratingStrategyPage_IsMinimalPuzzle"),
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal.ToString()
			),
			new(
				GetString("GeneratingStrategyPage_FirstAssignmentAttribute"),
				((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl switch
				{
					true => GetString("GeneratingStrategyPage_PearlPuzzle"),
					false => GetString("GeneratingStrategyPage_NormalPuzzle"),
					//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
				}
			)
		];
}
