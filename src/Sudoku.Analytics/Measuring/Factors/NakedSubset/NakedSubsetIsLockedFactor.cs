namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes which locked case a naked subset is.
/// </summary>
public sealed class NakedSubsetIsLockedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NakedSubsetStep.IsLocked), nameof(NakedSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NakedSubsetStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula
		=> static args => ((bool?)args![0]!, (int)args![1]!) switch { (true, 2) => -10, (true, 3) => -11, (false, _) => 1, _ => 0 };
}
