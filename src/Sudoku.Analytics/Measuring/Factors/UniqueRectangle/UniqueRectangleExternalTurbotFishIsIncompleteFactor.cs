namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the rectangle is incomplete.
/// </summary>
public sealed partial class UniqueRectangleExternalTurbotFishIsIncompleteFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalTurbotFishStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalTurbotFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
