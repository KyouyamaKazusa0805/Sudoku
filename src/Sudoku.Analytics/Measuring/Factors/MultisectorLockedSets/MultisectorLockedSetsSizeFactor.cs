namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern from <see cref="MultisectorLockedSetsStep"/>.
/// </summary>
/// <seealso cref="MultisectorLockedSetsStep"/>
public sealed class MultisectorLockedSetsSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultisectorLockedSetsStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultisectorLockedSetsStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((CellMap)args![0]!).Count);
}
