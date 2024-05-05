namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern for <see cref="ReverseBivalueUniversalGraveStep"/>.
/// </summary>
/// <seealso cref="ReverseBivalueUniversalGraveStep"/>
public sealed partial class ReverseBivalueUniversalGraveSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ReverseBivalueUniversalGraveStep.CompletePattern)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ReverseBivalueUniversalGraveStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((CellMap)args![0]!).Count);
}
