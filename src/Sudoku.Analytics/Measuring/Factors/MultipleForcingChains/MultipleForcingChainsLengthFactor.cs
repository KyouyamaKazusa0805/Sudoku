namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of a multiple forcing chains.
/// </summary>
public sealed class MultipleForcingChainsLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultipleForcingChainsStep.Complexity)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultipleForcingChainsStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => ChainingLength.GetLengthDifficulty((int)args![0]!);
}
