namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the grouped logic in a multiple forcing chains.
/// </summary>
public sealed class MultipleForcingChainsGroupedFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultipleForcingChainsStep.IsGrouped)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultipleForcingChainsStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 2 : 0;
}
