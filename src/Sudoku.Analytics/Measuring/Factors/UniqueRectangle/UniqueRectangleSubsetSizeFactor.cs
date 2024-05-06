namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="UniqueRectangleType3Step"/>.
/// </summary>
/// <seealso cref="UniqueRectangleType3Step"/>
public sealed partial class UniqueRectangleSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<UniqueRectangleType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleType3Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]!;
}
