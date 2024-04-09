namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs appeared in <see cref="ExocetBaseStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ExocetBaseStep"/>
public sealed class ExocetConjugatePairsCountFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0}.Length * 2";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExocetBaseStep.ConjugatePairs)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExocetBaseStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ExocetBaseStep { ConjugatePairs.Length: var length } => length << 1,
			_ => null
		};
}
