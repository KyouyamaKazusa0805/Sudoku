namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes whether a subset appeared in <see cref="UniqueRectangleType3Step"/> is hidden.
/// </summary>
/// <seealso cref="UniqueRectangleType3Step"/>
public sealed class UniqueRectangleSubsetIsHiddenFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<UniqueRectangleType3Step>.IsHidden)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 1 : 0;
}
