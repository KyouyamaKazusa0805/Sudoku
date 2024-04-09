namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a <see cref="NormalFishStep"/> is a Sashimi fish.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="NormalFishStep"/>
public sealed class NormalFishIsSashimiFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			true => {1} switch {{ 2 or 3 => 3, 4 => 4 }},
			false => 2,
			_ => 0
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalFishStep.IsSashimi), nameof(NormalFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalFishStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			NormalFishStep { Size: var size, IsSashimi: var isSashimi } => isSashimi switch
			{
				true => size switch { 2 or 3 => 3, 4 => 4 },
				false => 2,
				_ => 0
			},
			_ => null
		};
}
