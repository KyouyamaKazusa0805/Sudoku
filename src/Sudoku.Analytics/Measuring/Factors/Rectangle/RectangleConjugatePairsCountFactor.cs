namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs (strong links) appeared in a rectangle.
/// </summary>
public sealed partial class RectangleConjugatePairsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleWithConjugatePairStep.ConjugatePairs)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithConjugatePairStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((Conjugate[])args![0]!).Length);
}
