namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the grouped logic in a chain or a loop.
/// </summary>
public sealed class ChainGroupedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalChainStep.IsGrouped)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalChainStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 2 : 0;
}
