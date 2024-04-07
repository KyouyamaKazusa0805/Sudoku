namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in hidden subsets.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class HiddenSubsetSizeFactor(StepSearcherOptions options) : Factor(options)
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
	public override string[] ParameterNames => [nameof(HiddenSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HiddenSubsetStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			HiddenSubsetStep { Size: var size } => size switch { 2 => 0, 3 => 6, 4 => 20 },
			_ => null
		};
}
