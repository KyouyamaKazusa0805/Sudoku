namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the rectangle is an avoidable rectangle.
/// </summary>
public sealed class RectangleIsAvoidableFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleStep.IsAvoidable)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 1 : 0;
}
