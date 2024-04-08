namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the subset appeared in step is hidden subset.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
/// <param name="options"><inheritdoc/></param>
public abstract class AlmostSubsetIsHiddenFactor<TStep>(StepSearcherOptions options) : Factor(options)
	where TStep : Step, IPatternType3StepTrait<TStep>
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "{0} ? 1 : 0";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(IPatternType3StepTrait<TStep>.IsHidden)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch { TStep { IsHidden: var isHidden } => isHidden ? 1 : 0, _ => null };
}
