namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length property of <see cref="BlossomLoopStep"/>.
/// </summary>
/// <seealso cref="BlossomLoopStep"/>
[Obsolete]
public sealed class BlossomLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IComplexChainLengthTrait.ComplexLength)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => ChainingLength.GetLengthDifficulty((int)args![0]!);
}
