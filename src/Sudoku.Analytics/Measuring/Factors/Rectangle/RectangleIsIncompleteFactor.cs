namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a rectangle is incomplete.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
public abstract class RectangleIsIncompleteFactor<TStep> : Factor
	where TStep : UniqueRectangleStep, IIncompleteTrait
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "{0} ? 1 : 0";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(IIncompleteTrait.IsIncomplete)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch
		{
			IIncompleteTrait { IsIncomplete: var isIncomplete } => isIncomplete ? 1 : 0,
			_ => null
		};
}
