namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern for <see cref="ReverseBivalueUniversalGraveStep"/>.
/// </summary>
/// <seealso cref="ReverseBivalueUniversalGraveStep"/>
public sealed partial class ReverseBivalueUniversalGraveSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ReverseBivalueUniversalGraveStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => OeisSequences.A002024((int)args![0]!);
}
