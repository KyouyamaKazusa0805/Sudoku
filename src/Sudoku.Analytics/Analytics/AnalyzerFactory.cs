namespace Sudoku.Analytics;

/// <summary>
/// Represents a factory for construction of type <see cref="Analyzer"/>, with extra configuration.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class AnalyzerFactory
{
	/// <summary>
	/// Try to set randomized choosing.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="randomizedChoosing">Indicates whether the analyzer will adopt randomized algorithm to choose a step.</param>
	/// <returns>The result.</returns>
	public static Analyzer WithRandomizedChoosing(this Analyzer @this, bool randomizedChoosing)
	{
		@this.RandomizedChoosing = randomizedChoosing;
		return @this;
	}

	/// <summary>
	/// Try to set algorithm limits.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="ignoreLargeTimeComplexity">Indicates whether the analyzer ignores for large time-complexity step searchers.</param>
	/// <param name="ignoreLargeSpaceComplexity">Indicates whether the analyzer ignores for large space-complexity step searchers.</param>
	/// <returns>The result.</returns>
	public static Analyzer WithAlgorithmLimits(this Analyzer @this, bool ignoreLargeTimeComplexity, bool ignoreLargeSpaceComplexity)
	{
		@this.IgnoreSlowAlgorithms = ignoreLargeTimeComplexity;
		@this.IgnoreHighAllocationAlgorithms = ignoreLargeSpaceComplexity;
		return @this;
	}

	/// <summary>
	/// Try to set the variant culture for the specified <see cref="Analyzer"/> instance.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="culture">The culture to be set.</param>
	/// <returns>The result.</returns>
	public static Analyzer WithCulture(this Analyzer @this, CultureInfo? culture)
	{
		@this.CurrentCulture = culture;
		return @this;
	}

	/// <summary>
	/// Try to set "Apply All" option to an <see cref="Analyzer"/> instance.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="applyAll">The value to be set. The value will be assigned to property <see cref="Analyzer.IsFullApplying"/>.</param>
	/// <returns>The result.</returns>
	/// <seealso cref="Analyzer.IsFullApplying"/>
	public static Analyzer WithApplyAll(this Analyzer @this, bool applyAll)
	{
		@this.IsFullApplying = applyAll;
		return @this;
	}

	/// <summary>
	/// Try to set property <see cref="Analyzer.Options"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="options">
	/// The custom option instance. The value can be <see langword="null"/> if you want to revert with default value.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Analyzer.Options"/>
	public static Analyzer WithUserDefinedOptions(this Analyzer @this, StepSearcherOptions? options)
	{
		@this.Options = options ?? StepSearcherOptions.Default;
		return @this;
	}

	/// <summary>
	/// Try to set property <see cref="Analyzer.StepSearchers"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="stepSearchers">The custom collection of <see cref="StepSearcher"/>s.</param>
	/// <param name="level">Indicates the difficulty level preserved.</param>
	/// <returns>The result.</returns>
	/// <seealso cref="Analyzer.StepSearchers"/>
	/// <seealso cref="StepSearcher"/>
	public static Analyzer WithStepSearchers(this Analyzer @this, StepSearcher[] stepSearchers, DifficultyLevel level = default)
	{
		@this.StepSearchers = level switch
		{
			0 => stepSearchers,
			_ =>
			from stepSearcher in stepSearchers
			where Array.Exists(stepSearcher.Metadata.DifficultyLevelRange, l => l <= level)
			select stepSearcher
		};
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

	/// <summary>
	/// Try to set property <see cref="Analyzer.ConditionalOptions"/> with the specified value.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="options">
	/// The custom option instance. The value can be <see langword="null"/> if you want to revert with default value.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Analyzer.ConditionalOptions"/>
	internal static Analyzer WithConditionalOptions(this Analyzer @this, ConditionalOptions? options)
	{
		@this.ConditionalOptions = options;
		return @this;
	}
}
