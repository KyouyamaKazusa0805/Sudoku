namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the isolated rule of <see cref="UniqueRectangleSueDeCoqStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleSueDeCoqStep"/>
public sealed class RectangleSueDeCoqIsolatedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(UniqueRectangleSueDeCoqStep.IsCannibalistic), nameof(IIsolatedDigitTrait.ContainsIsolatedDigits)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleSueDeCoqStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => !(bool)args![0]! && (bool)args![1]! ? 1 : 0;
}
