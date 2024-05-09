namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a hidden subset is.
/// </summary>
public sealed class HiddenSubsetIsLockedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(HiddenSubsetStep.IsLocked), nameof(HiddenSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HiddenSubsetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? (int)args![1]! switch { 2 => -12, 3 => -13 } : 0;
}
