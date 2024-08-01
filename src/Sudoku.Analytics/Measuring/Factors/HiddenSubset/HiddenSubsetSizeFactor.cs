namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the size factor used in hidden subsets.
/// </summary>
public sealed class HiddenSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(HiddenSubsetStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HiddenSubsetStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20 };
}
