namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="DirectSubsetStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="DirectSubsetStepSearcher"/>
public sealed class DirectSubsetSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			2 => 0,
			3 => 6,
			4 => 20
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(DirectSubsetStep.SubsetCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(DirectSubsetStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			DirectSubsetStep { SubsetCells.Count: var cellsCount } => cellsCount switch
			{
				2 => 0,
				3 => 6,
				4 => 20
			},
			_ => null
		};
}
