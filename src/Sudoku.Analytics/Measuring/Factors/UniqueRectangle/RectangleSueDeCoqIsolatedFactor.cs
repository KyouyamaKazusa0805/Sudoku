namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the isolated rule of <see cref="UniqueRectangleWithSueDeCoqStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleWithSueDeCoqStep"/>
public sealed partial class RectangleSueDeCoqIsolatedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(UniqueRectangleWithSueDeCoqStep.IsCannibalistic), nameof(IIsolatedDigitTrait.ContainsIsolatedDigits)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithSueDeCoqStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => !(bool)args![0]! && (bool)args![1]! ? 1 : 0;
}
