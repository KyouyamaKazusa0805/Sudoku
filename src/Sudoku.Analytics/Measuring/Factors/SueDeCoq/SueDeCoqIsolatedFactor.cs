namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the isolated rule of <see cref="SueDeCoqStep"/>.
/// </summary>
/// <seealso cref="SueDeCoqStep"/>
public sealed class SueDeCoqIsolatedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(SueDeCoqStep.IsolatedDigitsMask)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(SueDeCoqStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (Mask)args![0]! != 0 ? 2 : 0;
}
