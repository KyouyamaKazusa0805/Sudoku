namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the subset size appeared in <see cref="UniqueLoopType3Step"/>.
/// </summary>
/// <seealso cref="UniqueLoopType3Step"/>
public sealed partial class UniqueLoopSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<UniqueLoopType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueLoopType3Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]!;
}
