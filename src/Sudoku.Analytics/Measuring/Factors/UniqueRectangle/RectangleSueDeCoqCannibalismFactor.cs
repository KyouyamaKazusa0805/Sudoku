namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="UniqueRectangleSueDeCoqStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleSueDeCoqStep"/>
public sealed class RectangleSueDeCoqCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleSueDeCoqStep.IsCannibalistic)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleSueDeCoqStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
