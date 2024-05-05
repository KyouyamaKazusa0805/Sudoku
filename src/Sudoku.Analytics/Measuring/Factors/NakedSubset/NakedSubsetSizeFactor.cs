namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in naked subsets.
/// </summary>
public sealed class NakedSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NakedSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NakedSubsetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20 };
}
