namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a subset size factor. This factor will be applied to type 3 in deadly patterns.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
public abstract class AlmostSubsetSizeFactor<TStep> : Factor
	where TStep : Step, IPatternType3StepTrait<TStep>
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "{0}";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(IPatternType3StepTrait<TStep>.SubsetSize)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch { TStep { SubsetSize: var size } => size, _ => null };
}
