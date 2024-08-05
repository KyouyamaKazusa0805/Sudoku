namespace Sudoku.Analytics;

/// <summary>
/// Represents a factory for construction of type <see cref="Collector"/>, with extra configuration.
/// </summary>
/// <seealso cref="Collector"/>
public static class CollectorFactory
{
	/// <summary>
	/// Sets the property <see cref="Collector.MaxStepsCollected"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="count">The number of maximum value.</param>
	/// <returns>The reference same as <see cref="Collector"/>.</returns>
	public static Collector WithMaxSteps(this Collector @this, int count)
	{
		@this.MaxStepsCollected = count;
		return @this;
	}

	/// <summary>
	/// Try to set property <see cref="Collector.Options"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="Collector"/> instance.</param>
	/// <param name="options">
	/// The custom option instance. The value can be <see langword="null"/> if you want to revert with default value.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Collector.Options"/>
	public static Collector WithUserDefinedOptions(this Collector @this, StepSearcherOptions? options)
	{
		@this.Options = options ?? StepSearcherOptions.Default;
		return @this;
	}

	/// <summary>
	/// Try to set the variant culture for the specified <see cref="Collector"/> instance.
	/// </summary>
	/// <param name="this">The current <see cref="Collector"/> instance.</param>
	/// <param name="formatProvider">The culture to be set.</param>
	/// <returns>The result.</returns>
	public static Collector WithCulture(this Collector @this, IFormatProvider? formatProvider)
	{
		@this.CurrentCulture = formatProvider;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Collector.StepSearchers"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="stepSearchers">The step searchers.</param>
	/// <returns>The reference same as <see cref="Collector"/>.</returns>
	public static Collector WithStepSearchers(this Collector @this, params StepSearcher[] stepSearchers)
	{
		@this.StepSearchers = stepSearchers;
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
	/// <seealso cref="StepSearcher"/>
	public static Collector WithStepSearcherSetters<TStepSearcher>(this Collector @this, Action<TStepSearcher> propertySetter)
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

	/// <summary>
	/// Sets the property <see cref="Collector.DifficultyLevelMode"/> with the target value.
	/// </summary>
	/// <param name="this">The collector instance.</param>
	/// <param name="collectingMode">The mode of the collecting steps.</param>
	/// <returns>The reference same as <see cref="Collector"/>.</returns>
	public static Collector WithSameLevelConfigruation(this Collector @this, CollectorDifficultyLevelMode collectingMode)
	{
		@this.DifficultyLevelMode = collectingMode;
		return @this;
	}
}
