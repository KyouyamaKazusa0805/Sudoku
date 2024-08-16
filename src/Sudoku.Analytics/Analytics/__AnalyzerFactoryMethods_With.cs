namespace Sudoku.Analytics;

public partial class __AnalyzerFactoryMethods_With
{
	/// <summary>
	/// Try to set property <see cref="Analyzer.StepSearchers"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="stepSearchers">The custom collection of <see cref="StepSearcher"/>s.</param>
	/// <param name="level">Indicates the difficulty level preserved.</param>
	/// <returns>The result.</returns>
	/// <seealso cref="Analyzer.StepSearchers"/>
	/// <seealso cref="StepSearcher"/>
	public static Analyzer WithStepSearchers(this Analyzer @this, StepSearcher[] stepSearchers, DifficultyLevel level = DifficultyLevel.Unknown)
		=> @this.WithStepSearchers(
			level == DifficultyLevel.Unknown
				? stepSearchers
				:
				from stepSearcher in stepSearchers
				where stepSearcher.Metadata.DifficultyLevelRange.Any(l => l <= level)
				select stepSearcher
		);
}
