namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs appeared in <see cref="ExocetBaseStep"/>.
/// </summary>
/// <seealso cref="ExocetBaseStep"/>
public sealed class ExocetConjugatePairsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExocetBaseStep.ConjugatePairs)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExocetBaseStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((Conjugate[])args![0]!).Length);
}
