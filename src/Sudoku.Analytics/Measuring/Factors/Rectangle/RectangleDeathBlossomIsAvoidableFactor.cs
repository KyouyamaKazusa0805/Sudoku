namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the factor 
/// </summary>
public sealed class RectangleDeathBlossomIsAvoidableFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RectangleDeathBlossomStep.IsAvoidable)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RectangleDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
