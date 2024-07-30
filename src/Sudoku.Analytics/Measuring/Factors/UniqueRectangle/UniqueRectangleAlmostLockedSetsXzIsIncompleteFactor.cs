namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the unique rectangle is incomplete.
/// </summary>
public sealed class UniqueRectangleAlmostLockedSetsXzIsIncompleteFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleAlmostLockedSetsXzStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleAlmostLockedSetsXzStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
