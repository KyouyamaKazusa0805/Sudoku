namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a <see cref="ComplexFishStep"/> is a Sashimi fish.
/// </summary>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishIsSashimiFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.IsSashimi), nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (bool?)args![0]! switch
		{
			false => (int)args[1]! switch { 2 or 3 or 4 => 2, 5 or 6 or 7 => 3, _ => 4 },
			true => (int)args[1]! switch { 2 or 3 => 3, 4 or 5 => 4, 6 => 5, 7 => 6, _ => 7 },
			_ => 0
		};
}
