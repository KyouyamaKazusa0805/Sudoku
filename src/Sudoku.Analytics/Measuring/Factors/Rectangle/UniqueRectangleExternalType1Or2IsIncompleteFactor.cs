namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the external unique rectangle is incomplete.
/// </summary>
public sealed class UniqueRectangleExternalType1Or2IsIncompleteFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleExternalType1Or2Step.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalType1Or2Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
