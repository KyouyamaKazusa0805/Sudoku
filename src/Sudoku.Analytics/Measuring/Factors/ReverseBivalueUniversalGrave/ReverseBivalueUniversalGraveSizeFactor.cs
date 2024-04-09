namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern for <see cref="ReverseBivalueUniversalGraveStep"/>.
/// </summary>
/// <seealso cref="ReverseBivalueUniversalGraveStep"/>
public sealed class ReverseBivalueUniversalGraveSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "A002024({0}.Count)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ReverseBivalueUniversalGraveStep.CompletePattern)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ReverseBivalueUniversalGraveStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ReverseBivalueUniversalGraveStep { CompletePattern.Count: var cellsCount } => A002024(cellsCount),
			_ => null
		};
}
