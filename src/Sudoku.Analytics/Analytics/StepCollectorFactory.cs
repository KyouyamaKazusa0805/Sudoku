namespace Sudoku.Analytics;

/// <summary>
/// Represents a factory for construction of type <see cref="StepCollector"/>, with extra configuration.
/// </summary>
/// <seealso cref="StepCollector"/>
public static class StepCollectorFactory
{
	/// <summary>
	/// Sets the property <see cref="StepCollector.MaxStepsGathered"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="count">The number of maximum value.</param>
	/// <returns>The reference same as <see cref="StepCollector"/>.</returns>
	public static StepCollector WithMaxSteps(this StepCollector @this, int count)
	{
		@this.MaxStepsGathered = count;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="StepCollector.StepSearchers"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="stepSearchers">The step searchers.</param>
	/// <returns>The reference same as <see cref="StepCollector"/>.</returns>
	public static StepCollector WithStepSearchers(this StepCollector @this, StepSearcher[] stepSearchers)
	{
		@this.StepSearchers = stepSearchers;
		return @this;
	}

	/// <inheritdoc cref="WithStepSearchers(StepCollector, StepSearcher[])"/>
	public static StepCollector WithStepSearchers(this StepCollector @this, IEnumerable<StepSearcher> stepSearchers)
	{
		@this.StepSearchers = [.. stepSearchers];
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="StepCollector.OnlyShowSameLevelTechniquesInFindAllSteps"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="sameLevel">A <see cref="bool"/> value indicating whether the collector only collects for same-level steps.</param>
	/// <returns>The reference same as <see cref="StepCollector"/>.</returns>
	public static StepCollector WithSameLevelConfigruation(this StepCollector @this, bool sameLevel)
	{
		@this.OnlyShowSameLevelTechniquesInFindAllSteps = sameLevel;
		return @this;
	}
}
