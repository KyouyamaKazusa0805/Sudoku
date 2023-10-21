using Sudoku.Analytics.Configuration;

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
	public static StepCollector WithMaxSteps(this StepCollector @this, Count count)
	{
		@this.MaxStepsGathered = count;
		return @this;
	}

	/// <summary>
	/// Try to set property <see cref="StepCollector.Options"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="StepCollector"/> instance.</param>
	/// <param name="options">
	/// The custom option instance. The value can be <see langword="null"/> if you want to revert with default value.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="StepCollector.Options"/>
	public static StepCollector WithUserDefinedOptions(this StepCollector @this, StepSearcherOptions? options)
	{
		@this.Options = options ?? StepSearcherOptions.Default;
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
	/// Sets the property <see cref="StepCollector.DifficultyLevelMode"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="collectingMode">The mode of the collecting steps.</param>
	/// <returns>The reference same as <see cref="StepCollector"/>.</returns>
	public static StepCollector WithSameLevelConfigruation(this StepCollector @this, StepCollectorDifficultyLevelMode collectingMode)
	{
		@this.DifficultyLevelMode = collectingMode;
		return @this;
	}
}
