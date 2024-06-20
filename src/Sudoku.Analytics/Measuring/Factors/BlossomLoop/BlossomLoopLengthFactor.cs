namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of a blossom loop.
/// </summary>
public sealed class BlossomLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BlossomLoopStep.Complexity)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => ChainingLength.GetLengthDifficulty((int)args![0]!);
}
