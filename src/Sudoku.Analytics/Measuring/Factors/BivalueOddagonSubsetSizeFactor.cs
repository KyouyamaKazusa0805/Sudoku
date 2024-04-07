namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures subset size of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonSubsetSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0}.Count / 2";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BivalueOddagonType3Step.ExtraCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonType3Step);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch { BivalueOddagonType3Step { ExtraCells.Count: var cellsCount } => cellsCount >> 1, _ => null };
}
