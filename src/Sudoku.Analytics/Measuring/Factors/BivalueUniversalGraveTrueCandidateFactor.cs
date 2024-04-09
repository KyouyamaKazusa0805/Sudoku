namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the true candidates in bi-value universal grave.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class BivalueUniversalGraveTrueCandidateFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "A002024({0}.Count)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BivalueUniversalGraveType2Step.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveType2Step);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			BivalueUniversalGraveType2Step { Cells.Count: var cellsCount } => A002024(cellsCount),
			_ => null
		};
}
