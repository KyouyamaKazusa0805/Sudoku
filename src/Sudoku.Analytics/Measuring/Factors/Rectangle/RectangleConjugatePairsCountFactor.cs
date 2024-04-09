namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs (strong links) appeared in a rectangle.
/// </summary>
public sealed class RectangleConjugatePairsCountFactor : Factor
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
