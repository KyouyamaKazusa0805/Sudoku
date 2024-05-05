namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a type that describes the size of subset appeared in <see cref="UniqueRectangleBurredSubsetStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleBurredSubsetStep"/>
public sealed partial class UniqueRectangleBurredSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleBurredSubsetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]!;
}
