namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a <see cref="ComplexFishStep"/> is a Sashimi fish.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishIsSashimiFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			false => {1} switch {{ 2 or 3 or 4 => 2, 5 or 6 or 7 => 3, _ => 4 }},
			true => {1} switch {{ 2 or 3 => 3, 4 or 5 => 4, 6 => 5, 7 => 6, _ => 7 }},
			_ => 0
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.IsSashimi), nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ComplexFishStep { Size: var size, IsSashimi: var isSashimi } => isSashimi switch
			{
				false => size switch { 2 or 3 or 4 => 2, 5 or 6 or 7 => 3, _ => 4 },
				true => size switch { 2 or 3 => 3, 4 or 5 => 4, 6 => 5, 7 => 6, _ => 7 },
				_ => 0
			},
			_ => null
		};
}
