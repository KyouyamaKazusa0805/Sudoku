namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether a <see cref="NormalFishStep"/> is a Sashimi fish.
/// </summary>
/// <seealso cref="NormalFishStep"/>
public sealed partial class NormalFishIsSashimiFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalFishStep.IsSashimi), nameof(NormalFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (bool?)args![0]! switch { true => (int)args![1]! switch { 2 or 3 => 3, 4 => 4 }, false => 2, _ => 0 };
}
