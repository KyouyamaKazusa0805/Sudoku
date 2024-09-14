namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the grouped logic in a blossom loop.
/// </summary>
public sealed class BlossomLoopGroupedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BlossomLoopStep.IsGrouped)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 2 : 0;
}
