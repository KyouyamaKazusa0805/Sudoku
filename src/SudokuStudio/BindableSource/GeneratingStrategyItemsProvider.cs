namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a generating strategy items provider.
/// </summary>
public sealed class GeneratingStrategyItemsProvider : IRunningStrategyItemsProvider
{
	/// <inheritdoc/>
	public IList<RunningStrategyItem> Items
		=> [
			new(GetString("GeneratingStrategyPage_DifficultyLevel"), string.Empty),
			new(GetString("GeneratingStrategyPage_Symmetry"), string.Empty),
			new(GetString("GeneratingStrategyPage_TechniquesMustIncluded"), string.Empty),
			new(GetString("GeneratingStrategyPage_IsMinimalPuzzle"), string.Empty),
			new(GetString("GeneratingStrategyPage_FirstAssignmentAttribute"), string.Empty)
		];
}
