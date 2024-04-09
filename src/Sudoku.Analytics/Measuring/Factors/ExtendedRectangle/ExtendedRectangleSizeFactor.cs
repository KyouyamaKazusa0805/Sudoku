namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of <see cref="ExtendedRectangleStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ExtendedRectangleStep"/>
public sealed class ExtendedRectangleSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "A004526({0}.Count) - 2";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedRectangleStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ExtendedRectangleStep { Cells.Count: var cellsCount } => A004526(cellsCount) - 2,
			_ => null
		};
}
