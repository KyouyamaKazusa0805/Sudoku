namespace Sudoku.Analytics;

/// <summary>
/// Represents a factory for construction of type <see cref="Analyzer"/>, with extra configuration.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class AnalyzerFactory
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
	{
		if (level == DifficultyLevel.Unknown)
		{
			return @this.WithStepSearchers(stepSearchers);
		}

		@this.StepSearchers =
			from stepSearcher in stepSearchers
			where stepSearcher.Metadata.DifficultyLevelRange.Any(l => l <= level)
			select stepSearcher;
		return @this;
	}

	/// <summary>
	/// Try to set property with the specified value for the <typeparamref name="TStepSearcher"/> type.
	/// If the target <see cref="StepSearcher"/> collection does not contain the step searcher instance
	/// of type <typeparamref name="TStepSearcher"/>, the assignment will be skipped, never throwing exceptions.
	/// </summary>
	/// <typeparam name="TStepSearcher">The type of the <see cref="StepSearcher"/>.</typeparam>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="propertySetter">The method to set the target property with new value.</param>
	/// <returns>The result.</returns>
	public static Analyzer WithStepSearcherSetters<TStepSearcher>(this Analyzer @this, Action<TStepSearcher> propertySetter)
		where TStepSearcher : StepSearcher
	{
		foreach (var stepSearcher in @this.ResultStepSearchers)
		{
			if (stepSearcher is TStepSearcher target)
			{
				propertySetter(target);
			}
		}
		return @this;
	}
}
