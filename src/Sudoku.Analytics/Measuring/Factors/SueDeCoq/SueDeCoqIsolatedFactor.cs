namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the isolated rule of <see cref="SueDeCoqStep"/>.
/// </summary>
/// <seealso cref="SueDeCoqStep"/>
public sealed class SueDeCoqIsolatedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IIsolatedDigitTrait.ContainsIsolatedDigits)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(SueDeCoqStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 2 : 0;
}
