namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the rectangle is incomplete.
/// </summary>
public sealed class UniqueRectangleExternalXyWingIsCompleteFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalXyWingStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalXyWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
