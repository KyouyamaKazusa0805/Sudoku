namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs (strong links) appeared in a rectangle.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class RectangleConjugatePairsCountFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0}.Length * 2";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleWithConjugatePairStep.ConjugatePairs)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithConjugatePairStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueRectangleWithConjugatePairStep { ConjugatePairs.Length: var length } => length << 1,
			_ => null
		};
}
