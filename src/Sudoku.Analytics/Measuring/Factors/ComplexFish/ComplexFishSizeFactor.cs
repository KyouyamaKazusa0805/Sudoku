namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of <see cref="ComplexFishStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			2 => 0,
			3 => 6,
			4 => 20,
			5 => 33,
			6 => 45,
			7 => 56,
			_ => 66
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ComplexFishStep { Size: var size } => size switch
			{
				2 => 0,
				3 => 6,
				4 => 20,
				5 => 33,
				6 => 45,
				7 => 56,
				_ => 66
			},
			_ => null
		};
}
