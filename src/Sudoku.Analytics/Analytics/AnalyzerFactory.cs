using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a factory for construction of type <see cref="Analyzer"/>, with extra configuration.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class AnalyzerFactory
{
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
		@this.StepSearchers = level == 0
			? stepSearchers
			: from stepSearcher in stepSearchers where Array.Exists(stepSearcher.DifficultyLevelRange, l => l <= level) select stepSearcher;
		return @this;
	}

	/// <summary>
	/// Operate an arbitary by checking the specified condition.
	/// </summary>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="condition">The condition to be checked.</param>
	/// <param name="trueMatch">The action will be handled when <paramref name="condition"/> is <see langword="true"/>.</param>
	/// <param name="falseMatch">The action will be handled when <paramref name="condition"/> is <see langword="false"/>.</param>
	/// <returns>The result.</returns>
	public static Analyzer WithActionIfMatch(this Analyzer @this, bool condition, Action<Analyzer> trueMatch, Action<Analyzer> falseMatch)
	{
		(condition ? trueMatch : falseMatch)(@this);
		return @this;
	}

	/// <inheritdoc cref="WithStepSearchers(Analyzer, StepSearcher[], DifficultyLevel)"/>
	public static Analyzer WithStepSearchers(this Analyzer @this, IEnumerable<StepSearcher> stepSearchers, DifficultyLevel level = default)
		=> @this.WithStepSearchers([.. stepSearchers], level);

	/// <summary>
	/// Try to set property with the specified value for the <typeparamref name="TStepSearcher"/> type.
	/// If the target <see cref="StepSearcher"/> collection does not contain the step searcher instance
	/// of type <typeparamref name="TStepSearcher"/>, the assignment will be skipped, never throwing exceptions.
	/// </summary>
	/// <typeparam name="TStepSearcher">The type of the <see cref="StepSearcher"/>.</typeparam>
	/// <param name="this">The current <see cref="Analyzer"/> instance.</param>
	/// <param name="propertySetter">The method to set the target property with new value.</param>
	/// <seealso cref="StepSearcher"/>
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
