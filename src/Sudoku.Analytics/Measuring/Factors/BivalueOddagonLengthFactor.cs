namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures length of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonLengthFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0}.Count / 2";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BivalueOddagonStep.LoopCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch { BivalueOddagonStep { LoopCells.Count: var cellsCount } => cellsCount >> 1, _ => null };
}
