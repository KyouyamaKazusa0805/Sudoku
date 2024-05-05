namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length property of <see cref="BlossomLoopStep"/>.
/// </summary>
/// <seealso cref="BlossomLoopStep"/>
public sealed class BlossomLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BlossomLoopStep.Chains)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => ChainingLength.GetLengthDifficulty(((MultipleForcingChains)args![0]!).Potentials.Sum(ChainingStep.AncestorsCountOf));
}
