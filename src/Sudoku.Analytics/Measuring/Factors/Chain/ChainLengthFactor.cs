namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of a <see cref="ChainOrLoop"/>.
/// </summary>
public sealed class ChainLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalChainStep.ChainLength)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalChainStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => ChainingLength.GetLengthDifficulty((int)args![0]!);
}
