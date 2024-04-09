namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures the size of the pattern appeared in a <see cref="ExtendedSubsetPrincipleStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ExtendedSubsetPrincipleStep"/>
public sealed class ExtendedSubsetPrincipleSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0}.Count switch { 3 or 4 => 0, 5 or 6 or 7 => 2, 8 or 9 => 4 }";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedSubsetPrincipleStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedSubsetPrincipleStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ExtendedSubsetPrincipleStep { Cells.Count: var count } => count switch
			{
				3 or 4 => 0,
				5 or 6 or 7 => 2,
				8 or 9 => 4
			},
			_ => null
		};
}
