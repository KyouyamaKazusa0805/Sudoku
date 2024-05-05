namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of a chain.
/// </summary>
public sealed partial class BasicChainLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ChainingStep.Complexity)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ChainingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => ChainingLength.GetLengthDifficulty((int)args![0]! - 2);
}
