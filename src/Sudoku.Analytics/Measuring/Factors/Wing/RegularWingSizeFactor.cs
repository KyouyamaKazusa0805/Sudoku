namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of regular wing comes from <see cref="RegularWingStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="RegularWingStep"/>
public sealed class RegularWingSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			3 => 0,
			4 => 2,
			5 => 4,
			6 => 7,
			7 => 10,
			8 => 13,
			9 => 16,
			_ => 20
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RegularWingStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RegularWingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			RegularWingStep { Size: var size } => size switch
			{
				3 => 0,
				4 => 2,
				5 => 4,
				6 => 7,
				7 => 10,
				8 => 13,
				9 => 16,
				_ => 20
			},
			_ => null
		};
}
