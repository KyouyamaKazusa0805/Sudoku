namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="UniqueMatrixType3Step"/>.
/// </summary>
/// <seealso cref="UniqueMatrixType3Step"/>
public sealed class UniqueMatrixSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<UniqueMatrixType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueMatrixType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]!;
}
