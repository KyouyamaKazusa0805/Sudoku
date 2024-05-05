namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the subset appeared in <see cref="DirectSubsetStep"/> is locked.
/// </summary>
/// <seealso cref="DirectSubsetStep"/>
public sealed partial class DirectSubsetIsLockedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(DirectSubsetStep.IsNaked), nameof(DirectSubsetStep.IsLocked), nameof(DirectSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(DirectSubsetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (bool)args![0]!
			? (bool?)args[1]! switch { true => (int)args[2]! switch { 2 => -10, 3 => -11 }, false => 1, _ => 0 }
			: (bool?)args[1]! switch { true => (int)args[2]! switch { 2 => -12, 3 => -13 }, _ => 0 };
}
