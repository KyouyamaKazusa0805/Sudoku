namespace Sudoku.Analytics;

/// <summary>
/// Provides a list of methods that can be used for construct chaining method invocations for configuration,
/// applying to an <see cref="Analyzer"/> instance.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class AnalyzerFactory
{
	/// <summary>
	/// Sets the property <see cref="Analyzer.RandomizedChoosing"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="randomizedChoosing">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithRandomizedChoosing(this Analyzer instance, bool randomizedChoosing)
	{
		instance.RandomizedChoosing = randomizedChoosing;
		return instance;
	}

	/// <summary>
	/// Sets the property <see cref="Analyzer.IsFullApplying"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="applyAll">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithApplyAll(this Analyzer instance, bool applyAll)
	{
		instance.IsFullApplying = applyAll;
		return instance;
	}

	/// <summary>
	/// Sets the property <see cref="Analyzer.IgnoreSlowAlgorithms"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="ignore">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithIgnoreHighTimeComplexityStepSearchers(this Analyzer instance, bool ignore)
	{
		instance.IgnoreSlowAlgorithms = ignore;
		return instance;
	}

	/// <summary>
	/// Sets the property <see cref="Analyzer.IgnoreHighAllocationAlgorithms"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="ignore">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithIgnoreHighSpaceComplexityStepSearchers(this Analyzer instance, bool ignore)
	{
		instance.IgnoreHighAllocationAlgorithms = ignore;
		return instance;
	}

	/// <summary>
	/// Sets the property <see cref="Analyzer.StepSearchers"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="stepSearchers">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithStepSearchers(this Analyzer instance, params StepSearcher[] stepSearchers)
	{
		instance.StepSearchers = stepSearchers;
		return instance;
	}

	/// <summary>
	/// Sets the property <see cref="Analyzer.Options"/> with the target value.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="options">The value to be set or updated.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer WithUserDefinedOptions(this Analyzer instance, StepGathererOptions options)
	{
		instance.Options = options;
		return instance;
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
	public static Analyzer WithStepSearchers(this Analyzer @this, StepSearcher[] stepSearchers, DifficultyLevel level = DifficultyLevel.Unknown)
		=> @this.WithStepSearchers(
			level == DifficultyLevel.Unknown
				? stepSearchers
				:
				from stepSearcher in stepSearchers
				where stepSearcher.Metadata.DifficultyLevelRange.Any(l => l <= level)
				select stepSearcher
		);

	/// <summary>
	/// Appends an element into the property <see cref="Analyzer.Setters"/>.
	/// </summary>
	/// <typeparam name="TStepSearcher">The type of step searcher.</typeparam>
	/// <param name="instance">The instance to be updated.</param>
	/// <param name="setter">The value to be added.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Analyzer AddStepSearcherSetter<TStepSearcher>(this Analyzer instance, Action<TStepSearcher> setter)
		where TStepSearcher : StepSearcher
	{
		instance.Setters.Add(
			s =>
			{
				if (s is TStepSearcher target)
				{
					setter(target);
				}
			}
		);
		return instance;
	}

	/// <summary>
	/// Appends an element into the property <see cref="Analyzer.Setters"/>.
	/// </summary>
	/// <param name="instance">The instance to be updated.</param>
	/// <param name="setters">The value to be added.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer AddStepSearcherSetter(this Analyzer instance, Action<StepSearcher> setters)
	{
		instance.Setters.Add(setters);
		return instance;
	}

	/// <summary>
	/// Appends an element into the property <see cref="Analyzer.Setters"/>.
	/// </summary>
	/// <param name="instance">The instance to be set or updated.</param>
	/// <param name="setters">A list of values to be added.</param>
	/// <returns>The value same as <see cref="Analyzer"/>.</returns>
	public static Analyzer AddStepSearcherSetter(this Analyzer instance, params ReadOnlySpan<Action<StepSearcher>> setters)
	{
		foreach (var element in setters)
		{
			instance.Setters.Add(element);
		}
		return instance;
	}
}
