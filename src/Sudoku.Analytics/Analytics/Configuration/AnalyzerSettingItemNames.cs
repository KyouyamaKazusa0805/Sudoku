namespace Sudoku.Analytics.Configuration;

/// <summary>
/// epresents a list of <see cref="string"/>s indicating the runtime identifier recognized by UI,
/// used by <see cref="SettingItemNameAttribute"/>.
/// </summary>
/// <seealso cref="SettingItemNameAttribute"/>
public static class AnalyzerSettingItemNames
{
	//
	// Analyzer & Collector property names
	//
	/// <inheritdoc cref="Analyzer.IsFullApplying"/>
	public const string LogicalSolverIsFullApplying = nameof(LogicalSolverIsFullApplying);

	/// <inheritdoc cref="Analyzer.IgnoreSlowAlgorithms"/>
	public const string LogicalSolverIgnoresSlowAlgorithms = nameof(LogicalSolverIgnoresSlowAlgorithms);

	/// <inheritdoc cref="Analyzer.IgnoreHighAllocationAlgorithms"/>
	public const string LogicalSolverIgnoresHighAllocationAlgorithms = nameof(LogicalSolverIgnoresHighAllocationAlgorithms);

	/// <inheritdoc cref="Collector.DifficultyLevelMode"/>
	public const string DifficultyLevelMode = nameof(DifficultyLevelMode);

	/// <inheritdoc cref="Collector.MaxStepsGathered"/>
	public const string StepGathererMaxStepsGathered = nameof(StepGathererMaxStepsGathered);

	/// <summary>
	/// Indicates the supported techniques used in ittoryu mode.
	/// </summary>
	public const string IttoryuSupportedTechniques = nameof(IttoryuSupportedTechniques);
}
