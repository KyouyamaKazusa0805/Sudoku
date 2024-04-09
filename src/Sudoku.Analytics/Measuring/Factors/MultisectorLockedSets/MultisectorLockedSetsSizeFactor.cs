namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern from <see cref="MultisectorLockedSetsStep"/>.
/// </summary>
/// <seealso cref="MultisectorLockedSetsStep"/>
public sealed class MultisectorLockedSetsSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "A002024({0}.Count)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultisectorLockedSetsStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultisectorLockedSetsStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			MultisectorLockedSetsStep { Cells.Count: var cellsCount } => A002024(cellsCount),
			_ => null
		};
}
